using IntelligentEquipmentCenter.UControl;
using SQLite.SQLite;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IntelligentEquipmentCenter.UiForm
{
    public partial class LoginDialog : Form
    {
        //string[] nameList = new string[5];
        //string[] passwordList = new string[5];
        public static int EditStatus = 1;//0表示编辑，1表示禁止编辑
        public static int LoginStatus = 0;//0
        public static string GetMD5Hash(string input)
        {
            // 创建一个MD5对象
            using (MD5 md5 = MD5.Create())
            {
                // 将输入字符串转换为字节数组
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                // 计算散列
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                // 将字节转换为十六进制字符串
                StringBuilder hashString = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashString.Append(hashBytes[i].ToString("X2"));
                }
                return hashString.ToString();
            }
        }

        private bool visedisop;

        public bool Visedisop
        {
            get { return visedisop; }
            set { visedisop = value;
            }
        }


        public LoginDialog()
        {
            InitializeComponent();
         

           //加载密码

           //XmlDocument xmlDoc = new XmlDocument();
           // xmlDoc.Load("xml/LoginInfo.xml");

           // //Console.WriteLine(xmlDoc.OuterXml);
           // int i = 0;
           // foreach (XmlElement buttonElement in xmlDoc.GetElementsByTagName("User"))
           // {
           //     nameList[i] = buttonElement.GetAttribute("name");
           //     passwordList[i] = buttonElement.GetAttribute("password");
           //     i++;
           // }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            string index = cbbUser.Text;
            string password = txtPassword.Text;
            DataSet set5 =EHomePage.SQLiteHelpers.ExecuteDataSet5("UidEnTry", "工号", cbbUser.Text, "用户密码", GetMD5Hash(txtPassword.Text.Trim()));

            if (set5!=null && set5.Tables[0].Rows.Count>0)
            {
                this.DialogResult = DialogResult.OK;
                txtPassword.Text = "";
                switch (set5.Tables[0].Rows[0]["用户权限"])
                {
                    case "AMD":
                        LoginStatus = 5;
                        break;
                    case "OP":
                        LoginStatus = 1;
                        break;
                    case "ME":
                        LoginStatus = 2;
                        break;
                    case "QE":
                        LoginStatus = 3;
                        break;
                    case "PE":
                        LoginStatus = 4;
                        break;
                    default:
                        LoginStatus = 1;
                        break;

                }
                string PLC_user = set5.Tables[0].Rows[0]["工号"].ToString();
                //更新数据库
                EHomePage.SQLiteHelpers.Update("CurreUser", PLC_user);

                if (ModbusClass.isConnected())
                {
                    ModbusClass.WriteString(2140, PLC_user);//当前人工号
                    ModbusClass.WriteSingleRegister(2704, (short)LoginStatus);
                }

            }
            else
            {
                txtPassword.Text = "";
                MessageBox.Show("用户名或密码错误", "提示");
            }

        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            LoginDialog.LoginStatus = 1;
            this.DialogResult = DialogResult.Cancel;
        }

    }
}

//using System;
//using System.Security.Cryptography;
//using System.Text;
 
//public class MD5Example
//{
//    public static string GetMD5Hash(string input)
//    {
//        // 创建一个MD5对象
//        using (MD5 md5 = MD5.Create())
//        {
//            // 将输入字符串转换为字节数组
//            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
//            // 计算散列
//            byte[] hashBytes = md5.ComputeHash(inputBytes);
//            // 将字节转换为十六进制字符串
//            StringBuilder hashString = new StringBuilder();
//            for (int i = 0; i < hashBytes.Length; i++)
//            {
//                hashString.Append(hashBytes[i].ToString("X2"));
//            }
//            return hashString.ToString();
//        }
//    }

//    public static void Main()
//    {
//        string originalString = "Hello, World!";
//        string md5Hash = GetMD5Hash(originalString);
//        Console.WriteLine($"The MD5 hash of '{originalString}' is: {md5Hash}");
//    }
//}
