using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Spire.Xls;

namespace IntelligentEquipmentCenter.UiForm
{
    public partial class BootScreen : Form
    {
        public BootScreen()
        {
            InitializeComponent();

            Thread thread = new Thread(dateTimeThread);
            thread.IsBackground = true;
            thread.Start();
        }

        //获取系统时间线程
        private void dateTimeThread()
        {
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    if (txtDateTime.InvokeRequired)
                    {
                        txtDateTime.Invoke(new Action(() =>
                        {
                            try
                            {
                                txtDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
                            }
                            catch (Exception ex)
                            {
                                //HPageBreak;
                            }

                        }));
                    }
                }
                catch 
                {
                    break;
                }

            }
        }
    }

    
}
