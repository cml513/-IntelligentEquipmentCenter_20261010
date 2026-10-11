using IntelligentEquipmentCenter.UControl;
using NetWorkHelper.ICommond;
using NetWorkHelper.IModels;
using NetWorkHelper.TCP;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    internal class TcpCon
    {
        //服务器
        public ITcpServer server = new ITcpServer();
        //连上服务器的客户端
        public IClient Client = new IClient();
        //接收到的数据
        public string Data;
        //接收数据的动态
        public bool ReceiveState;
        //服务器状态信息
        public string ServerState;
        //客户端状态
        public bool ClientState;
        //记录端口
        public int _port;

        public delegate void EventHandleData(string s);

        public static event EventHandleData HandleData2d;

        public static event EventHandleData HandleData3d;
        
        //服务器开启
        public void OpenServer(string ip,int port)
        {
           
            closeAllCommunication(server);

            server.ServerIp = ip;

            server.ServerPort = port;

            _port = port;

            server.CheckTime = 2000;

            server.OnRecevice += Server_OnRecevice;

            server.OnStateInfo += Server_OnStateInfo;

            server.OnOnlineClient += Server_OnOnlineClient;

            server.OnOfflineClient += Server_OnOfflineClient;

            

            server.Start();

        }

        //开启前清理
        private void closeAllCommunication(ITcpServer server)
        {

            if (server.IsStartListening)
            {
                server.Stop();
            }
            server.OnRecevice -= Server_OnRecevice;

            server.OnStateInfo -= Server_OnStateInfo;

            server.OnOnlineClient -= Server_OnOnlineClient;

            server.OnOfflineClient -= Server_OnOfflineClient;

        }

        //监控客户端下线
        private void Server_OnOfflineClient(object sender, TcpServerClientEventArgs e)
        {
            ClientState = false;
        }

        //监控服务器接收数据
        private void Server_OnRecevice(object sender, TcpServerReceviceaEventArgs e)
        {
            //更新接收动态
            ReceiveState = true;
            
            //接收到的数据
            Data = System.Text.Encoding.UTF8.GetString(e.Data);

            if (_port == 5000)
            {
                HandleData2d(Data);         
            }
            else
            {
                HandleData3d(Data);
            }
        }

        //服务器状态
        private void Server_OnStateInfo(object sender, TcpServerStateEventArgs e)
        {
            ServerState = e.Msg;
        }

        //监控客户端上线
        private void Server_OnOnlineClient(object sender, TcpServerClientEventArgs e)
        {

            Client = e.IClient;

            ClientState = true;

        }
    }
}
