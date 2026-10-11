using System.IO;
using System.Text;


namespace TF.File
{
    /// <summary>
    /// 追加位置
    /// Begin  开始
    /// End    末尾
    /// </summary>
    public enum AppendPos
    {
        Begin,
        End
    }
    public class Txt : ExFile
    {
        //StreamReader/StreamWriter:操作字符数据，通过编码转化为字节可能会有数据丢失，适合小文件读取
        //FileStream:操作字节或字节数组，有些操作，比如随机文件访问(访问文件中间某点的数据)

        /// <summary>
        /// StreamWriter写入内容 使用简体中文编码
        /// </summary>
        /// <param name="path">文件完整路径</param>
        /// <param name="data">数据</param>
        /// <param name="append">是否追加 true追加</param>
        /// <returns>false 路径错误</returns>
        public static bool SWrite(string path, string data, bool append = false)
        {
            if (!CreateFile(path))
            {
                return false;
            }
            if (FileIsOpen(path) == 1)
            {
                return false;
            }
            StreamWriter sw = new StreamWriter(path, append, Encoding.GetEncoding("gb2312"));
            sw.Write(data);

            sw.Flush();
            sw.Close();

            return true;

        }
        /// <summary>
        /// StreamWriter写入行 使用简体中文编码
        /// </summary>
        /// <param name="path">文件完整路径</param>
        /// <param name="data">数据</param>
        /// <param name="append">是否追加 true追加</param>
        public static bool SWriteLine(string path, string data, bool append = true)
        {

            if (!CreateFile(path))
            {
                return false;
            }
            if (FileIsOpen(path) == 1)
            {
                return false;
            }
            StreamWriter sw = new StreamWriter(path, append, Encoding.GetEncoding("gb2312"));
            sw.WriteLine(data);
            sw.Flush();
            sw.Close();
            return true;

        }
        /// <summary>
        /// StreamReader读取
        /// </summary>
        /// <param name="path">文件完整路径</param>
        /// <returns>返回数据，失败为""</returns>
        public static string SRead(string path)
        {
            string data = "";
            if (!System.IO.File.Exists(path))
            {
                return data;
            }
            StreamReader sr = new StreamReader(path, Encoding.GetEncoding("gb2312"));
            data = sr.ReadToEnd();
            sr.Close();

            return data;
        }

        /// <summary>
        /// FileStream文件写入
        /// </summary>
        /// <param name="path">文件完整路径 </param>
        /// <param name="data">数据</param>
        /// <param name="append">是否追加</param>
        /// <param name="pos">追加位置</param>
        public static bool FWrite(string path, string data, bool append = false, AppendPos pos = AppendPos.End)
        {
            string path_buff = Path.GetFileName(path);
            if (path_buff == null || path_buff == "")
            {
                return false;
            }

            byte[] by_data = Encoding.UTF8.GetBytes(data);
            FileStream fs;

            if (append)
            {
                fs = new FileStream(path, (System.IO.File.Exists(path) ? FileMode.Open : FileMode.Create), FileAccess.Write);
                switch (pos)
                {
                    case AppendPos.Begin:
                        fs.Seek(0, SeekOrigin.Begin);
                        break;
                    default:
                        fs.Seek(0, SeekOrigin.End);
                        break;
                }
            }
            else
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            }

            fs.Write(by_data, 0, by_data.Length);
            fs.Flush();
            fs.Close();
            return true;
        }

        /// <summary>
        /// FileStream文件读取
        /// </summary>
        /// <param name="path">文件完整路径</param>
        /// <returns>返回数据，失败为""</returns>
        public static string FRead(string path)
        {
            string st_data = "";
            if (!System.IO.File.Exists(path))
            {
                return st_data;
            }
            //创建文件流
            FileStream fs = new FileStream(path, FileMode.Open);
            //移动光标
            fs.Seek(0, SeekOrigin.Begin);
            byte[] by_data = new byte[fs.Length];
            //读取文件内容
            fs.Read(by_data, 0, by_data.Length);
            //把byte数组按编码转化为字符串
            st_data = Encoding.UTF8.GetString(by_data);

            fs.Flush();
            fs.Close();

            return st_data;
        }
    }
}
