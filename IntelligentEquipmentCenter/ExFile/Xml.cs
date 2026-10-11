using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TF.File
{
    public class Xml : ExFile
    {
        protected string m_xmlFilePath = "";
        protected string m_rootNodeName = "";
        protected XmlDocument m_objXmlDoc = new XmlDocument();
        public string LastErrorMsg = "";

        /// <summary>
        /// 创建xml文件对象
        /// </summary>
        /// <param name="file">完整的文件路径</param>
        /// <param name="root">根节点名曾</param>
        /// <param name="over">是否覆盖原有文件</param>
        public Xml(string file, string root, bool over = false)
        {
            try
            {
                //如果覆盖模式，则强行创建一个xml文档
                if (over)
                {
                    m_objXmlDoc.AppendChild(m_objXmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));//设置xml的版本，格式信息
                    m_objXmlDoc.AppendChild(m_objXmlDoc.CreateElement("", root, ""));//创建根元素
                    m_objXmlDoc.Save(file);//保存
                }
                else //否则，检查文件是否存在，不存在则创建
                {
                    if (!(System.IO.File.Exists(file)))
                    {
                        m_objXmlDoc.AppendChild(m_objXmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                        m_objXmlDoc.AppendChild(m_objXmlDoc.CreateElement("", root, ""));
                        m_objXmlDoc.Save(file);
                    }
                }

                m_objXmlDoc.Load(file);
                m_xmlFilePath = file;
                m_rootNodeName = root;
            }
            catch (System.Exception ex)
            {
                LastErrorMsg = "Xml：" + ex.Message;
            }
        }

        /// <summary>
        /// 保存xml文件
        /// </summary>
        public void Save()
        {
            try
            {
                //保存时添加保存时间
                InsertComment("//" + m_rootNodeName, "保存时间 " + Date() + " " + Time() + " " + Week());

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                //settings.IndentChars = " ";
                settings.Encoding = Encoding.UTF8;
                using (XmlWriter xmlWriter = XmlWriter.Create(m_xmlFilePath, settings))
                {
                    m_objXmlDoc.Save(xmlWriter);
                }
            }
            catch (Exception ex)
            {
                LastErrorMsg = "Save：" + ex.Message;
            }
        }

        /// <summary>
        /// 设置节点注释
        /// </summary>
        /// <param name="node">节点路径 node/elem</param>
        /// <param name="comment"></param>
        /// <param name="insertBefore">插入位置</param>
        public void InsertComment(string node, string comment, bool insertBefore = true)
        {
            XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
            if (objNode == null)
            {
                LastErrorMsg = "InsertComment:节点 " + node + " 不存在!";
                return;
            }
            XmlComment xComment = m_objXmlDoc.CreateComment(comment);
            if (insertBefore)
                objNode.ParentNode.InsertBefore(xComment, objNode);
            else
                objNode.ParentNode.InsertAfter(xComment, objNode);
        }

        /// <summary>
        /// 插入节点元素
        /// </summary>
        /// <param name="node">节点路径</param>
        /// <param name="element">元素名</param>
        /// <param name="Content">元素内容</param>
        public void InsertNode(string node, string element, string content = "")
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "InsertNode：节点 " + node + " 不存在!";
                    return;
                }
                XmlElement objChildElement = m_objXmlDoc.CreateElement(element);

                if (!string.IsNullOrWhiteSpace(content))
                    objChildElement.InnerText = content;

                objNode.AppendChild(objChildElement);
            }
            catch (Exception ex)
            {
                LastErrorMsg = "InsertNode：" + ex.Message;
            }
        }

        /// <summary>
        /// 插入节点元素，具有单个属性
        /// </summary>
        /// <param name="node">节点路径</param>
        /// <param name="element">元素名</param>
        /// <param name="attribute">属性名</param>
        /// <param name="atrValue">属性值</param>
        /// <param name="elmContent">元素内容</param>
        public void InsertNode(string node, string element, string attribute, string atrValue, string elmContent = "")
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "InsertNode：节点 " + node + " 不存在!";
                    return;
                }
                XmlElement objElement = m_objXmlDoc.CreateElement(element);
                objElement.SetAttribute(attribute, atrValue);

                if (!string.IsNullOrWhiteSpace(elmContent))
                    objElement.InnerText = elmContent;

                objNode.AppendChild(objElement);
            }
            catch (Exception ex)
            {
                LastErrorMsg = "InsertNode：" + ex.Message;
            }
        }

        /// <summary>
        /// 插入节点元素，具有多个属性
        /// </summary>
        /// <param name="node">节点路径</param>
        /// <param name="element">元素名</param>
        /// <param name="attributes">属性列表</param>
        /// <param name="atrValues">属性值</param>
        /// <param name="elmContent">元素内容</param>
        public void InsertNode(string node, string element, string[] attributes, string[] atrValues, string elmContent = "")
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "InsertNode：节点 " + node + " 不存在!";
                    return;
                }
                XmlElement objElement = m_objXmlDoc.CreateElement(element);
                for (int i = 0; i <= attributes.GetUpperBound(0); i++)
                {
                    objElement.SetAttribute(attributes[i], atrValues[i]);
                }

                if (!string.IsNullOrWhiteSpace(elmContent))
                    objElement.InnerText = elmContent;

                objNode.AppendChild(objElement);
            }
            catch (Exception ex)
            {
                LastErrorMsg = "InsertNode：" + ex.Message;
            }
        }

        /// <summary>
        /// 插入节点的子节点
        /// </summary>
        /// <param name="node">节点目录</param>
        /// <param name="chlid">子节点名</param>
        /// <param name="elem">元素名</param>
        /// <param name="content">元素内容</param>
        public void InsertNode(string node, string chlid, string elem, string content = "")
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "InsertNode：节点 " + node + " 不存在!";
                    return;
                }
                XmlElement objChildNode = m_objXmlDoc.CreateElement(chlid);
                objNode.AppendChild(objChildNode);//插入节点

                XmlElement objElement = m_objXmlDoc.CreateElement(elem);

                if (!string.IsNullOrWhiteSpace(node))
                    objElement.InnerText = content;

                objChildNode.AppendChild(objElement);//插入子节点
            }
            catch (Exception ex)
            {
                LastErrorMsg = "InsertNode：" + ex.Message;
            }
        }

        /// <summary>
        /// 修改节点内容
        /// </summary>
        /// <param name="node">节点路径</param>
        /// <param name="content"></param>
        public void ModifyNode(string node, string content)
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "ModifyNode：节点 " + node + " 不存在!";
                    return;
                }
                objNode.InnerText = content;
            }
            catch (Exception ex)
            {
                LastErrorMsg = "ModifyNode：" + ex.Message;
            }
        }

        /// <summary>
        /// 修改节点的属性值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        public void ModifyNode(string node, string attribute, string value)
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "ModifyNode：节点 " + node + " 不存在!";
                    return;
                }

                ((XmlElement)(objNode)).SetAttribute(attribute, value);
            }
            catch (Exception ex)
            {
                LastErrorMsg = "ModifyNode：" + ex.Message;
            }
        }

        /// <summary>
        /// 修改节点所有内容
        /// </summary>
        /// <param name="node"></param>
        /// <param name="content"></param>
        /// <param name="attributes"></param>
        /// <param name="values"></param>
        public void ModifyNode(string node, string content, string[] attributes, string[] values)
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "ModifyNode：节点 " + node + " 不存在!";
                    return;
                }

                objNode.InnerText = content;
                objNode.Attributes.RemoveAll();
                for (int i = 0; i <= attributes.GetUpperBound(0); i++)
                {
                    ((XmlElement)(objNode)).SetAttribute(attributes[i], values[i]);
                }
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
            }
        }

        /// <summary>
        /// 查找节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool FindNode(string node)
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LastErrorMsg = "FindNode：" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 查找节点的子节点数
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int FindChildNodesCount(string node)
        {
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "FindChildNodesCount：节点 " + node + " 不存在!";
                    return 0;
                }

                if (!objNode.HasChildNodes)
                    return 0;

                return objNode.ChildNodes.Count;
            }
            catch (Exception ex)
            {
                LastErrorMsg = "FindChildNodesCount：" + ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// 查找节点的子节点 名称集合
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string[] FindChildNodesList(string node)
        {
            string[] NodesList = new string[0];
            try
            {
                XmlNode objNode = m_objXmlDoc.SelectSingleNode(node);
                if (objNode == null)
                {
                    LastErrorMsg = "FindChildNodesList：节点 " + node + " 不存在!";
                    return NodesList;
                }

                if (!objNode.HasChildNodes)
                    return NodesList;

                XmlNodeList xnList = objNode.ChildNodes;
                NodesList = new string[xnList.Count];
                for (int i = 0; i < xnList.Count; ++i)
                {
                    XmlNode xn = xnList.Item(i);
                    NodesList[i] = xn.Name;
                }
                return NodesList;
            }
            catch (Exception ex)
            {
                LastErrorMsg = "FindChildNodesList：" + ex.Message;
                return NodesList;
            }
        }

        /// <summary>
        /// 查找节点内容
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string FindNodeValue(string node)
        {
            XmlNode root = m_objXmlDoc.SelectSingleNode(node);
            if (root != null)
            {
                return (root.SelectSingleNode(node)).InnerText;
            }
            else
                return "";
        }

        /// <summary>
        /// 查找节点所有 属性名集合
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string[] FindNodeAttributes(string node)
        {
            string[] list = new string[0];
            try
            {
                XmlNode root = m_objXmlDoc.SelectSingleNode(node);
                if (root != null)
                {
                    XmlAttributeCollection ac = root.Attributes;
                    if (root.Attributes.Count > 0)
                    {
                        list = new string[root.Attributes.Count];
                        int i = 0;
                        foreach (XmlAttribute attribute in ac)
                        {
                            list[i] = attribute.Name;//节点下的属性名称
                            i++;
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return list;
            }
        }
        /// <summary>
        /// 查找节点所有 属性值集合
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public string[] FindNodeAttributesValue(string node)
        {
            string[] list = new string[0];
            try
            {
                XmlNode root = m_objXmlDoc.SelectSingleNode(node);
                if (root != null)
                {
                    XmlAttributeCollection ac = root.Attributes;
                    if (root.Attributes.Count > 0)
                    {
                        list = new string[root.Attributes.Count];
                        int i = 0;
                        foreach (XmlAttribute attribute in ac)
                        {
                            list[i] = root.Attributes[attribute.Name].Value;//节点下的属性值
                            i++;
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return list;
            }
        }

        /// <summary>
        /// 查找节点指定属性的值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string FindNodeAttributeValue(string node, string attr)
        {
            string attributeValue = "";
            try
            {
                XmlNode root = m_objXmlDoc.SelectSingleNode(node);
                if (root != null)
                {
                    XmlAttributeCollection ac = root.Attributes;
                    foreach (XmlAttribute attribute in ac)
                    {
                        if (attribute.Name == attr)
                        {
                            attributeValue = root.Attributes[attribute.Name].Value;//节点下的属性值
                        }
                    }
                }
                return attributeValue;
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
                return attributeValue;
            }
        }

        /// <summary>
        /// 移除选定节点集的所有属性
        /// </summary>
        /// <param name="node"></param>
        public void RemoveAllAttribute(string node)
        {
            XmlNodeList xnl = m_objXmlDoc.SelectNodes(node);
            if (xnl != null)
            {
                foreach (XmlNode xn in xnl)
                {
                    xn.Attributes.RemoveAll();
                }
            }
        }

        /// <summary>
        /// 删除节点，及该节点下的所有子节点
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(string node)
        {
            try
            {
                string newNodePath = node.Substring(0, node.LastIndexOf("/"));
                m_objXmlDoc.SelectSingleNode(newNodePath).RemoveChild(m_objXmlDoc.SelectSingleNode(node));
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.Message;
            }
        }

        #region 获取日期、时间、星期
        public string Date()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd");
        }
        public string Time()
        {
            return System.DateTime.Now.ToString("HH:mm:ss:fff");
        }
        public string Week()
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            return week;
        }
        #endregion

        /// <summary>
        /// 类的序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="file">文件路径</param>
        /// <param name="obj">类</param>
        /// <returns>返回 Ok表示成功</returns>
        public static string Serialize<T>(string file, T obj)
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    CreateFile(file);
                }
                //Create 注意Create 与 OpenOrCreate 的区别， OpenOrCreate只是写入，不覆盖
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(obj.GetType());

                    xs.Serialize(fs, obj);
                    fs.Close();
                    return "Ok";
                }
            }
            catch (Exception ex)
            {
                return "进行Xml序列化时异常：" + ex.Message;
            }
        }
        /// <summary>
        /// 类的反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">完整文件路径</param>
        /// <param name="obj">返回值</param>
        /// <returns></returns>
        public static string Deserialize<T>(string file, ref T obj) where T : class
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    obj = null;
                    return "进行Xml反序列化时异常：文件不存在";
                }

                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    obj = xs.Deserialize(fs) as T;
                    return "Ok";
                }
            }
            catch (Exception ex)
            {
                obj = null;
                return "进行Xml反序列化时异常：" + ex.Message + "," + ex.InnerException.Message;
            }
        }

        /// <summary>
        /// 二进制序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string BinSerialize<T>(string file, T obj)
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    CreateFile(file);
                }
                //Create 注意Create 与 OpenOrCreate 的区别， OpenOrCreate只是写入，不覆盖
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    bf.Serialize(fs, obj);
                    fs.Close();
                    return "Ok";
                }
            }
            catch (Exception ex)
            {
                return "进行Xml序列化时异常：" + ex.Message;
            }
        }
        /// <summary>
        /// 二进制反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">完整文件路径</param>
        /// <param name="obj">返回值</param>
        /// <returns></returns>
        public static string BinDeserialize<T>(string file, ref T obj) where T : class
        {
            try
            {
                if (!System.IO.File.Exists(file))
                {
                    obj = null;
                    return "进行Xml反序列化时异常：文件不存在";
                }

                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    obj = bf.Deserialize(fs) as T;

                    return "Ok";
                }
            }
            catch (Exception ex)
            {
                obj = null;
                return "进行Xml反序列化时异常：" + ex.Message + "," + ex.InnerException.Message;
            }
        }

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">需要复制的类型</param>
        /// <returns>返回对象</returns>
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null)
            {
                return default(T);
            }
            T res = default(T);
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                res = (T)xml.Deserialize(ms);
                ms.Close();
            }
            return res;
        }

    }
}
