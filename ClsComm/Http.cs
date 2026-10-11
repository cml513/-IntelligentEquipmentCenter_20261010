using System;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Generic;

namespace ClsComm
{
    [Serializable]
    [XmlRoot("HttpClt")]
    [XmlType("HttpClt")]
    public class HttpClt : Comm
    {
        private string addr;

        [XmlElement("Addr"), Category("通讯参数"), DisplayName("远程地址")]
        public string Addr
        {
            set
            {
                if (value != addr)
                {
                    addr = value;
                }
            }
            get { return addr; }
        }

        public HttpClt()
        {
            addr = "";
        }

        ~HttpClt()
        { 
        
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool Send(string data, out string msg)
        {
            IsConnected = true;
            if (addr == "")
            {
                msg = "未设置地址！";
                return false;
            }
            msg = Send(addr, data);
            return true;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static string Send(string addr, string strData)
        {
            try
            {
                //创建Http请求，并初始化
                HttpWebRequest request = null;
                request = WebRequest.Create(addr) as HttpWebRequest;
                //设置发送方式 GET:获取资源  PUT：传输文件 post 传输数据
                request.Method = "post";
                //设置标头 数据格式和编码
                request.ContentType = "application/json;charset=utf-8";
                //发送数据
                byte[] data = Encoding.UTF8.GetBytes(strData);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Dispose();
                    stream.Close();
                }
                //接收数据
                return HttpClt.GetResponse(request.GetResponse() as HttpWebResponse);
                
            }
            catch (Exception ex)
            {
                return "{\r\n  \"success\": \"false\",\r\n  \"message\": \"" + ex.Message + "\"\r\n}";
            }
        }
        /// <summary>
        /// 获取请求的数据
        /// </summary>
        private static string GetResponse(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
    }

    [Serializable]
    [XmlRoot("HttpSer")]
    [XmlType("HttpSer")]
    public class HttpSer : Comm
    {
        private string addr;
        private HttpListener m_Listerner = new HttpListener();

        [XmlElement("Addr"), Category("通讯参数"), DisplayName("本机地址")]
        public string Addr
        {
            set
            {
                if (value != addr)
                {
                    addr = value;
                }
            }
            get { return addr; }
        }

        public override bool Connect(out string msg)
        {
            msg = "Ok";
            if (addr == "")
            {
                msg = "未设置地址！";
                return false;
            }

            if (Open(addr, out msg))
            {
                IsConnected = true;
                return true;
            }
            else
            {
                IsConnected = false;
                return false;
            }
        }

        public override bool Close()
        {
            IsConnecting = false;
            IsConnected = false;
            m_Listerner.EndGetContext(null);
            m_Listerner.Stop();
            return true;
        }

        //HTTP监听
        public bool Open(string addr, out string msg)
        {
            msg = "";
            if (!m_Listerner.IsListening)
            {
                try
                {
                    m_Listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
                    m_Listerner.Prefixes.Add(addr);//监控地址
                    m_Listerner.Start();
                    m_Listerner.BeginGetContext(ListenerHandle, m_Listerner);
                }
                catch (Exception ex)
                {
                    msg = "服务启动失败" + ex.Message;
                    return false;
                }
                return true;
            }
            else
            {
                msg = "服务已启动";
                return true;
            }
        }

        public bool IsListening()
        {
            return m_Listerner == null ? false : m_Listerner.IsListening;
        }

        //监听
        private void ListenerHandle(IAsyncResult result)
        {
            StringBuilder buffer = new StringBuilder();
            try
            {
                if (m_Listerner.IsListening)
                {
                    m_Listerner.BeginGetContext(ListenerHandle, result);
                    HttpListenerContext context = m_Listerner.EndGetContext(result);
                    //解析Request请求
                    HttpListenerRequest request = context.Request;
                    string message = "";
                    string resultObjVO = "";//定义返回客户端的信息
                    string  success = "";
                    switch (request.HttpMethod)
                    {
                        case "POST":
                            {
                                Stream stream = context.Request.InputStream;
                                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                                message = reader.ReadToEnd();
                                success = OnReviceEvent(message);
                            }
                            break;
                        case "GET":
                            {
                                var data = request.QueryString;
                            }
                            break;
                    }
                    //构造Response响应
                    HttpListenerResponse response = context.Response;
                    response.ContentType = "application/json;charset=UTF-8";
                    response.ContentEncoding = Encoding.UTF8;
                    response.AppendHeader("Content-Type", "application/json;charset=UTF-8");
                    if (success=="Ok")
                    {
                        string a = request.QueryString["messageName"];
                        a = request.Url.ToString();

                        buffer.AppendFormat("\"{0}\":\"{1}\"", "status", "200");
                        buffer.AppendFormat(",\"{0}\":{1}", "code", "null");
                        buffer.AppendFormat(",\"{0}\":\"{1}\"", "message", "SUCCESS");
                        buffer.AppendFormat(",\"{0}\":{1}", "success", "true");
                        buffer.AppendFormat(",\"{0}\":\"{1}\"", "url", request.RawUrl);
                        buffer.AppendFormat(",\"{0}\":{1}", "resultObjVO", "baseResultObjVO");
                    }
                    else
                    {
                        buffer.AppendFormat("\"{0}\":\"{1}\"", "status", "500");
                        buffer.AppendFormat(",\"{0}\":\"{1}\"", "code", "500");
                        buffer.AppendFormat(",\"{0}\":\"{1}\"", "message", message);
                        buffer.AppendFormat(",\"{0}\":{1}", "success", "false");
                        buffer.AppendFormat(",\"{0}\":{1}", "url", "null");
                        buffer.AppendFormat(",\"{0}\":{1}", "resultObjVO", "null");
                    }
                    resultObjVO = "{" + buffer.ToString() + "}";
                    resultObjVO = resultObjVO.Replace("baseResultObjVO", message);

                    using (StreamWriter writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
                    {
                        writer.Write(resultObjVO);
                        writer.Close();
                        response.Close();
                    }
                }
            }
            catch (Exception)
            {
                //ex.Message;
            }
        }
    }
}
