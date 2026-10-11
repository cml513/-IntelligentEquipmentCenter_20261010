using IntelligentEquipmentCenter.UControl;
using SQLite.SQLite;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IntelligentEquipmentCenter.ClientJson;

namespace IntelligentEquipmentCenter.UiForm
{
    public partial class FProductionInfos : UserControl
    {
        private string _sn;
        string DBAddress;
       // public SQLiteHelper SQLiteHelpers;
        DataSet dataSet;
        public FProductionInfos()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 外部传入条码查询（替代原弹窗构造）
        /// </summary>
        public void QueryBySn(string sn)
        {
            string tableNameCur = "testRecord" + DateTime.Now.Date.ToString("yyyyMMdd");
            InitialDgv(tableNameCur, sn);
        }
        public void InitialDgv(string tablename, string sn=null)
        {
            _sn= sn;
            txt_sn.Text = _sn;
            SQLiteParameter[] parameter = new SQLiteParameter[]
            {
                new SQLiteParameter("总成条码",_sn)
            };

            string sql;
            if (sn!=null&& sn.Length==18)
            sql = string.Format("SELECT * FROM {0} WHERE 总成条码 = '{1}' ", tablename, sn);
            else if (sn != null && sn.Length == 21)
            {
                sql = string.Format("SELECT * FROM {0} WHERE 产品编码 = '{1}' ", tablename, sn);
            }
            else if (sn=="NG"|| sn == "OK")
            {
                sql = string.Format("SELECT * FROM {0} WHERE 测试总结果 = '{1}' ", tablename, sn);
            }
            else if(sn == null)
             sql = string.Format("SELECT * FROM {0} ", tablename);
            else 
            {
                sql = string.Format("SELECT * FROM {0} WHERE 工单号 = '{1}' ", tablename, sn);
            }
            dgvDatas.ClearAll();
            dataSet = EHomePage.SQLiteHelpers.ExecuteDataSet(sql, null);
            if (dataSet != null)
            {
                dgvDatas.DataSource = dataSet.Tables[0];
            }
        }


        private void dgvDatas_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Sortdgv(sender, e);
        }

        //表格自加序
        private void Sortdgv(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowidx = (e.RowIndex + 1).ToString();
            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var headerBounds = new System.Drawing.Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowidx, new Font("宋体", 12), SystemBrushes.ControlText, headerBounds, centerFormat);
        }


        private void dgvDatas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string name = dgvDatas.Columns[e.ColumnIndex].Name;
            if (name.Equals("测试总结果") || name.Equals("螺丝浮高检测判断结果") ||name.Equals("螺丝扭矩检测判断结果") || name.Equals("电批旋转角度检测判断结果"))
            {
                string value = e.Value.ToString();
                if(e.Value != null&& value == "OK")
                {
                    e.CellStyle.BackColor = Color.DarkGreen;
                    e.CellStyle.ForeColor = Color.White;

                }
                else if(e.Value != null && value == "NG")
                {
                    e.CellStyle.BackColor = Color.DarkRed;
                    e.CellStyle.ForeColor = Color.White;

                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DateTime starttime=Convert.ToDateTime(time_down.Text);
            DateTime endtime = Convert.ToDateTime(time_up.Text);
            if (starttime>= endtime)
            {
                MessageBox.Show("开始时间大于或等于结束时间！","提示");
                return;
            }
            var TableNames = GenerateTableNames(starttime, endtime);
            //提取界面参数，避免后台线程访问控件
            string snText = txt_sn.Text.Trim();
            string resText = txt_res.Text.Trim();
            string peiText = txt_pei.Text.Trim();
            string timeDown = time_down.Text;
            string timeUp = time_up.Text;

            btnSelect.Enabled = false;
            //查询放后台线程，防止大数据量时界面卡死
            Task.Run(() =>
            {
                List<DataSet> results = new List<DataSet>();
                foreach (string TableName in TableNames)
                {
                    if (!TableWxists(TableName)) continue;
                    string sql = $"SELECT * FROM {TableName}  ";
                    sql += string.Format("WHERE 测试时间 >= '{0}' AND 测试时间 <= '{1}' ", timeDown, timeUp);

                    if (snText.Length == 18)
                        sql += string.Format(" AND 总成条码 = '{0}' ", snText);
                    else if (snText.Length == 21)
                        sql += string.Format(" AND 产品编码 = '{0}' ", snText);

                    if (resText == "NG" || resText == "OK")
                        sql += string.Format(" AND 测试总结果 = '{0}' ", resText);

                    if (!peiText.IsNullOrEmpty())
                        sql += string.Format(" AND 当前配方名称 = '{0}' ", peiText);

                    DataSet dataSet1 = EHomePage.SQLiteHelpers.ExecuteDataSet(sql, null);
                    if (dataSet1 != null && dataSet1.Tables[0].Rows.Count > 0)
                        results.Add(dataSet1);
                }
                DataSet merged = MargeDataSet(results);
                int resultCount = results.Count;

                //绑定回UI线程
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        dgvDatas.ClearAll();
                        dataSet = merged;
                        if (resultCount > 0)
                        {
                            dgvDatas.DataSource = merged.Tables[0];
                        }
                        // 自动调整列宽以适应内容
                        dgvDatas.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    }
                    finally
                    {
                        btnSelect.Enabled = true;
                    }
                }));
            });
        }

        private void FProductionInfos_Load(object sender, EventArgs e)
        {
            time_down.Text = DateTime.Now.ToString("yyyy-MM-dd") + " " + "00:00:00";
            time_up.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        StringBuilder csvConet = new StringBuilder();
        //导出
        private void uiButton1_Click(object sender, EventArgs e)
        {
            csvConet.Clear();
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "保存文件";
                saveFileDialog.DefaultExt = "xls";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    if (dataSet != null)
                    {
                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            string res2 = "";
                            if (i == 0)
                            {
                                string res = "";

                                foreach (var item in EHomePage.Stutable)
                                {
                                    res += item.Key + ",";
                                }
                                csvConet.AppendLine(res);
                            }

                            for (int j = 0; j < EHomePage.Stutable.Count; j++)
                            {
                                if (j < EHomePage.Stutable.Count - 1)
                                    res2 += dataSet.Tables[0].Rows[i][j].ToString() + ",";
                                else
                                    res2 += dataSet.Tables[0].Rows[i][j].ToString();
                            }
                            csvConet.AppendLine(res2);
                        }
                        File.WriteAllText(filePath, csvConet.ToString(), Encoding.UTF8);
                    }
                }
            }

        }


        private List<string> GenerateTableNames(DateTime starttime, DateTime endtime)
        {
            var tableNames=new List<string>();
            DateTime CurrentDate = starttime;
            while (CurrentDate<= endtime)
            {
                string tableName = $"testRecord{CurrentDate:yyyyMMdd}";
                tableNames.Add(tableName);
                CurrentDate= CurrentDate.AddDays(1);
            }
            return tableNames;
        }

        private bool TableWxists(string tableName)
        {
            string sql = string.Format("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name= '{0}' ", tableName);
            object res = EHomePage.SQLiteHelpers.ExecuteScalar(sql,null);
            if (Convert.ToInt16(res)>0)
            {
                return true;
            }
            else return false;
        }

        private DataSet MargeDataSet(List<DataSet> dataSets)
        {
            if (dataSets==null|| dataSets.Count==0)
            {
                return new DataSet();
            }
            DataSet ds = new DataSet();
            foreach (DataSet item in dataSets)
            {
                if (item!=null)
                {
                    ds.Merge(item);
                }
            }
            return ds;
        }


    }
}
