using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter.UControl
{
    public partial class LightStat : UserControl
    {
        public delegate void ClickDelegate(string name);
        public event ClickDelegate ClickEvent;
        public LightStat()
        {
            InitializeComponent();
        }

        private bool _lightMode=false;

        public bool lightMode
        {
            get { return _lightMode; }
            set { _lightMode = value;
                setBtnStatus(_lightMode);
            }
        }


        public void setBtnStatus(bool status)
        {
            if (pictSafe.InvokeRequired)
            {
                    if (status)
                    {
                        pictSafe.Invoke(new Action(() =>
                        {
                            pictSafe.Image = global::IntelligentEquipmentCenter.Properties.Resources.绿灯1;
                            //pictSafe.Text = txtBtnDown;
                        }));
                    }
                    else
                    {
                        pictSafe.Invoke(new Action(() =>
                        {
                            pictSafe.Image = global::IntelligentEquipmentCenter.Properties.Resources.灰灯1;
                            //pictSafe.BackColor = Color.SeaGreen;
                        }));

                    }

            }
            else
            {
                if (status)
                {
                    //pictSafe.Text = txtBtnDown;
                    pictSafe.Image = global::IntelligentEquipmentCenter.Properties.Resources.绿灯1;
                }
                else
                {
                    //pictSafe.Text = txtBtnUp;
                    pictSafe.Image = global::IntelligentEquipmentCenter.Properties.Resources.灰灯1;
                }

            }
        }

        private string titleName;

        public string TitleName
        {
            get { return titleName; }
            set { titleName = value; 
                label1.Text = titleName;
            }
        }

        private void pictSafe_Click(object sender, EventArgs e)
        {
            ClickEvent?.Invoke(TitleName);
        }
    }
}
