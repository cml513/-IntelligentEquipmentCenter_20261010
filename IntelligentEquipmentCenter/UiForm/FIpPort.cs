using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter.UPage
{
    public partial class FIpPort : UIForm
    {
        public FIpPort()
        {
            InitializeComponent();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (txtIP.Text != ""&&txtPort.Text!="")
            {               
                string strO = string.Empty;
                using (StreamReader sr = new StreamReader("cfg\\plc.cfg"))
                {
                    strO = sr.ReadToEnd();
                    sr.Close();
                }
                string[] strS = strO.Split('\n');
                for (int i = 0; i < strS.Length; i++)
                {
                    string s = strS[i];
                    if (s.Contains("IP"))
                    {
                        strS[i] = s.Replace(s.Substring(s.IndexOf('\"')), string.Format("\"{0}\"", txtIP.Text));

                    }
                    if (s.Contains("PORT"))
                    {
                        strS[i]= s.Replace(s.Substring(s.IndexOf('\"')), string.Format("\"{0}\"", txtPort.Text));
                    }

                }
                string strN = string.Join("\n", strS);
                File.WriteAllText("cfg\\plc.cfg", strN);
            }
           this.Dispose();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
