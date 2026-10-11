using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    /// <summary>
    /// 程序日志输出工具类
    /// </summary>
    class Logger
    {
        ///<summary>
        ///log所在目录
        /// </summary>
        private const string LOG_DIR = "log";

        ///<summary>
        ///log全路径
        /// </summary>
        private const string LOG_PATH = "log\\{0}.log";

        private static Queue<Log> logs = new Queue<Log>();

        private static Boolean isSaving = false;

        private static Thread saveThread;

        public static EventWaitHandle _wh = new AutoResetEvent(false);

        public struct LogType
        {
            public static string ERROR { get { return "ERROR"; } }
            public static string WARNING { get { return "WARNING"; } }
            public static string INFO { get { return "INFO"; } }

        }

        public static void Start()
        {
            try
            {
                isSaving = true;
                saveThread = new Thread(save);
                saveThread.IsBackground = true;
                saveThread.Start();
            }
            catch(Exception e)
            {

            }
        }

        public void Stop()
        {
            try
            {
                _wh.Close();
            }
            catch
            {

            }
        }

        private static void save()
        {
            while(true)
            {
                if (!isSaving) continue;
                try
                {
                    Log log = null;
                    lock(logs.GetType())
                    {
                        if (logs.Count > 0)
                        {
                            log = logs.Dequeue();
                            if (log == null) continue;

                        }
                        if(log!=null)
                        {
                            StreamWriter sw = null;
                            string logStr;
                            string today = log.now.ToString("yyyy-MM-dd");

                            try
                            {
                                if(!Directory.Exists(LOG_DIR))
                                {
                                    Directory.CreateDirectory(LOG_DIR);
                                }

                                if(!File.Exists(string.Format(LOG_PATH, today)))
                                {
                                    File.Create(string.Format(LOG_PATH, today)).Close();
                                }
                                sw = new StreamWriter(string.Format(LOG_PATH, today),true);


                                if(log.ex != null)
                                {
                                    //输出log日期
                                    //logStr = "信息："+log.ex.Message;
                                    //logStr += Environment.NewLine + log.ex.StackTrace;2024/5/31 12:08:46
                                    logStr = DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss") + " [" + log.logPlace + "] " + log.logType + "：";
                                    logStr += log.ex.Message;
                                    logStr += log.ex.StackTrace;


                                }
                                else
                                {
                                    //logStr = "信息：" + log.message;
                                    logStr = DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss") + " [" + log.logPlace + "] " + log.logType + "：";
                                    logStr += log.message;
                                }

                                sw.WriteLine(logStr);


                            }
                            catch
                            {

                            }
                            finally
                            {
                                
                                if(sw != null)
                                {
                                    sw.Close();
                                }
                            }

                            
                        }
                        else
                        {
                            _wh.WaitOne();
                        }

                    }
                        
                }
                catch(Exception ex)
                {

                }
            }

        }

        public static void push(Log log)
        {
            logs.Enqueue(log);
            _wh.Set();
        }

    }


    class Log
    {
        public string logType;
        public string logPlace;
        public string message;
        public DateTime now;
        public Exception ex;

        public Log(string logType, string logPlace, string message)
        {
            this.logType = logType;
            this.logPlace = logPlace;
            this.message = message;
            this.now = DateTime.Now;
            //this.ex = ex;
        }

        public Log(string logType, string logPlace, Exception ex)
        {
            this.logType = logType;
            this.logPlace = logPlace;
            //this.message = message;
            this.now = DateTime.Now;
            this.ex = ex;
        }
    }
}
