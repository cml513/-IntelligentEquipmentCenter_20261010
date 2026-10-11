using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IntelligentEquipmentCenter.ClientJson;

namespace IntelligentEquipmentCenter.UiForm
{
    public partial class Frm_Linedata : Form
    {
        public Frm_Linedata()
        {
            InitializeComponent();
        }
        private UILineOption option1;
        private UILineOption option2;
        private UILineOption option3;

        private List<double> timedatas =new List<double>();
        private List<double> niujudatas = new List<double>();
        private List<double> jiaodudatas = new List<double>();

        private void uiButton1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "选择文件";
                    openFileDialog.InitialDirectory = "D:\\电气资料\\TTA_HOST_V3.1.2\\TTA_HOST_V3.1.2\\data\\curve";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.CheckPathExists = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = uiTextBox1.Text = openFileDialog.FileName;
                        Frm_Linedata_Load(null,null);
                        timedatas.Clear();
                        niujudatas.Clear();
                        jiaodudatas.Clear();
                        DataTable datatable= ImportData.ImportToDG(filePath, 0,4,true);
                       foreach (DataRow row in datatable.Rows)
                        {
                            if (row[3].ToString().Length<3)
                            {
                                continue;
                            }
                            timedatas.Add(Convert.ToDouble(row[1]));
                            niujudatas.Add(Convert.ToDouble(row[2]));
                            jiaodudatas.Add(Convert.ToDouble(row[3]));
                        }
                        AddChartData1("线条1", timedatas,niujudatas);
                        AddChartData2("线条2", timedatas, jiaodudatas);
                        AddChartData3("线条3", jiaodudatas, niujudatas);
                        SetChartineStyle();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("数据模板不对，请重新选择");
            }
        }

        private void Frm_Linedata_Load(object sender, EventArgs e)
        {
            uiLineChart1.Option.Clear();
            uiLineChart2.Option.Clear();
            uiLineChart3.Option.Clear();
            option1 = uiLineChart1.Option;
            option2 = uiLineChart2.Option;
            option3 = uiLineChart3.Option;

        }

        private void AddChartData1(string name, List<double> arrx, List<double> arry)
        {
            var series = option1.AddSeries(new UILineSeries("Line1"));
            for (int i = 0; i < arrx.Count; i++)
            {
                series.Add(arrx[i], arry[i]);
            }
            //series.SymbolColor = UIColor.Blue;

            //series = option1.AddSeries(new UILineSeries("Line4"));
            //for (int i = 0; i < arrx.Count; i++)
            //{
            //    series.Add(arrx[i], jiaodudatas[i]);
            //}
            series.SymbolColor = UIColor.Red;

            uiLineChart1.Option.AddSeries(series);
            //uiLineChart1.SetOption(option1);
        }

        private void AddChartData2(string name, List<double> arrx, List<double> arry)
        {
            var series = option2.AddSeries(new UILineSeries("Line2"));
            for (int i = 0; i < arrx.Count; i++)
            {
                series.Add(arrx[i], arry[i]);
            }
       
            uiLineChart2.Option.AddSeries(series);
        }
        private void AddChartData3(string name, List<double> arrx, List<double> arry)
        {
            var series = option3.AddSeries(new UILineSeries("Line3"));
            for (int i = 0; i < arrx.Count; i++)
            {
                series.Add(arrx[i], arry[i]);
            }
            uiLineChart3.Option.AddSeries(series);
        }
        private void SetChartineStyle()
        {
            uiLineChart1.Option.ShowZeroLine = true;
            uiLineChart1.Option.ShowZeroValue = true;
            uiLineChart1.Option.XAxis.Clear();
            var uiTitle=new UITitle { Text = "电批测试数据"};
            uiLineChart1.Option.Title = uiTitle;
            uiLineChart1.Option.XAxis.Name = "时间";
            uiLineChart1.Option.YAxis.Name = "扭矩";
            uiLineChart1.Option.XAxis.ShowGridLine = true;
            uiLineChart1.Option.YAxis.ShowGridLine = true;
            uiLineChart1.Option.XAxisType = UIAxisType.Value;
            uiLineChart1.Option.XAxis.SetRange(0, timedatas.Max());
            uiLineChart1.Option.YAxis.SetRange(0, niujudatas.Max());
            uiLineChart1.Refresh();

            uiLineChart2.Option.ShowZeroLine = true;
            uiLineChart2.Option.ShowZeroValue = true;
            uiLineChart2.Option.XAxis.Clear();
            var uiTitle2 = new UITitle { Text = "电批测试数据" };
            uiLineChart2.Option.Title = uiTitle2;
            uiLineChart2.Option.XAxis.Name = "时间";
            uiLineChart2.Option.YAxis.Name = "角度";
            uiLineChart2.Option.XAxis.ShowGridLine = true;
            uiLineChart2.Option.YAxis.ShowGridLine = true;
            uiLineChart2.Option.XAxisType = UIAxisType.Value;
            uiLineChart2.Option.XAxis.SetRange(0, timedatas.Max());
            uiLineChart2.Option.YAxis.SetRange(0, jiaodudatas.Max());
            uiLineChart2.Refresh();

            uiLineChart3.Option.ShowZeroLine = true;
            uiLineChart3.Option.ShowZeroValue = true;
            uiLineChart3.Option.XAxis.Clear();
            var uiTitle3 = new UITitle { Text = "电批测试数据" };
            uiLineChart3.Option.Title = uiTitle3;
            uiLineChart3.Option.XAxis.Name = "角度";
            uiLineChart3.Option.YAxis.Name = "扭矩";
            uiLineChart3.Option.XAxis.ShowGridLine = true;
            uiLineChart3.Option.YAxis.ShowGridLine = true;
            uiLineChart3.Option.XAxisType = UIAxisType.Value;
            uiLineChart3.Option.XAxis.SetRange(0, jiaodudatas.Max());
            uiLineChart3.Option.YAxis.SetRange(0, niujudatas.Max());
            uiLineChart3.Refresh();

        }
    }
}
