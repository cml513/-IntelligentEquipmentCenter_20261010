using System;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MQTTnet.Packets;

namespace ClsComm
{

    [Serializable]
    [XmlRoot("MqttClt")]
    [XmlType("MqttClt")]
    public class MqttClt : Comm
    {
        private string ip;
        private int port;
        private string username;
        private string password;
        private string clientid;
        private List<string> topics;

        [XmlElement("IP"), Category("通讯参数"), DisplayName("IP地址")]
        public string IP
        {
            set
            {
                if (value != ip)
                {
                    ip = value;
                }
            }
            get { return ip; }
        }

        [XmlElement("Port"), Category("通讯参数"), DisplayName("端口号")]
        public int Port
        {
            set
            {
                if (value != port)
                {
                    port = value;
                }
            }
            get { return port; }
        }

        [XmlElement("Username"), Category("通讯参数"), DisplayName("用户名")]
        public string Username
        {
            set
            {
                if (value != username)
                {
                    username = value;
                }
            }
            get { return username; }
        }
        [XmlElement("Password"), Category("通讯参数"), DisplayName("密码")]
        public string Password
        {
            set
            {
                if (value != password)
                {
                    password = value;
                }
            }
            get { return password; }
        }
        [XmlElement("Clientid"), Category("通讯参数"), DisplayName("客户端ID")]
        public string Clientid
        {
            set
            {
                if (value != clientid)
                {
                    clientid = value;
                }
            }
            get { return clientid; }
        }

        [XmlArray("TopicList")]
        [XmlArrayItem("Topic", IsNullable = false), Category("订阅主题"), DisplayName("列表")]
        public List<string> Topics
        {
            get { return topics; }
            set { topics = value; }
        }

        [XmlElement("Topic"), Category("订阅主题"), DisplayName("主题")]
        public string Topic { get; set; } = "/data/DeviceID";

        [XmlElement("SyncInterval"), Category("同步间隔s"), DisplayName("同步间隔s")]
        public int SyncInterval { get; set; } =1;
        

        private MQTTnet.Client.MqttClient m_MqttClient;
        public MqttClt()
        {
            ip = "127.0.0.1";
            port = 5000;
            username = "null";
            password = "null";
            clientid = "1";
            topics = new List<string>();
        }

        ~MqttClt()
        {
            if (m_MqttClient != null)
            {
                m_MqttClient.Dispose();
            }
        }

        public override bool Connect(out string msg)
        {
            msg = "Ok";
            Task.Run(async () => { await ConnectServerAsync(); });
            return true;
        }

        public override bool Close()
        {
            Task.Run(async () => { await m_MqttClient.DisconnectAsync(); });
            IsConnecting = false;
            IsConnected = false;
            return true;
        }

        public override bool Send(string data, out string msg)
        {
            msg = "Ok";
            string[] ary_data = data.Split(';');
            if (ary_data.Length != 2)
            {
                msg = "数据格式异常：主题;内容";
                return false;
            }
            return Publish(ary_data[0], ary_data[1], out msg);
        }


        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="msg"> 返回信息</param>
        /// <returns>成功 true 失败 false</returns>
        public bool Subscribe(string topic, out string msg)
        {
            msg = "Ok";

            topic = topic.Trim();

            if (string.IsNullOrEmpty(topic))
            {
                msg = "订阅主题不能为空！";
                return false;
            }
            if (m_MqttClient == null)
            {
                msg = "MQTT客户端未实例化！";
                return false;
            }
            if (!m_MqttClient.IsConnected)
            {
                msg = "MQTT客户端尚未连接！";
                return false;
            }
            Task.Run(async () => { await Subscribe(topic); });
            return true;
        }

        /// <summary>
        /// 发布主题  内容为空删除主题的发布
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="content">内容</param>
        /// <param name="msg"></param>
        /// <param name="qos">服务质量</param>
        /// <param name="retain">保持</param>
        /// <returns></returns>
        public bool Publish(string topic, string content, out string msg, bool retain = false, MqttQualityOfServiceLevel qos = MqttQualityOfServiceLevel.ExactlyOnce)
        {
            msg = "Ok";

            topic = topic.Trim();

            if (string.IsNullOrEmpty(topic))
            {
                msg = "发布主题不能为空！";
                return false;
            }
            if (m_MqttClient == null)
            {
                msg = "MQTT客户端未实例化！";
                return false;
            }
            if (!m_MqttClient.IsConnected)
            {
                msg = "MQTT客户端尚未连接！";
                return false;
            }

            content = content.Trim();

            Task.Run(async () => { await Publish(topic, content, qos, retain); });
            return true;
        }

        private async Task ConnectServerAsync()
        {
            // Create a new MQTT client.
            try
            {
                if (m_MqttClient == null || m_MqttClient.IsConnected == false)
                {

                    var factory = new MqttFactory();
                    m_MqttClient = (MQTTnet.Client.MqttClient)factory.CreateMqttClient();

                    var options = new MqttClientOptionsBuilder().WithTcpServer(ip, port)
                       .WithCredentials(username, password)
                       .WithClientId(clientid) // Port is optional
                       .Build();
                    //设置超时时间
                    var timeoutToken = new CancellationTokenSource(TimeSpan.FromSeconds(2000));

                    IsConnecting = true;

                    //绑定回调函数
                    m_MqttClient.ConnectedAsync += ConnectedCallBack;
                    m_MqttClient.DisconnectedAsync += DisconnectedCallBack;
                    m_MqttClient.ApplicationMessageReceivedAsync += ApplicationMessageCallBack;
                    //两种方式
                    //mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(new Func<MqttClientConnectedEventArgs, Task>(OnConnected));
                    await m_MqttClient.ConnectAsync(options, timeoutToken.Token);

                }

            }
            catch (Exception ex)
            {
                OnErrorEvent("连接到MQTT服务器异常！" + ex.Message);

            }
        }
        private Task ConnectedCallBack(MqttClientConnectedEventArgs arg)
        {

            string msg;
            //OnConnectEvent(arg.ConnectResult.ReasonString);
            for (int i = 0; i < topics.Count; i++)
            {
                Subscribe(topics[i], out msg);
            }
            if (Topic!="")
            {
                Subscribe(Topic, out msg);
            }
            IsConnecting = false;
            IsConnected = true;
            OnConnectEvent("连接：" + arg.ConnectResult.ToString());
            return Task.CompletedTask;
        }
        private Task DisconnectedCallBack(MqttClientDisconnectedEventArgs arg)
        {

            if (IsConnected)
            {
                OnConnectEvent("尝试断线重连");

                var options = new MqttClientOptionsBuilder()
                     .WithClientId(clientid)
                     .WithTcpServer(ip, port)
                     .WithCredentials(username, password)
                     //.WithTls()
                     .WithCleanSession()
                     .Build();

                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    try
                    {
                        await m_MqttClient.ConnectAsync(options);
                    }
                    catch (Exception ex)
                    {
                        OnErrorEvent("重连异常：" + ex.Message);
                    }
                });
            }
            else
            {
                IsConnecting = false;
                IsConnected = false;
                if (arg.ConnectResult!=null)
                {
                    OnConnectEvent("断开：" + arg.ConnectResult.ToString());
                }        
            }
            return Task.CompletedTask;
        }
        private Task ApplicationMessageCallBack(MqttApplicationMessageReceivedEventArgs arg)
        {
            string Topic = arg.ApplicationMessage.Topic;
            string Payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            string QoS = arg.ApplicationMessage.QualityOfServiceLevel.ToString();
            string Retain = arg.ApplicationMessage.Retain.ToString();

            OnReviceEvent(Topic + "," + Payload);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 发布主题
        /// </summary>
        /// <param name="topic"> 主题</param>
        /// <param name="content">内容</param>
        /// <param name="qos">Mqtt 服务质量等级</param>
        /// <param name="retain">是否保留</param>
        /// <returns></returns>
        private async Task Publish(string topic, string content, MqttQualityOfServiceLevel qos, bool retain)
        {
            try
            {
                MqttApplicationMessageBuilder message = new MqttApplicationMessageBuilder()
                           .WithTopic(topic)
                           .WithPayload(content)
                           .WithRetainFlag(retain)
                           .WithQualityOfServiceLevel(qos);

                await m_MqttClient.PublishAsync(message.Build());
            }
            catch (Exception ex)
            {
                OnErrorEvent(topic + "主题发布异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic">主题</param>
        /// <returns></returns>
        private async Task Subscribe(string topic)
        {
            try
            {
                // Subscribe to a topic
                await m_MqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .WithAtMostOnceQoS()
                    .Build()
                    );
                //m_CurSubscribe = topic;
            }
            catch (Exception ex)
            {
                OnErrorEvent(topic + "主题订阅异常：" + ex.Message);
            }
        }
    }
    [Serializable]
    [XmlRoot("MqttSer")]
    [XmlType("MqttSer")]
    public class MqttSer : Comm
    {
        private string ip;
        private int port;
        private string username;
        private string password;

        private List<string> clientid;

        private List<TopicModel> allTopics;
        public List<TopicModel> AllTopics
        {
            get { return allTopics; }
            set
            {
                if (allTopics != value)
                {
                    allTopics = value;
                    this.OnPropertyChanged("AllTopics");
                }

            }
        }

        private List<string> allClients;
        public List<string> AllClients
        {
            get { return allClients; }
            set
            {
                if (allClients != value)
                {
                    allClients = value;
                    this.OnPropertyChanged("AllClients");
                }

            }
        }
        [XmlElement("IP"), Category("通讯参数"), DisplayName("IP地址")]
        public string IP
        {
            set
            {
                if (value != ip)
                {
                    ip = value;
                }
            }
            get { return ip; }
        }

        [XmlElement("Port"), Category("通讯参数"), DisplayName("端口号")]
        public int Port
        {
            set
            {
                if (value != port)
                {
                    port = value;
                }
            }
            get { return port; }
        }

        [XmlElement("Username"), Category("通讯参数"), DisplayName("账号")]
        public string Username
        {
            set
            {
                if (value != username)
                {
                    username = value;
                }
            }
            get { return username; }
        }
        [XmlElement("Password"), Category("通讯参数"), DisplayName("密码")]
        public string Password
        {
            set
            {
                if (value != password)
                {
                    password = value;
                }
            }
            get { return password; }
        }
        [XmlElement("Clientid"), Category("通讯参数"), DisplayName("用户ID")]
        public List<string> Clientid
        {
            set
            {
                if (value != clientid)
                {
                    clientid = value;
                }
            }
            get { return clientid; }
        }


        private MqttServer m_MqttServer;
        private async Task StartServerAsync()
        {
            try
            {
                if (m_MqttServer == null || m_MqttServer.IsStarted == false)
                {

                    var factory = new MqttFactory();

                    var options = new MqttServerOptionsBuilder()
                        .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Parse(ip))
                        .WithDefaultEndpointPort(port)
                        .WithDefaultCommunicationTimeout(TimeSpan.FromMilliseconds(2000))
                       .Build();

                    m_MqttServer = factory.CreateMqttServer(options);

                    //设置超时时间
                    IsConnecting = true;


                    IsConnecting = false;
                    IsConnected = true;
                    //绑定回调函数
                    m_MqttServer.StartedAsync += StartCallBack;//绑定服务端启动事件
                    m_MqttServer.StoppedAsync += StoppedCallBack;//绑定服务端停止事件
                    m_MqttServer.RetainedMessagesClearedAsync += RetainedMessagesCleareCallBack;//主题删除事件
                    m_MqttServer.RetainedMessageChangedAsync += RetainedMessageChangedCallBack;//主题改变事件
                    m_MqttServer.ClientConnectedAsync += ClientConnectedCallBack;//绑定客户端连接事件
                    m_MqttServer.ClientDisconnectedAsync += ClientDisconnectedCallBack;//绑定客户端断开事件
                    m_MqttServer.ClientSubscribedTopicAsync += ClientSubscribedTopicCallBack;//绑定客户端订阅主题事件
                    m_MqttServer.ClientUnsubscribedTopicAsync += ClientUnsubscribedTopicCallBack;//绑定客户端退订主题事件
                    m_MqttServer.ValidatingConnectionAsync += ValidatingConnectionCallBack;
                    await m_MqttServer.StartAsync();
                }

            }
            catch (Exception ex)
            {
                OnErrorEvent("MQTT服务器打开异常！" + ex.Message);

            }
        }

        public override bool Close()
        {
            Task.Run(async () => { await m_MqttServer.StopAsync(); });
            IsConnecting = false;
            IsConnected = false;
            return true;
        }

        private Task StartCallBack(EventArgs arg)
        {
            IsConnected = true;
            OnConnectEvent("服务端打开！");
            return Task.CompletedTask;
        }
        private Task StoppedCallBack(EventArgs arg)
        {
            OnConnectEvent("服务端关闭！");
            return Task.CompletedTask;
        }

        private Task RetainedMessagesCleareCallBack(EventArgs arg)
        {
            return Task.CompletedTask;
        }
        private Task RetainedMessageChangedCallBack(RetainedMessageChangedEventArgs arg)
        {
            return Task.CompletedTask;
        }
        private Task ClientConnectedCallBack(ClientConnectedEventArgs arg)
        {
            clientid.Add(arg.ClientId);
            OnConnectEvent($"客户端：{arg.ClientId}连接！");
            return Task.CompletedTask;
        }
        private Task ClientDisconnectedCallBack(ClientDisconnectedEventArgs arg)
        {
            clientid.Remove(arg.ClientId);
            var query = allTopics.Where(t => t.Clients.Contains(arg.ClientId));
            if (query.Any())
            {
                var tmp = query.ToList();
                foreach (var model in tmp)
                {
                    allTopics.Remove(model);
                    model.Clients.Remove(arg.ClientId);
                    allTopics.Add(model);
                }
            }

            OnConnectEvent($"客户端：{arg.ClientId}断开！");
            return Task.CompletedTask;
        }

        private Task ClientSubscribedTopicCallBack(ClientSubscribedTopicEventArgs arg)
        {
            if (allTopics.Any(t => t.TopicFilter.Topic == arg.TopicFilter.Topic))
            {
                TopicModel model = allTopics.First(t => t.TopicFilter.Topic == arg.TopicFilter.Topic);
                allTopics.Remove(model);
                model.Clients.Add(arg.ClientId);
                allTopics.Add(model);
            }
            else
            {
                TopicModel model = new TopicModel();
                model.Clients = new List<string> { arg.ClientId };
                model.TopicFilter = arg.TopicFilter;
                allTopics.Add(model);
            }

            return Task.CompletedTask;
        }
        private Task ClientUnsubscribedTopicCallBack(ClientUnsubscribedTopicEventArgs arg)
        {
            if (allTopics.Any(t => t.TopicFilter.Topic == arg.TopicFilter))
            {
                TopicModel model = allTopics.First(t => t.TopicFilter.Topic == arg.TopicFilter);
                allTopics.Remove(model);
                model.Clients.Remove(arg.ClientId);

                if (model.Clients.Count > 0)
                {
                    allTopics.Add(model);
                }
            }
            return Task.CompletedTask;
        }

        private Task ValidatingConnectionCallBack(ValidatingConnectionEventArgs arg)
        {
            //if (arg.ClientId != clientid)
            //{
            //    arg.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            //}

            if (arg.UserName != username)
            {
                arg.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }

            if (arg.Password != password)
            {
                arg.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            }

            return Task.CompletedTask;
        }

        public async void Publish(string topic, string payload, MqttQualityOfServiceLevel qos, bool retain)
        {
            try
            {
                MqttApplicationMessageBuilder message = new MqttApplicationMessageBuilder()
                           .WithTopic(topic)
                           .WithPayload(payload)
                           .WithRetainFlag(retain)
                           .WithQualityOfServiceLevel(qos);

                await m_MqttServer.UpdateRetainedMessageAsync(message.Build());
            }
            catch (Exception ex)
            {
                OnErrorEvent(topic + "主题发布异常：" + ex.Message);
            }
        }
    }

    public class TopicModel
    {
        public TopicModel()
        {
            clients = new List<string>();
            topicFilter = new MqttTopicFilter();

        }
        private MqttTopicFilter topicFilter;
        public MqttTopicFilter TopicFilter
        {
            get { return topicFilter; }
            set
            {
                if (topicFilter != value)
                {
                    topicFilter = value;
                }
            }
        }

        private List<string> clients;
        /// <summary>
        /// 订阅此主题的客户端
        /// </summary>
        public List<string> Clients
        {
            get { return clients; }
            set
            {
                if (clients != value)
                {
                    clients = value;
                }
            }
        }
    }
}