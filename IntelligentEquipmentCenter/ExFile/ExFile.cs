using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System;

namespace TF.File
{
    public enum FileType
    {
        All,
        Txt,
        Xml,
        Ini,
        Csv
    }

    public class ExFile
    {
        //Directory
        //File
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="file">文件完整路径</param>
        /// <returns>false 路径错误</returns>
        public static bool CreateFile(string file, bool cover = false)
        {
            try
            {
                //先创建文件夹，在创建文件，文件夹不存在直接创建文件会报错
                string path_buff = Path.GetDirectoryName(file);
                if (path_buff == null || path_buff == "")
                {
                    return false;
                }
                if (!Directory.Exists(path_buff))
                {
                    Directory.CreateDirectory(path_buff);
                }
                path_buff = Path.GetFileName(file);
                if (path_buff == null || path_buff == "")
                {
                    return false;
                }
                if (System.IO.File.Exists(file))
                {
                    if (!cover)
                    {
                        return true;
                    }
                    System.IO.File.Delete(file);
                }
                //一定要关闭，不然会占用文件
                System.IO.File.Create(file).Close();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file">文件完整路径</param>
        /// <returns></returns>
        public static bool DeleteFile(string file)
        {
            string path_buff = Path.GetFileName(file);

            if (path_buff == null || path_buff == "")
            {
                return false;
            }
            if (!System.IO.File.Exists(file))
            {
                return true;
            }
            System.IO.File.Delete(file);
            return true;
        }

        /// <summary>
        ///  从时间先后的顺序，删除指定数量的文件
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        /// <param name="maxNum">保存的最大文件数</param>
        public static void DeleteFileInDir(string dir, int maxNum)
        {
            try
            {
                if (!Directory.Exists(dir) || Path.GetDirectoryName(dir) == "")
                {
                    return;
                }

                DirectoryInfo di = new DirectoryInfo(dir);
                List<FileInfo> ls = new List<FileInfo>();
                //通过GetFiles方法，获取di目录中的所有文件
                foreach (FileInfo fi in di.GetFiles())
                {
                    ls.Add(fi);
                }
                if (ls.Count >= maxNum)
                {
                    ls = ls.OrderBy(p => p.CreationTime).ToList();//升序
                    //ls.OrderByDescending(p => p.CreationTime);//降序
                    for (int i = 0; i <= (ls.Count - maxNum); i++)
                    {
                        System.IO.File.Delete(ls[i].FullName);
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="dest">新文件路径</param>
        /// <param name="over">是否覆盖</param>
        /// <returns>false 路径错误</returns>
        public static bool CopyFile(string source, string dest, bool over = true)
        {
            try
            {
                if (source == "" || dest == "" || source == dest)
                {
                    return false;
                }
                if (!System.IO.File.Exists(source))
                {
                    return false;
                }
                System.IO.File.Copy(source, dest, over);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="dest">新文件路径</param>
        /// <returns></returns>
        public static bool MoveFile(string source, string dest)
        {
            try
            {
                if (source == "" || dest == "" || source == dest)
                {
                    return false;
                }
                if (!System.IO.File.Exists(source))
                {
                    return false;
                }
                System.IO.File.Move(source, dest);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        /// <returns>false 路径错误</returns>
        public static bool CreateDir(string dir)
        {
            try
            {
                if (dir == "")
                {
                    return false;
                }
                if (Directory.Exists(dir))
                {
                    return true;
                }
                Directory.CreateDirectory(dir);

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="recursive">是否删除子目录和文件</param>
        /// <returns>false 路径错误</returns>
        public static bool DeletDir(string dir, bool recursive = true)
        {
            try
            {
                if (dir == "")
                {
                    return false;
                }
                if (!Directory.Exists(dir))
                {
                    return true;
                }
                Directory.Delete(dir, recursive);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="srcdir"></param>
        /// <param name="desdir"></param>
        public static bool CopyDir(string source, string dest, bool overwrite)
        {
            if (source == dest)
            {
                return false;
            }
            bool ret = false;
            try
            {
                source = source.EndsWith(@"\") ? source : source + @"\";
                dest = dest.EndsWith(@"\") ? dest : dest + @"\";

                if (Directory.Exists(source))
                {
                    if (Directory.Exists(dest) == false)
                        Directory.CreateDirectory(dest);

                    foreach (string fls in Directory.GetFiles(source))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(dest + flinfo.Name, overwrite);
                    }
                    foreach (string drs in Directory.GetDirectories(source))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        //递归
                        if (CopyDir(drs, dest + drinfo.Name, overwrite) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch (System.Exception)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// 将文件或目录及其内容移到新位置。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns>false 路径错误</returns>
        public static bool MoveDir(string source, string dest)
        {
            try
            {
                if (source == "" || dest == "" || source == dest)
                {
                    return false;
                }
                if (!Directory.Exists(source))
                {
                    return false;
                }
                Directory.Move(source, dest);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
        /// <summary>
        /// 获取目录下指定类型的文件名称
        /// </summary>
        /// <param name="dir">目录</param>
        /// <param name="type">需要获取文件名称的类型</param>
        /// <returns>返回名称数组，失败返回为空</returns>
        public static string[] DirFiles(string dir, FileType type = FileType.All, bool sort = false)
        {
            string[] ary_name = new string[] { };

            string path = Path.GetDirectoryName(dir);
            if (path == null || path == "")
            {
                return ary_name;
            }

            if (!Directory.Exists(dir))
            {
                return ary_name;
            }

            List<string> lis_name = new List<string>();
            string str_type;
            switch (type)
            {
                case FileType.Txt:
                    str_type = ".txt";
                    break;
                case FileType.Xml:
                    str_type = ".xml";
                    break;
                case FileType.Ini:
                    str_type = ".ini";
                    break;
                case FileType.Csv:
                    str_type = ".csv";
                    break;
                default:
                    str_type = "";
                    break;
            }

            if (sort)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                List<FileInfo> ls = new List<FileInfo>();
                //通过GetFiles方法，获取di目录中的所有文件
                ls.AddRange(di.GetFiles());
                ls = ls.OrderBy(p => p.CreationTime).ToList();//升序

                for (int i = 0; i < ls.Count; i++)
                {
                    if (type == FileType.All)
                    {
                        lis_name.Add(ls[i].Name);
                    }
                    else
                    {
                        if (ls[i].Name.Contains(str_type))
                        {
                            lis_name.Add(ls[i].Name.Replace(str_type, ""));
                        }
                    }
                }
            }
            else
            {
                ary_name = Directory.GetFiles(dir);

                for (int i = 0; i < ary_name.Length; i++)
                {
                    ary_name[i] = Path.GetFileName(ary_name[i]);
                    if (type == FileType.All)
                    {
                        lis_name.Add(ary_name[i]);
                    }
                    else
                    {
                        if (ary_name[i].Contains(str_type))
                        {
                            lis_name.Add(ary_name[i].Replace(str_type, ""));
                        }
                    }

                }
            }
            return lis_name.ToArray();
        }

        /// <summary>
        /// 获取文件编码，通过文件流
        /// </summary>
        /// <param name="file">完整文件名</param>
        /// <returns></returns>
        public static Encoding GetFileEncod(string file)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            Encoding r = GetFileEncod(fs);
            fs.Close();
            return r;
        }

        /// <summary>
        ///获取文件编码， 通过二进制文件流
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static Encoding GetFileEncod(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new System.Exception("非预期的byte格式");
            }
            return true;
        }


        private const int OF_READWRITE = 2;
        private const int OF_SHARE_DENY_NONE = 0x40;
        private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        public static int FileIsOpen(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                return -1;
            }
            IntPtr handle = _lopen(file, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (handle == HFILE_ERROR)
            {
                //会导致文件被占用
                CloseHandle(handle);
                return 1;
            }
            CloseHandle(handle);
            return 0;
        }

    }
}
