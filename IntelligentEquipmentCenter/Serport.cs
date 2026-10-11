using ClsComm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    public class Serport
    {
        #region 实例化
        private static readonly object Lock = new object();
        private static Serport _instance;

        public static Serport Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Serport();
                    }
                }
                return _instance;
            }
        }

        public Serport()
        {
        }

        #endregion

        public static string RecE50C = "";
        public static SerialPort ComE50C = new SerialPort() { Name = "读卡器" };

        public static string RecE50D = "";
        public static SerialPort ComE50D = new SerialPort() { Name = "待定" };

        public static string RecE50E = "";
        public static SerialPort ComE50E = new SerialPort() { Name = "待定" };

        public static string RecE50F = "";
        public static SerialPort ComE50F = new SerialPort() { Name = "待定" };
    }


    public class Communic
    {
        private static readonly object Lock = new object();
        private static Communic _instance;

        public static Communic Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Communic();
                    }
                }
                return _instance;
            }
        }

        public static string RecfeedA = "";
        public static ConcurrentQueue<string> RecfeedAQueue = new ConcurrentQueue<string>();
        public static TcpClt TcpCltfeedingA= new TcpClt() { Name = "扫码枪" };

        public static string RecfeedB = "";
       
        public static TcpClt TcpCltfeedingB = new TcpClt() { Name = "待定" };

        public static string RecfeedC = "";
        public static TcpClt TcpCltfeedingC = new TcpClt() { Name = "待定" };

        public static string RecfeedD = "";
        public static TcpClt TcpCltfeedingD = new TcpClt() { Name = "待定" };
    }
}
