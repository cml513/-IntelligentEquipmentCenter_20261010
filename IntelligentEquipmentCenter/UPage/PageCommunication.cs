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
using Newtonsoft.Json.Linq;
using IntelligentEquipmentCenter.UControl;
using System.Xml;
using System.IO;
using System.Diagnostics;
using Sunny.UI.Win32;
using ClsComm;
using System.Threading;
using IntelligentEquipmentCenter.UiForm;
using static IntelligentEquipmentCenter.ClientJson;

namespace IntelligentEquipmentCenter.UPage
{
    public partial class PageCommunication : UserControl
    {
        //输入的IP端口
        public static List<Communication> saveIpPort = new List<Communication>();
        //输入的IP端口
        public static List<CommunicationSer> saveIpPortser = new List<CommunicationSer>();
        //已连接IP端口
        public static List<string[]> connectedIpPort = new List<string[]>();       
        //点击表格连接的行信息
        public string _ip;
        public string _port;
        public string _type;
        //点击连接的行号
        public int _index;
        //ID
        public int _count=100;
        
        //未编辑
        public delegate void ConnectPLC();
        public static event ConnectPLC _connectPLCevent;
        public struct Communication
        {
            public int ID;
            public string CommName;
            public string CommIP;
            public string CommPort;
            public string CommType;

        }
        public struct CommunicationSer
        {
            public string CommSerName;
            public string ComPort;
            public string BaudRate;
            public string DataBits;
            public string Parity;
            public string StopBits;
        }


        public static PageCommunication Instance { get; private set; }

        //软件启动时是否已自动连接TCP通讯，避免重复连接
        private bool _tcpAutoInited = false;

        public PageCommunication()
        {
            InitializeComponent();
            cbbType.SelectedIndex = 0;
            Instance = this;
                    
        }

        private void 断开连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void 开始连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connectedIpPort.Add(new string[] { _ip,_port });
        }

        //添加
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (saveIpPort?.Count > 4) return;
            
            if (txtName.Text.Trim().IsNullOrEmpty() || txtIP.Text.Trim().IsNullOrEmpty() || txtPort.Text.Trim().IsNullOrEmpty()|| cbbType.SelectedItem.ToString().IsNullOrEmpty())
            {
                MessageBox.Show("输入不能为空，请重新输入！");
            }
            else
            {
                Communication comm = new Communication();
                comm.ID = ++_count;
                comm.CommName = txtName.Text.Trim();
                comm.CommIP = txtIP.Text.Trim();
                comm.CommPort = txtPort.Text.Trim();
                comm.CommType = cbbType.SelectedItem.ToString();
                saveIpPort.Add(comm);
                dgvCommunication.Rows.Add(new object[] { comm.CommName, comm.CommType, comm.CommIP, comm.CommPort });
                SaveInfomation();
            }
            

        }

        //获取按钮所在行
        private void dgvCommunication_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
          
                if (dgvCommunication.CurrentCell.OwningColumn == dgvCommunication.Columns["Column8"])
                {
                    switch (e.RowIndex)
                    {
                        case 0:
                        if (dgvCommunication[4, 0].Style.BackColor != Color.Green) return;
                            Task.Run(new Action(() =>
                            {
                                Communic.RecfeedA = "";
                                Communic.TcpCltfeedingA.Send("start", out string MSG);
                                while (true)
                                {
                                    if (Communic.RecfeedA != "")
                                    {
                                        Logger.push(new Log(Logger.LogType.INFO, "EHomePage", Communic.RecfeedA));
                                        txt_result.Invoke(new Action(() =>
                                        {
                                            txt_result.Clear();
                                            txt_result.Text = "思谋扫码枪:"+Communic.RecfeedA;
                                        }));
                                        Communic.RecfeedA = "";
                                        break;
                                    }
                                    Thread.Sleep(10);
                                }
                            }));
                            break;
                        case 1:
                        if (dgvCommunication[4, 1].Style.BackColor != Color.Green) return;
                        Task.Run(new Action(() =>
                            {
                                Communic.RecfeedB = "";
                                Communic.TcpCltfeedingB.Send("PON", out string MSG);
                                while (true)
                                {
                                    if (Communic.RecfeedB != "")
                                    {
                                        Logger.push(new Log(Logger.LogType.INFO, "EHomePage", Communic.RecfeedB));
                                        txt_result.Invoke(new Action(() =>
                                        {
                                            txt_result.Clear();
                                            txt_result.Text = "得力捷扫码枪:" + Communic.RecfeedB;
                                        }));
                                        Communic.RecfeedB = "";
                                        break;
                                    }
                                    Thread.Sleep(10);
                                }
                            }));
                            break;
                        case 2:
                        if (dgvCommunication[4, 2].Style.BackColor != Color.Green) return;
                        Task.Run(new Action(() =>
                            {
                                Communic.RecfeedC = "";
                                Communic.TcpCltfeedingC.Send(" ", out string MSG);
                                while (true)
                                {
                                    if (Communic.RecfeedC != "")
                                    {
                                        Logger.push(new Log(Logger.LogType.INFO, "EHomePage", Communic.RecfeedC));
                                        txt_result.Invoke(new Action(() =>
                                        {
                                            txt_result.Clear();
                                            txt_result.Text = Communic.RecfeedC;
                                        }));
                                        Communic.RecfeedC = "";
                                        break;
                                    }
                                    Thread.Sleep(10);
                                }
                            }));
                            break;
                        case 3:
                        if (dgvCommunication[4, 3].Style.BackColor != Color.Green) return;
                        Task.Run(new Action(() =>
                            {
                                Communic.RecfeedD = "";
                                Communic.TcpCltfeedingD.Send(" ", out string MSG);
                                while (true)
                                {
                                    if (Communic.RecfeedD!= "")
                                    {
                                        Logger.push(new Log(Logger.LogType.INFO, "EHomePage", Communic.RecfeedD));
                                        txt_result.Invoke(new Action(() =>
                                        {
                                            txt_result.Clear();
                                            txt_result.Text = Communic.RecfeedD;
                                        }));
                                        Communic.RecfeedD = "";
                                        break;
                                    }
                                    Thread.Sleep(10);
                                }
                            }));
                            break;
                    }

                }

            if (dgvCommunication.CurrentCell.OwningColumn == dgvCommunication.Columns["Column4"])
            {
                switch (e.RowIndex)
                {
                    case 0:
                        ConIp(0);
                        break;
                    case 1:
                        ConIp(1);
                        break;
                    case 2:
                        ConIp(2);
                        break;
                    case 3:
                        ConIp(3);
                        break;
                }
            }

        }





        //tcp时才选择客户端或服务器
        private void cbbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbType.SelectedIndex==0)
            {
                cbbtcpType.Enabled = true;
            }
            else
            {
                cbbType.Enabled = false;
            }
        }

        void CurrentConnect()
        {
            if(_type.ToLower()=="tcp")
            {
                TcpCon tcp = new TcpCon();
                if (!tcp.server.IsStartListening)
                {
                    try
                    {
                        tcp.OpenServer(_ip, int.Parse(_port));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("尝试连接出现错误，原因：" + ex.Message);
                    }
                   

                }
            }
            else if(_type.ToLower()=="plc")
            {
                //连接代码
            }
           
            
        }
        void CheckReConnect()
        {
            foreach (string[] s in connectedIpPort)
            {
                if (s[2].ToLower()=="tcp")
                {
                    TcpCon tcp = new TcpCon();
                    if (!tcp.server.IsStartListening)
                    {
                        try
                        {
                            tcp.OpenServer(s[0], int.Parse(s[1]));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("尝试连接出现错误，原因："+ex.Message);
                        }
                       
                    }
                }
                else if (s[2].ToLower()=="plc")
                {
                    //连接代码
                }
            }
        }

        void SaveInfomation()
        {
           
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration dec = xmlDocument.CreateXmlDeclaration(version: "1.0", encoding: "utf-8", standalone: null);
            xmlDocument.AppendChild(dec);
            XmlElement xmlElement = xmlDocument.CreateElement(name: "IPPORT");
            foreach (Communication s in saveIpPort)
            {
                XmlElement xmlelement = xmlDocument.CreateElement(name: "device");
                xmlelement.SetAttribute("ID", s.ID.ToString());
                xmlelement.SetAttribute("ip",s.CommIP );
                xmlelement.SetAttribute("port", s.CommPort);
                xmlelement.SetAttribute("type", s.CommType);              
                xmlelement.SetAttribute("name", s.CommName);               
                xmlElement.AppendChild(xmlelement);
            }

            xmlDocument.AppendChild(xmlElement);
            xmlDocument.Save("xml/CommunicationInfo.xml");          
            
        }

        void SaveInfomationser()
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration dec = xmlDocument.CreateXmlDeclaration(version: "1.0", encoding: "utf-8", standalone: null);
            xmlDocument.AppendChild(dec);
            XmlElement xmlElement = xmlDocument.CreateElement(name: "IPPORT");
            foreach (CommunicationSer s in saveIpPortser)
            {
                XmlElement xmlelement = xmlDocument.CreateElement(name: "device");
                xmlelement.SetAttribute("CommSerName", s.CommSerName);
                xmlelement.SetAttribute("ComPort", s.ComPort);
                xmlelement.SetAttribute("BaudRate", s.BaudRate);
                xmlelement.SetAttribute("DataBits", s.DataBits);
                xmlelement.SetAttribute("Parity", s.Parity);
                xmlelement.SetAttribute("StopBits", s.StopBits);
                xmlElement.AppendChild(xmlelement);
            }

            xmlDocument.AppendChild(xmlElement);
            xmlDocument.Save("xml/CommunicationInfoser.xml");

        }
        void ReadInfoser()
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load("xml/CommunicationInfoser.xml");
                foreach (XmlElement Element in xmlDoc.GetElementsByTagName("device"))
                {

                    CommunicationSer s = new CommunicationSer();
                    s.CommSerName = Element.GetAttribute("CommSerName");
                    s.BaudRate = Element.GetAttribute("BaudRate");
                    s.ComPort = Element.GetAttribute("ComPort");
                    s.DataBits = Element.GetAttribute("DataBits");
                    s.Parity = Element.GetAttribute("Parity");
                    s.StopBits = Element.GetAttribute("StopBits");
                    saveIpPortser.Add(s);
                    uiDataGridView1.Rows.Add(new object[] { s.CommSerName, s.ComPort, s.BaudRate, s.DataBits,s.StopBits,s.Parity });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取IP端口文件失败，原因：" + ex.Message);
            }

        }



        void ReadInfo()
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load("xml/CommunicationInfo.xml");              
                foreach (XmlElement Element in xmlDoc.GetElementsByTagName("device"))
                {
                  
                    Communication s = new Communication();
                    s.ID = int.Parse(Element.GetAttribute("ID"));
                    s.CommIP = Element.GetAttribute("ip");
                    s.CommPort = Element.GetAttribute("port");
                    s.CommName = Element.GetAttribute("name");
                    s.CommType= Element.GetAttribute("type");
                    saveIpPort.Add(s);
                    dgvCommunication.Rows.Add(new object[] { s.CommName, s.CommType, s.CommIP, s.CommPort });
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("读取IP端口文件失败，原因："+ex.Message);
            }
            
        }

        private void PageCommunication_Load(object sender, EventArgs e)
        {
            //TCP通讯：软件启动时已自动连接，这里保证页面加载时也已连接（内部有去重保护）
            AutoConnectTcp();

            //串口通讯：打开通讯界面时连接
            if (File.Exists("xml/CommunicationInfoser.xml"))
            {
                ReadInfoser();
            }

            ConSerPort();
        }

        /// <summary>
        /// 软件启动时自动建立TCP通讯连接，无需手动打开通讯界面。
        /// </summary>
        public void AutoConnectTcp()
        {
            if (_tcpAutoInited) return;
            _tcpAutoInited = true;

            //读取TCP的IP端口信息
            if (File.Exists("xml/CommunicationInfo.xml"))
            {
                ReadInfo();
            }

            ConIp();
        }

        //添加信息
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (saveIpPortser?.Count > 4) return;
            if (Com_CommSerName.Text.Trim().IsNullOrEmpty() || Comport.Text.Trim().IsNullOrEmpty() || ComStopBits.Text.Trim().IsNullOrEmpty() || ComParity.SelectedItem.ToString().IsNullOrEmpty()||
                ComDataBits.Text.Trim().IsNullOrEmpty()|| ComBaudRate.Text.Trim().IsNullOrEmpty())
            {
                MessageBox.Show("输入不能为空，请重新输入！");
            }
            else
            {
                CommunicationSer s = new CommunicationSer();
                s.CommSerName = Com_CommSerName.Text;
                s.BaudRate = ComBaudRate.Text;
                s.ComPort = Comport.Text;
                s.DataBits = ComDataBits.Text;
                s.Parity = ComParity.Text;
                s.StopBits = ComStopBits.Text;
                saveIpPortser.Add(s);
                uiDataGridView1.Rows.Add(new object[] { s.CommSerName, s.ComPort, s.BaudRate, s.DataBits, s.StopBits, s.Parity });
                SaveInfomationser();
            }
        }

        public void ConIp()
        {
            if (saveIpPort == null || saveIpPort.Count == 0) return;
            for (int i = 0; i < saveIpPort.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        Communic.TcpCltfeedingA.RemotIp = saveIpPort[i].CommIP;
                        Communic.TcpCltfeedingA.RemotPort =Convert.ToInt32( saveIpPort[i].CommPort);
                        Communic.TcpCltfeedingA.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL1);
                        if (Communic.TcpCltfeedingA.Connect(out string msg))
                        {
                            dgvCommunication[4, 0].Style.BackColor = Color.Green;
                           EHomePage.visionbool = true;
                        }
                        else
                        {
                            dgvCommunication[4, 0].Style.BackColor = SystemColors.Control;
                            EHomePage.visionbool = false;
                        }  
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName+ msg));
                        break;
                    case 1:
                        Communic.TcpCltfeedingB.RemotIp = saveIpPort[i].CommIP;
                        Communic.TcpCltfeedingB.RemotPort = Convert.ToInt32(saveIpPort[i].CommPort);
                        Communic.TcpCltfeedingB.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL2);
                        if (Communic.TcpCltfeedingB.Connect(out string msg2))
                        {
                            dgvCommunication[4, 1].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            dgvCommunication[4, 1].Style.BackColor = SystemColors.Control;
                        }
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName + msg2));
                        break;
                    case 2:
                        Communic.TcpCltfeedingC.RemotIp = saveIpPort[i].CommIP;
                        Communic.TcpCltfeedingC.RemotPort = Convert.ToInt32(saveIpPort[i].CommPort);
                        Communic.TcpCltfeedingC.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL3);
                        if (Communic.TcpCltfeedingC.Connect(out string msg3))
                        {
                            dgvCommunication[4, 2].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            dgvCommunication[4, 2].Style.BackColor = SystemColors.Control;
                        }
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName + msg3));
                        break;
                    case 3:
                        Communic.TcpCltfeedingD.RemotIp = saveIpPort[i].CommIP;
                        Communic.TcpCltfeedingD.RemotPort = Convert.ToInt32(saveIpPort[i].CommPort);
                        Communic.TcpCltfeedingD.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL4);
                        if (Communic.TcpCltfeedingD.Connect(out string msg4))
                        {
                            dgvCommunication[4, 3].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            dgvCommunication[4, 3].Style.BackColor = SystemColors.Control;
                        }
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName + msg4));
                        break;
                }
            }
        }

        public void ConIp(int i)
        {
                switch (i)
                {
                    case 0:
                    //if (dgvCommunication[4, 0].Style.BackColor == Color.Green) return;
                     Communic.TcpCltfeedingA.RemotIp = saveIpPort[i].CommIP;
                     Communic.TcpCltfeedingA.RemotPort = Convert.ToInt32(saveIpPort[i].CommPort);
                    if (Communic.TcpCltfeedingA.Connect(out string msg))
                    {
                        dgvCommunication[4, 0].Style.BackColor = Color.Green;
                        EHomePage.visionbool = true;
                    }
                    else
                    {
                        dgvCommunication[4, 0].Style.BackColor = SystemColors.Control;
                        Communic.TcpCltfeedingA.IsConnected = false;
                        EHomePage.visionbool = false;
                    }
                    Communic.TcpCltfeedingA.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL1);
                    
                     Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName + msg));
                     break;
                    case 1:
                    //if (dgvCommunication[4, 1].Style.BackColor == Color.Green) return;
                        Communic.TcpCltfeedingB.RemotIp = saveIpPort[i].CommIP;
                        Communic.TcpCltfeedingB.RemotPort = Convert.ToInt32(saveIpPort[i].CommPort);
                    if (Communic.TcpCltfeedingB.Connect(out string msg2))
                    {
                        dgvCommunication[4, 1].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        dgvCommunication[4, 1].Style.BackColor = SystemColors.Control;
                        Communic.TcpCltfeedingB.IsConnected = false;
                    }
                    Communic.TcpCltfeedingB.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL2);
                       
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName + msg2));
                        break;
                    case 2:
                    //if (dgvCommunication[4, 2].Style.BackColor == Color.Green) return;
                    Communic.TcpCltfeedingC.RemotIp = saveIpPort[i].CommIP;
                        Communic.TcpCltfeedingC.RemotPort = Convert.ToInt32(saveIpPort[i].CommPort);
                    if (Communic.TcpCltfeedingC.Connect(out string msg3))
                    {
                        dgvCommunication[4, 2].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        dgvCommunication[4, 2].Style.BackColor = SystemColors.Control;
                        Communic.TcpCltfeedingC.IsConnected = false;
                    }
                    Communic.TcpCltfeedingC.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL3);
                     
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName + msg3));
                        break;
                    case 3:
                    //if (dgvCommunication[4, 3].Style.BackColor == Color.Green) return;
                    Communic.TcpCltfeedingD.RemotIp = saveIpPort[i].CommIP;
                        Communic.TcpCltfeedingD.RemotPort = Convert.ToInt32(saveIpPort[i].CommPort);
                    if (Communic.TcpCltfeedingD.Connect(out string msg4))
                    {
                        dgvCommunication[4, 3].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        dgvCommunication[4, 3].Style.BackColor = SystemColors.Control;
                        Communic.TcpCltfeedingD.IsConnected = false;
                    }
                    Communic.TcpCltfeedingD.ReviceEvent += new ClsComm.Comm.DelegateBackMsg(ReviceEventAttachL4);
                       
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[i].CommName + msg4));
                        break;
                }
            
        }

        private string ReviceEventAttachL4(string msg)
        {
            try
            {
                Communic.RecfeedD = msg;
                Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[3].CommName + msg));
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string ReviceEventAttachL3(string msg)
        {
            try
            {
                Communic.RecfeedC = msg;
                Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[2].CommName + msg));
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string ReviceEventAttachL2(string msg)
        {
            try
            {
               
                Communic.RecfeedB = msg;
                Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[1].CommName + msg));
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string ReviceEventAttachL1(string msg)
        {
            try
            {
                Communic.RecfeedA = msg;
                Communic.RecfeedAQueue.Enqueue(msg);
                Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPort[0].CommName + msg));
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void ConSerPort()
        {
            if (saveIpPortser == null || saveIpPortser.Count == 0) return;
            for (int i = 0; i < saveIpPortser.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        //初始化串口
                        Serport.ComE50C.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50C.BaudRate =Convert.ToInt16( saveIpPortser[i].BaudRate);
                        Serport.ComE50C.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits); 
                        Serport.ComE50C.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50C.StopBits = GetStopBits(saveIpPortser[i].StopBits);
                        if (Serport.ComE50C.Connect(out string msg))
                        {
                            uiDataGridView1[6, 0].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 0].Style.BackColor = SystemColors.Control;
                        }
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg));
                        Serport.ComE50C.ReviceEvent += ComE50C_ReviceEvent;
                        break;
                    case 1:
                        //初始化串口
                        Serport.ComE50D.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50D.BaudRate = Convert.ToInt16(saveIpPortser[i].BaudRate);
                        Serport.ComE50D.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits);
                        Serport.ComE50D.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50D.StopBits = GetStopBits(saveIpPortser[i].StopBits);
                        if (Serport.ComE50D.Connect(out string msg2))
                        {
                            uiDataGridView1[6, 1].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 1].Style.BackColor = SystemColors.Control;
                        }
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg2));
                        Serport.ComE50D.ReviceEvent += ComE50D_ReviceEvent;
                        break;
                    case 2:
                        //初始化串口
                        Serport.ComE50E.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50E.BaudRate = Convert.ToInt16(saveIpPortser[i].BaudRate);
                        Serport.ComE50E.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits);
                        Serport.ComE50E.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50E.StopBits = GetStopBits(saveIpPortser[i].StopBits);
                   
                        if (Serport.ComE50E.Connect(out string msg3))
                        {
                            uiDataGridView1[6, 2].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 2].Style.BackColor = SystemColors.Control;
                        }

                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg3));
                        Serport.ComE50E.ReviceEvent += ComE50E_ReviceEvent;
                        break;
                    case 3:
                        //初始化串口
                        Serport.ComE50F.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50F.BaudRate = Convert.ToInt16(saveIpPortser[i].BaudRate);
                        Serport.ComE50F.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits);
                        Serport.ComE50F.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50F.StopBits = GetStopBits(saveIpPortser[i].StopBits);
               
                        if (Serport.ComE50F.Connect(out string msg4))
                        {
                            uiDataGridView1[6, 3].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 3].Style.BackColor = SystemColors.Control;
                        }

                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg4));
                        Serport.ComE50F.ReviceEvent += ComE50F_ReviceEvent;
                        break;
                }
            }
        }

        public void ConSerPort(int i)
        {
                switch (i)
                {
                    case 0:
                    //初始化串口
                    if (uiDataGridView1[6, 0].Style.BackColor == Color.Green) return;
                        Serport.ComE50C.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50C.BaudRate = Convert.ToInt16(saveIpPortser[i].BaudRate);
                        Serport.ComE50C.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits);
                        Serport.ComE50C.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50C.StopBits = GetStopBits(saveIpPortser[i].StopBits);
                        if (Serport.ComE50C.Connect(out string msg))
                        {
                            uiDataGridView1[6, 0].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 0].Style.BackColor = SystemColors.Control;
                        
                    }
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg));
                        Serport.ComE50C.ReviceEvent += ComE50C_ReviceEvent;
                        break;
                    case 1:
                    //初始化串口
                    if (uiDataGridView1[6, 1].Style.BackColor == Color.Green) return;
                    Serport.ComE50D.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50D.BaudRate = Convert.ToInt16(saveIpPortser[i].BaudRate);
                        Serport.ComE50D.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits);
                        Serport.ComE50D.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50D.StopBits = GetStopBits(saveIpPortser[i].StopBits);
                        if (Serport.ComE50D.Connect(out string msg2))
                        {
                            uiDataGridView1[6, 1].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 1].Style.BackColor = SystemColors.Control;
             
                    }
                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg2));
                        Serport.ComE50D.ReviceEvent += ComE50D_ReviceEvent;
                        break;
                    case 2:
                    //初始化串口
                    if (uiDataGridView1[6, 2].Style.BackColor == Color.Green) return;
                    Serport.ComE50E.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50E.BaudRate = Convert.ToInt16(saveIpPortser[i].BaudRate);
                        Serport.ComE50E.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits);
                        Serport.ComE50E.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50E.StopBits = GetStopBits(saveIpPortser[i].StopBits);

                        if (Serport.ComE50E.Connect(out string msg3))
                        {
                            uiDataGridView1[6, 2].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 2].Style.BackColor = SystemColors.Control;
                     
                    }

                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg3));
                        Serport.ComE50E.ReviceEvent += ComE50E_ReviceEvent;
                        break;
                    case 3:
                    if (uiDataGridView1[6, 3].Style.BackColor == Color.Green) return;
                    //初始化串口
                    Serport.ComE50F.ComPort = saveIpPortser[i].ComPort;
                        Serport.ComE50F.BaudRate = Convert.ToInt16(saveIpPortser[i].BaudRate);
                        Serport.ComE50F.DataBits = Convert.ToInt16(saveIpPortser[i].DataBits);
                        Serport.ComE50F.Parity = GetParity(saveIpPortser[i].Parity);
                        Serport.ComE50F.StopBits = GetStopBits(saveIpPortser[i].StopBits);

                        if (Serport.ComE50F.Connect(out string msg4))
                        {
                            uiDataGridView1[6, 3].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            uiDataGridView1[6, 3].Style.BackColor = SystemColors.Control;
                 
                    }

                        Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[i].CommSerName + msg4));
                        Serport.ComE50F.ReviceEvent += ComE50F_ReviceEvent;
                        break;
                }
           
        }
        private string ComE50F_ReviceEvent(string msg)
        {
            Serport.RecE50F = "";
            string[] hexbytes = msg.Split('-');
            byte[] bytes = new byte[hexbytes.Length];
            for (int i = 0; i < hexbytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexbytes[i], 16);
            }
            string s = Encoding.ASCII.GetString(bytes);
            if (s.Contains("\r\n") )
            {
                Serport.RecE50F = s.Substring(0, s.IndexOf("\r\n"));
            }
            else
            {
                Serport.RecE50F = "";
            }
           
            Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[3].CommSerName + Serport.RecE50F));
            return "";
        }

        private string ComE50E_ReviceEvent(string msg)
        {
            Serport.RecE50E = "";
            string[] hexbytes = msg.Split('-');
            byte[] bytes = new byte[hexbytes.Length];
            for (int i = 0; i < hexbytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexbytes[i], 16);
            }
            string s = Encoding.ASCII.GetString(bytes);
            if (s.Contains("\r\n"))
            {
                Serport.RecE50E = s.Substring(0, s.IndexOf("\r\n"));
            }
            else
            {
                Serport.RecE50E = "";
            }
            Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[2].CommSerName + Serport.RecE50E));
            return "";
        }

        private string ComE50D_ReviceEvent(string msg)
        {
            Serport.RecE50D = "";
            string[] hexbytes = msg.Split('-');
            byte[] bytes = new byte[hexbytes.Length];
            for (int i = 0; i < hexbytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexbytes[i], 16);
            }
            string s = Encoding.ASCII.GetString(bytes);
            if (s.Contains("\r\n"))
            {
                string s1 = s.Substring(0, s.IndexOf("\r\n"));
                Serport.RecE50C = s1;
            }
            else
            {
                Serport.RecE50C = s;
            }
            Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[1].CommSerName + Serport.RecE50D));
            return "";
        }

        private string ComE50C_ReviceEvent(string msg)
        {
            Serport.RecE50C = "";
            string[] hexbytes = msg.Split('-');
            byte[] bytes = new byte[hexbytes.Length];
            for (int i = 0; i < hexbytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexbytes[i], 16);
            }
            string s = Encoding.ASCII.GetString(bytes);
            if (s.Contains("\r"))
            {
                string s1 = s.Substring(0, s.IndexOf("\r"));
                Serport.RecE50C = s1.Substring(1);
            }
            else
            {
                Serport.RecE50C = "";
            }
            Logger.push(new Log(Logger.LogType.INFO, "PageCommunication", saveIpPortser[0].CommSerName + Serport.RecE50C));
            return "";
        }

        public System.IO.Ports.Parity GetParity(string msg)
        {
            if (msg== "Even")
            {
                return System.IO.Ports.Parity.Even;
            }
            else if (msg=="Odd")
            {
                return System.IO.Ports.Parity.Odd;
            }
            else
            {
                return System.IO.Ports.Parity.None;
            }
        }

        public System.IO.Ports.StopBits GetStopBits(string msg)
        {
            if (msg == "One")
            {
                return System.IO.Ports.StopBits.One;
            }
            else
            {
                return System.IO.Ports.StopBits.Two;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Communic.TcpCltfeedingA.Send("start",out string MSG);
        }

        //双击事件
        private void uiDataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (uiDataGridView1.CurrentCell.OwningColumn == uiDataGridView1.Columns["Column9"])
            {
                switch (e.RowIndex)
                {
                    case 0:
                        ConSerPort(0);
                        break;
                    case 1:
                        ConSerPort(1);
                        break;
                    case 2:
                        ConSerPort(2);
                        break;
                    case 3:
                        ConSerPort(3);
                        break;
                }
            }
        }

        private void 自动运行PLC触发地址ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          


        }

        //删除某行
        private void btnDel_Click(object sender, EventArgs e)
        {
     
            if (saveIpPort.Count>0&& dgvCommunication.CurrentCell.RowIndex>=0&&
                dgvCommunication.CurrentRow!=null)
            {
              DialogResult dialog=  MessageBox.Show("确定删除当前选中行吗？", "警告", MessageBoxButtons.OKCancel);
                if (dialog==DialogResult.OK)
                {
                    saveIpPort.RemoveAt(dgvCommunication.CurrentCell.RowIndex);
                    dgvCommunication.Rows.RemoveAt(dgvCommunication.CurrentCell.RowIndex);
                    //保存数据
                    SaveInfomation();
                    //读取
                    // ReadInfo();
                }
            }
        }

        //助手
        private void uiButton4_Click(object sender, EventArgs e)
        {
            Process.Start("exe\\串口协议调试工具1.0.exe");
        }

        
        private void btn_Del2_Click(object sender, EventArgs e)
        {
            if (saveIpPortser.Count > 0 && uiDataGridView1.CurrentCell.RowIndex >= 0 &&
              uiDataGridView1.CurrentRow != null)
            {
                DialogResult dialog = MessageBox.Show("确定删除当前选中行吗？", "警告", MessageBoxButtons.OKCancel);
                if (dialog == DialogResult.OK)
                {
                    saveIpPortser.RemoveAt(uiDataGridView1.CurrentCell.RowIndex);
                    uiDataGridView1.Rows.RemoveAt(uiDataGridView1.CurrentCell.RowIndex);
                    //保存数据
                    SaveInfomationser();
                    //读取
                    // ReadInfo();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          
        }
    }
}
