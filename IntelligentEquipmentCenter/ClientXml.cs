using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;



namespace IntelligentEquipmentCenter
{
    public class ClientXML
    {
        private string 站点;  //SITE
        private string 工序;  //OPERATION
        private string 工位;  //RESOURCE
        private string NC_NODE;
        private string IP;
        private string PORT;
        private string URL;  //example:http://10.8.5.56:60/api/DeviceInterface/IntegrationServlet?InterType=XML

        private string 用户名;


        public ClientXML()
        {
            //读取文件
            string filePath = "cfg\\mesxml.cfg";
            StreamReader reader = new StreamReader(filePath);
            string str = reader.ReadToEnd();
            reader.Close();
            //解析文件并赋值给变量
            站点 = matchInfo(str, "SITE");
            工序 = matchInfo(str, "OPERATION");
            工位 = matchInfo(str, "RESOURCE");
            IP = matchInfo(str, "IP");
            PORT = matchInfo(str, "PORT");
            URL = "Http://" + IP + ":" + PORT + matchInfo(str, "URL");
            NC_NODE = matchInfo(str, "NC_NODE");
        }


        private string matchInfo(string str, string target)
        {
            Regex reg = new Regex(target + "=\"(.+)\"");
            Match match = reg.Match(str);
            return match.Groups[1].Value;
        }

        //第一步登录用户验证
        //string info = null;
        //登录状态 = clientxml.Login(textBox1.Text, textBox2.Text, ref info);
        public int Login(string username, string password, ref string info)
        {
           
            //TCP连接相机
            //ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  //客户端套接字               
            //                                                                                             //richTextBox1.Text = "*" + DateTime.Now.ToString("yy-MM-dd hh:mm:ss  ") + "正在连接...\r\n" ;
            //IPAddress IP = IPAddress.Parse(textBox4.Text);  //服务器IP
            //int Port = int.Parse(textBox3.Text);  //服务器端口号
            //IPEndPoint ipEndPoint = new IPEndPoint(IP, Port);  //指定的端口号和服务器IP建立一个IPEndPoint对象
            //ClientSocket.Connect(ipEndPoint);  //向服务器发出连接请求


            //return Post(str)

            用户名 = username;

            //组装XML报文
            //StringBuilder str = new StringBuilder();
            //str.Append("<PRODUCTION_REQUEST><USER><SITE>" +
            //    站点 + "</SITE><NAME>" +
            //    用户名 + "</NAME><PWD>" +
            //    password + "</PWD></USER></PRODUCTION_REQUEST>");
            //发送报文
            //return Post(str.ToString());


            //组装XML报文
            XmlDocument xddProduction = new XmlDocument();

            XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
            XmlElement xePro2 = xddProduction.CreateElement(name: "USER");
            XmlElement xePro3 = xddProduction.CreateElement(name: "SITE");
            XmlElement xePro4 = xddProduction.CreateElement(name: "NAME");
            XmlElement xePro5 = xddProduction.CreateElement(name: "PWD");

            xePro3.InnerText = 站点;
            xePro4.InnerText = 用户名;
            xePro5.InnerText = password;

            xePro2.AppendChild(xePro3);
            xePro2.AppendChild(xePro4);
            xePro2.AppendChild(xePro5);

            xePro1.AppendChild((XmlElement)xePro2);

            xddProduction.AppendChild((XmlElement)xePro1);

            //xddProduction.Save(filename: @"D:\test\production.xml");
            //发送报文
            return Post(xddProduction.OuterXml.ToString(), ref info);
        }


        public int SendStart(string code_information, ref string info)
        {
            //组装XML报文
            //StringBuilder str = new StringBuilder();
            //str.Append("<PRODUCTION_REQUEST><START><SFC_LIST><SFC><SITE>" +
            //        站点 + "</SITE><ACTIVITY>XML</ACTIVITY><ID>" +
            //        code_information + "</ID><RESOURCE>" +
            //        工位 + "</RESOURCE><OPERATION>" +
            //        工序 + "</OPERATION><USER>" +
            //        用户名 + "</USER><QTY>1</QTY><DATE_TIME></DATE_TIME></SFC></SFC_LIST></START></PRODUCTION_REQUEST>!erpautogy03!1234567@byd"
            //        );
            ////发送报文
            //return Post(str.ToString());


            //组装XML报文
            XmlDocument xddProduction = new XmlDocument();

            XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
            XmlElement xePro2 = xddProduction.CreateElement(name: "START");//
            XmlElement xePro3 = xddProduction.CreateElement(name: "SFC_LIST");//
            XmlElement xePro4 = xddProduction.CreateElement(name: "SFC");//

            XmlElement xePro41 = xddProduction.CreateElement(name: "SITE");//
            XmlElement xePro42 = xddProduction.CreateElement(name: "ACTIVITY");//
            XmlElement xePro43 = xddProduction.CreateElement(name: "ID");//
            XmlElement xePro44 = xddProduction.CreateElement(name: "RESOURCE");//
            XmlElement xePro45 = xddProduction.CreateElement(name: "OPERATION");//
            XmlElement xePro46 = xddProduction.CreateElement(name: "USER");//
            XmlElement xePro47 = xddProduction.CreateElement(name: "QTY");//
            XmlElement xePro48 = xddProduction.CreateElement(name: "DATE_TIME");//

            xePro41.InnerText = 站点;
            xePro42.InnerText = "XML";
            xePro43.InnerText = code_information;
            xePro44.InnerText = 工位;
            xePro45.InnerText = 工序;
            xePro46.InnerText = 用户名;
            xePro47.InnerText = "1";
            xePro48.InnerText = "";

            xePro2.AppendChild(xePro3);
            xePro3.AppendChild(xePro4);

            xePro4.AppendChild(xePro41);
            xePro4.AppendChild(xePro42);
            xePro4.AppendChild(xePro43);
            xePro4.AppendChild(xePro44);
            xePro4.AppendChild(xePro45);
            xePro4.AppendChild(xePro46);
            xePro4.AppendChild(xePro47);
            xePro4.AppendChild(xePro48);

            xePro1.AppendChild(xePro2);

            xddProduction.AppendChild(xePro1);

            //xddProduction.Save(filename: @"D:\test\SendStart.xml");
            //发送报文
            return Post(xddProduction.OuterXml.ToString() + "!erpautogy03!1234567@byd", ref info);


        }


        public int SendMessage(string code_state, string code_information, string PIN_state, ref string info)
        {
            //return Post(str)

            //组装报文
            //TODO:组装合格信息
            string 合格信息 = "OK";
            string 不合格信息 = "NG";
            StringBuilder str = new StringBuilder();



            if (code_state == "OK" && PIN_state == "OK")
            {
                //str.Append("PASS" +
                //    "<PRODUCTION_REQUEST><COMPLETE><SFC_LIST><SFC><SITE>" +
                //    站点 + "</SITE><ACTIVITY>XML</ACTIVITY><ID>" +
                //    code_information + "</ID><RESOURCE>" +
                //    工位 + "</RESOURCE><OPERATION>" +
                //    工序 + "</OPERATION><USER>" +
                //    用户名 + "</USER><QTY>1</QTY><DATE_TIME></DATE_TIME><DATE_STARTED></DATE_STARTED></SFC></SFC_LIST></COMPLETE></PRODUCTION_REQUEST>!erpautogy03!1234567@byd!PASS," +
                //    合格信息
                //);
                //组装XML报文
                XmlDocument xddProduction = new XmlDocument();

                XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
                XmlElement xePro2 = xddProduction.CreateElement(name: "COMPLETE");//
                XmlElement xePro3 = xddProduction.CreateElement(name: "SFC_LIST");//
                XmlElement xePro4 = xddProduction.CreateElement(name: "SFC");//

                XmlElement xePro41 = xddProduction.CreateElement(name: "SITE");//
                XmlElement xePro42 = xddProduction.CreateElement(name: "ACTIVITY");//
                XmlElement xePro43 = xddProduction.CreateElement(name: "ID");//
                XmlElement xePro44 = xddProduction.CreateElement(name: "RESOURCE");//
                XmlElement xePro45 = xddProduction.CreateElement(name: "OPERATION");//
                XmlElement xePro46 = xddProduction.CreateElement(name: "USER");//
                XmlElement xePro47 = xddProduction.CreateElement(name: "QTY");//
                XmlElement xePro48 = xddProduction.CreateElement(name: "DATE_TIME");//
                XmlElement xePro49 = xddProduction.CreateElement(name: "DATE_STARTED");//


                xePro41.InnerText = 站点;
                xePro42.InnerText = "XML";
                xePro43.InnerText = code_information;
                xePro44.InnerText = 工位;
                xePro45.InnerText = 工序;
                xePro46.InnerText = 用户名;
                xePro47.InnerText = "1";
                xePro48.InnerText = "";
                xePro49.InnerText = "";

                xePro2.AppendChild(xePro3);
                xePro3.AppendChild(xePro4);

                xePro4.AppendChild(xePro41);
                xePro4.AppendChild(xePro42);
                xePro4.AppendChild(xePro43);
                xePro4.AppendChild(xePro44);
                xePro4.AppendChild(xePro45);
                xePro4.AppendChild(xePro46);
                xePro4.AppendChild(xePro47);
                xePro4.AppendChild(xePro48);
                xePro4.AppendChild(xePro49);

                xePro1.AppendChild(xePro2);

                xddProduction.AppendChild(xePro1);
                //xddProduction.Save(filename: @"D:\test\SendMessagePass.xml");



                str.Append("PASS" + xddProduction.OuterXml.ToString() + "!erpautogy03!1234567@byd!PASS," + 合格信息);

            }
            else
            {
                //str.Append("ERROR" +
                //    "<PRODUCTION_REQUEST><NC_LOG_COMPLETE><SITE>" +
                //    站点 + "</SITE><OWNER TYPE=\"USER\">" +
                //    用户名 + "</OWNER><NC_CONTEXT>" +
                //    code_information + "</NC_CONTEXT><QTY>1</QTY><IDENTIFIER></IDENTIFIER><FAILURE_ID></FAILURE_ID><DEFECT_COUNT>1</DEFECT_COUNT><COMMENTS></COMMENTS><DATE_TIME></DATE_TIME><RESOURCE>" +
                //    工位 + "</RESOURCE><OPERATION>" +
                //    工序 + "</OPERATION><ROOT_CAUSE-OPER></ROOT_CAUSE-OPER><NC_CODE>" +
                //    NC_NODE + "</NC_CODE><ACTIVITY>XML</ACTIVITY></NC_LOG_COMPLETE></PRODUCTION_REQUEST>!erpautogy03!1234567@byd!ERROR," +
                //    不合格信息
                //    );
                //组装XML报文
                XmlDocument xddProduction = new XmlDocument();

                XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
                XmlElement xePro2 = xddProduction.CreateElement(name: "NC_LOG_COMPLETE");//
                //XmlElement xePro3 = xddProduction.CreateElement(name: "SFC_LIST");//
                //XmlElement xePro4 = xddProduction.CreateElement(name: "SFC");//

                XmlElement xePro41 = xddProduction.CreateElement(name: "SITE");//
                XmlElement xePro42 = xddProduction.CreateElement(name: "OWNER");//
                XmlElement xePro43 = xddProduction.CreateElement(name: "NC_CONTEXT");//
                XmlElement xePro44 = xddProduction.CreateElement(name: "QTY");//
                XmlElement xePro45 = xddProduction.CreateElement(name: "IDENTIFIER");//
                XmlElement xePro46 = xddProduction.CreateElement(name: "FAILURE_ID");//
                XmlElement xePro47 = xddProduction.CreateElement(name: "DEFECT_COUNT");//
                XmlElement xePro48 = xddProduction.CreateElement(name: "COMMENTS");//
                XmlElement xePro49 = xddProduction.CreateElement(name: "DATE_TIME");//

                XmlElement xePro4a = xddProduction.CreateElement(name: "RESOURCE");//
                XmlElement xePro4b = xddProduction.CreateElement(name: "OPERATION");//
                XmlElement xePro4c = xddProduction.CreateElement(name: "ROOT_CAUSE_OPER");//
                XmlElement xePro4d = xddProduction.CreateElement(name: "NC_CODE");//
                XmlElement xePro4e = xddProduction.CreateElement(name: "ACTIVITY");//


                xePro41.InnerText = 站点;
                xePro42.SetAttribute("TYPE", "USER");
                xePro42.InnerText = 用户名;
                xePro43.InnerText = code_information;
                xePro44.InnerText = "1";
                xePro45.InnerText = "";
                xePro46.InnerText = "";
                xePro47.InnerText = "1";
                xePro48.InnerText = "";
                xePro49.InnerText = "";

                xePro4a.InnerText = 工位;//
                xePro4b.InnerText = 工序;//
                xePro4c.InnerText = "";//
                xePro4d.InnerText = NC_NODE;//
                xePro4e.InnerText = "XML";

                //xePro2.AppendChild(xePro3);
                //xePro3.AppendChild(xePro4);

                xePro2.AppendChild(xePro41);
                xePro2.AppendChild(xePro42);
                xePro2.AppendChild(xePro43);
                xePro2.AppendChild(xePro44);
                xePro2.AppendChild(xePro45);
                xePro2.AppendChild(xePro46);
                xePro2.AppendChild(xePro47);
                xePro2.AppendChild(xePro48);
                xePro2.AppendChild(xePro49);
                xePro2.AppendChild(xePro49);

                xePro2.AppendChild(xePro4a);
                xePro2.AppendChild(xePro4b);
                xePro2.AppendChild(xePro4c);
                xePro2.AppendChild(xePro4d);
                xePro2.AppendChild(xePro4e);

                xePro1.AppendChild(xePro2);

                xddProduction.AppendChild(xePro1);
                //xddProduction.Save(filename: @"D:\test\SendMessageError.xml");

                str.Append("ERROR" + xddProduction.OuterXml.ToString() + "!erpautogy03!1234567@byd!ERROR," + 不合格信息);
            }

            //发送报文
            return Post(str.ToString(), ref info);
        }
        /// <summary>
        /// 设备状态标准
        /// </summary>
        /// <param name="state">设备状态</param>
        /// <param name="info">返回报文</param>
        /// <returns></returns>
        public int DeviceState(string state, ref string info)
        {
            //组装XML报文
            XmlDocument xddProduction = new XmlDocument();
            XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
            XmlElement xePro2 = xddProduction.CreateElement(name: "STATUS");
            XmlElement xePro21 = xddProduction.CreateElement(name: "SITE");
            XmlElement xePro22 = xddProduction.CreateElement(name: "USER");
            XmlElement xePro23 = xddProduction.CreateElement(name: "RESOURCE");
            XmlElement xePro24 = xddProduction.CreateElement(name: "OPERATION");
            XmlElement xePro25 = xddProduction.CreateElement(name: "STATUS_CODE");
            xePro21.InnerText = 站点;
            xePro22.InnerText = 用户名;
            xePro23.InnerText = 工位;
            xePro24.InnerText = 工序;
            xePro25.InnerText = state;
            xePro2.AppendChild(xePro21);
            xePro2.AppendChild(xePro22);
            xePro2.AppendChild(xePro23);
            xePro2.AppendChild(xePro24);
            xePro2.AppendChild(xePro25);
            xePro1.AppendChild(xePro2);
            xddProduction.AppendChild(xePro1);
            return Post(xddProduction.OuterXml.ToString(),ref info);
        }

        /// <summary>
        /// 上传夹具剩余使用次数
        /// </summary>
        /// <param name="clampID">夹具编号</param>
        /// <param name="time">数量</param>
        /// <param name="info">返回报文</param>
        /// <returns></returns>
        public int ClampRestTime(string clampID,int time,ref string info)
        {
            XmlDocument xddProduction = new XmlDocument();

            XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
            XmlElement xePro2 = xddProduction.CreateElement(name: "CLAMP");//根元素
            XmlElement xePro21 = xddProduction.CreateElement(name: "SITE");
            XmlElement xePro22 = xddProduction.CreateElement(name: "CLAMPID");
            XmlElement xePro23 = xddProduction.CreateElement(name: "NUM");
            xePro21.InnerText = 站点;
            xePro22.InnerText = clampID;
            xePro23.InnerText = time.ToString();
            xePro2.AppendChild(xePro21);
            xePro2.AppendChild(xePro22);
            xePro2.AppendChild(xePro23);
            xePro1.AppendChild(xePro2);

            xddProduction.AppendChild(xePro1);
            return Post(xddProduction.OuterXml.ToString(), ref info);
            //return Post(xddProduction.OuterXml.ToString(), ref info);


        }

        //离散装配前查询已装配组件请求
        public int AssembleSfc(string code_information, ref string info)
        {
            //组装XML报文
            XmlDocument xddProduction = new XmlDocument();

            XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
            XmlElement xePro2 = xddProduction.CreateElement(name: "ASSEMBLE_SFC");

            XmlElement xePro3 = xddProduction.CreateElement(name: "SITE");
            XmlElement xePro4 = xddProduction.CreateElement(name: "SFC");


            xePro3.InnerText = 站点;
            xePro4.InnerText = code_information;

            xePro2.AppendChild(xePro3);
            xePro2.AppendChild(xePro4);

            xePro1.AppendChild((XmlElement)xePro2);

            xddProduction.AppendChild((XmlElement)xePro1);


            //xddProduction.Save(filename: @"D:\test\AssembleSfc.xml");


            //发送报文
            return Post(xddProduction.OuterXml.ToString(), ref info);
        }

        public int AssembleComponents(string code_information, ref string info)
        {
            //未验证通过

            //组装XML报文
            XmlDocument xddProduction = new XmlDocument();

            XmlElement xePro1 = xddProduction.CreateElement(name: "PRODUCTION_REQUEST");//根元素
            XmlElement xePro2 = xddProduction.CreateElement(name: "ASSEMBLE_COMPONENTS");

            XmlElement xePro21 = xddProduction.CreateElement(name: "USER");
            XmlElement xePro22 = xddProduction.CreateElement(name: "SITE");
            XmlElement xePro23 = xddProduction.CreateElement(name: "PARENT_SFC");
            XmlElement xePro24 = xddProduction.CreateElement(name: "OPERATION");
            XmlElement xePro25 = xddProduction.CreateElement(name: "RESOURCE");
            XmlElement xePro26 = xddProduction.CreateElement(name: "CHECK_OPER");
            XmlElement xePro27 = xddProduction.CreateElement(name: "EVENT");
            XmlElement xePro28 = xddProduction.CreateElement(name: "IDENTIFIER_LIST");

            XmlElement xePro281 = xddProduction.CreateElement(name: "IDENTIFIER_OBJECT");
            XmlElement xePro282 = xddProduction.CreateElement(name: "IDENTIFIER_OBJECT");

            XmlElement xePro2811 = xddProduction.CreateElement(name: "IDENTIFIER");
            XmlElement xePro2812 = xddProduction.CreateElement(name: "REVISION");
            XmlElement xePro2813 = xddProduction.CreateElement(name: "QTY");
            XmlElement xePro2814 = xddProduction.CreateElement(name: "ASSY_DATA_VALUES");

            XmlElement xePro28141 = xddProduction.CreateElement(name: "ASSY_DATA");
            XmlElement xePro281411 = xddProduction.CreateElement(name: "DATA_FIELD");
            XmlElement xePro281412 = xddProduction.CreateElement(name: "DATA_ATTR");

            XmlElement xePro2821 = xddProduction.CreateElement(name: "IDENTIFIER");
            XmlElement xePro2822 = xddProduction.CreateElement(name: "REVISION");
            XmlElement xePro2823 = xddProduction.CreateElement(name: "QTY");
            XmlElement xePro2824 = xddProduction.CreateElement(name: "ASSY_DATA_VALUES");

            XmlElement xePro28241 = xddProduction.CreateElement(name: "ASSY_DATA");
            XmlElement xePro282411 = xddProduction.CreateElement(name: "DATA_FIELD");
            XmlElement xePro282412 = xddProduction.CreateElement(name: "DATA_ATTR");

            xePro21.InnerText = 用户名;
            xePro22.InnerText = 站点;
            xePro23.InnerText = code_information;
            xePro24.InnerText = 工序;
            xePro25.InnerText = 工位;
            xePro26.InnerText = "True";
            xePro27.InnerText = "baseFinished:AssemblyPoint";

            xePro2811.InnerText = "物料号 1";
            xePro2812.InnerText = "版本号 1";
            xePro2813.InnerText = "数量";

            xePro281411.InnerText = "EXTERNAL_LOT";
            xePro281412.InnerText = "散件 SFC 号 1(来料类型：外部批次)";

            xePro2821.InnerText = "物料号 2";
            xePro2822.InnerText = "版本号 2";
            xePro2823.InnerText = "数量";

            xePro282411.InnerText = "INVENTORY_ID_SFC";
            xePro282412.InnerText = "散件 SFC 号 2(来料类型：库存批次)";


            xePro2.AppendChild(xePro21);
            xePro2.AppendChild(xePro22);
            xePro2.AppendChild(xePro23);
            xePro2.AppendChild(xePro24);
            xePro2.AppendChild(xePro25);
            xePro2.AppendChild(xePro26);
            xePro2.AppendChild(xePro27);
            xePro2.AppendChild(xePro28);

            xePro28.AppendChild(xePro281);
            xePro28.AppendChild(xePro282);

            xePro281.AppendChild(xePro2811);
            xePro281.AppendChild(xePro2812);
            xePro281.AppendChild(xePro2813);
            xePro281.AppendChild(xePro2814);

            xePro2814.AppendChild(xePro28141);
            xePro28141.AppendChild(xePro281411);
            xePro28141.AppendChild(xePro281412);

            xePro282.AppendChild(xePro2821);
            xePro282.AppendChild(xePro2822);
            xePro282.AppendChild(xePro2823);
            xePro282.AppendChild(xePro2824);

            xePro2824.AppendChild(xePro28241);
            xePro28241.AppendChild(xePro282411);
            xePro28241.AppendChild(xePro282412);

            xePro1.AppendChild((XmlElement)xePro2);

            xddProduction.AppendChild((XmlElement)xePro1);

            //xddProduction.Save(filename: @"D:\test\AssembleComponents.xml");
            //发送报文
            return Post(xddProduction.OuterXml.ToString() + "!erpautogy03!1234567@byd", ref info);
        }


        private int Post(string str, ref string info)
        {
            //return 1:正常运行
            //return 0:与服务器连接成功，服务器验证失败
            //return -1: 发送报文失败

            //组建报文

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "text/xml;charset=utf-8";
            byte[] bytes = Encoding.UTF8.GetBytes(str);

            //尝试连接
            try
            {
                Stream RequestStream = request.GetRequestStream();
                RequestStream.Write(bytes, 0, bytes.Length);
                RequestStream.Close();
            }
            catch   //发送失败->网络出现问题
            {
                return -1;
            }

            //发送成功，解析返回
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();


            //Console.WriteLine(response.CharacterSet.ToLower());   //解析返回报文格式->结果为iso-8859-1

            //设置编码格式读取返回内容
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));
            string responseToStr = reader.ReadToEnd().ToString();
            reader.Close();
            response.Close();

            //System.Console.WriteLine(responseToStr);

           HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseToStr);
            string fileName = "logxml.html";

            html.Save(fileName);

            String resultStr = null;
            String infoStr = null;

            foreach (HtmlNode span in html.DocumentNode.SelectNodes("//td[@align='left']"))
            {
                string innerText = span.InnerText;
                if (resultStr != null)
                {
                    infoStr = innerText;
                }
                else
                {
                    resultStr = innerText;
                }
            }

            string[] strlist = infoStr.Split(';');



            try
            {
                string rend = strlist[4];
                info = rend.Remove(rend.Count() - 3, 3);
                Console.WriteLine(info);
            }
            catch
            {
                return 0;

            }

            return (resultStr == "State(Y-Success，N-Wrong）：  Y") ? 1 : 0;







            //将返回写入文件
            //string fileName = "logxml.html";
            //StreamWriter writer = new StreamWriter(fileName);
            //writer.WriteLine(responseToStr);
            //writer.Close();

            ////使用正则解析返回报文并得到Y/N
            //Match match;
            //Regex reg = new Regex("</b>(Y|N)</td>");
            //match = reg.Match(responseToStr);
            //return (match.Groups[1].Value == "Y") ? 1 : 0;




            ///用于读取返回的Xml文档
            //XmlTextReader objxmlReader = new XmlTextReader(reader);
            //XmlDocument xmldocument = new XmlDocument();
            //xmldocument.Load(objxmlReader);
            //objxmlReader.Close();
        }

        ///方法测试
        //static void Main(string[] args)
        //{
        //    Client client = new Client();
        //    client.Login("4867942", "123456");
        //    client.SendStart();
        //    client.SendMessage("OK", "12345678F000000008", "OK");
        //}
    }
}


