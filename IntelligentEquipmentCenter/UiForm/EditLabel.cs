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
    public partial class EditLabel : Form
    {
        public EditLabel()
        {
            InitializeComponent();
        }

        private string labelTxt;
        public string LabelTxt
        {
            get
            {
                labelTxt = txtLable.Text;
                return labelTxt;
            }
            set
            {
                labelTxt = value;
                txtLable.Text = labelTxt;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

    }
}
