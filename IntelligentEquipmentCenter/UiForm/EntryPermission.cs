using IntelligentEquipmentCenter.UControl;
using SQLite.SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter.UiForm
{
    public partial class EntryPermission : Form
    {
        public EntryPermission()
        {
            InitializeComponent();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        //增加权限,需放入本地数据库

        public static  bool IsEntry=false;
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (txtAdress.Text.Trim()=="")
            {
                MessageBox.Show("用户名不能为空！");
                return;
            }
         
            if (txt_uid.Text.Trim() == "")
            {
                MessageBox.Show("UID不能为空，请刷卡！");
                return;
            }
            if (txt_user.Text.Trim() == "")
            {
                MessageBox.Show("工号不能为空，请手动输入！");
                return;
            }
            if (uiTextBox1.Text.Trim()=="")
            {
                MessageBox.Show("用户密码不能为空，请手动输入！");
                return;
            }
            if (uiTextBox2.Text.Trim() == "")
            {
                MessageBox.Show("MES账号不能为空，请手动输入！");
                return;
            }
            if (uiTextBox3.Text.Trim() == "")
            {
                MessageBox.Show("MES密码不能为空，请手动输入！");
                return;
            }
            object res2 = EHomePage.SQLiteHelpers.QueryOne("UidEnTry", "UID", txt_uid.Text.Trim());
            if (res2 != null)
            {
                MessageBox.Show("UID已经录入！请不要重复录入！");
                return;
            }

            object res3 = EHomePage.SQLiteHelpers.QueryOne("UidEnTry", "工号", txt_user.Text.Trim());
            if (res3 != null)
            {
                MessageBox.Show("工号已经录入！请不要重复录入！");
                return;
            }
            Dictionary<string, object> dic = new Dictionary<string, object>();

            dic.Add("用户名", txtAdress.Text.Trim());
            dic.Add("用户密码", LoginDialog.GetMD5Hash(uiTextBox1.Text.Trim()));
            dic.Add("用户权限", cbbAdressType.Text);
            dic.Add("UID", txt_uid.Text.Trim());
            dic.Add("最后登陆时间", "2000-01-01 00:00:00");
            dic.Add("登录次数", 0);
            dic.Add("工号", txt_user.Text.Trim());
            dic.Add("MES账号", uiTextBox2.Text.Trim());
            dic.Add("MES密码", uiTextBox3.Text.Trim());
            int res = EHomePage.SQLiteHelpers.InsertData("UidEnTry", dic);
            DataSet dataSet = EHomePage.SQLiteHelpers.ExecuteDataSet("SELECT * from UidEnTry", null);
           dataGridView1.DataSource = dataSet.Tables[0];
            EHomePage.AddDataChage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "工号" + EHomePage.Getuser() + ",添加权限工号为:" + txt_user.Text.Trim() + ",UID为:" + txt_uid.Text.Trim());
        }

        //查看已有的权限，看是否需要删除
        private void uiButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentCell == null) return;
                string count = dataGridView1.CurrentCell.Value.ToString();
                if (count.Length>0)
                {
                    DialogResult dialog = MessageBox.Show("确定删除当前选中行吗？", "警告",MessageBoxButtons.OKCancel);
                    if (dialog == DialogResult.OK )
                    {
                        EHomePage.SQLiteHelpers.Delete("UidEnTry", "工号", count);
                        DataSet dataSet = EHomePage.SQLiteHelpers.ExecuteDataSet("SELECT * from UidEnTry", null);
                        dataGridView1.DataSource = dataSet.Tables[0];
                    }
                }
                EHomePage.AddDataChage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "工号" + EHomePage.Getuser() + ",添加权限工号为:" + txt_user.Text.Trim() + ",UID为:" + txt_uid.Text.Trim());
            }
            catch (Exception)
            {

            }
          
        }

        private void EntryPermission_Load(object sender, EventArgs e)
        {
            IsEntry = true;
            string sql = "SELECT * from UidEnTry";
            cbbAdressType.Text = "OP";
            try
            {
                DataSet dataSet = EHomePage.SQLiteHelpers.ExecuteDataSet(sql, null);
                if (dataSet != null)
                {
                    dataGridView1.DataSource = dataSet.Tables[0];
                }
            }
            catch (Exception)
            {

            }
            
        }


        private  string uid;

        public  string UID
        {
            get { return uid; }
            set { uid = value;
            }
        }


        private void EntryPermission_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsEntry = false;
            Serport.RecE50C = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Serport.RecE50C != "")
            {
                txt_uid.Clear();
                txt_uid.Text = Serport.RecE50C;
            }
        }

        private void txt_uid_DoubleClick(object sender, EventArgs e)
        {
            Serport.RecE50C = "";
            txt_uid.Clear();
        }

    }
}
