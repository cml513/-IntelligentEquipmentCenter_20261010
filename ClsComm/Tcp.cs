using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

namespace ClsComm
{

    [Serializable]
    [XmlRoot("TcpClt")]
    [XmlType("TcpClt")]
    public class TcpClt : Comm
    {
        private Socket sktClient;
        TcpClient tcpClient =new TcpClient();
        private bool isClose;
        //接收缓冲区
        private const int MaxDataBuffer = 1024;
        private byte[] RecvBuffer = new byte[MaxDataBuffer];
        //粘包/拆包累积缓冲区
        private readonly StringBuilder recvAccum = new StringBuilder();
        //发送缓冲区
        private byte[] SendBuffer;
        //心跳包定时器
        private System.Timers.Timer tmrHert = new System.Timers.Timer();
        //锁
        private Mutex mtxSend = new Mutex();
        //本地
        private string localIp;
        private int localPort;
        //远程
        private string remotIp;
        private int remotPort;
        //重连
        private int reconnectInterval = 3000;
        //心跳
        private string heartSendData = "IfOnline";
        private string heartRecvData = "IfOnline";
        private string heartBackData = "IsOnline";
        private uint heartInterval = 10000;
        private uint heartBackTimes = 1;
        private uint HeartSendCount = 0;
        private uint HeartBackCount = 0;

        [XmlIgnore, ReadOnly(true), Category("状态"), DisplayName("本地IP地址")]
        public string LocalIp
        {
            set
            {
                if (value != localIp)
                {
                    localIp = value;
                    OnPropertyChanged();
                }

            }
            get { return localIp; }
        }

        [XmlIgnore, ReadOnly(true), Category("状态"), DisplayName("本地端口号")]
        public int LocalPort
        {
            set
            {
                if (value != localPort)
                {
                    localPort = value;
                    OnPropertyChanged();
                }

            }
            get { return localPort; }
        }

        [XmlElement("RemotIp"), Category("通讯参数"), DisplayName("远程IP地址")]
        public string RemotIp
        {
            set
            {
                if (value != remotIp)
                {
                    remotIp = value;
                }
            }
            get { return remotIp; }
        }

        [XmlElement("RemotPort"), Category("通讯参数"), DisplayName("远程端口号")]
        public int RemotPort
        {
            set
            {
                if (value != remotPort)
                {
                    remotPort = value;
                }
            }
            get { return remotPort; }
        }

        [XmlElement("ReconnectInterval"), Category("心跳管理"), DisplayName("重连间隔ms")]
        public int ReconnectInterval
        {
            get { return reconnectInterval; }
            set
            {
                if (value == reconnectInterval)
                {
                    return;
                }
                if (value < 1000)
                    value = 1000;
                if (value > 10000)
                    value = 10000;
                reconnectInterval = value;
            }
        }

        [ReadOnly(true)]
        [XmlElement("HeartSendData"), Category("心跳管理"), DisplayName("发送数据")]
        public string HeartSendData
        {
            set
            {
                if (value != heartSendData)
                    heartSendData = value;
            }
            get { return heartSendData; }
        }

        [ReadOnly(true)]
        [XmlElement("HeartRecvData"), Category("心跳管理"), DisplayName("接收数据")]
        public string HeartRecvData
        {
            set
            {
                if (value != heartRecvData)
                    heartRecvData = value;
            }
            get { return heartRecvData; }
        }

        [ReadOnly(true)]
        [XmlElement("HeartBackData"), Category("心跳管理"), DisplayName("反馈数据")]
        public string HeartBackData
        {
            set
            {
                if (value != heartBackData)
                    heartBackData = value;
            }
            get { return heartBackData; }
        }

        [XmlElement("HeartInterval"), Category("心跳管理"), DisplayName("心跳间隔ms")]
        public uint HeartInterval
        {
            set
            {
                if (value != heartInterval)
                {
                    if (value < 1000)
                        value = 1000;
                    heartInterval = value;
                }
            }
            get { return heartInterval; }
        }

        [XmlElement("HeartBackTimes"), Category("心跳管理"), DisplayName("反馈次数")]
        public uint HeartBackTimes
        {
            set
            {
                if (value != heartBackTimes)
                    heartBackTimes = value;
            }
            get { return heartBackTimes; }
        }

        public TcpClt()
        {
            Type = CommType.TcpClt;
            remotIp = "127.0.0.1";
            remotPort = 5000;
            IsConnected = false;
            IsConnecting = false;
            isClose = true;
            sktClient = null;
            tmrHert.Elapsed += new System.Timers.ElapsedEventHandler(HeartTimeEvent);
        }

        ~TcpClt()
        {
            IsConnected = false;
            IsConnecting = false;
            isClose = true;
            sktClient = null;
            StopHeart();
        }

        //心跳包定时器事件
        private void HeartTimeEvent(object sender, ElapsedEventArgs e)
        {
            string msg;

            if (string.IsNullOrEmpty(heartSendData))
                heartSendData = "IsOnline";

            if (HeartSendCount < heartBackTimes)
            {
                if (HeartBackCount > 0)//连续多次发送心跳包,对方有响应,说明通讯没有断开
                {
                    HeartSendCount = 0;
                    HeartBackCount = 0;
                }
                else
                {
                    Send(heartSendData + "\r\n", out msg);
                    HeartSendCount++;
                }
            }
            else
            {
                if (HeartBackCount == 0)//连续多次发送心跳包,对方没有响应,说明通讯断开，关闭然后重连
                {
                    Close(false);
                    Reconnect(out msg);
                }
                HeartSendCount = 0;
                HeartBackCount = 0;
            }
        }

        /// <summary>
        /// 打开心跳
        /// </summary>
        public void StartHeart()
        {
            if (string.IsNullOrEmpty(this.heartSendData))
                this.heartSendData = "IsOnline";

            if (this.heartInterval < 1000)
                this.heartInterval = 1000;

            tmrHert.Interval = this.heartInterval;
            tmrHert.Enabled = true;
            HeartSendCount = 0;
            HeartBackCount = 0;
        }

        /// <summary>
        /// 打开心跳
        /// </summary>
        /// <param name="send">发送数据</param>
        /// <param name="interval">心跳间隔</param>
        public void StartHeart(string send, uint interval)
        {
            if (string.IsNullOrEmpty(send))
                send = "IsOnline";

            if (interval < 1000)
                interval = 1000;

            this.heartSendData = send;
            this.heartInterval = interval;
            tmrHert.Interval = interval;
            tmrHert.Enabled = true;
            HeartSendCount = 0;
            HeartBackCount = 0;
        }

        /// <summary>
        /// 停止心跳
        /// </summary>
        public void StopHeart()
        {
            tmrHert.Enabled = false;
            HeartSendCount = 0;
            HeartBackCount = 0;
        }

        /// <summary>
        /// 检测IP地址格式是否有效
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckIpPortSpec(string ip, int port, out string msg)
        {
            IPAddress serverIP;
            if (!IPAddress.TryParse(ip, out serverIP))
            {
                msg = "传入的IP " + ip + " 无效！";
                return false;
            }

            int _iPort;
            if (!int.TryParse(port.ToString(), out _iPort))
            {
                msg = "传入的端口 " + port.ToString() + " 无效！";
                return false;
            }
            msg = "Ok";
            return true;
        }

        public bool IsPingIpOK(string strIP)
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply reply = ping.Send(strIP, 1000);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    //ping通
                    return true;
                }
                else
                {
                    //ping失败
                    return false;
                }
            }
            catch
            {
                //ping失败
                return false;
            }
        }

        //private bool IsRightIP(string strIP)
        //{
        //    //利用正则表达式判断字符串是否符合IPv4格式
        //    if(System
        //}

        /// <summary>
        /// 探测指定 IP:Port 上是否有服务端在监听
        /// </summary>
        public bool IsServerAlive(string ip, int port, out string msg1)
        {
            int timeoutMs = 2000;
            msg1 = "";
            try
            {
                if (!CheckIpPortSpec(ip, port, out msg1))
                    return false;

                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(ip, port, null, null);
                    bool ok = result.AsyncWaitHandle.WaitOne(timeoutMs, false);

                    if (!ok)
                    {
                        msg1 = $"探测超时：{ip}:{port} 无响应";
                        client.Close();
                        return false;
                    }

                    client.EndConnect(result); // 若端口无人监听，这里抛 SocketException(10061)
                    msg1 = $"探测成功：{ip}:{port} 服务端在线";
                    return true;
                }
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10061)
                    msg1 = $"服务端未启用：{ip}:{port} 端口未监听";
                else if (ex.ErrorCode == 10060)
                    msg1 = $"连接超时：{ip}:{port}";
                else
                    msg1 = $"探测失败：{ip}:{port}，错误码 {ex.ErrorCode}，{ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                msg1 = "探测异常：" + ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 通讯连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Connect(string ip, int port, out string msg)
        {
       

            msg = "Connect:Ok";

            if (IsConnected)
            {
                msg = "Connect:已连接过服务端！";
                Close();
            }

            isClose = false;
            IsConnecting = false;

            try
            {
                //判断IP与端口是否有效
                if (!CheckIpPortSpec(ip, port, out msg))
                {
                    msg = "Connect:服务端IP地址格式不正确！";
                    return false;
                }

                //tcpClient.Connect(ip, port);
                //if (!tcpClient.Connected) return false;

                if (!IsServerAlive(ip,port,out string msg2))
                {
                    msg = "Connect:服务端IP地址或端口不存在！";
                    return false;
                }

                //缓存服务端IP与端口
                remotIp = ip;
                remotPort = port;
                //创建客户端套接字 
              
                sktClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sktClient.BeginConnect(new IPEndPoint(IPAddress.Parse(remotIp), remotPort), new AsyncCallback(ConnectCallBack), sktClient);

                msg = "Connect:开始连接服务端";

                OnConnectEvent(msg);
                return true;
            }
            catch (InvalidOperationException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Connect:" + msg);
                return false;
            }
            catch (SocketException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Connect:" + msg);
                return false;
            }
        }
        /// <summary>
        /// 重连
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool Reconnect(out string msg)
        {
            msg = "Reconnect:Ok";
            try
            {
                //创建客户端套接字 
                sktClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sktClient.BeginConnect(new IPEndPoint(IPAddress.Parse(remotIp), remotPort), new AsyncCallback(ConnectCallBack), sktClient);

                msg = "Reconnect:开始重连服务端";
                OnConnectEvent(msg);
                return true;
            }
            catch (InvalidOperationException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Reconnect:" + msg);
                return false;
            }
            catch (SocketException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Reconnect:" + msg);
                return false;
            }
            catch (Exception ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Reconnect:" + msg);
                return false;
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="set_close"></param>
        /// <returns></returns>
        public bool Close(bool set_close = true)
        {
            string msg = "Close:Ok";

            if (set_close)
                isClose = true;

            IsConnected = false;
            IsConnecting = false;

            HeartSendCount = 0;
            HeartBackCount = 0;

            if (sktClient != null)
            {
                try
                {
                    //关闭Socket之前，首选需要把双方的Socket Shutdown掉
                    sktClient.Shutdown(SocketShutdown.Both);
                }
                catch (NullReferenceException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                    return false;
                }
                catch (ObjectDisposedException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                    return false;
                }
                catch (SocketException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                    return false;
                }

                try
                {
                    //Shutdown掉Socket后主线程停止10ms，保证Socket的Shutdown完成
                    System.Threading.Thread.Sleep(10);

                    //关闭Socket,清理资源
                    sktClient.Close();
                    sktClient = null;
                    msg = "Close:断开连接";
                    OnConnectEvent(msg);
                }
                catch (NullReferenceException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                    return false;
                }
                catch (ObjectDisposedException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                    return false;
                }
                catch (SocketException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="strSend">字符串格式数据</param>
        /// <param name="BytesSend">字节数据</param>
        /// <param name="IsStrSend">true 为字符串发送</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Send(string strSend, Byte[] BytesSend, bool IsStrSend, out string msg)
        {
            if (!IsConnected)
            {
                msg = "Send:未连接服务端,请先连接服务端。";
                return false;
            }
            mtxSend.WaitOne();
            try
            {
                if (IsStrSend)
                {
                    SendBuffer = System.Text.Encoding.UTF8.GetBytes(strSend);
                    sktClient.BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, asyncResult =>
                    {
                        sktClient.EndSend(asyncResult);
                    }, null);
                    Array.Clear(SendBuffer, 0, SendBuffer.Length);
                }
                else
                {
                    sktClient.BeginSend(BytesSend, 0, BytesSend.Length, SocketFlags.None, asyncResult =>
                    {
                        sktClient.EndSend(asyncResult);
                    }, null);
                    Array.Clear(BytesSend, 0, BytesSend.Length);
                }

                msg = "Send:Ok";
                return true;
            }
            catch (System.NullReferenceException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Send:" + msg);

                Close(false);
                Thread.Sleep(reconnectInterval);//重连时间

                Reconnect(out msg);
                return false;
            }
            catch (ObjectDisposedException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Send:" + msg);

                Close(false);
                Thread.Sleep(reconnectInterval);//重连时间

                Reconnect(out msg);
                return false;
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10060 || ex.ErrorCode == 10061)
                {
                    OnConnectEvent("Send:服务端未开启");
                }
                else if (ex.ErrorCode == 10051)
                {
                    OnConnectEvent("Send:服务端IP地址不存在!");
                }
                else
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Send:" + msg);

                    Close(false);
                }

                Thread.Sleep(reconnectInterval);//重连时间
                Reconnect(out msg);
                return false;
            }
            finally
            {
                mtxSend.ReleaseMutex();
            }
        }
        private void ConnectCallBack(IAsyncResult iar)
        {
            string msg = "";
            if (isClose) return;

            IsConnected = false;

            try
            {
                IsConnecting = true;

                Socket client = (Socket)iar.AsyncState;
                client.EndConnect(iar);

                remotIp = ((System.Net.IPEndPoint)sktClient.RemoteEndPoint).Address.ToString();//获取远程连接IP
                remotPort = ((System.Net.IPEndPoint)sktClient.RemoteEndPoint).Port;//获取远程连接Port
                LocalIp = ((System.Net.IPEndPoint)client.LocalEndPoint).Address.ToString();//获取本地IP
                LocalPort = ((System.Net.IPEndPoint)client.LocalEndPoint).Port;//获取本地Port

                IsConnected = true;
                IsConnecting = false;
                OnConnectEvent("ConnectCallBack:连接上服务端:本地IP:" + LocalIp + ",本地端口:" + LocalPort.ToString());
                //连接成功，开始接收数据
                Array.Clear(RecvBuffer, 0, RecvBuffer.Length); //清空接收数据缓冲区

                sktClient.BeginReceive(RecvBuffer, 0, MaxDataBuffer, SocketFlags.None, new AsyncCallback(ReceiveCallBack), sktClient);
            }
            catch (System.NullReferenceException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("ConnectCallBack:" + msg);

                //Close(false);
                Thread.Sleep(reconnectInterval);//重连时间

                Reconnect(out msg);
            }
            catch (ObjectDisposedException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("ConnectCallBack:" + msg);
                //Close(false);
                Thread.Sleep(reconnectInterval);//重连时间

                Reconnect(out msg);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10060 || ex.ErrorCode == 10061)
                {
                    OnConnectEvent("ConnectCallBack:服务端未开启");
                }
                else if (ex.ErrorCode == 10051)
                {
                    OnConnectEvent("ConnectCallBack:服务端IP地址不存在！");
                }
                else
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("ConnectCallBack:" + msg);

                    Close(false);
                }

                Thread.Sleep(reconnectInterval);//重连时间

                Reconnect(out msg);
            }
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
            string msg;

            if (isClose) return;

            try
            {
                Socket sckReceive = (Socket)ar.AsyncState;
                if (!sckReceive.Connected) return;

                int revLength = sckReceive.EndReceive(ar);
                if (revLength == 0)
                {
                    OnConnectEvent("ReceiveCallBack:服务端断开！");
                    Thread.Sleep(reconnectInterval);//重连时间
                    Reconnect(out msg);
                }
                else
                {
                    //只取实际接收到的字节数
                    reciveData = System.Text.Encoding.UTF8.GetString(RecvBuffer, 0, revLength);
                    Array.Clear(RecvBuffer, 0, RecvBuffer.Length);

                    //心跳计数复位（保留心跳机制本身的计数，但不拦截数据）
                    HeartSendCount = 0;
                    HeartBackCount = 0;

                    // 收到心跳包数据时回发心跳响应，但不影响正常数据往外传
                    string heartCheck = reciveData;
                    if (heartCheck.IndexOf('\r') > -1)
                        heartCheck = heartCheck.Substring(0, heartCheck.IndexOf('\r'));
                    if (heartCheck == heartRecvData)
                    {
                        Send(heartBackData, null, true, out msg);
                    }

                    // 无论什么数据，一律触发接收事件
                    if (!string.IsNullOrEmpty(reciveData))
                    {
                        OnReviceEvent(reciveData);
                    }

                    //继续接收数据
                    sktClient.BeginReceive(RecvBuffer, 0, MaxDataBuffer, SocketFlags.None, new AsyncCallback(ReceiveCallBack), sktClient);
                }
            }
            catch (System.NullReferenceException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("ReceiveCallBack:" + msg);

                Thread.Sleep(reconnectInterval);
                Reconnect(out msg);
            }
            catch (ObjectDisposedException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("ReceiveCallBack:" + msg);

                Thread.Sleep(reconnectInterval);
                Reconnect(out msg);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10060 || ex.ErrorCode == 10061)
                {
                    OnConnectEvent("ReceiveCallBack:服务端未开启");
                }
                else if (ex.ErrorCode == 10051)
                {
                    OnConnectEvent("ReceiveCallBack:服务端IP地址不存在！");
                }
                else
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("ReceiveCallBack:" + msg);

                    Close(false);
                }

                Thread.Sleep(reconnectInterval);
                Reconnect(out msg);
            }
        }
        public override bool Connect(out string msg)
        {
            return Connect(remotIp, remotPort, out msg);
        }

        public override bool Close()
        {
            return Close();
        }
        public override bool Send(string strSend, out string msg)
        {
            return Send(strSend, null, true, out msg);
        }
        public override bool Send(Byte[] BytesSend, out string msg)
        {
            return Send("", BytesSend, false, out msg);
        }
    }

    [Serializable]
    [XmlRoot("TcpSer")]
    [XmlType("TcpSer")]
    public class TcpSer : Comm
    {
        private Socket sktServer;
        //客户端套接字列表
        private Dictionary<string, Socket> dicSktClient = new Dictionary<string, Socket>();
        private Dictionary<string, bool> dicClientStatus = new Dictionary<string, bool>();
        private List<string> lstListen = new List<string>();

        private bool isClose;
        //接收缓冲区
        private const int MaxDataBuffer = 1024;
        private byte[] RecvBuffer = new byte[MaxDataBuffer];
        //发送缓冲区
        private byte[] SendBuffer;
        //心跳包定时器
        private System.Timers.Timer tmrHert = new System.Timers.Timer();
        //锁
        private Mutex mtxSend = new Mutex();
        //本地
        private string localIp;
        private int localPort;
        //远程
        private string curIpPort;
        //重连
        private int reconnectInterval = 3000;
        //心跳
        private string heartSendData = "IfOnline";
        private string heartRecvData = "IfOnline";
        private string heartBackData = "IsOnline";
        private uint heartInterval = 10000;
        private uint heartBackTimes = 1;
        private uint HeartSendCount = 0;
        private uint HeartBackCount = 0;

        [XmlIgnore, Browsable(true)]
        public List<string> LstListen
        {
            get { return lstListen; }
        }

        [XmlElement("LocalIp"), Category("通讯参数"), DisplayName("本地IP地址")]
        public string LocalIp
        {
            set
            {
                if (value != localIp)
                {
                    localIp = value;
                }
            }
            get { return localIp; }
        }

        [XmlElement("LocalPort"), Category("通讯参数"), DisplayName("本地端口号")]
        public int LocalPort
        {
            set
            {
                if (value != localPort)
                {
                    localPort = value;
                }
            }
            get { return localPort; }
        }

        [XmlIgnore, Category("状态"), DisplayName("监听列表"), TypeConverter(typeof(CurIpPortConverter))]
        public string CurRemotIp
        {
            set
            {
                if (value != curIpPort)
                {
                    curIpPort = value;
                    OnPropertyChanged();
                }
            }
            get { return curIpPort; }
        }

        [XmlElement("ReconnectInterval"), Category("心跳管理"), DisplayName("重连间隔ms")]
        public int ReconnectInterval
        {
            get { return reconnectInterval; }
            set
            {
                if (value == reconnectInterval)
                {
                    return;
                }
                if (value < 1000)
                    value = 1000;
                if (value > 10000)
                    value = 10000;
                reconnectInterval = value;
            }
        }

        [XmlElement("HeartSendData"), Category("心跳管理"), DisplayName("发送数据")]
        public string HeartSendData
        {
            set
            {
                if (value != heartSendData)
                    heartSendData = value;
            }
            get { return heartSendData; }
        }

        [XmlElement("HeartRecvData"), Category("心跳管理"), DisplayName("接收数据")]
        public string HeartRecvData
        {
            set
            {
                if (value != heartRecvData)
                    heartRecvData = value;
            }
            get { return heartRecvData; }
        }

        [XmlElement("HeartBackData"), Category("心跳管理"), DisplayName("反馈数据")]
        public string HeartBackData
        {
            set
            {
                if (value != heartBackData)
                    heartBackData = value;
            }
            get { return heartBackData; }
        }

        [XmlElement("HeartInterval"), Category("心跳管理"), DisplayName("心跳间隔ms")]
        public uint HeartInterval
        {
            set
            {
                if (value != heartInterval)
                {
                    if (value < 1000)
                        value = 1000;
                    heartInterval = value;
                }
            }
            get { return heartInterval; }
        }

        [XmlElement("HeartBackTimes"), Category("心跳管理"), DisplayName("反馈次数")]
        public uint HeartBackTimes
        {
            set
            {
                if (value != heartBackTimes)
                    heartBackTimes = value;
            }
            get { return heartBackTimes; }
        }

        public TcpSer()
        {
            Type = CommType.TcpSer;
            localIp = "127.0.0.1";
            localPort = 5000;

            IsConnected = false;
            isClose = false;

            dicClientStatus.Clear();
            lstListen.Clear();
            dicSktClient.Clear();

            HeartSendCount = 0;
            HeartBackCount = 0;
            tmrHert.Elapsed += new System.Timers.ElapsedEventHandler(HeartTimeEvent);
        }

        ~TcpSer()
        {
            IsConnected = false;
            isClose = true;
            sktServer = null;
            StopHeart();
        }

        //心跳包定时器事件
        private void HeartTimeEvent(object sender, ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(heartSendData))
                heartSendData = "IsOnline";

            string msg = "";

            if (HeartSendCount < heartBackTimes)
            {
                if (HeartBackCount > 0)//连续多次发送心跳包,对方有响应,说明通讯没有断开
                {
                    HeartSendCount = 0;
                    HeartBackCount = 0;
                }
                else
                {
                    for (int i = 0; i < lstListen.Count; ++i)
                    {
                        Send(lstListen[i], heartSendData + "\r\n", null, true, out msg);
                    }

                    HeartSendCount++;
                }
            }
            else
            {
                if (HeartBackCount == 0)//连续多次发送心跳包,对方没有响应,说明通讯断开，关闭然后重连
                {
                    HeartSendCount = 0;

                    Close();

                    Open(localIp, localPort, out msg);
                }
            }
        }

        public void StartHeart()
        {
            if (string.IsNullOrEmpty(this.heartSendData))
                this.heartSendData = "IsOnline";

            if (this.heartInterval < 1000)
                this.heartInterval = 1000;

            tmrHert.Interval = this.heartInterval;
            tmrHert.Enabled = true;
            HeartSendCount = 0;
            HeartBackCount = 0;
        }

        public void StartHeart(string data, uint interval)
        {
            if (string.IsNullOrEmpty(data))
                data = "IsOnline";

            if (interval < 1000)
                interval = 1000;

            this.heartSendData = data;
            this.heartInterval = interval;
            tmrHert.Interval = interval;
            tmrHert.Enabled = true;
            HeartSendCount = 0;
            HeartBackCount = 0;
        }

        public void StopHeart()
        {
            tmrHert.Enabled = false;
            HeartSendCount = 0;
            HeartBackCount = 0;
        }

        public bool CheckIpPortSpec(string ip, int port, out string msg)
        {
            IPAddress serverIP;
            if (!IPAddress.TryParse(ip, out serverIP))
            {
                msg = "传入的IP " + ip + " 无效！";
                return false;
            }

            int _iPort;
            if (!int.TryParse(port.ToString(), out _iPort))
            {
                msg = "传入的端口 " + port.ToString() + " 无效！";
                return false;
            }

            msg = "Ok";
            return true;
        }

        private bool IsPingIpOK(string ip)
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply reply = ping.Send(ip, 1000);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    //ping通
                    return true;
                }
                else
                {
                    //ping失败
                    return false;
                }
            }
            catch
            {
                //ping失败
                return false;
            }
        }

        public bool Open(string ip, int port, out string msg)
        {
            if (IsConnected)
            {
                msg = "Open:已开启过服务端！";
                return false;
            }

            isClose = false;
            dicClientStatus.Clear();
            lstListen.Clear();
            dicSktClient.Clear();

            //清空接收数据缓冲区
            Array.Clear(RecvBuffer, 0, MaxDataBuffer);

            try
            {
                //判断IP与端口是否有效
                if (!CheckIpPortSpec(ip, port, out msg))
                {
                    return false;
                }

                if (!IsPingIpOK(ip))
                {
                    msg = "Open:服务端IP地址不存在！";
                    return false;
                }

                //缓存本地IP与端口
                localIp = ip;
                localPort = port;

                if (sktServer == null)
                    sktServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //服务端绑定IP与端口
                if (!sktServer.IsBound)
                {
                    sktServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    sktServer.Bind(new IPEndPoint(IPAddress.Parse(localIp), localPort));
                }

                //开始监听、接收客户端连接
                sktServer.Listen(0);
                sktServer.BeginAccept(new AsyncCallback(AcceptConnectCallback), sktServer);

                IsConnected = true;
                msg = "Open:开启服务端";
                OnConnectEvent(msg);
                return true;
            }
            catch (InvalidOperationException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Open:" + msg);
                return false;
            }
            catch (SocketException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 10049:
                        msg = "服务端IP地址不存在!";
                        break;
                    default:
                        msg = "异常信息:" + ex.Message + "\r\n";
                        msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                        msg += "异常方法:" + ex.TargetSite + "\r\n";
                        msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                        OnErrorEvent("Open:" + msg);
                        break;
                }
                return false;
            }

        }

        public override bool Connect(out string msg)
        {
            return Open(localIp, localPort, out msg);
        }
        public override bool Close()
        {
            string msg;
            IsConnected = false;
            isClose = true;
            HeartSendCount = 0;
            HeartBackCount = 0;

            //先关闭已连接的客户端
            if (dicSktClient.Count > 0)
            {
                foreach (string key in dicSktClient.Keys)
                {
                    if (dicSktClient.ContainsKey(key) && dicSktClient[key] != null)
                    {
                        try
                        {
                            //关闭Socket之前，首选需要把双方的Socket Shutdown掉
                            dicSktClient[key].Shutdown(SocketShutdown.Both);
                        }
                        catch (NullReferenceException ex)
                        {
                            msg = "异常信息:" + ex.Message + "\r\n";
                            msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                            msg += "异常方法:" + ex.TargetSite + "\r\n";
                            msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                            OnErrorEvent("Close:" + msg);
                        }
                        catch (ObjectDisposedException ex)
                        {
                            msg = "异常信息:" + ex.Message + "\r\n";
                            msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                            msg += "异常方法:" + ex.TargetSite + "\r\n";
                            msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                            OnErrorEvent("Close:" + msg);
                        }
                        catch (SocketException ex)
                        {
                            msg = "异常信息:" + ex.Message + "\r\n";
                            msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                            msg += "异常方法:" + ex.TargetSite + "\r\n";
                            msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                            OnErrorEvent("Close:" + msg);
                        }

                        //Shutdown掉Socket后主线程停止10ms，保证Socket的Shutdown完成
                        System.Threading.Thread.Sleep(10);

                        try
                        {
                            //关闭Socket,清理资源
                            dicSktClient[key].Close();
                            //m_ClientsSocket[key] = null;
                        }
                        catch (NullReferenceException ex)
                        {
                            msg = "异常信息:" + ex.Message + "\r\n";
                            msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                            msg += "异常方法:" + ex.TargetSite + "\r\n";
                            msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                            OnErrorEvent("Close:" + msg);
                        }
                        catch (ObjectDisposedException ex)
                        {
                            msg = "异常信息:" + ex.Message + "\r\n";
                            msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                            msg += "异常方法:" + ex.TargetSite + "\r\n";
                            msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                            OnErrorEvent("Close:" + msg);
                        }
                        catch (SocketException ex)
                        {
                            msg = "异常信息:" + ex.Message + "\r\n";
                            msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                            msg += "异常方法:" + ex.TargetSite + "\r\n";
                            msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                            OnErrorEvent("Close:" + msg);
                        }
                    }
                }
            }

            dicClientStatus.Clear();
            lstListen.Clear();
            dicSktClient.Clear();

            //再关闭服务端
            if (sktServer != null)
            {
                try
                {
                    //关闭Socket之前，首选需要把双方的Socket Shutdown掉
                    sktServer.Shutdown(SocketShutdown.Both);
                }
                catch (NullReferenceException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                }
                catch (ObjectDisposedException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                }
                catch (SocketException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                }

                //Shutdown掉Socket后主线程停止10ms，保证Socket的Shutdown完成
                System.Threading.Thread.Sleep(10);

                try
                {
                    //关闭Socket,清理资源
                    sktServer.Close();
                    sktServer = null;
                    OnConnectEvent("Close:服务端关闭。");
                    return true;
                }
                catch (NullReferenceException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                }
                catch (ObjectDisposedException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                }
                catch (SocketException ex)
                {
                    msg = "异常信息:" + ex.Message + "\r\n";
                    msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                    msg += "异常方法:" + ex.TargetSite + "\r\n";
                    msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                    OnErrorEvent("Close:" + msg);
                }
            }
            return false;
        }

        public bool Send(string clientIpPort, string strSend, Byte[] bytSend, bool isStr, out string msg)
        {

            if (!IsConnected)
            {
                msg = "Send:服务端未开启,请先开启服务端。";
                return false;
            }
            if (clientIpPort == null)
            {
                msg = "Send:没有客户端连接。";
                return false;
            }
            clientIpPort = clientIpPort.Trim();
            if (!dicClientStatus.ContainsKey(clientIpPort))
            {
                msg = "Send:客户端<" + clientIpPort + ">不存在";
                return false;
            }

            if (!dicClientStatus[clientIpPort] || !dicSktClient.ContainsKey(clientIpPort))
            {
                msg = "Send:客户端<" + clientIpPort + ">未连接上。";
                return false;
            }

            mtxSend.WaitOne();
            try
            {
                if (isStr)
                {
                    SendBuffer = System.Text.Encoding.Default.GetBytes(strSend);
                    dicSktClient[clientIpPort].BeginSend(SendBuffer, 0, SendBuffer.Length, SocketFlags.None, asyncResult =>
                    {
                        dicSktClient[clientIpPort].EndSend(asyncResult);
                    }, null);
                    Array.Clear(SendBuffer, 0, SendBuffer.Length);
                }
                else
                {
                    dicSktClient[clientIpPort].BeginSend(bytSend, 0, bytSend.Length, SocketFlags.None, asyncResult =>
                    {
                        dicSktClient[clientIpPort].EndSend(asyncResult);
                    }, null);
                    Array.Clear(bytSend, 0, bytSend.Length);
                }

                msg = "Send:Ok";
                return true;
            }
            catch (System.NullReferenceException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Close:" + msg);

                Close();

                Open(localIp, localPort, out msg);
                return false;
            }
            catch (ObjectDisposedException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Close:" + msg);

                Close();

                Open(localIp, localPort, out msg);
                return false;
            }
            catch (SocketException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("Close:" + msg);

                Close();

                Open(localIp, localPort, out msg);
                return false;
            }
            finally
            {
                mtxSend.ReleaseMutex();
            }
        }

        public override bool Send(string data, out string msg)
        {
            if (CurRemotIp == null && CurRemotIp == "")
            {
                msg = "Send:当前未选择发送地址！";
                return false;
            }
            return Send(CurRemotIp, data, out msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientIpPort">"127.0.0.1,5000"</param>
        /// <param name="strSend"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool Send(string clientIpPort, string strSend, out string msg)
        {
            return Send(clientIpPort, strSend, null, true, out msg);
        }

        public bool Send(string clientIpPort, Byte[] bytSend, out string msg)
        {
            return Send(clientIpPort, "", bytSend, false, out msg);
        }

        private void AcceptConnectCallback(IAsyncResult ar)
        {
            string msg;
            if (isClose) return;

            isClose = false;

            try
            {
                Socket serverSocket = (Socket)ar.AsyncState;// 获取传入的原始套接字
                Socket clientSocket = serverSocket.EndAccept(ar);//获取客户端套接字
                string curRemotIp = ((System.Net.IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();//获取远程连接IP
                int curRemotPort = ((System.Net.IPEndPoint)clientSocket.RemoteEndPoint).Port;//获取远程连接Port
                string curRemoteIpPort = curRemotIp + ":" + curRemotPort.ToString();


                //保存客户端IP与连接状态
                if (!dicClientStatus.ContainsKey(curRemoteIpPort))
                    dicClientStatus.Add(curRemoteIpPort, true);
                else
                    dicClientStatus[curRemoteIpPort] = true;

                //保存客户端套接字
                if (!dicSktClient.ContainsKey(curRemoteIpPort))
                    dicSktClient.Add(curRemoteIpPort, clientSocket);
                else
                    dicSktClient[curRemoteIpPort] = clientSocket;

                //保存客户端Ip与端口
                if (!lstListen.Contains(curRemoteIpPort))
                {
                    lstListen.Add(curRemoteIpPort);
                    CurRemotIp = curRemoteIpPort;
                }

                OnConnectEvent("<" + curRemoteIpPort + ">客户端连接上。");

                //开始接受来自该客户端的数据 
                clientSocket.BeginReceive(RecvBuffer, 0, MaxDataBuffer, SocketFlags.None, new AsyncCallback(ReceiveCallBack), clientSocket);

                //继续接受连接请求(允许多个客户端连接)
                serverSocket.BeginAccept(new AsyncCallback(AcceptConnectCallback), ar.AsyncState);

            }
            catch (NullReferenceException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("AcceptConnect:" + msg);

                Close();
                Open(localIp, localPort, out msg);
            }
            catch (ObjectDisposedException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("AcceptConnect:" + msg);

                Close();
                Open(localIp, localPort, out msg);
            }
            catch (SocketException ex)
            {
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("AcceptConnect:" + msg);

                Close();
                Open(localIp, localPort, out msg);
            }

        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            if (isClose) return;

            try
            {
                //传入的原始套接字
                Socket SocketServer = (Socket)ar.AsyncState;
                string curRemoteIP = ((System.Net.IPEndPoint)SocketServer.RemoteEndPoint).Address.ToString();//获取远程连接IP
                int curRemotePort = ((System.Net.IPEndPoint)SocketServer.RemoteEndPoint).Port;//获取远程连接Port
                string curRemoteIpPort = curRemoteIP + ":" + curRemotePort.ToString();

                int revLength = SocketServer.EndReceive(ar);
                if (revLength == 0)
                {
                    //移除该客户端连接状态
                    if (dicClientStatus.ContainsKey(curRemoteIpPort))
                    {
                        dicClientStatus.Remove(curRemoteIpPort);
                    }

                    //移除该客户端套接字
                    if (dicSktClient.ContainsKey(curRemoteIpPort))
                    {
                        dicSktClient.Remove(curRemoteIpPort);
                    }

                    //移除客户端Ip与端口
                    if (lstListen.Contains(curRemoteIpPort))
                    {
                        lstListen.Remove(curRemoteIpPort);
                        if (curRemoteIpPort != CurRemotIp)
                        {
                            CurRemotIp = "";
                        }
                    }
                    OnConnectEvent("<" + curRemoteIpPort + ">客户端断开。");
                }
                else
                {
                    //将Byte数据转化为字符串
                    reciveData = System.Text.Encoding.Default.GetString(RecvBuffer, 0, MaxDataBuffer);
                    reciveData = "<" + curRemoteIpPort + ">" + reciveData;
                    reciveData= reciveData.Replace("\\r\\n", "\r\n");
                    if (reciveData.IndexOf('\r') > -1)
                    {
                        reciveData = reciveData.Substring(0, reciveData.IndexOf('\r'));
                    }
                    //数据通过委托往外传
                    OnReviceEvent(reciveData);

                    #region 心跳包响应
                    HeartSendCount = 0;
                    HeartBackCount = 0;

                    if (reciveData.IndexOf('\r') > -1)
                    {
                        reciveData = reciveData.Substring(0, reciveData.IndexOf('\r'));
                    }
                    CurRemotIp = curRemoteIpPort;
                    //如果收到心跳包数据,则发送心跳包响应数据
                    if (reciveData == "<" + curRemoteIpPort + ">" + heartRecvData)
                    {
                        string msg;
                        Send(curRemoteIpPort, heartBackData, null, true, out msg);
                    }
                    #endregion
                    //继续接收数据
                    SocketServer.BeginReceive(RecvBuffer, 0, MaxDataBuffer, SocketFlags.None, new AsyncCallback(ReceiveCallBack), SocketServer);
                }
            }
            catch (System.NullReferenceException ex)
            {
                string msg;
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("ReceiveCallBack:" + msg);
                Close();

                Open(localIp, localPort, out msg);
            }
            catch (ObjectDisposedException ex)
            {
                string msg;
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("ReceiveCallBack:" + msg);
                Close();

                Open(localIp, localPort, out msg);

            }
            catch (SocketException ex)
            {
                string msg = "";
                msg = "异常信息:" + ex.Message + "\r\n";
                msg += "异常类型:" + ex.GetType().ToString() + "\r\n";
                msg += "异常方法:" + ex.TargetSite + "\r\n";
                msg += "调用堆栈:" + ex.StackTrace + "\r\n";
                OnErrorEvent("ReceiveCallBack:" + msg);
                Close();

                Open(localIp, localPort, out msg);
            }
        }

        private class CurIpPortConverter : StringConverter
        {
            public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (context != null && context.Instance is TcpSer)
                {
                    List<string> values = new System.Collections.Generic.List<string>();

                    for (int i = 0; i < (context.Instance as TcpSer).lstListen.Count; i++)
                    {
                        values.Add((context.Instance as TcpSer).lstListen[i]);
                    }
                    return new StandardValuesCollection(values);
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
