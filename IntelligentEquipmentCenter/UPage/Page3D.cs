using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetWorkHelper.TCP;
using NetWorkHelper.IModels;
using NetWorkHelper.ICommond;

namespace IntelligentEquipmentCenter.UPage
{
    public partial class Page3D : UserControl
    {
        private static ITcpServer server = new ITcpServer();
        private static IClient Client2D = new IClient();
        public static string[] ReceiveData = new string[30];
        public Page3D()
        {
            InitializeComponent();

        }

        private void OpenServer()
        {
            string IP = "192.168.1.100";
            string Port = "4000";
            server.ServerIp = IP;
            server.ServerPort = int.Parse(Port);
            server.OnRecevice += Server_OnRecevice;
            server.OnStateInfo += Server_OnStateInfo;
            server.OnOnlineClient += Server_OnOnlineClient;
            server.OnOfflineClient += Server_OnOfflineClient;
            server.Start();
        }
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

        private void Server_OnOfflineClient(object sender, TcpServerClientEventArgs e)
        {
            //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} 2d客户端下线！", DateTime.Now));
        }

        private void Server_OnRecevice(object sender, TcpServerReceviceaEventArgs e)
        {
            //接收到的数据
            string MSG = e.Data.ToString();
            //数据处理
            HandleData(MSG);
            //发送消息
            string sendMsg = "0";
            server.SendData(Client2D, sendMsg);
        }

        private void Server_OnStateInfo(object sender, TcpServerStateEventArgs e)
        {
            //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} {1}", DateTime.Now, e.Msg));
        }

        private void Server_OnOnlineClient(object sender, TcpServerClientEventArgs e)
        {
            Client2D = e.IClient;
            //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} 2d客户端上线！", DateTime.Now));
        }

        private void HandleData(string msg)
        {
            if (!msg.IsNullOrEmpty())
            {
                try
                {
                    msg = msg.Trim();
                    string[] Msg = msg.Split(';');
                    foreach (string s in Msg)
                    {
                        string[] m = s.Split(",");
                        switch (m[0])
                        {

                            case "M1":
                                ReceiveData[0] = m[1];
                                break;
                            case "测试":
                                ReceiveData[22] = m[1];
                                break;
                            case "M17":
                                ReceiveData[23] = m[1];
                                break;
                            case "M22":
                                ReceiveData[24] = m[1];
                                break;
                            case "M5":
                                ReceiveData[25] = m[1];
                                break;
                            case "M4":
                                ReceiveData[26] = m[1];
                                break;
                            case "M21":
                                ReceiveData[26] = m[1];
                                break;
                            case "M3":
                                ReceiveData[26] = m[1];
                                break;
                            case "M11":
                                ReceiveData[26] = m[1];
                                break;
                            case "M12":
                                ReceiveData[26] = m[1];
                                break;
                            case "M15":
                                ReceiveData[26] = m[1];
                                break;
                            case "M24":
                                ReceiveData[26] = m[1];
                                break;
                            case "M20":
                                ReceiveData[26] = m[1];
                                break;
                            case "M14":
                                ReceiveData[26] = m[1];
                                break; 
                            case "M13":
                                ReceiveData[26] = m[1];
                                break; 
                            case "M8":
                                ReceiveData[26] = m[1];
                                break;
                            case "M18":
                                ReceiveData[26] = m[1];
                                break; 
                            case "M26":
                                ReceiveData[26] = m[1];
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("解析数据发生错误，原因：" + ex.Message);
                }

            }
            else
            {
                MessageBox.Show("数据为空，请检查是否收到的数据");
            }

        }

        
    }
}
