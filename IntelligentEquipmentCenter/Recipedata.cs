using IntelligentEquipmentCenter.UiForm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TF.File;
using static IntelligentEquipmentCenter.ClientJson;

namespace IntelligentEquipmentCenter
{
    public  class Recipedata
    {
        //当前产品保存路径
        private static string PathConfig = Environment.CurrentDirectory + "\\xml\\Recipe\\Config.ini";
        //产品保存基础路径
        private static string PathBase = Environment.CurrentDirectory + "\\xml\\Recipe\\Default.xml";

        private static string curRecipe = "";
        //工站参数
        public static RecipeData Data = new RecipeData();

        //工站参数
        public static Barcode BarData = new Barcode();

        //产品列表
        private static List<string> lstRecipe = new List<string>();

        /// <summary>
        /// 获取当前产品
        /// </summary>
        public static string GetCur
        {
            get => curRecipe;
        }

        /// <summary>
        /// 获取当前产品列表
        /// </summary>
        public static string[] GetList
        {
            get => lstRecipe.ToArray();
        }
        /// <summary>
        /// 加载当前产品及产品列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool LoadRecipe(out string msg)
        {
            if (!System.IO.File.Exists(PathConfig))
            {
                msg = "文件不存在";
                return false;
            }

            string buf = Ini.Read("产品参数", "当前产品", "", PathConfig, out msg);
            if (buf == "")
            {
                return false;
            }

            lstRecipe.Clear();
            string[] key = Ini.GetAllItemKeys(PathConfig, "产品列表", out msg);
            if (key == null)
            {
                return false;
            }

            curRecipe = buf;
            lstRecipe.AddRange(key);
            return true;
        }

        /// <summary>
        /// 保存当前产品及产品列表
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool SaveRecipe(out string msg)
        {
            if (!Ini.Write("产品参数", "当前产品", curRecipe, $"{PathConfig}.tmp", out msg))
            {
                return false;
            }
            Ini.DeleteSection($"{PathConfig}.tmp", "产品列表", out msg);
            bool res = true;
            foreach (var t in lstRecipe)
            {
                if (!Ini.Write("产品列表", t, "", $"{PathConfig}.tmp", out msg))
                {
                    return false;
                }
            }
            if (!Ini.GetAllSectionItems($"{PathConfig}.tmp", "产品列表").Any() || "" == Ini.Read("产品参数", "当前产品", "", $"{PathConfig}.tmp", out msg))
            {
                res = false;
            }
            else
            {
                ExFile.CopyFile($"{PathConfig}.tmp", PathConfig, true);
            }
            if (res)
            {
                ExFile.CopyFile(PathConfig, PathConfig.Replace("Config", "Config~"));
            }

            return res;
        }

        /// <summary>
        /// 加载当前产品数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool LoadData(out string msg)
        {
            return LoadData(curRecipe, out msg);
        }

        /// <summary>
        /// 通过名称加载产品
        /// </summary>
        /// <param name="name"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool LoadData(string name, out string msg)
        {
            if (!lstRecipe.Contains(name))
            {
                msg = "产品名称不存在";
                return false;
            }

            RecipeData __data = new RecipeData();
            msg = Xml.Deserialize(PathBase.Replace("Default", name), ref __data);
            if (__data is null)
            {
                return false;
            }

            Data = __data;
            ;
            if (msg != "Ok")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Load(out string msg)
        {
            msg = "当前产品名为空";
            if (LoadRecipe(out msg))
            {
                if (LoadData(out msg))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Save(out string msg)
        {
            msg = "当前产品名为空";

            if (curRecipe != "" && SaveRecipe(out msg))
            {
                msg = Xml.Serialize(PathBase.Replace("Default", curRecipe), Data);
                if (msg != "Ok")
                {
                    return false;
                }
                msg = Xml.Serialize(PathBase.Replace("Default", curRecipe + "~"), Data);
                /*
                 * 备份
                 */
                System.IO.File.WriteAllText(PathBase + "json", JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented));
                if (msg != "Ok")
                {
                    msg = "备份" + msg;
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 增加产品
        /// </summary>
        /// <param name="name">产品名称</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Add(string name, out string msg)
        {
            msg = "Ok";
            if (lstRecipe.Contains(name))
            {
                msg = $"{name}产品已存在";
                return false;
            }

            curRecipe = name;
            lstRecipe.Add(name);
            return true;
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="name">产品名称</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Del(string name, out string msg, bool delFiel = true)
        {
            msg = "Ok";
            //删除产品文件？
            if (delFiel)
            {
                if (!ExFile.DeleteFile(PathBase.Replace("Default", name)))
                {
                    msg = $"删除{name}产品文件失败";
                    return false;
                }
            }

            return lstRecipe.Remove(name);
        }

        /// <summary>
        /// 删除当前产品
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="delFiel">是否删除产品文件</param>
        /// <returns></returns>
        public static bool Del(out string msg, bool delFiel = true)
        {
            msg = "Ok";
            //删除产品文件？
            if (delFiel)
            {
                if (!ExFile.DeleteFile(PathBase.Replace("Default", curRecipe)))
                {
                    msg = $"删除{curRecipe}产品文件失败";
                    return false;
                }
            }

            return lstRecipe.Remove(curRecipe);
        }

        /// <summary>
        /// 改变当前产品，同时切换产品文件
        /// </summary>
        /// <param name="name">产品名称</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Change(string name, out string msg)
        {
            if (name == curRecipe)
            {
                msg = $"{name}已是当前产品";
                return true;
            }

            if (!lstRecipe.Contains(name))
            {
                msg = $"{name}产品不存在";
                return false;
            }
            curRecipe = name;
            if (!SaveRecipe(out msg))
            {
                msg = $"保存{name}产品异常：{msg}";
                return false;
            }
            if (!LoadData(name, out msg))
            {
                return false;
            }
            return true;
        }
    }

    //配方数据
    public class RecipeData
    {
        /// <summary>
        /// 配方名称
        /// </summary>
        public string RecipeName { get; set; }

        /// <summary>
        /// 产线名称
        /// </summary>
        public string ProductionName { get; set; }
        /// <summary>
        /// 工位名称
        /// </summary>
        public string WorkpositionName { get; set; }

        /// <summary>
        /// 工装编号
        /// </summary>
        public string ToolingNum { get; set; }

        /// <summary>
        /// 动态检测参数列表（名字/上限/下限/PLC地址/单位）
        /// </summary>
        public List<RecipeParameter> Parameters { get; set; } = new List<RecipeParameter>();

        // ===== 以下为兼容旧字段，从 Parameters 映射 =====
        // 约定索引：0=螺丝浮高(FatTime)，1=螺丝扭矩(FatSpeed)，2=电批旋转角度(FatArea)

        private RecipeParameter P(int i) =>
            (Parameters != null && Parameters.Count > i) ? Parameters[i] : new RecipeParameter();

        /// <summary>螺丝浮高检测上限</summary>
        [System.Xml.Serialization.XmlIgnore]
        public float FatTimeUp { get => P(0).UpLimit; set => P(0).UpLimit = value; }
        /// <summary>螺丝浮高检测下限</summary>
        [System.Xml.Serialization.XmlIgnore]
        public float FatTimedown { get => P(0).DownLimit; set => P(0).DownLimit = value; }
        /// <summary>螺丝浮高实际值PLC地址</summary>
        [System.Xml.Serialization.XmlIgnore]
        public string FatTimePlcAddress { get => P(0).PlcAddress; set => P(0).PlcAddress = value; }

        /// <summary>螺丝扭矩检测上限</summary>
        [System.Xml.Serialization.XmlIgnore]
        public float FatSpeedUp { get => P(1).UpLimit; set => P(1).UpLimit = value; }
        /// <summary>螺丝扭矩检测下限</summary>
        [System.Xml.Serialization.XmlIgnore]
        public float FatSpeeddown { get => P(1).DownLimit; set => P(1).DownLimit = value; }
        /// <summary>螺丝扭矩实际值PLC地址</summary>
        [System.Xml.Serialization.XmlIgnore]
        public string FatSpeedPlcAddress { get => P(1).PlcAddress; set => P(1).PlcAddress = value; }

        /// <summary>电批旋转角度检测上限</summary>
        [System.Xml.Serialization.XmlIgnore]
        public float FatAreaUp { get => P(2).UpLimit; set => P(2).UpLimit = value; }
        /// <summary>电批旋转角度检测下限</summary>
        [System.Xml.Serialization.XmlIgnore]
        public float FatAreadown { get => P(2).DownLimit; set => P(2).DownLimit = value; }
        /// <summary>电批旋转角度实际值PLC地址</summary>
        [System.Xml.Serialization.XmlIgnore]
        public string FatAreaPlcAddress { get => P(2).PlcAddress; set => P(2).PlcAddress = value; }
    }

    /// <summary>
    /// 配方检测参数（一行）
    /// </summary>
    public class RecipeParameter
    {
        /// <summary>参数名字，如"螺丝浮高"</summary>
        public string Name { get; set; } = "";
        /// <summary>上限值</summary>
        public float UpLimit { get; set; } = 0;
        /// <summary>下限值</summary>
        public float DownLimit { get; set; } = 0;
        /// <summary>实际值PLC地址，如 D2604</summary>
        public string PlcAddress { get; set; } = "";
        /// <summary>PLC地址数据类型：int / float / string</summary>
        public string DataType { get; set; } = "float";
        /// <summary>字符串类型时从PLC读取的字节数（仅 DataType=string 时有效，默认20）</summary>
        public int StringLength { get; set; } = 20;
        /// <summary>结果PLC地址（仅无上下限参数生效，只读）：PC从该地址读外部写回的判定结果（int：1=OK，其余=NG）；有上下限参数时该字段被忽略</summary>
        public string ResultPlcAddress { get; set; } = "";
        /// <summary>是否需要上下限判定。true=上下限判定（默认）；false=只看结果，结果由外部写入结果地址，PC读取</summary>
        public bool NeedLimit { get; set; } = true;
        /// <summary>单位，如 mm / N.m / R</summary>
        public string Unit { get; set; } = "";
        /// <summary>是否在主页表格显示（实际值+结果两列）</summary>
        public bool ShowOnUI { get; set; } = true;
    }






    //MES请求上传的配方号和工单号

    public class ToggleRecipe
    {
        /// <summary>
        /// 产线名称
        /// </summary>
        public string lineName { get; set; }
        //
        /// <summary>
        /// 工单号
        /// </summary>
        public string workOrderNumber { get; set; }
        /// <summary>
        /// 工装编码
        /// </summary>
        public string toolingCode { get; set; }
        /// <summary>
        /// 配方名称
        /// </summary>
        public string formulaName { get; set; }
    }

}
