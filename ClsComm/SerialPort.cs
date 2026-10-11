using System;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace ClsComm
{
    [Serializable]
    [XmlRoot("SerialPort")]
    [XmlType("SerialPort")]
    public class SerialPort : Comm
    {
        //创建串口对象
        private System.IO.Ports.SerialPort serPort = null;
        private string comPort;
        private int baudRate;
        private Parity parity;
        private StopBits stopBits;
        private int dataBits;
        ////锁
        private Mutex mtxSend = new Mutex();

        [XmlElement("ComPort"), CategoryAttribute("通讯参数"), DisplayNameAttribute("端口号"), TypeConverter(typeof(ComPortConverter))]
        public string ComPort
        {
            set
            {
                if (value != comPort)
                {
                    comPort = value;
                }
            }
            get { return comPort; }
        }

        [XmlElement("BaudRate"), CategoryAttribute("通讯参数"), DisplayNameAttribute("波特率")]
        public int BaudRate
        {
            set
            {
                if (value != baudRate)
                {
                    baudRate = value;
                }
            }
            get { return baudRate; }
        }

        [XmlElement("Parity"), CategoryAttribute("通讯参数"), DisplayNameAttribute("校验位")]
        [Description("None 无校验\r\nOdd 奇校验\r\nEven 偶校验\r\nMark 奇偶校验位保留为1\r\nSpace 奇偶校验位保留为0")]
        public Parity Parity
        {
            set
            {
                if (value != parity)
                {
                    parity = value;
                }
            }
            get { return parity; }
        }

        [XmlElement("StopBits"), CategoryAttribute("通讯参数"), DisplayNameAttribute("停止位")]
        [Description("None 无\r\nOne 1\r\nTwo 2\r\nOnePointFive 1.5")]
        public StopBits StopBits
        {
            set
            {
                if (value != stopBits)
                {
                    stopBits = value;
                }
            }
            get { return stopBits; }
        }

        [XmlElement("DataBit"), CategoryAttribute("通讯参数"), DisplayNameAttribute("数据位")]
        public int DataBits
        {
            set
            {
                if (value != dataBits)
                {
                    dataBits = value;
                }
            }
            get { return dataBits; }
        }

        public SerialPort()
        {
            comPort = "COM1";
            baudRate = 115200;
            parity = Parity.None;
            stopBits = StopBits.One;
            dataBits = 8;
            Type = CommType.SerialPort;
            serPort = new System.IO.Ports.SerialPort();
            IsConnected = false;
        }

        //~SerialPort()
        //{
        //    //IsConnected = false;
        //    //if (serPort == null) return;
        //    //if (serPort.IsOpen)
        //    //{
        //    //    serPort.Close();
        //    //}
        //}

        /// <summary>
        /// 设置串口参数
        /// </summary>
        /// <param name="comPortName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool SetPortParam(string comPortName, int baudRate, Parity parity, int dataBits, StopBits stopBits, out string msg)
        {
            try
            {
                if (serPort.IsOpen)
                    serPort.Close();
                ComPort = comPortName;
                BaudRate = baudRate;
                Parity = parity;
                DataBits = dataBits;
                StopBits = stopBits;

                //设置串口参数
                serPort.PortName = comPortName;//端口号
                serPort.BaudRate = baudRate;//波特率 9600 115200
                serPort.Parity = parity;//奇偶校验 无
                serPort.DataBits = dataBits;//数据位8
                serPort.StopBits = stopBits;//停止位 1

                //_serialPort.Handshake = Handshake.None;
                //_serialPort.RtsEnable = true;
                //_serialPort.ReadTimeout = 3000;
                //_serialPort.NewLine = "/r/n";
                //serPort.Encoding = System.Text.Encoding.GetEncoding("GB2312");//注意：如果没有这句，发送或接收中文字符是乱码

                //设置触发DataReceived事件的字节数为1
                serPort.ReceivedBytesThreshold = 1;
                //接收到一个字节时，也会触发DataReceived事件
                serPort.DataReceived += new SerialDataReceivedEventHandler(Receive);

                msg = "SetPortParam:串口参数设置成功";
                return true;
            }
            catch (Exception ex)
            {
                msg = "SetPortParam:" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="comPort">端口号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Connect(string comPort, int baudRate, Parity parity, int dataBits, StopBits stopBits, out string msg)
        {
            if (!SetPortParam(comPort, baudRate, parity, dataBits, stopBits, out msg))
            {
                return false;
            }

            //如果串口是打开的，先关闭
            if (serPort.IsOpen)
                serPort.Close();
            try
            {
                //打开串口
                serPort.Open();
                IsConnected = true;
                msg = "Connect:串口打开成功";
                OnConnectEvent(msg);
                return true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                msg = "Connect:串口打开失败：" + ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool Connect(out string msg)
        {
            return Connect(comPort, baudRate, parity, dataBits, stopBits, out msg);
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            //如果串口处于打开状态,则关闭
            //if (serPort.IsOpen)
            //{
            IsConnected = false;
            Thread.Sleep(20);
            serPort.Close();
            //}
            return true;
        }

        //接收串口数据事件
        void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(20);
                byte[] _data = new byte[serPort.BytesToRead];
                serPort.Read(_data, 0, _data.Length);
                if (_data.Length == 0) { return; }
                reciveData = BitConverter.ToString(_data);
                Array.Clear(_data, 0, _data.Length);
                serPort.DiscardInBuffer();  //清空接收缓冲区   
                OnReviceEvent(reciveData);
            }
            catch (Exception)
            {
                //throw ex;
            }
        }

        //发送数据
        public override bool Send(string strData, out string msg)
        {
            msg = "Send:Ok";

            if (IsConnected)
            {
                mtxSend.WaitOne();
                try
                {
                    serPort.Write(strData);
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
                finally
                {
                    mtxSend.ReleaseMutex();
                }
                return true;
            }
            else
            {
                msg = "Send:串口未打开!";
                return false;
            }
        }

        public override bool Send(byte[] bytData, out string msg)
        {
            msg = "Send:Ok";

            if (IsConnected)
            {
                mtxSend.WaitOne();
                try
                {
                    serPort.Write(bytData, 0, bytData.Length);
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    return false;
                }
                finally
                {
                    mtxSend.ReleaseMutex();
                }
                return true;
            }
            else
            {
                msg = "Send:串口未打开!";
                return false;
            }
        }

        private class ComPortConverter : StringConverter
        {
            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (context != null && context.Instance is TcpSer)
                {
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();
                    Array.Sort(ports);
                    return new StandardValuesCollection(ports);
                }
                return base.GetStandardValues(context);
            }
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
        }
    }
}
