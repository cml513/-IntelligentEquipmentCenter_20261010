using System;
using System.Collections.Generic;
using System.Linq;
using static IntelligentEquipmentCenter.ClientJson;

namespace IntelligentEquipmentCenter
{
    /// <summary>
    /// 配置数据来源解析器
    /// 把配置文件（StutableConfig.xml / DgvDataConfig.xml）中的 Source 字符串解析为实际值。
    /// Source 格式：类型:目标[:数据类型]，不分大小写
    ///   Recipe:WorkpositionName         配方属性
    ///   Mes:ProduceCode                 MES数据属性
    ///   Plc:D900:Int                    PLC 16位整数
    ///   Plc:D900:Float                  PLC 32位浮点数（读2个寄存器）
    ///   Plc:D900:String:20              PLC 字符串（20字节）
    ///   Scanner:                        扫码枪当前条码
    ///   Vision:                         视觉结果
    ///   Const:固定内容                   固定字符串
    ///   Dynamic:自定义字段                MesdataDLS.DynamicParams 中的自定义字段
    /// 无前缀（旧格式）：先配方属性、再MES数据属性，最后兜底取 DynamicParams 自定义字段
    /// </summary>
    public static class ConfigValueResolver
    {
        /// <summary>解析PLC地址，如"D900"-&gt;900</summary>
        public static int ParsePlcAddress(string plcAddress)
        {
            if (string.IsNullOrEmpty(plcAddress)) return 0;
            string addr = plcAddress.Trim().TrimStart('D', 'd');
            return int.TryParse(addr, out int address) ? address : 0;
        }

        /// <summary>反射取对象属性值，属性不存在返回 null</summary>
        public static string GetPropValue(object obj, string propName)
        {
            if (obj == null || string.IsNullOrEmpty(propName)) return null;
            var prop = obj.GetType().GetProperty(propName);
            if (prop == null) return null;
            return prop.GetValue(obj)?.ToString();
        }

        /// <summary>
        /// 取 DynamicParams 自定义字段值，字段不存在返回 null
        /// 用于在不修改 MesdataDLS 类的情况下扩展配置字段
        /// </summary>
        public static string GetDynamicValue(MesdataDLS mes, string key)
        {
            if (mes?.DynamicParams == null || string.IsNullOrEmpty(key)) return null;
            return mes.DynamicParams.TryGetValue(key, out string value) ? value : null;
        }

        /// <summary>从PLC读值：parts[1]=地址(如D900)，parts[2]=数据类型(Int/Float/String)，parts[3]=字符串字节数</summary>
        public static string GetPlcValue(string[] parts)
        {
            if (parts == null || parts.Length < 3 || !ModbusClass.isConnected()) return "";
            int addr = ParsePlcAddress(parts[1]);
            string dataType = parts[2].Trim().ToUpperInvariant();
            try
            {
                switch (dataType)
                {
                    case "INT":
                        int[] regs = ModbusClass.ReadHoldingRegisters(addr, 1);
                        return (regs != null && regs.Length > 0) ? regs[0].ToString() : "";
                    case "FLOAT":
                        return ModbusClass.ReadDoubleRegister(addr).ToString("f3");
                    case "STRING":
                        int byteLen = parts.Length > 3 ? (int.TryParse(parts[3], out int len) ? len : 20) : 20;
                        return ModbusClass.ReadString(addr, byteLen)?.Trim() ?? "";
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 按 Source 配置取值
        /// </summary>
        /// <param name="source">来源配置字符串</param>
        /// <param name="mes">MES数据对象（MesdataDLS）</param>
        /// <param name="recipe">配方数据对象（RecipeData）</param>
        /// <param name="scannerValue">扫码枪当前条码</param>
        /// <param name="visionValue">视觉结果</param>
        public static string Resolve(string source, MesdataDLS mes, RecipeData recipe, string scannerValue, string visionValue)
        {
            if (string.IsNullOrEmpty(source)) return "";
            string[] parts = source.Split(':');
            string type = parts[0].Trim();

            switch (type.ToUpperInvariant())
            {
                case "RECIPE":
                    return GetPropValue(recipe, parts.Length > 1 ? parts[1].Trim() : "");
                case "MES":
                    {
                        string key = parts.Length > 1 ? parts[1].Trim() : "";
                        //先取 MesdataDLS 属性，取不到再兜底 DynamicParams 自定义字段
                        return GetPropValue(mes, key) ?? GetDynamicValue(mes, key) ?? "";
                    }
                case "DYNAMIC":
                    //自定义字段：直接取 DynamicParams
                    return GetDynamicValue(mes, parts.Length > 1 ? parts[1].Trim() : "") ?? "";
                case "PLC":
                    return GetPlcValue(parts);
                case "SCANNER":
                    return scannerValue ?? "";
                case "VISION":
                    return visionValue ?? "";
                case "CONST":
                    //固定字符串：Const:后面的内容原样返回
                    return parts.Length > 1 ? source.Substring(source.IndexOf(':') + 1) : "";
                default:
                    //无前缀旧格式：先配方属性、再MES数据属性，最后兜底 DynamicParams 自定义字段
                    string v = GetPropValue(recipe, source);
                    if (v != null) return v;
                    v = GetPropValue(mes, source);
                    if (v != null) return v;
                    return GetDynamicValue(mes, source) ?? "";
            }
        }

        /// <summary>
        /// 批量读取PLC浮点数（32位，占2个寄存器），按地址排序后合并连续段，减少Modbus请求次数
        /// </summary>
        /// <param name="items">键(列标识) + PLC地址 的列表</param>
        public static Dictionary<string, double> ReadFloatBatch(List<KeyValuePair<string, int>> items)
        {
            var result = new Dictionary<string, double>();
            if (items == null || items.Count == 0 || !ModbusClass.isConnected()) return result;

            var sorted = items.Where(x => !string.IsNullOrEmpty(x.Key)).OrderBy(x => x.Value).ToList();
            int i = 0;
            while (i < sorted.Count)
            {
                int startAddr = sorted[i].Value;
                int endAddr = startAddr + 2; //double占2个寄存器
                int j = i + 1;
                //合并地址连续的段（单段不超过125个寄存器上限）
                while (j < sorted.Count && sorted[j].Value <= endAddr && sorted[j].Value + 2 - startAddr <= 125)
                {
                    endAddr = Math.Max(endAddr, sorted[j].Value + 2);
                    j++;
                }
                int[] regs = ModbusClass.ReadHoldingRegisters(startAddr, endAddr - startAddr);
                if (regs != null)
                {
                    for (int k = i; k < j; k++)
                    {
                        int offset = sorted[k].Value - startAddr;
                        if (offset + 1 < regs.Length)
                        {
                            float fv = ModbusClass.CombineToFloat(regs[offset], regs[offset + 1]);
                            result[sorted[k].Key] = Math.Round(fv, 3);
                        }
                    }
                }
                i = j;
            }
            return result;
        }
    }
}