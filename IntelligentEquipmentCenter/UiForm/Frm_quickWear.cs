using IntelligentEquipmentCenter.UControl;
using SQLite.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter
{
    public partial class Frm_quickWear : Form
    {
        DataTable dataTableX=null;
        public Frm_quickWear()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            dataTableX = ImportData.ImportToDG("cfg\\易损件.xls", 0, 3, true);
            //DgInit(dataTableX);
           DataSet data=  EHomePage.SQLiteHelpers.ExecuteDataSet3(tableNameCur,true);
          if (data!=null)
          {
              dataGridView1.DataSource = data.Tables[0]; 
          }
        }




        private void btn_load_Click(object sender, EventArgs e)
        {
            //dataGridView1.Rows.Clear();
            dataTableX = ImportData.ImportToDG("cfg\\易损件.xls", 0, 3, true);
            //DgInit(dataTableX);

        }

        private void btn_open_Click(object sender, EventArgs e)
        {
            ImportData.OpenFile("cfg\\易损件.xls");
        }

        ClientJson clientJson = new ClientJson();
        private void btn_mes_Click(object sender, EventArgs e)
        {
            //获取表
            DataSet data = EHomePage.SQLiteHelpers.ExecuteDataSet3(tableNameCur, true);
            if (data != null)
            {
                if (data.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                    {

                        var data1 = new
                        {
                            qwpWorkStation = "工位1",
                            machineName = "丝杆打螺丝",
                            qwpPosition = data.Tables[0].Rows[i][0].ToString(),
                            qwpName = data.Tables[0].Rows[i][1].ToString(),
                            qwpTheoreticalUseTimes = data.Tables[0].Rows[i][2],
                            qwpUsedTimes = 6000,
                            qwpResidualUseTimes = 2000
                        };
                        clientJson.Post("http://192.168.20.201:8088/api/quickwearpart/save/", clientJson.Js(data1));
                    }
                }
            }
        }

        public void DgInit(DataTable dataTable)
        {
            try
            {

                dataGridView1.Rows.Clear();
                foreach (DataRow row in dataTable.Rows)
                {
                    //创建一个新的DataGridView行
                    int rowIndex = dataGridView1.Rows.Add();

                    for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
                    {
                       dataGridView1.Rows[rowIndex].Cells[columnIndex ].Value = row[columnIndex];
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public static SQLiteHelper SQLiteHelpers = null;
        private string DBAddress;
        private Dictionary<string, object> Stutable = new Dictionary<string, object>();
        private string tableNameCur = "QuickWear";//当前数据库表名
        //刷新数据库
        private void uiButton1_Click(object sender, EventArgs e)
        {
            Stutable.Clear();
            //创建表
            Stutable.Add("易损件位置", "text");
            Stutable.Add("易损件名称", "text");
            Stutable.Add("易损件理论使用次数", "int");
            Stutable.Add("易损件已使用次数", "int");
            try
            {
                //打开数据库
                string local = System.AppDomain.CurrentDomain.BaseDirectory;
                DBAddress = local + "\\testRecord.db3";
                SQLiteHelper.CreateDB(DBAddress);
                SQLiteHelper.CreateTable(tableNameCur, DBAddress, Stutable);

                //更新表
                EHomePage.SQLiteHelpers.Delete(tableNameCur, null, 0);

                //更新表
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (DataRow row in dataTableX.Rows)
                {
                    if (row[0].ToString().Length < 2) break;
                    dic.Clear();
                    for (int columnIndex = 0; columnIndex < dataTableX.Columns.Count; columnIndex++)
                    {
                        switch (columnIndex)
                        {
                            case 0:
                                dic.Add("易损件位置", row[columnIndex]);
                                break;
                            case 1:
                                dic.Add("易损件名称", row[columnIndex]);
                                break;
                            case 2:
                                dic.Add("易损件理论使用次数", row[columnIndex]);
                                dic.Add("易损件已使用次数",int.Parse( uiTextBox1.Text.Trim()));
                                break;
                        }
                    }
                    EHomePage.SQLiteHelpers.InsertData(tableNameCur, dic);
                }


                DataSet data = EHomePage.SQLiteHelpers.ExecuteDataSet3(tableNameCur, true);
                if (data != null)
                {
                    dataGridView1.DataSource = data.Tables[0];
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("更新失败:" +ex.Message);
            }
          

        }
    }

    public class quickWeardata
    {
        /// <summary>
        /// 工位名称
        /// </summary>
        public string WorkpositionName { get; set; }

        /// <summary>
        /// 机台名称
        /// </summary>
        public string MachineName { get; set;}

        /// <summary>
        /// 易损件位置
        /// </summary>
        public string QwpPosition { get; set; }

        /// <summary>
        /// 易损件名称
        /// </summary>
        public string QwpName { get; set; }

        /// <summary>
        /// 易损件理论使用次数
        /// </summary>
        public int QwpTheoreticalUseTimes { get; set; }

        /// <summary>
        /// 易损件已使用次数
        /// </summary>
        public int QwpUsedTimes { get; set; }
        /// <summary>
        /// 易损件剩余使用次数
        /// </summary>
        public int QwpResidualUseTimes { get; set; }
    }
}
