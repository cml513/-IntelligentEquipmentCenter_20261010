using IntelligentEquipmentCenter.UControl;
using IntelligentEquipmentCenter.UPage;
using MisubKaiFa.CSDLL;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WebAPI编写;
using static IntelligentEquipmentCenter.ClientJson;


namespace IntelligentEquipmentCenter.UiForm
{
    public partial class MainWindow : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public static bool[] 读M_part1 = new bool[1000];

        public static bool[] 读M = new bool[1000];
        public static int[] 读D_16位 = new int[2000];

        public static bool mesCon=false;

        //PLC
        private string modbusIp = "";
        private string modbusPort = "";
        private int indexStart = 1;
        private int indexPause = 2;
        private int indexReset = 3;
        public static string title = "XXXX自动线";

        private LoginDialog loginDialog;

        
        public MainWindow()
        {
            InitializeComponent();
            Logger.Start();
            Logger.push(new Log(Logger.LogType.INFO, "mainwindow", "打开软件成功"));
            loginDialog = new LoginDialog();
            loginDialog.FormBorderStyle = FormBorderStyle.Sizable;


            #region 加载Page界面

            //加载主页面page
            {
                EHomePage instance = CGenericSingletonU<EHomePage>.CreateInstrance();
                instance.Dock = DockStyle.Fill;

                PublicPanel.Controls.Add(instance);
                PublicPanel.Controls["EHomePage"].BringToFront();
            }

            //加载page
            {

                Frm_Repice instance = CGenericSingletonU<Frm_Repice>.CreateInstrance();
                instance.Dock = DockStyle.Fill;
                PublicPanel.Controls.Add(instance);

            }

            //加载数据查询page
            {
                FProductionInfos instance = CGenericSingletonU<FProductionInfos>.CreateInstrance();
                instance.Dock = DockStyle.Fill;
                PublicPanel.Controls.Add(instance);
            }


            #endregion


            timer1.Start();
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            #region 加载配置文件

            //读取配方
            if (!Recipedata.Load(out string msg))
            {
                MessageBox.Show("配方：" + msg);
            }
            Frm_Repice.names = Recipedata.GetList;

            if (File.Exists(PathConfig.Instance.ConfigPath))
            {
                Config.Instance = JsonFile.ReadJsonFile<Config>(PathConfig.Instance.ConfigPath);
            }
            else
            {
                JsonFile.WriteJsonFile<Config>(PathConfig.Instance.ConfigPath, Config.Instance);
            }
            if (File.Exists(PathConfig.Instance.ProjectPath))
            {
                Project.Instance = JsonFile.ReadJsonFile<Project>(PathConfig.Instance.ProjectPath);
            }
            else
            {
                JsonFile.WriteJsonFile<Project>(PathConfig.Instance.ProjectPath, Project.Instance);
            }


            #endregion


            //软件启动时自动建立TCP通讯连接（无需手动打开通讯界面）
            PageCommunication.Instance?.AutoConnectTcp();

            //加载IP地址
            ReadPLCinfo();

            //连接PLC
            bool ret = ModbusClass.ConnectToServer(modbusIp, int.Parse(modbusPort));

            if (ret)
            {
                //setPlcStatus(true);
                Thread thread = new Thread(readPlcValueThread);
                thread.IsBackground = true;
                thread.Start();
            }



          
            //配方加载后，刷新主页数据库列和显示列
            CGenericSingletonU<EHomePage>.CreateInstrance().RefreshRecipeConfig();
      

            new Auto().SetMeAutoStart();

            //接口服务
            //this.httpServer = new HttpServerAPI();
            //this.httpServer.HandleRequestFunc = HandleRequest;
            //this.httpServer.Start(8002);


            btn_close.FlatStyle= FlatStyle.Flat;
            btn_close.FlatAppearance.BorderColor = btn_close.BackColor;
            btn_close.FlatAppearance.BorderSize = 1;

            btn_min.FlatStyle = FlatStyle.Flat;
            btn_min.FlatAppearance.BorderColor = btn_min.BackColor;
            btn_min.FlatAppearance.BorderSize = 1;

            btn_max.FlatStyle = FlatStyle.Flat;
            btn_max.FlatAppearance.BorderColor = btn_max.BackColor;
            btn_max.FlatAppearance.BorderSize = 1;

            lt_PLC.ClickEvent += Lt_PLC_ClickEvent1;
        }


        private void Lt_PLC_ClickEvent(string name)
        {
   
        }

        public string PlcType = "";
        //加载IP端口
        void ReadPLCinfo()
        {
            //读取文件
            string filePath = "cfg\\plc.cfg";
            StreamReader reader = new StreamReader(filePath);
            string str = reader.ReadToEnd();
            reader.Close();
            //解析文件并赋值给变量
            string IP = matchInfo(str, "IP");
            string PORT = matchInfo(str, "PORT");
            PlcType= matchInfo(str, "PLCType");
            modbusIp = IP;
            modbusPort = PORT;

            indexStart = matchInfo(str, "Start").ToInt();
            indexPause = matchInfo(str, "Pause").ToInt();
            indexReset = matchInfo(str, "Reset").ToInt();

            title = matchInfo(str, "name");
            txtTitleLabel.Text = title;

           // Logger.push(new Log(Logger.LogType.INFO, "mainwindow", "加载plc.cfg完成"));
        }


        //正则匹配
        private string matchInfo(string str, string target)
        {
            Regex reg = new Regex(target + "=\"(.+)\"");
            Match match = reg.Match(str);
            return match.Groups[1].Value;
        }


        public static bool isPLCconnet = false;


        private void readPlcValue()
        {
          
            读M_part1 = ModbusClass.ReadCoils(0, 1000);

            Array.Copy(读M_part1, 0, 读M, 0, 读M_part1.Length);

        }
        private void readPlcValueThread()
        {
            //NormalPageX pageX = null;
            bool plcStatus = false;//PLC连接状态
            bool autoStatus = true;//自动运行状态

            // bool tag = false;
    

            
            DateTime curDateTag = DateTime.Now;
            DateTime curDate = DateTime.Now;
            TimeSpan curTimeSpan = TimeSpan.Zero;

            while (true)
            {
                if (!ModbusClass.isConnected())
                {
                    ModbusClass.DisconnectForServer();
                    ReadPLCinfo();
                    ModbusClass.ConnectToServer(modbusIp, int.Parse(modbusPort));
                    Thread.Sleep(100);
                    if(!ModbusClass.isConnected())
                    {
                        plcStatus = false;
                    }
                    Console.WriteLine("连接断开");
                    if (lt_PLC.InvokeRequired)
                        lt_PLC.Invoke(new Action(() => lt_PLC.lightMode = false));
                    else
                        lt_PLC.lightMode = false;
                }
                else 
                {
                    try
                    {
                        //更新界面连接状态
                        if(plcStatus==false)
                        {
                            plcStatus = true;
                        }

                        if (!ModbusClass.ReadCoils(900, 1)[0])
                            ModbusClass.WriteSingleCoil(900, true);//心跳M50
                        Thread.Sleep(10);
                        if (!ModbusClass.ReadCoils(900, 1)[0])
                        {
                            ModbusClass.DisconnectForServer();
                            ReadPLCinfo();
                            ModbusClass.ConnectToServer(modbusIp, int.Parse(modbusPort));
                            continue;
                        }
                        //读取PLC所有寄存器和线圈
                        readPlcValue();
                        if (lt_PLC.InvokeRequired)
                            lt_PLC.Invoke(new Action(() => lt_PLC.lightMode = true));
                        else
                            lt_PLC.lightMode = true;

                        //更新手动连接状态
                        int status1 = ModbusClass.ReadHoldingRegisters(0, 1)[0];

                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        Logger.push(new Log(Logger.LogType.ERROR, "MainWindow", "PLC通讯异常：" + ex.Message));
                    }


                }


            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
         
            DialogResult res = MessageBox.Show("确定要退出程序吗？","确定退出",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (res == DialogResult.No)
            {
                    e.Cancel = true;
            }
            else
            {
                ModbusClass.WriteCoil(900,false);
                EHomePage.InitThread = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            //权限,在非自动运行状态下
            toolStripTimeLabel.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            lt_mes.lightMode = EHomePage.visionbool;
            lt_vision.lightMode = EHomePage.visionbool;

        }

        private void txtTitleLabel_Click(object sender, EventArgs e)
        {
            if (LoginDialog.LoginStatus >= 4)
            {
                EditLabel form = new EditLabel();
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK && form.LabelTxt != "")
                {
                    txtTitleLabel.Text = form.LabelTxt;
                    string strO = string.Empty;
                    using (StreamReader sr = new StreamReader("cfg\\plc.cfg"))
                    {
                        strO = sr.ReadToEnd();
                        sr.Close();
                    }
                    string[] strS = strO.Split('\n');
                    for (int i = 0; i < strS.Length; i++)
                    {
                        string s = strS[i];
                        if (s.Contains("name"))
                        {
                            strS[i] = s.Replace(s.Substring(s.IndexOf('\"')), string.Format("\"{0}\"\n", txtTitleLabel.Text));

                        }
                    }
                    string strN = string.Join("\n", strS);
                    File.WriteAllText("cfg\\plc.cfg", strN);
                }
                form.Dispose();
            }
           
        }


        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //通讯关闭
            Communic.TcpCltfeedingA?.Close();
            Communic.TcpCltfeedingB?.Close();
            Communic.TcpCltfeedingC?.Close();
            Communic.TcpCltfeedingD?.Close();
            Serport.ComE50C?.Close();
            Serport.ComE50D?.Close();
            Serport.ComE50E?.Close();
            Serport.ComE50F?.Close();
        }


        private string HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string data)
        {
           if (request.HttpMethod == "POST"&& request.RawUrl == "/ToggleRe")
            {
                   ToggleRecipe person = JsonConvert.DeserializeObject<ToggleRecipe>(data); // data 就是post上来的数据信息,可以转换为强类型
                if (person!=null&& person.formulaName!=""&& person.workOrderNumber!=""
                        && person.lineName != "")
                 {
                    //获取到请求，切换配方
                    if (!Frm_Repice.ChangeRecipe(person.formulaName))
                    {
                    
                        response.StatusCode = 500;
                        Logger.push(new Log(Logger.LogType.INFO, "MainWindow", "切换配方失败"));
                        var product = new
                        {
                            code = 500,
                            state = "N",
                            message = "切换配方失败",
                        };

                        string json = JsonConvert.SerializeObject(product);
                        return json;
                    }
                    else
                    {
                        response.StatusCode = 200;
                       
                        Logger.push(new Log(Logger.LogType.INFO, "MainWindow", "切换配方成功"));
                        var product = new
                        {
                            code = 200,
                            state = "Y",
                            message = "切换配方成功",
                        };

                        string json = JsonConvert.SerializeObject(product);
                        return json;
                    }
                   
                }
                 else
                   {
                    response.StatusCode = 500;
                    Logger.push(new Log(Logger.LogType.INFO, "MainWindow", "无效方法或参数"));
                    var product2 = new
                        {
                            code = 500,
                            state = "N",
                            message = "无效方法或参数",
                        };

                        string json2 = JsonConvert.SerializeObject(product2);
                        return json2;
                    }
            }
            else
            {
                Logger.push(new Log(Logger.LogType.INFO, "MainWindow", "无效请求"));
                var product = new
                {
                    code = 500,
                    state = "N",
                    message = "无效请求",
                };

                string json = JsonConvert.SerializeObject(product);
                return json;
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_user_Click(object sender, EventArgs e)
        {
            if (LoginDialog.LoginStatus >= 5)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        using (var dia = new EntryPermission())
                        {
                            dia.ShowDialog();
                        }
                    }));
                }
                else
                {
                    using (var dia = new EntryPermission())
                    {
                        dia.ShowDialog();
                    }
                }
            }
            else
            {
                MessageBox.Show("您没有权限打开，请登录超级管理员使用！");
            }
        }

        private void btn_datafind_Click(object sender, EventArgs e)
        {
            
            PublicPanel.Controls["FProductionInfos"].BringToFront();
        }

        private void btn_config_Click(object sender, EventArgs e)
        {
            if (LoginDialog.LoginStatus >= 4)
            {
                PublicPanel.Controls["Frm_Repice"].BringToFront();
            }
            else
            {
                MessageBox.Show("您没有权限打开！");
            }
        }

        private void btn_userex_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    using (var dia= new LoginDialog())
                    {
                        dia.ShowDialog();
                    }
                }));
            }
            else
            {
                using (var dia = new LoginDialog())
                {
                    dia.ShowDialog();
                }
            }
        }


        #region  窗体拖动

        private Point mouseOff;//鼠标移动位置变量
        private bool leftFlag;//标签是否为左键
        private void Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        private void Frm_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }
        private void Frm_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }


        #endregion

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_max_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) { this.WindowState = FormWindowState.Normal; }
            else { this.WindowState = FormWindowState.Maximized; }
        }

        private void btn_min_Click(object sender, EventArgs e)
        {
          this.WindowState = FormWindowState.Minimized;
          
        }
        private void Lt_PLC_ClickEvent1(string name)
        {
            if (LoginDialog.LoginStatus >= 3)
            {
                FIpPort pagetcp = new FIpPort();
                pagetcp.ShowDialog();
                ReadPLCinfo();
            }
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            PublicPanel.Controls["EHomePage"].BringToFront();
        }
    }


}

