using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace TF.File
{
    public class Csv : ExFile
    {
        public static void Write(DataTable dt, string file)//table数据写入csv
        {
            FileInfo fi = new FileInfo(file);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            string data = "";

            for (int i = 0; i < dt.Columns.Count; i++)//写入列名
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++) //写入各行数据
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                      || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }
                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }

        public static DataTable Read(string file)//从csv读取数据返回table
        {
            Encoding encoding = GetFileEncod(file); //Encoding.ASCII;//
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(file, System.IO.FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(fs, encoding);

            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }


        /// <summary>
        /// 保存字典
        /// </summary>
        /// <typeparam name="Tv"></typeparam>
        /// <typeparam name="Tk"></typeparam>
        /// <param name="dic"></param>
        /// <param name="path"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Save<Tv, Tk>(Dictionary<Tk, Tv> dic, string path, out string msg)
        {
            try
            {
                msg = "Ok";
                if (!System.IO.File.Exists(path))
                {
                    if (!CreateFile(path))
                    {
                        msg = "创建文件失败";
                        return false;
                    }
                }

                var propertyInfos = typeof(Tv).GetFields();
                var csvList = new List<string>();
                var titlesb = new StringBuilder();
                titlesb.Append("Key");
                titlesb.Append(',');
                for (int i = 0; i < propertyInfos.Count(); i++)
                {
                    titlesb.Append(propertyInfos[i].Name);
                    if (i < propertyInfos.Count() - 1)
                    {
                        titlesb.Append(',');
                    }
                }
                csvList.Add(titlesb.ToString());
                foreach (var ins in dic)
                {
                    var @value = ins.Value;
                    var datasb = new StringBuilder();
                    datasb.Append(ins.Key);
                    datasb.Append(',');
                    for (int i = 0; i < propertyInfos.Count(); i++)
                    {
                        datasb.Append(propertyInfos[i].GetValue(@value).ToString());
                        if (i < propertyInfos.Count() - 1)
                        {
                            datasb.Append(',');
                        }
                    }
                    csvList.Add(datasb.ToString());
                }
                System.IO.File.WriteAllLines(path, csvList.ToArray(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tk"></typeparam>
        /// <typeparam name="Tv"></typeparam>
        /// <param name="path"></param>
        /// <param name="msg"></param>
        /// <returns>失败返回字典长度为0</returns>
        public static bool Load<Tk, Tv>(ref Dictionary<Tk, Tv> dic, string path, out string msg) where Tv : class
        {
            msg = "Ok";
            dic = new Dictionary<Tk, Tv>();
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    msg = "文件路径不存在";
                    return false;
                }
                var csvList = System.IO.File.ReadLines(path, Encoding.UTF8).ToList();
                var propertyInfos = typeof(Tv).GetFields();
                for (var i = 1; i < csvList.Count(); i++)
                {
                    var data = csvList[i].Split(',');
                    var tValue = typeof(Tv);
                    if (tValue.FullName == null) continue;
                    var obj = tValue.Assembly.CreateInstance(tValue.FullName);
                    for (int idx = 0; idx < propertyInfos.Length; idx++)
                    {
                        var info = propertyInfos[idx];
                        info.SetValue(obj, Convert.ChangeType(data[idx + 1], info.FieldType));
                    }
                    dic.Add((Tk)Convert.ChangeType(data[0], typeof(Tk)), obj as Tv);
                }
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
    }
}
