using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class ModbusRtu
    {
        private SerialPort MyCom;

        private byte ucCRCHi = byte.MaxValue;

        private byte ucCRCLo = byte.MaxValue;

        private int CurrentAddr;

       /// <summary>
        private SimpleHybirdLock InteractiveLock;
       /// </summary>

        private static readonly byte[] aucCRCHi = new byte[256]
        {
            0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
            128, 65, 0, 193, 129, 64, 1, 192, 128, 65,
            0, 193, 129, 64, 0, 193, 129, 64, 1, 192,
            128, 65, 1, 192, 128, 65, 0, 193, 129, 64,
            0, 193, 129, 64, 1, 192, 128, 65, 0, 193,
            129, 64, 1, 192, 128, 65, 1, 192, 128, 65,
            0, 193, 129, 64, 1, 192, 128, 65, 0, 193,
            129, 64, 0, 193, 129, 64, 1, 192, 128, 65,
            0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
            128, 65, 0, 193, 129, 64, 0, 193, 129, 64,
            1, 192, 128, 65, 1, 192, 128, 65, 0, 193,
            129, 64, 1, 192, 128, 65, 0, 193, 129, 64,
            0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
            128, 65, 0, 193, 129, 64, 0, 193, 129, 64,
            1, 192, 128, 65, 0, 193, 129, 64, 1, 192,
            128, 65, 1, 192, 128, 65, 0, 193, 129, 64,
            0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
            128, 65, 0, 193, 129, 64, 1, 192, 128, 65,
            0, 193, 129, 64, 0, 193, 129, 64, 1, 192,
            128, 65, 0, 193, 129, 64, 1, 192, 128, 65,
            1, 192, 128, 65, 0, 193, 129, 64, 1, 192,
            128, 65, 0, 193, 129, 64, 0, 193, 129, 64,
            1, 192, 128, 65, 1, 192, 128, 65, 0, 193,
            129, 64, 0, 193, 129, 64, 1, 192, 128, 65,
            0, 193, 129, 64, 1, 192, 128, 65, 1, 192,
            128, 65, 0, 193, 129, 64
        };

        private static readonly byte[] aucCRCLo = new byte[256]
        {
            0, 192, 193, 1, 195, 3, 2, 194, 198, 6,
            7, 199, 5, 197, 196, 4, 204, 12, 13, 205,
            15, 207, 206, 14, 10, 202, 203, 11, 201, 9,
            8, 200, 216, 24, 25, 217, 27, 219, 218, 26,
            30, 222, 223, 31, 221, 29, 28, 220, 20, 212,
            213, 21, 215, 23, 22, 214, 210, 18, 19, 211,
            17, 209, 208, 16, 240, 48, 49, 241, 51, 243,
            242, 50, 54, 246, 247, 55, 245, 53, 52, 244,
            60, 252, 253, 61, 255, 63, 62, 254, 250, 58,
            59, 251, 57, 249, 248, 56, 40, 232, 233, 41,
            235, 43, 42, 234, 238, 46, 47, 239, 45, 237,
            236, 44, 228, 36, 37, 229, 39, 231, 230, 38,
            34, 226, 227, 35, 225, 33, 32, 224, 160, 96,
            97, 161, 99, 163, 162, 98, 102, 166, 167, 103,
            165, 101, 100, 164, 108, 172, 173, 109, 175, 111,
            110, 174, 170, 106, 107, 171, 105, 169, 168, 104,
            120, 184, 185, 121, 187, 123, 122, 186, 190, 126,
            127, 191, 125, 189, 188, 124, 180, 116, 117, 181,
            119, 183, 182, 118, 114, 178, 179, 115, 177, 113,
            112, 176, 80, 144, 145, 81, 147, 83, 82, 146,
            150, 86, 87, 151, 85, 149, 148, 84, 156, 92,
            93, 157, 95, 159, 158, 94, 90, 154, 155, 91,
            153, 89, 88, 152, 136, 72, 73, 137, 75, 139,
            138, 74, 78, 142, 143, 79, 141, 77, 76, 140,
            68, 132, 133, 69, 135, 71, 70, 134, 130, 66,
            67, 131, 65, 129, 128, 64
        };

        public int SendTimeOut { get; set; } = 2000;


        public int ReceiveTimeOut { get; set; } = 2000;


        public DataFormat DataFormat { get; set; } = DataFormat.ABCD;


        public ModbusRtu()
        {
            MyCom = new SerialPort();
            InteractiveLock = new SimpleHybirdLock();
        }

        public ModbusRtu(DataFormat dataFormat)
        {
            MyCom = new SerialPort();
            InteractiveLock = new SimpleHybirdLock();
            DataFormat = dataFormat;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="iBaudRate">波特率</param>
        /// <param name="iPortNo">端口号</param>
        /// <param name="iDataBits">数据位</param>
        /// <param name="iParity">校验位</param>
        /// <param name="iStopBits">停止位</param>
        /// <returns></returns>
        public bool OpenMyComm(int iBaudRate, string iPortNo, int iDataBits, Parity iParity, StopBits iStopBits)
        {
            try
            {
                if (MyCom.IsOpen)
                {
                    MyCom.Close();
                }

                MyCom.BaudRate = iBaudRate;
                MyCom.PortName = iPortNo;
                MyCom.DataBits = iDataBits;
                MyCom.Parity = iParity;
                MyCom.StopBits = iStopBits;
                MyCom.ReadTimeout = ReceiveTimeOut;
                MyCom.WriteTimeout = SendTimeOut;
                MyCom.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool ClosePort()
        {
            if (MyCom.IsOpen)
            {
                MyCom.Close();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 读取word数据，输出寄存器4x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <param DataFormat="dataFormat">格式</param>
        /// <returns></returns>
        public UInt16 ReadUIntKeepReg(int iDevAdd, int iAddress, int iLength, DataFormat dataFormat)
        {
            byte[] result = ReadKeepReg(iDevAdd, iAddress, iLength);
            string wordvalue = string.Empty;
            if (result != null && result.Length == iLength * 2)
            {
                for (int i = 0; i < result.Length; i += 2)
                {
                    wordvalue += BitConverter.ToUInt16(ModbusRtu.GetIntByteArray(result, i, dataFormat), 0).ToString() + " ";
                }
                return Convert.ToUInt16(wordvalue);
            }
            return 0;
        }

        /// <summary>
        /// 读取int16数据，输出寄存器4x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <param DataFormat="dataFormat">格式</param>
        /// <returns></returns>
        public Int16 ReadInt16KeepReg(int iDevAdd, int iAddress, int iLength, DataFormat dataFormat)
        {
            byte[] result = ReadKeepReg(iDevAdd, iAddress, iLength);
            string intvalue = string.Empty;
            if (result != null && result.Length == iLength * 2)
            {
                for (int i = 0; i < result.Length; i += 2)
                {
                    intvalue += BitConverter.ToInt16(GetIntByteArray(result, i, dataFormat), 0).ToString() + " ";
                }
                return Convert.ToInt16(intvalue);
            }
            return 0;
        }


        /// <summary>
        /// 读取int16数据，输出寄存器4x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <param DataFormat="dataFormat">格式</param>
        /// <returns></returns>
        public Int32 ReadInt32KeepReg(int iDevAdd, int iAddress, int iLength, DataFormat dataFormat)
        {
            byte[] result = ReadKeepReg(iDevAdd, iAddress, iLength*2);
            string dintvalue = string.Empty;
            if (result != null && result.Length == iLength * 4)
            {
                for (int i = 0; i < result.Length; i += 4)
                {
                    dintvalue += BitConverter.ToInt32(GetSingleByteArray(result, i, dataFormat), 0).ToString() + " ";
                }
                return Convert.ToInt32(dintvalue);
            }
            return 0;
        }

        /// <summary>
        /// 读取float数据，输出寄存器4x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <param DataFormat="dataFormat">格式</param>
        /// <returns></returns>
        public float ReadfloatKeepReg(int iDevAdd, int iAddress, int iLength, DataFormat dataFormat)
        {
            byte[] result = ReadKeepReg(iDevAdd, iAddress, iLength*2);
            string floatvalue = string.Empty;
            if (result != null && result.Length == iLength * 4)
            {
                for (int i = 0; i < result.Length; i += 4)
                {
                    floatvalue += BitConverter.ToSingle(GetSingleByteArray(result, i, dataFormat), 0).ToString() + " ";
                    return (float)Convert.ToDouble(floatvalue);
                }
            }
            return 0;
        }


        /// <summary>
        /// 读取word数据，输出寄存器4x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <returns></returns>
        public byte[] ReadKeepReg(int iDevAdd, int iAddress, int iLength)
        {
            byte[] array = null;
            CurrentAddr = iDevAdd;
            byte[] array2 = new byte[8]
            {
                (byte)iDevAdd,
                3,
                (byte)((iAddress - iAddress % 256) / 256),
                (byte)(iAddress % 256),
                (byte)((iLength - iLength % 256) / 256),
                (byte)(iLength % 256),
                0,
                0
            };
            Crc16(array2, 6);
            array2[6] = ucCRCLo;
            array2[7] = ucCRCHi;
            byte[] response = new byte[5 + 2 * iLength];
            if (SendData(array2, ref response))
            {
                return GetByteArray(response, 3, 2 * iLength);
            }

            return null;
        }

        /// <summary>
        /// 读取word数据，输入寄存器3x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <returns></returns>
        public byte[] ReadInputReg(int iDevAdd, int iAddress, int iLength)
        {
            byte[] array = null;
            CurrentAddr = iDevAdd;
            byte[] array2 = new byte[8]
            {
                (byte)iDevAdd,
                4,
                (byte)((iAddress - iAddress % 256) / 256),
                (byte)(iAddress % 256),
                (byte)((iLength - iLength % 256) / 256),
                (byte)(iLength % 256),
                0,
                0
            };
            Crc16(array2, 6);
            array2[6] = ucCRCLo;
            array2[7] = ucCRCHi;
            byte[] response = new byte[5 + 2 * iLength];
            if (SendData(array2, ref response))
            {
                return GetByteArray(response, 3, 2 * iLength);
            }

            return null;
        }

        /// <summary>
        /// 写入word数据，输出寄存器4x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="SetValue">值</param>
        /// <returns></returns>
        public bool PreSetKeepReg(int iDevAdd, int iAddress, object SetValue)
        {
            byte[] array = new byte[8];
            CurrentAddr = iDevAdd;
            array[0] = (byte)iDevAdd;
            array[1] = 6;
            array[2] = (byte)((iAddress - iAddress % 256) / 256);
            array[3] = (byte)(iAddress % 256);
            byte[] array2 = new byte[2];
            if (SetValue is ushort)
            {
                array2 = BitConverter.GetBytes((ushort)SetValue);
            }
            else if (SetValue is short)
            {
                array2 = BitConverter.GetBytes((short)SetValue);
            }

            DataFormat dataFormat = DataFormat;
            DataFormat dataFormat2 = dataFormat;
            if (dataFormat2 == DataFormat.ABCD || dataFormat2 == DataFormat.CDAB)
            {
                array[4] = array2[0];
                array[5] = array2[1];
            }
            else
            {
                array[4] = array2[1];
                array[5] = array2[0];
            }

            Crc16(array, 6);
            array[6] = ucCRCLo;
            array[7] = ucCRCHi;
            byte[] response = new byte[8];
            if (SendData(array, ref response))
            {
                return ByteArrayEquals(array, response);
            }

            return false;
        }

        /// <summary>
        /// 写入浮点数据，输出寄存器4x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="SetValue">值</param>
        /// <returns></returns>
        public bool PreSetFloatKeepReg(int iDevAdd, int iAddress, object SetValue)
        {
            byte[] array = new byte[13];
            CurrentAddr = iDevAdd;
            array[0] = (byte)iDevAdd;
            array[1] = 16;
            array[2] = (byte)((iAddress - iAddress % 256) / 256);
            array[3] = (byte)(iAddress % 256);
            array[4] = 0;
            array[5] = 2;
            array[6] = 4;
            byte[] array2 = new byte[4];
            array2 = ((SetValue is float) ? BitConverter.GetBytes((float)SetValue) : ((!(SetValue is uint)) ? BitConverter.GetBytes((int)SetValue) : BitConverter.GetBytes((uint)SetValue)));
            switch (DataFormat)
            {
                case DataFormat.ABCD:
                    array[7] = array2[0];
                    array[8] = array2[1];
                    array[9] = array2[2];
                    array[10] = array2[3];
                    break;
                case DataFormat.BADC:
                    array[7] = array2[1];
                    array[8] = array2[0];
                    array[9] = array2[3];
                    array[10] = array2[2];
                    break;
                case DataFormat.CDAB:
                    array[7] = array2[2];
                    array[8] = array2[3];
                    array[9] = array2[0];
                    array[10] = array2[1];
                    break;
                case DataFormat.DCBA:
                    array[7] = array2[3];
                    array[8] = array2[2];
                    array[9] = array2[1];
                    array[10] = array2[0];
                    break;
            }

            Crc16(array, 11);
            array[11] = ucCRCLo;
            array[12] = ucCRCHi;
            byte[] response = new byte[8];
            if (SendData(array, ref response))
            {
                byte[] byteArray = GetByteArray(response, 0, 6);
                Crc16(byteArray, 6);
                byte[] byteArray2 = GetByteArray(response, 6, 2);
                if (ByteArrayEquals(GetByteArray(array, 0, 6), byteArray) && byteArray2[0] == ucCRCLo && byteArray2[1] == ucCRCHi)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// 读取 输出线圈0x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <returns></returns>
        public byte[] ReadOutputStatus(int iDevAdd, int iAddress, int iLength)
        {
            byte[] array = new byte[8];
            CurrentAddr = iDevAdd;
            int num = 0;
            num = ((iLength % 8 != 0) ? (iLength / 8 + 1) : (iLength / 8));
            array[0] = (byte)iDevAdd;
            array[1] = 1;
            array[2] = (byte)((iAddress - iAddress % 256) / 256);
            array[3] = (byte)(iAddress % 256);
            array[4] = (byte)((iLength - iLength % 256) / 256);
            array[5] = (byte)(iLength % 256);
            Crc16(array, 6);
            array[6] = ucCRCLo;
            array[7] = ucCRCHi;
            byte[] response = new byte[5 + num];
            if (SendData(array, ref response))
            {
                return GetByteArray(response, 3, num);
            }

            return null;
        }

        /// <summary>
        /// 读取 输入状态1x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">长度</param>
        /// <returns></returns>
        public byte[] ReadInputStatus(int iDevAdd, int iAddress, int iLength)
        {
            byte[] array = new byte[8];
            CurrentAddr = iDevAdd;
            int num = 0;
            num = ((iLength % 8 != 0) ? (iLength / 8 + 1) : (iLength / 8));
            array[0] = (byte)iDevAdd;
            array[1] = 2;
            array[2] = (byte)((iAddress - iAddress % 256) / 256);
            array[3] = (byte)(iAddress % 256);
            array[4] = (byte)((iLength - iLength % 256) / 256);
            array[5] = (byte)(iLength % 256);
            Crc16(array, 6);
            array[6] = ucCRCLo;
            array[7] = ucCRCHi;
            byte[] response = new byte[5 + num];
            if (SendData(array, ref response))
            {
                return GetByteArray(response, 3, num);
            }

            return null;
        }

        /// <summary>
        /// 写入 输出线圈0x
        /// </summary>
        /// <param name="iDevAdd">站号</param>
        /// <param name="iAddress">地址</param>
        /// <param name="iLength">值</param>
        /// <returns></returns>
        public bool ForceCoil(int iDevAdd, int iAddress, bool SetValue)
        {
            byte[] array = new byte[8];
            CurrentAddr = iDevAdd;
            array[0] = (byte)iDevAdd;
            array[1] = 5;
            array[2] = (byte)((iAddress - iAddress % 256) / 256);
            array[3] = (byte)(iAddress % 256);
            if (SetValue)
            {
                array[4] = byte.MaxValue;
            }
            else
            {
                array[4] = 0;
            }

            array[5] = 0;
            Crc16(array, 6);
            array[6] = ucCRCLo;
            array[7] = ucCRCHi;
            byte[] response = new byte[8];
            if (SendData(array, ref response))
            {
                return ByteArrayEquals(array, response);
            }

            return false;
        }

        private byte[] HexStringToByteArray(string S, int start, int end)
        {
            byte[] array = null;
            if (S != null && S.Length > start + end)
            {
                string[] array2 = S.Trim().Split(' ');
                string[] array3 = new string[array2.Length - start - end];
                for (int i = 0; i < array3.Length; i++)
                {
                    array3[i] = array2[i + start];
                }

                array = new byte[array3.Length];
                for (int j = 0; j < array3.Length; j++)
                {
                    array[j] = Convert.ToByte(array3[j], 16);
                }
            }

            return array;
        }

        private bool ByteArrayEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null)
            {
                return false;
            }

            if (b1.Length != b2.Length)
            {
                return false;
            }

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private byte[] GetByteArray(byte[] byteArr, int start, int length)
        {
            byte[] array = new byte[length];
            if (byteArr != null && byteArr.Length >= length)
            {
                for (int i = 0; i < length; i++)
                {
                    array[i] = byteArr[i + start];
                }
            }

            return array;
        }

        private bool SendData(byte[] sendByte, ref byte[] response)
        {
            InteractiveLock.Enter();
            try
            {
                MyCom.Write(sendByte, 0, sendByte.Length);
                MyCom.DiscardInBuffer();
                MyCom.DiscardOutBuffer();
                GetResponse(ref response);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                InteractiveLock.Leave();
            }
        }

        private void GetResponse(ref byte[] response)
        {
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = (byte)MyCom.ReadByte();
            }
        }

        private void Crc16(byte[] pucFrame, int usLen)
        {
            int num = 0;
            ucCRCHi = byte.MaxValue;
            ucCRCLo = byte.MaxValue;
            ushort num2 = 0;
            while (usLen-- > 0)
            {
                num2 = (ushort)(ucCRCLo ^ pucFrame[num++]);
                ucCRCLo = (byte)(ucCRCHi ^ aucCRCHi[num2]);
                ucCRCHi = aucCRCLo[num2];
            }
        }



        /// <summary>
        /// 双字节顺序转换：1为AB，2为BA
        /// </summary>
        /// <param name="byteArr"></param>
        /// <param name="start"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] GetIntByteArray(byte[] byteArr, int start, DataFormat type)
        {
            byte[] Res = new byte[2];
            if (byteArr != null && byteArr.Length >= start + 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Res[i] = byteArr[i + start];
                }
                byte[] Res1 = new byte[2];
                switch (type)
                {
                    case DataFormat.ABCD:
                    case DataFormat.CDAB:
                        Res1 = Res;
                        break;
                    case DataFormat.BADC:
                    case DataFormat.DCBA:
                        Res1[1] = Res[0];
                        Res1[0] = Res[1];
                        break;
                    default:
                        break;
                }
                return Res1;
            }
            return Res;
        }

        /// <summary>
        /// 四字节顺序转换：1为ABCD，2为BADC，3为CDAB，4为 DCBA
        /// </summary>
        /// <param name="byteArr"></param>
        /// <param name="start"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] GetSingleByteArray(byte[] byteArr, int start, DataFormat type)
        {
            byte[] Res = new byte[4];
            if (byteArr != null && byteArr.Length >= start + 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Res[i] = byteArr[i + start];
                }
                byte[] Res1 = new byte[4];
                switch (type)
                {
                    case DataFormat.ABCD:
                        Res1 = Res;
                        break;
                    case DataFormat.BADC:
                        Res1[0] = Res[1];
                        Res1[1] = Res[0];
                        Res1[2] = Res[3];
                        Res1[3] = Res[2];
                        break;
                    case DataFormat.CDAB:
                        Res1[0] = Res[2];
                        Res1[1] = Res[3];
                        Res1[2] = Res[0];
                        Res1[3] = Res[1];
                        break;
                    case DataFormat.DCBA:
                        Res1[0] = Res[3];
                        Res1[1] = Res[2];
                        Res1[2] = Res[1];
                        Res1[3] = Res[0];
                        break;
                    default:
                        break;
                }
                return Res1;
            }
            return Res;
        }

    }

    public enum DataFormat
    {
        ABCD,
        BADC,
        CDAB,
        DCBA
    }



    public sealed class SimpleHybirdLock : IDisposable
    {
        private bool disposedValue = false;

        private int m_waiters = 0;

        private AutoResetEvent m_waiterLock = new AutoResetEvent(initialState: false);

        public bool IsWaitting => m_waiters != 0;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                m_waiterLock.Close();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        public void Enter()
        {
            if (Interlocked.Increment(ref m_waiters) != 1)
            {
                m_waiterLock.WaitOne();
            }
        }

        public void Leave()
        {
            if (Interlocked.Decrement(ref m_waiters) != 0)
            {
                m_waiterLock.Set();
            }
        }
    }


}
