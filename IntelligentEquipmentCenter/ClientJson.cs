using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using Sunny.UI.Win32;





namespace IntelligentEquipmentCenter
{
    public class ClientJson
    {
        //用户验证接口url,post
        //private static string url1 = "http://10.8.5.57:60/api/DeviceInterface/IntegrationServlet?InterType=JSON";

        //配置文件读取
        public string _SITE;
        public string _OPERATION;
        public string _RESOURCE;
        public string _NC_CODE;
        public string _IP;
        public string _PORT;
        public string _URL;
        public string _USER;
        public string _PWD;
        public string _URLF;

        //全局变量
        public string _sn;
        public string _state;
        public string _message;



        //登录验证
        public class userLogin
        {
            public string factoryCode;
            public string userName;
            public string passWord;
            public string machineCode;
        }


        //条码start
        public class lineCode
        {
            public string machineCode;
            public string snNumber;
            public string testFile;
            public string testFileVersion;
            public string supplierCode;
            public string stationCode;
            public string operationCode;
        }

        //条码pass标准
        public class passStandar
        {
            public string machineCode;
            public string snNumber;
            public string testFile;
            public string testFileVersion;
            public string supplierCode;
            public string stationCode;
            public string operationCode;
            public JArray testInfos;
        }

        //条码error标准
        public class errorStandar
        {
            public string machineCode;
            public string snNumber;
            public string badType;
            public string supplierCode;
            public string stationCode;
            public string operationCode;
            public JArray testInfos;
        }
        //上传数据
        public class Mesdata
        {
            /// <summary>
            /// 总成条码
            /// </summary>
            public string AsBarcode { get; set; }
            /// <summary>
            /// 油脂批次码
            /// </summary>
            public string FatCode { get; set; }
            /// <summary>
            /// 产线名称
            /// </summary>
            public string ProductionName { get; set; }
            /// <summary>
            /// 工位名称
            /// </summary>
            public string WorkpositionName { get; set; }
            //
            /// <summary>
            /// 工单号
            /// </summary>
            public string JONumber { get; set; }
            /// <summary>
            /// 工装编号
            /// </summary>
            public string ToolingNum { get; set; }
            /// <summary>
            /// 产品编码
            /// </summary>
            public string ProduceCode { get; set; }
            /// <summary>
            /// 配方名称
            /// </summary>
            public string RecipeName { get; set; }

            /// <summary>
            /// 操作人工号
            /// </summary>
            public string CurreUser { get; set; }
            /// <summary>
            /// 测试时间
            /// </summary>
            public string TestTime { get; set; }
            /// <summary>
            /// 测试结果
            /// </summary>
            public string TestResult { get; set; }
            /// <summary>
            /// 节拍
            /// </summary>
            public float CT { get; set; }

            /// <summary>
            /// 加油脂时间上限
            /// </summary>
            public float FatTimeUp { get; set; } = 0;
            /// <summary>
            /// 加油脂时间下限
            /// </summary>
            public float FatTimedown { get; set; } = 0;
            /// <summary>
            /// 加油脂实际时间
            /// </summary>
            public float FatTimeReal { get; set; }
            /// <summary>
            /// 加油脂实际时间结果
            /// </summary>
            public string FatTimeResult { get; set; }
            /// <summary>
            /// 加油脂速度上限
            /// </summary>
            public float FatSpeedUp { get; set; } = 0;
            /// <summary>
            /// 加油脂速度下限
            /// </summary>
            public float FatSpeeddown { get; set; } = 0;
            /// <summary>
            /// 加油脂实际速度
            /// </summary>
            public float FatSpeedReal { get; set; } = 0;
            /// <summary>
            /// 加油脂速度结果
            /// </summary>
            public string FatSpeedResult { get; set; }
            /// <summary>
            /// 加油脂相识度上限
            /// </summary>
            public float FatAreaUp { get; set; } = 0;
            /// <summary>
            /// 加油脂相识度下限
            /// </summary>
            public float FatAreadown { get; set; } = 0;
            /// <summary>
            /// 加油脂相识度实际图片名称
            /// </summary>
            public string FatAreaImage { get; set; }
            /// <summary>
            /// 加油脂相识度结果
            /// </summary>
            public string FatAreaResult { get; set; }
            /// <summary>
            /// 丝杆加油脂时间上限
            /// </summary>
            public float ScrewTimeUp { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂时间下限
            /// </summary>
            public float ScrewTimedown { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂实际时间
            /// </summary>
            public float ScrewTimeReal { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂实际时间结果
            /// </summary>
            public string ScrewTimeResult { get; set; }
            /// <summary>
            /// 丝杆加油脂速度上限
            /// </summary>
            public float ScrewSpeedUp { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂速度下限
            /// </summary>
            public float ScrewSpeeddown { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂实际速度
            /// </summary>
            public float ScrewSpeedReal { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂速度结果
            /// </summary>
            public string ScrewSpeedResult { get; set; }
            /// <summary>
            /// 丝杆加油脂相识度上限
            /// </summary>
            public float ScrewAreaUp { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂相识度下限
            /// </summary>
            public float ScrewAreadown { get; set; } = 0;
            /// <summary>
            /// 丝杆加油脂相识度实际图片名称
            /// </summary>
            public string ScrewAreaImage { get; set; }
            /// <summary>
            /// 丝杆加油脂相识度结果
            /// </summary>
            public string ScrewAreaResult { get; set; }
        }

        //上传数据
        public class MesdataDLS
        {
            /// <summary>
            /// 总成条码
            /// </summary>
            public string AsBarcode { get; set; }

            /// <summary>
            /// 产线名称
            /// </summary>
            public string ProductionName { get; set; }
            /// <summary>
            /// 工位名称
            /// </summary>
            public string WorkpositionName { get; set; }
            //
            /// <summary>
            /// 工单号
            /// </summary>
            public string JONumber { get; set; }
            /// <summary>
            /// 工装编号
            /// </summary>
            public string ToolingNum { get; set; }
            /// <summary>
            /// 产品编码
            /// </summary>
            public string ProduceCode { get; set; }
            /// <summary>
            /// 配方名称
            /// </summary>
            public string RecipeName { get; set; }

            /// <summary>
            /// 操作人工号
            /// </summary>
            public string CurreUser { get; set; }
            /// <summary>
            /// 测试时间
            /// </summary>
            public string TestTime { get; set; }
            /// <summary>
            /// 测试结果
            /// </summary>
            public string TestResult { get; set; }
            /// <summary>
            /// 节拍
            /// </summary>
            public float CT { get; set; } = 0;

            /// <summary>
            /// 动态检测参数（配方驱动）：key=参数名，value=实际值/结果等
            /// 键约定："{名字}实际值"、"{名字}结果"、"{名字}上限"、"{名字}下限"
            /// </summary>
            public Dictionary<string, string> DynamicParams { get; set; } = new Dictionary<string, string>();

        }

        //上传数据
        public class MesdataZP
        {
            /// <summary>
            /// 总成条码
            /// </summary>
            public string AsBarcode { get; set; }

            /// <summary>
            /// 产线名称
            /// </summary>
            public string ProductionName { get; set; }
            /// <summary>
            /// 工位名称
            /// </summary>
            public string WorkpositionName { get; set; }
            //
            /// <summary>
            /// 工单号
            /// </summary>
            public string JONumber { get; set; }
            /// <summary>
            /// 工装编号
            /// </summary>
            public string ToolingNum { get; set; }
            /// <summary>
            /// 产品编码
            /// </summary>
            public string ProduceCode { get; set; }
            /// <summary>
            /// 配方名称
            /// </summary>
            public string RecipeName { get; set; }

            /// <summary>
            /// 操作人工号
            /// </summary>
            public string CurreUser { get; set; }
            /// <summary>
            /// 测试时间
            /// </summary>
            public string TestTime { get; set; }
            /// <summary>
            /// 测试结果
            /// </summary>
            public string TestResult { get; set; }
            /// <summary>
            /// 节拍
            /// </summary>
            public float CT { get; set; } = 0;

        }

        //报警信息
        public class AlarmRecord
        {
            /// <summary>
            /// 工位
            /// </summary>
            public string errorStation { get; set; } = "工位1";
            /// <summary>
            /// 设备名
            /// </summary>
            public string machineName { get; set; } = "设备名";
            /// <summary>
            /// 错误类型
            /// </summary>
            public string errorType { get; set; } = "触发停机";
            /// <summary>
            /// 错误信息
            /// </summary>
            public string errorMessage { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            public string startTime { get; set; } = DateTime.Now.ToString();
            /// <summary>
            /// 结束时间
            /// </summary>
            public string endTime { get; set; } = "";

        }

        //测试项数据
        public class data
        {
            public string testItem;
            public string testParameter;
            public string testValue;
        }
        public string Js(object instance)
        {
            string json = string.Empty;
            json = JsonConvert.SerializeObject(instance, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }

        //返回接口json数据
        public string Js(string className)
        {

           // userLogin userLogin = new userLogin();
           // userLogin.site = _SITE;
           // userLogin.productionRequest = "USER";
           // userLogin.user = _USER;
           // userLogin.pwd = _PWD;
           //JObject a= JObject.Parse(JsonConvert.SerializeObject(userLogin));
           // JArray b=new JArray();
           // b.Add(a);
           // string s=b.ToString();

            string json = string.Empty;
            Type type = Type.GetType(className);
            object instance = new object();
            switch (className)
            {
                case "userLogin":
                    userLogin userLogin = new userLogin();
                    //userLogin.site = _SITE;
                    //userLogin.productionRequest = "USER";
                    //userLogin.user = _USER;
                    //userLogin.pwd = _PWD;
                    instance = userLogin;
                    break;
                case "lineCode":
                    lineCode lineCode = new lineCode();
                    //lineCode.productionRequest = "START";
                    //lineCode.site = _SITE;
                    //lineCode.sfc = _sn;
                    //lineCode.resource = _RESOURCE;
                    //lineCode.operation = _OPERATION;
                    //lineCode.user = _USER;
                    instance = lineCode;
                    break;
                case "passStandar":
                    passStandar passStandar = new passStandar();
                    //passStandar.site = _SITE;
                    //passStandar.productionRequest = "PASS";
                    //passStandar.sfc = _sn;
                    //passStandar.resource = _RESOURCE;
                    //passStandar.operation = _OPERATION;
                    //passStandar.user = _USER;
                    //passStandar.testResult = "PASS";
                    //passStandar.testFile = " ";
                    //passStandar.version = "test1";
                    //测试数据，必填
                    //JArray arrayP = new JArray();
                    //data daP = new data();
                    //daP.testItem = "软件版本号";
                    //daP.testParameter = "比对结果";
                    //daP.testValue = "success";
                    //JObject jsdaP = JObject.Parse(JsonConvert.SerializeObject(daP));
                    //arrayP.Add(jsdaP);
                    //passStandar.dataList = arrayP;
                    instance = passStandar;
                    break;
                case "errorStandar":
                    errorStandar errorStandar = new errorStandar();
                    //errorStandar.site = _SITE;
                    //errorStandar.sfc = _sn;
                    //errorStandar.productionRequest = "ERROR";
                    //errorStandar.user = _USER;
                    //errorStandar.qty = 1;
                    //errorStandar.defectCount = 1;
                    //errorStandar.resource = _RESOURCE;
                    //errorStandar.operation = _OPERATION;
                    //errorStandar.ncCode = _NC_CODE;
                    //errorStandar.testResult = "ERROR";
                    //errorStandar.testFile = " ";
                    //errorStandar.version = "test1";
                    //测试数据，必填
                    //JArray arrayE = new JArray();
                    //data daE = new data();
                    //daE.testItem = "软件版本号";
                    //daE.testParameter = "比对结果";
                    //daE.testValue = "fail";
                    //JObject jsdaE = JObject.Parse(JsonConvert.SerializeObject(daE));
                    //arrayE.Add(jsdaE);
                    //errorStandar.dataList = arrayE;
                    instance = errorStandar;
                    break;               
                default:
                    throw new Exception("输入类名错误");

            }
            json = JsonConvert.SerializeObject(instance, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }

        //发送post请求返回响应
        public string Post(string Url, string jsonParas)
        {
            string strURL = Url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json";

            string paraUrlCoded = jsonParas;
            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流 

            Stream writer;
            try
            {
                //获取用于写入请求数据的Stream对象
                writer = request.GetRequestStream();
                //将请求参数写入流                                    
                writer.Write(payload, 0, payload.Length);
            }
            catch (Exception)
            {
                writer = null;
                MessageBox.Show("连接服务器失败!");
            }
            //关闭请求流
            writer.Close();


            HttpWebResponse response;
            try
            {
                //获得响应流   
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            Stream s = response.GetResponseStream();
            StreamReader sRead = new StreamReader(s);
            string postContent = sRead.ReadToEnd();
            sRead.Close();
            //返回Json数据
            return postContent;
        }

        //发送get请求返回响应
        public string HttpGet(string url)
        {

            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Proxy = null;
                wbRequest.Method = "GET";
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
                    {
                        string reslut = sReader.ReadToEnd();
                        return reslut;
                    }
                }
            }
            catch (Exception e)
            {
                string reslut = e.Message;
                //txtLog.Text = txtLog.Text.Insert(0, DateTime.Now + " " + "Get请求版本号发生错误，错误原因：" + reslut + "\n");
                return reslut;
            }

        }

        //读取配置文件
        public void ReadConfig()
        {
            //读取文件
            string filePath = "cfg\\mes.cfg";
            StreamReader reader = new StreamReader(filePath);
            string str = reader.ReadToEnd();
            reader.Close();

            //解析文件并赋值给变量
            _SITE = matchInfo(str, "SITE");
            _OPERATION = matchInfo(str, "OPERATION");
            _RESOURCE = matchInfo(str, "RESOURCE");
            _NC_CODE = matchInfo(str, "NC_CODE");
            _IP = matchInfo(str, "IP");
            _PORT = matchInfo(str, "PORT");
            _URL = matchInfo(str, "URL");
            _USER = matchInfo(str, "USER");
            _PWD = matchInfo(str, "PASSWORD");


            _URLF = "http://" + _IP + ":" + _PORT + _URL;
        }

        //正则匹配获取参数
        private string matchInfo(string str, string target)
        {
            Regex reg = new Regex(target + "=\"(.+)\"");
            Match match = reg.Match(str);
            return match.Groups[1].Value;
        }

        //打开配置文件
        public static void OpenFile(string path)
        {
            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(path);
            process.StartInfo = psi;

            try
            {
                process.Start();
                try
                {
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                    process = null;

                }

            }
        }

        //用户验证
        public string Login()
        {
            try
            {
                JObject respone = JObject.Parse(Post(_URLF, Js("userLogin")));
                _state = (string)respone["state"];
                _message = (string)respone["message"];

                //txtLog.Text = txtLog.Text.Insert(0, DateTime.Now + " " + "用户验证结果：" + _state + "\n");
                return _state;
            }
            catch (Exception ex)
            {
                //txtLog.Text = txtLog.Text.Insert(0, DateTime.Now + " " + "发送用户验证请求失败，失败原因：" + ex.Message + "\n");
                MessageBox.Show("发送用户验证失败，请重新连接服务器...");
                return ex.Message;
            }

            
        }

        //条码过站
        public string UpLoadBarCode()
        {          
             //条码上传
             try
             {
                 JObject res = JObject.Parse(Post(_URLF, Js("lineCode")));
                 string res_state = (string)res["state"];
                 //txtLog.Text = txtLog.Text.Insert(0, DateTime.Now + " " + "条码进站结果：" + res_state + "\n");                   
                 if (res_state == "N")
                 {
                     string msg = (string)res["message"];
                     MessageBox.Show(msg);
                     //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} {1}！\n", DateTime.Now, msg));
                     return msg;
                 }
                 else
                 {
                     return res_state;
                 }
                
             }
             catch (Exception ex)
             {
                 string ms= "发送START请求失败，错误原因："+ex.Message;
                 MessageBox.Show(ms);
                 return ms;
                 //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} 发送START请求失败，错误原因：{0}！\n", DateTime.Now, ex.Message));
             }

           
        }

        //待上传的测试数据
        public JObject UploadData(JObject o)
        {
            try
            {
                return o;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        //上传结果到MES
        private void UploadResultToMes(int fullResult,JObject obj)
        {
           
            if (fullResult ==1)
            {
                //上传OK
                try
                {
                    JObject result = JObject.Parse(Post(_URLF, Js("passStandar")));
                    string state_re = (string)result["state"];
                    if (state_re == "N")
                    {
                        //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} 上传结果OK失败：{1}\n", DateTime.Now, (string)result["message"]));
                       
                    }
                    else
                    {
                        //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0}  上传结果OK成功！\n", DateTime.Now));
                        //IsUploadSuccess = "0";
                        //server.SendData(client, IsUploadSuccess);
                    }
                }
                catch (Exception ex)
                {
                    //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0}  发送PASS请求失败，错误原因：{0}！\n", DateTime.Now, ex.Message));
                    //IsUploadSuccess = "1";
                    //server.SendData(client, IsUploadSuccess);
                }

            }
            else if (fullResult ==0) 
            {
                //上传NG
                try
                {
                    JObject result = JObject.Parse(Post(_URLF, Js("errorStandar")));
                    string state_re = (string)result["state"];
                    if (state_re == "N")
                    {
                        //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} 上传结果NG失败：{1}\n", DateTime.Now, (string)result["message"]));
                        //IsUploadSuccess = "1";
                        //server.SendData(client, IsUploadSuccess);
                    }
                    else
                    {
                        //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} 上传结果NG成功！\n", DateTime.Now));
                        //IsUploadSuccess = "0";
                        //server.SendData(client, IsUploadSuccess);
                    }
                }
                catch (Exception ex)
                {
                    //txtLog.Text = txtLog.Text.Insert(0, string.Format("{0} 发送ERROR请求失败，错误原因：{0}！\n", DateTime.Now, ex.Message));
                    //IsUploadSuccess = "1";
                    //server.SendData(client, IsUploadSuccess);
                }
            }

        }
    }

    //过站信息
    public class linebarCode
    {
        /// <summary>
        /// 机台名称
        /// </summary>
        public string stationName { get; set; } = "";

        /// <summary>
        /// 产品编码
        /// </summary>
        public string productionCode { get; set; } = "";
    }

    /// <summary>
    /// 保存总成条码
    /// </summary>
    public class SavebarCode
    {
        /// <summary>
        /// 总成条码
        /// </summary>
        public string generalBarcode { get; set; } = "";

        /// <summary>
        /// 产品编码
        /// </summary>
        public string productionCode { get; set; } = "";
    }



}
