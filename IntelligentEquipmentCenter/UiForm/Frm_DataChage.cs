using IntelligentEquipmentCenter.UControl;
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
    public partial class Frm_DataChage : Form
    {
        public Frm_DataChage()
        {
            InitializeComponent();
        }

        private void Frm_DataChage_Load(object sender, EventArgs e)
        {
            dgvDatas.ClearAll();
            DataSet set1  = EHomePage.SQLiteHelpers.ExecuteDataSet3("StutableChange", true);
            if (set1!= null)
            {
                dgvDatas.DataSource = set1.Tables[0];
            }
            // 自动调整列宽以适应内容  
            dgvDatas.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
    }
}
