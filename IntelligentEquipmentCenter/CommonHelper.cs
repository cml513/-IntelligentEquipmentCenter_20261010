using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace IntelligentEquipmentCenter
{
    /// <summary>
    /// 通用帮助方法（与业务无关的纯工具方法）
    /// </summary>
    public static class CommonHelper
    {
        /// <summary>
        /// 安全的跨线程 UI 调用，防止窗口关闭时 ObjectDisposedException
        /// </summary>
        public static void SafeInvoke(Control ctrl, Action action)
        {
            try
            {
                if (ctrl.InvokeRequired)
                    ctrl.Invoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidAsynchronousStateException) { }
        }

        /// <summary>
        /// 当日志行数超过 maxLines 时，删除前 removeLines 行，防止无限增长导致界面卡顿
        /// </summary>
        public static void TrimLogText(RichTextBox box, int maxLines = 2000, int removeLines = 1000)
        {
            if (box == null) return;
            var lines = box.Lines;
            if (lines.Length > maxLines)
            {
                box.Lines = lines.Skip(removeLines).ToArray();
            }
        }

        /// <summary>正则匹配获取属性值，如 MatchAttr(str, "IP") 取 IP="1.2.3.4" 中的 1.2.3.4</summary>
        public static string MatchAttr(string str, string target)
        {
            if (string.IsNullOrEmpty(str)) return "";
            Regex reg = new Regex(target + "=\"(.+)\"");
            Match match = reg.Match(str);
            return match.Groups[1].Value;
        }

        /// <summary>把字符串转换为ASCII码数组，发送给PLC</summary>
        public static int[] ToAsciiInts(string res)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(res ?? "");
            int[] value = new int[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                value[i] = bytes[i];
            }
            return value;
        }

        /// <summary>发送POST请求返回响应内容</summary>
        public static string PostJson(string url, string jsonParas)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] payload = Encoding.UTF8.GetBytes(jsonParas);
            request.ContentLength = payload.Length;
            Stream writer = null;
            try
            {
                writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("连接服务器失败!");
            }

            HttpWebResponse response;
            try
            {
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
            return postContent;
        }

        /// <summary>读取XML文件中指定节点的整数值，失败返回0</summary>
        public static int LoadXmlInt(string filePath, string nodeName)
        {
            try
            {
                int index = 0;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                foreach (XmlElement element in xmlDoc.GetElementsByTagName(nodeName))
                {
                    int.TryParse(element.InnerText, out index);
                }
                return index;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>把整数值写入XML文件的指定节点</summary>
        public static void SaveXmlInt(string filePath, string rootName, string nodeName, int value)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement root = xmlDoc.CreateElement(rootName);
            root.InnerText = "";

            XmlElement node = xmlDoc.CreateElement(nodeName);
            node.InnerText = value.ToString();

            xmlDoc.AppendChild(root);
            root.AppendChild(node);
            xmlDoc.Save(filePath);
        }
    }
}