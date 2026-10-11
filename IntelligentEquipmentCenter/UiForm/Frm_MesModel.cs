using IntelligentEquipmentCenter.UControl;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter.UiForm
{
    public partial class Frm_MesModel : Form
    {
        public Frm_MesModel()
        {
            InitializeComponent();
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Frm_MesModel_Load(object sender, EventArgs e)
        {
            if (EHomePage.MESModel == "单机") rb_dj.Checked = true;
           else if (EHomePage.MESModel == "离线") rb_lx.Checked = true;
            else  rb_zx.Checked = true;
        }

        private void ub_zx_MouseDown(object sender, MouseEventArgs e)
        {
            UIRadioButton radioButton = sender as UIRadioButton;
            if (!radioButton.Checked)
            {
                DialogResult res = MessageBox.Show("确定要切换模式吗？", "切换模式", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    radioButton.Checked = true;
                    if (radioButton.Text.Contains("在线"))
                    {
                        EHomePage.MESModel = "在线";
                    }
                    else if (radioButton.Text.Contains("离线"))
                    {
                        EHomePage.MESModel = "离线";
                    }
                    else if (radioButton.Text.Contains("单机"))
                    {
                        EHomePage.MESModel = "单机";
                    }
                }
                radioButton.Capture = false;
            }
        }
    }
}
