using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace ClsComm
{
    public enum FuncCode
    {
        读线圈 = 1,
        读离散量,
        读保持寄存器,
        读输入寄存器,
        写单个线圈,
        写单个寄存器,

        写多个线圈 = 15,
        写多个寄存器
    }
    //地址码 功能码 起始地址  读取个数  校验码
    public class ModbusRTU
    {
        /// <summary>
        /// 获取读取命令
        /// </summary>
        /// <param name="station">站号</param>
        /// <param name="funcode">功能码</param>
        /// <param name="addr">寄存器起始地址</param>
        /// <param name="lenght">读取数据个数</param>
        /// <returns></returns>
        public static byte[] GetReadCmd(byte station, FuncCode funcode, ushort addr, ushort lenght)
        {        
            //拼接报文
            List<byte> cmd = new List<byte>();

            cmd.Add(station);
            cmd.Add((byte)funcode);
            //BitConvert默认是 小端 显示，也就是说它的计算是按照CDAB计算的
            cmd.Add((BitConverter.GetBytes(addr)[1]));    //SendCommand.Add((byte)(iStartAdd / 256));   // 高位
            cmd.Add((BitConverter.GetBytes(addr)[0]));    //SendCommand.Add((byte)(iStartAdd % 256));   // 低位
            //BitConvert默认是 小端 显示，也就是说它的计算是按照CDAB计算的    
            cmd.Add((BitConverter.GetBytes(lenght)[1]));      //SendCommand.Add((byte)(iLength / 256));     // 高位
            cmd.Add((BitConverter.GetBytes(lenght)[0]));      //SendCommand.Add((byte)(iLength % 256));     // 低位

            byte[] CRC = Comm.CRC16(cmd.ToArray());
            cmd.AddRange(CRC);

            return cmd.ToArray();
        }

        /// <summary>
        /// 写单个线圈/单个寄存器
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetWriteSingle(byte station, FuncCode funcode, ushort addr, object value)
        {
            //拼接报文
            List<byte> cmd = new List<byte>();
            cmd.Add(station);
            cmd.Add((byte)funcode);
            cmd.Add((BitConverter.GetBytes(addr)[1]));    //SendCommand.Add((byte)(iStartAdd / 256));    // 高位
            cmd.Add((BitConverter.GetBytes(addr)[0]));    //SendCommand.Add((byte)(iStartAdd % 256));    // 低位
            if (funcode == FuncCode.写单个线圈)
            {
                cmd.Add((byte)((bool)value ? 0xFF : 0x00));//固定写法：FF 00表示闭合 00 00表示断开，其他数值非法
                cmd.Add(0x00);
            }
            else if (funcode == FuncCode.写单个寄存器)
            {
                cmd.Add(BitConverter.GetBytes((ushort)value)[1]);    // 高位
                cmd.Add(BitConverter.GetBytes((ushort)value)[0]);    // 低位  
            }
            else
                return new byte[] { };

            byte[] CRC = Comm.CRC16(cmd.ToArray());
            cmd.AddRange(CRC);

            return cmd.ToArray();
        }

        /// <summary>
        /// 获取写入多个命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="lenght"></param>
        /// <param name="byteLength"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetWriteMultiple(byte station, FuncCode funcode, ushort addr, ushort lenght, ushort byteLength, byte[] check = null)
        {
            //拼接报文
            List<byte> cmd = new List<byte>();
            cmd.Add(station);
            cmd.Add((byte)funcode);
            cmd.Add((BitConverter.GetBytes(addr)[1]));   //SendCommand.Add((byte)(iStartAdd / 256));    // 高位
            cmd.Add((BitConverter.GetBytes(addr)[0]));   //SendCommand.Add((byte)(iStartAdd % 256));    // 低位
            cmd.Add((BitConverter.GetBytes(lenght)[1]));     //SendCommand.Add((byte)(iLength / 256));      // 高位
            cmd.Add((BitConverter.GetBytes(lenght)[0]));     //SendCommand.Add((byte)(iLength % 256));      // 低位
            cmd.Add((BitConverter.GetBytes(byteLength)[0]));
            return cmd.ToArray();
        }

        /// <summary>
        /// 写入多个线圈命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="iLength"></param>
        /// <param name="values"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetWriteCoils(byte station, FuncCode funcode, ushort addr, ushort iLength, bool[] values, byte[] check = null)
        {
            funcode = FuncCode.写多个线圈;
            int byteLength = GetByteLength((byte)funcode, iLength);

            List<byte> cmd = GetWriteMultiple(station, funcode, addr, iLength, (ushort)byteLength).ToList();

            byte[] dataByte = GetByteFromBits(values);
            for (int i = 0; i < byteLength; i++)
            {
                cmd.Add(dataByte[i]);
            }

            byte[] CRC = Comm.CRC16(cmd.ToArray());
            cmd.AddRange(CRC);
            return cmd.ToArray();
        }

        /// <summary>
        /// 写入多个单字寄存器命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="iLength"></param>
        /// <param name="values"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetWriteRegisters(byte station, FuncCode funcode, ushort addr, ushort iLength, ushort[] values, byte[] check = null)
        {
            funcode = FuncCode.写多个寄存器;

            int byteLength = GetByteLength((byte)funcode, iLength);

            List<byte> cmd = GetWriteMultiple(station, funcode, addr, iLength, (ushort)byteLength).ToList();

            for (int i = 0; i < iLength; i++)
            {
                cmd.Add((BitConverter.GetBytes(values[i]))[1]);
                cmd.Add((BitConverter.GetBytes(values[i]))[0]);
            }

            byte[] CRC = Comm.CRC16(cmd.ToArray());
            cmd.AddRange(CRC);
            return cmd.ToArray();
        }

        /// <summary>
        /// 根据请求的数据个数计算出数据字节的长度
        /// 读线圈时一个字节表示8个bool，读寄存器时两个字节表示一个short
        /// 需要用线圈或寄存器的个数计算数据字节数
        /// </summary>
        /// <param name="iFuncCode"></param>
        /// <param name="iLength"></param>
        /// <returns></returns>
        public static ushort GetByteLength(byte iFuncCode, ushort iLength)
        {
            switch (iFuncCode)
            {
                case 0x01:
                    return (ushort)(iLength % 8 == 0 ? iLength / 8 : iLength / 8 + 1);
                case 0x02:
                    return (ushort)(iLength % 8 == 0 ? iLength / 8 : iLength / 8 + 1);
                case 0x03:
                    return (ushort)(iLength * 2);
                case 0x04:
                    return (ushort)(iLength * 2);
                case 0x0F:
                    return (ushort)(iLength % 8 == 0 ? iLength / 8 : iLength / 8 + 1);
                case 0x10:
                    return (ushort)(iLength * 2);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 将一个列表中的 bool 值按照每 8 个 bool 一个字节转换为字节数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="reverse">true，表示转换格式为ABCD 顺序；false，表示转换格式为 DCBA 倒序</param>
        /// <returns></returns>
        public static byte[] GetByteFromBits(bool[] source, bool reverse = false)
        {
            int Length = source.Length;
            //字节的个数
            int byteLength = Length % 8 == 0 ? Length / 8 : Length / 8 + 1;

            List<bool> sourcrList = source.ToList<bool>();
            //数组不满8的整数倍，后几位填充false
            if (Length % 8 != 0)
            {
                for (int i = 0; i < byteLength * 8 - Length; i++)
                {
                    sourcrList.Add(false);
                }
            }
            byte[] resByte = new byte[byteLength];
            for (int i = 0; i < byteLength; i++)
            {
                bool[] tempArry = new bool[8];
                Array.Copy(sourcrList.ToArray(), i * 8, tempArry, 0, 8);
                resByte[i] = GetByteFrom8Bits(tempArry, reverse);
            }

            return resByte;
        }

        /// <summary>
        /// 将 8 个bool值转换为一个字节
        /// </summary>
        /// <param name="source"></param>
        /// <param name="reverse">reverse = false 将 source 数组倒序；reverse = true 将source数组顺序 或 保持当前不变</param>
        /// <returns></returns>
        public static byte GetByteFrom8Bits(bool[] source, bool reverse = false)
        {
            byte res = 0;
            if (!reverse)
            {
                Array.Reverse(source);
            }
            for (int i = 0; i < 8; i++)
            {
                res <<= 1;
                if (source[i])
                {
                    res += Convert.ToByte(source[i]);
                }
            }
            return res;
        }
    }

    public class ModbusTCP
    {
        /// <summary>
        /// 获取随机校验头
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static byte[] GetCheckHead(int seed)
        {
            var random = new Random(DateTime.Now.Millisecond + seed);
            return new byte[] { (byte)random.Next(255), (byte)random.Next(255) };
        }

        /// <summary>
        /// 获取读取命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="lenght"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetReadCmd(byte station, FuncCode funcode, ushort addr, ushort lenght, byte[] check = null)
        {
            byte[] cmd = new byte[12];

            cmd[0] = (check?[0] ?? 0x19);
            cmd[1] = (check?[1] ?? 0xB2);//Client发出的检验信息
            cmd[2] = (0x00);
            cmd[3] = (0x00);//表示tcp/ip 的协议的modbus的协议
            cmd[4] = (0x00);
            cmd[5] = (0x06);//表示的是该字节以后的字节长度

            cmd[6] = (station);
            cmd[7] = ((byte)funcode);
            cmd[8] = (BitConverter.GetBytes(addr)[1]);
            cmd[9] = (BitConverter.GetBytes(addr)[0]);//寄存器地址
            cmd[10] = (BitConverter.GetBytes(lenght)[1]);
            cmd[11] = (BitConverter.GetBytes(lenght)[0]);//表示request 寄存器的长度(寄存器个数)
            return cmd.ToArray();
        }

        /// <summary>
        /// 获取写入单个命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="value"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetWriteSingle(byte station, FuncCode funcode, ushort addr, object value, byte[] check = null)
        {
            byte[] cmd = new byte[12];
            cmd[0] = check?[0] ?? 0x19;
            cmd[1] = check?[1] ?? 0xB2;//Client发出的检验信息 
            cmd[2] = (0x00);
            cmd[3] = (0x00);//表示tcp/ip 的协议的modbus的协议
            cmd[4] = (0x00);
            cmd[5] = (0x06);//表示的是该字节以后的字节长度

            cmd[6] = station;//站号
            cmd[7] = ((byte)funcode); //功能码
            cmd[8] = BitConverter.GetBytes(addr)[1];
            cmd[9] = BitConverter.GetBytes(addr)[0];//寄存器地址

            if (funcode == FuncCode.写单个线圈)
            {
                cmd[10] = (byte)((bool)value ? 0xFF : 0x00);     //此处只可以是FF表示闭合00表示断开，其他数值非法
                cmd[11] = 0x00;
            }
            if (funcode == FuncCode.写单个寄存器)
            {
                cmd[10] = (BitConverter.GetBytes((ushort)value)[1]);    // 高位
                cmd[11] = (BitConverter.GetBytes((ushort)value)[0]);    // 低位
            }
            return cmd;
        }
        /// <summary>
        /// 获取写入多个命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="lenght"></param>
        /// <param name="byteLength"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetWriteMultiple(byte station, FuncCode funcode, ushort addr, ushort lenght, ushort byteLength, byte[] check = null)
        {
            byte[] SendCommand = new byte[13];
            SendCommand[0] = check?[0] ?? 0x19;
            SendCommand[1] = check?[1] ?? 0xB2;//检验信息，用来验证response是否串数据了           
            SendCommand[2] = (0x00);
            SendCommand[3] = (0x00);//表示tcp/ip 的协议的modbus的协议
            SendCommand[4] = BitConverter.GetBytes(7 + byteLength)[1];
            SendCommand[5] = BitConverter.GetBytes(7 + byteLength)[0];

            SendCommand[6] = station;//站号
            SendCommand[7] = (byte)funcode; //功能码
            SendCommand[8] = BitConverter.GetBytes(addr)[1];
            SendCommand[9] = BitConverter.GetBytes(addr)[0];//寄存器地址
            SendCommand[10] = BitConverter.GetBytes(lenght)[1]; ;
            SendCommand[11] = BitConverter.GetBytes(lenght)[0]; ;
            SendCommand[12] = (byte)(byteLength);

            return SendCommand;
        }

        /// <summary>
        /// 获取写入多个线圈命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="iLength"></param>
        /// <param name="values"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetWriteCoils(byte station, FuncCode funcode, ushort addr, ushort iLength, bool[] values, byte[] check = null)
        {
            int byteLength = GetByteLength((byte)funcode, iLength);

            List<byte> cmd = GetWriteMultiple(station, funcode, addr, iLength, (ushort)byteLength).ToList();

            byte[] dataByte =GetByteFromBits(values);
            for (int i = 0; i < byteLength; i++)
            {
                cmd.Add(dataByte[i]);
            }
            return cmd.ToArray();
        }

        /// <summary>
        /// 获取写入多个寄存器命令
        /// </summary>
        /// <param name="station"></param>
        /// <param name="funcode"></param>
        /// <param name="addr"></param>
        /// <param name="iLength"></param>
        /// <param name="values"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static byte[] GetWriteRegisters(byte station, FuncCode funcode, ushort addr, ushort iLength, ushort[] values, byte[] check = null)
        {
            int byteLength = GetByteLength((byte)funcode, iLength);

            List<byte> cmd  = GetWriteMultiple(station, funcode, addr, iLength, (ushort)byteLength).ToList();

            for (int i = 0; i < iLength; i++)
            {
                cmd.Add((BitConverter.GetBytes(values[i]))[1]);
                cmd.Add((BitConverter.GetBytes(values[i]))[0]);
            }
            return cmd.ToArray();
        }


        /// <summary>
        /// 根据请求的数据个数计算出数据字节的长度
        /// 读线圈时一个字节表示8个bool，读寄存器时两个字节表示一个short
        /// 需要用线圈或寄存器的个数计算数据字节数
        /// </summary>
        /// <param name="iFuncCode"></param>
        /// <param name="iLength"></param>
        /// <returns></returns>
        public static ushort GetByteLength(byte iFuncCode, ushort iLength)
        {
            switch (iFuncCode)
            {
                case 0x01:
                    return (ushort)(iLength % 8 == 0 ? iLength / 8 : iLength / 8 + 1);
                case 0x02:
                    return (ushort)(iLength % 8 == 0 ? iLength / 8 : iLength / 8 + 1);
                case 0x03:
                    return (ushort)(iLength * 2);
                case 0x04:
                    return (ushort)(iLength * 2);
                case 0x0F:
                    return (ushort)(iLength % 8 == 0 ? iLength / 8 : iLength / 8 + 1);
                case 0x10:
                    return (ushort)(iLength * 2);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 将一个列表中的 bool 值按照每 8 个 bool 一个字节转换为字节数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="reverse">true，表示转换格式为ABCD 顺序；false，表示转换格式为 DCBA 倒序</param>
        /// <returns></returns>
        public static byte[] GetByteFromBits(bool[] source, bool reverse = false)
        {
            int Length = source.Length;
            //字节的个数
            int byteLength = Length % 8 == 0 ? Length / 8 : Length / 8 + 1;

            List<bool> sourcrList = source.ToList<bool>();
            //数组不满8的整数倍，后几位填充false
            if (Length % 8 != 0)
            {
                for (int i = 0; i < byteLength * 8 - Length; i++)
                {
                    sourcrList.Add(false);
                }
            }
            byte[] resByte = new byte[byteLength];
            for (int i = 0; i < byteLength; i++)
            {
                bool[] tempArry = new bool[8];
                Array.Copy(sourcrList.ToArray(), i * 8, tempArry, 0, 8);
                resByte[i] = GetByteFrom8Bits(tempArry, reverse);
            }

            return resByte;
        }

        /// <summary>
        /// 将 8 个bool值转换为一个字节
        /// </summary>
        /// <param name="source"></param>
        /// <param name="reverse">reverse = false 将 source 数组倒序；reverse = true 将source数组顺序 或 保持当前不变</param>
        /// <returns></returns>
        public static byte GetByteFrom8Bits(bool[] source, bool reverse = false)
        {
            byte res = 0;
            if (!reverse)
            {
                Array.Reverse(source);
            }
            for (int i = 0; i < 8; i++)
            {
                res <<= 1;
                if (source[i])
                {
                    res += Convert.ToByte(source[i]);
                }
            }
            return res;
        }
    }
}
