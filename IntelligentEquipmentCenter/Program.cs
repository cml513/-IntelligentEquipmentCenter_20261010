using IntelligentEquipmentCenter.UControl;
using IntelligentEquipmentCenter.UiForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;

namespace IntelligentEquipmentCenter
{

    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //string cpuNumber = "BFEBFBFF000506E3";

            //if(getCpuSerialNumber() != cpuNumber)
            //{
            //    MessageBox.Show("系统配置出现问题，请联系管理员！", "系统BUG");
            //    return;
            //}

            Process currentProcess= Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
            if (processes.Length > 1 ) {
                MessageBox.Show("程序已经在运行中！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            //BootScreen form1 = new BootScreen();
            //form1.Show();
            //LoginDialog loginDialog = new LoginDialog();
            MainWindow window = new MainWindow();
            //if (loginDialog.ShowDialog() == DialogResult.OK)
            //{
            //    //form1.Hide();
            //    form1.Close();
            //    form1.Dispose();
            //}
            //else
            //{
            //    //form1.Hide();
            //    form1.Close();
            //    form1.Dispose();
            //}

           
            Application.Run(window);

            
        }

        //public static string getCpuSerialNumber()
        //{
        //    string strCpu = null;
        //    ManagementClass myCpu = new ManagementClass("win32_Processor");
        //    ManagementObjectCollection myCpuCollection = myCpu.GetInstances();
        //    foreach (ManagementObject obj in myCpuCollection)
        //    {
        //        strCpu = obj.Properties["Processorid"].Value.ToString();
        //        break;
        //    }
        //    return strCpu;
        //}




        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var msg = e.ExceptionObject as Exception;
            Logger.push(new Log(Logger.LogType.INFO, "Program", msg));
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Logger.push(new Log(Logger.LogType.INFO, "Program", e.Exception.Message));
        }


    }
}
