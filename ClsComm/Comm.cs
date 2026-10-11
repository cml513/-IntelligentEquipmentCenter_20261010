using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace ClsComm
{
    public enum CommType
    {
        None,
        SerialPort,
        TcpClt,
        TcpSer,
        HttpClt,
        HttpSer,
        MqttClt,
        MqttSer
    }

    [Serializable]
    [XmlRoot("Comm")]
    [XmlInclude(typeof(SerialPort))]//序列化包含子类的类型
    [XmlInclude(typeof(TcpClt))]
    [XmlInclude(typeof(TcpSer))]

    public class Comm
    {
        //变量
        private string name = "Comm";
        private bool isConnected = false;
        private bool isConnecting = false;
        private CommType type = CommType.None;
        protected string reciveData;
        private List<CommCmd> lstCmd = new List<CommCmd>();

        //事件
        //[NonSerialized]
        //public int a = 0;
        public delegate void DelegateMsg(string msg);
        public event DelegateMsg ConnectEvent;
        public event DelegateMsg ErrorEvent;

        public delegate string  DelegateBackMsg(string msg);
        public event DelegateBackMsg ReviceEvent;

        [XmlIgnore, Browsable(false),ReadOnly(true)]
        public string ReciveData
        {
            get { return reciveData; }
        }

        //属性
        [XmlElement("Name"), Category("参数"), DisplayName("名称")]
        public string Name
        {
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
            get { return name; }
        }

        [XmlElement("Type"), Category("参数"), DisplayName("类型")]
        public CommType Type
        {
            set
            {
                if (value != type)
                {
                    type = value;
                }
            }
            get { return type; }
        }

        [XmlIgnore, ReadOnly(true), Category("状态"), DisplayName("已连接")]
        public bool IsConnected
        {
            set
            {
                if (value != isConnected)
                {
                    isConnected = value;
                    OnPropertyChanged();
                }
            }
            get { return isConnected; }
        }

        [XmlIgnore, ReadOnly(true), Category("状态"), DisplayName("连接中")]
        public bool IsConnecting
        {
            set
            {
                if (value != isConnecting)
                {
                    isConnecting = value;
                    OnPropertyChanged();
                }
            }
            get { return isConnecting; }
        }

        [XmlArray("CmdList")]
        [XmlArrayItem("Cmd", IsNullable = false), Category("命令列表"), DisplayName("列表")]
        public List<CommCmd> LstCmd
        {
            get { return lstCmd; }
            set { lstCmd = value; }
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool Connect(out string msg)
        {
            msg = "Ok";
            IsConnected = true;
            return true;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public virtual bool Close()
        {
            IsConnecting = false;
            IsConnected = false;
            return true;
        }

        /// <summary>
        /// 按字符串发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool Send(string data, out string msg)
        {
            msg = "Ok";
            return true;
        }

        /// <summary>
        /// 按字节发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool Send(byte[] data, out string msg)
        {
            msg = "Ok";
            return true;
        }

        /// <summary>
        /// 把字符串转化为asc码16进制字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="hex"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool StrToAscII(string str, out string hexStr, out string msg)
        {
            try
            {
                byte[] InBytes;
                byte[] lastBytes;
                byte cut;
                string _stringOut = "";

                cut = 0x00;
                //用 System.Text.Encoding.ASCII.GetBytes(InString)有问题  //与 Default.GetBytes区别？
                InBytes = System.Text.Encoding.Default.GetBytes(str);
                if (BtyCut(InBytes, cut, out lastBytes, out msg))
                {
                    if (BtyToAscII(lastBytes, out _stringOut, out msg))
                    {
                        hexStr = _stringOut;
                        return true;
                    }
                    else
                    {
                        hexStr = _stringOut;
                        return false;
                    }
                }
                else
                {
                    hexStr = _stringOut;
                    return false;
                }
            }
            catch (Exception ex)
            {
                hexStr = "";
                msg = ex.Message;
                return false;
            }
        }
        public static bool BtyToAscII(byte[] arrBty, out string hexStr, out string msg)
        {
            try
            {
                string returnStr = "";
                for (int i = 0; i <= arrBty.Length - 1; ++i)
                {
                    returnStr += arrBty[i].ToString("X2") + " ";
                }
                hexStr = returnStr;
                msg = "Ok";
                return true;
            }
            catch (Exception ex)
            {
                hexStr = "";
                msg = ex.Message;
                return false;
            }
        }
        public static bool BtyCut(byte[] arrBty, byte cutBty, out byte[] outBty, out string msg)
        {
            try
            {
                List<Byte> list = new List<byte>();
                list.AddRange(arrBty);
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    if (list[i] == cutBty)
                    {
                        list.RemoveAt(i);
                    }
                }

                Byte[] lastBytes = new Byte[list.Count];
                for (int i = 0; i <= list.Count - 1; ++i)
                {
                    lastBytes[i] = list[i];
                }
                outBty = lastBytes;
                msg = "Ok";
                return true;
            }
            catch (Exception ex)
            {
                outBty = null;
                msg = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 16进制转化为字符串
        /// </summary>
        /// <param name="asc"></param>
        /// <param name="str"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool AscIIToStr(string asc, out string str, out string msg)
        {
            msg = "AscIIToStr:Ok";
            str = "";
            try
            {
                asc = asc.Replace(" ", "");
                byte[] byt = new byte[asc.Length / 2];
                for (int i = 0; i < byt.Length; i++)
                {
                    str = asc.Substring(i * 2, 2);
                    byt[i] = Convert.ToByte(str, 16);
                }
                str = System.Text.Encoding.Default.GetString(byt);
            }
            catch (Exception ex)
            {
                msg = "AscIIToStr:" + ex.Message;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 16进制字符转转化为字节
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] AscIIToBty(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// BCC异或校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] BCC(byte[] data)
        {
            int temp = 0;

            for (int i = 0; i < data.Length; i++)
            {
                temp = temp ^ data[i];
            }
            byte[] result = new byte[1];

            result[0] = Convert.ToByte(temp);
            return result;
        }

        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CRC16(byte[] data)
        {
            byte[] returnVal = new byte[2];
            byte CRC16Lo, CRC16Hi, CL, CH, SaveHi, SaveLo;
            int i, Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x01;
            CH = 0xA0;
            for (i = 0; i < data.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ data[i]);//每一个数据与CRC寄存器进行异或
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1);//高位右移一位
                    CRC16Lo = (byte)(CRC16Lo >> 1);//低位右移一位
                    if ((SaveHi & 0x01) == 0x01)//如果高位字节最后一位为
                    {
                        CRC16Lo = (byte)(CRC16Lo | 0x80);//则低位字节右移后前面补 否则自动补0
                    }
                    if ((SaveLo & 0x01) == 0x01)//如果LSB为1，则与多项式码进行异或
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }

            if (BitConverter.GetBytes(1)[0] == 1)
            {//小端
                returnVal[0] = CRC16Lo;//CRC低位
                returnVal[1] = CRC16Hi;//CRC高位
            }
            else
            {//大端
                returnVal[0] = CRC16Hi;//CRC高位
                returnVal[1] = CRC16Lo;//CRC低位
            }

            return returnVal;
        }
        /// <summary>
        /// 获取函数名称
        /// </summary>
        /// <param name="index"> 0当前函数名  1上级调用</param>
        /// <returns></returns>
        public static string GetFuncName(int index = 1)
        {
            //index 指定的堆栈帧
            //index =0 获取当前函数的名称
            //index =1 获取上一级调用的函数名称
            //...
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            return stackTrace.GetFrame(index).GetMethod().ToString();
        }
        #region 事件
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected virtual void OnConnectEvent(string msg)
        {
            if (ConnectEvent != null) ConnectEvent(msg);
        }
        protected virtual string  OnReviceEvent(string msg)
        {
            if (ReviceEvent != null)
                return ReviceEvent(msg);
            else
                return "";
        }
        protected virtual void OnErrorEvent(string msg)
        {
            if (ErrorEvent != null) ErrorEvent(msg);
        }

        #endregion
    }

    [Serializable]
    [XmlRootAttribute("CommCmd")]
    [TypeConverterAttribute(typeof(CommCmdConverter))]
    public class CommCmd
    {
        private string name;
        private string data;

        [XmlElement("Name"), CategoryAttribute("命令"), DisplayNameAttribute("名称")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlElement("Data"), CategoryAttribute("命令"), DisplayNameAttribute("数据")]
        public string Data
        {
            get { return data; }
            set { data = value; }
        }
    }

    public class CommCmdConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context,
                                   System.Type destinationType)
        {
            if (destinationType == typeof(CommCmd))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                 value is CommCmd)
            {
                CommCmd so = (CommCmd)value;
                return "CommCmd";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is CommCmd)
            {
                //做些处理
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
