using MySql.Data.MySqlClient;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter
{
    internal class MysqlClass
    {
        //string mysqlcon = "server=localhost;database=mysql;user=root;password=mysql123..";
        string mysqlcon = "server=localhost;database=mysql;user=root;password=123456";
        private MySqlConnection conn = null;
        private MySqlCommand cmd = null;

        public MysqlClass()
        {

        }

        public bool connectToSql()
        {
            conn = new MySqlConnection(mysqlcon);
            cmd = conn.CreateCommand();
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //获取命令
        public MySqlCommand getSqlCommand(string command)
        {
            //MySqlCommand cmd2 = new MySqlCommand(strComm, conn);
            cmd.CommandText = command;
            return cmd;
        }

        //判断表是否存在
        public bool isExists(string tableName)
        {
            try
            {
                string command = "select * from " + tableName + ";";
                MySqlCommand sqlCommand = getSqlCommand(command);
                MySqlDataReader dataReader =  sqlCommand.ExecuteReader();
                dataReader.Close();
                //sqlCommand.EndExecuteReader();
                

                return true;
            }
            catch(Exception ex)
            {            
                return false;
            }
            
        }

        //创建报警表格

        //      CREATE TABLE `new_schema`.`new_table` (
        //`idnew_table` INT NOT NULL,
        //`new_tablecol` DATE NULL,
        //`new_tablecol1` DATETIME NULL,
        //`new_tablecol2` DATETIME NULL,
        //PRIMARY KEY(`idnew_table`));

        //INSERT INTO `new_schema`.`new_table` (`idnew_table`, `new_tablecol`, `new_tablecol1`, `new_tablecol2`) VALUES ('1', '2024-05-10', '2024-05-10 14:09', '2024-05-10 15:09');
        public bool createWarmTables(string tableName)
        {
            try
            {
                if(isExists(tableName))
                {
                    return true;
                }
                else
                {
                    string command = "create table new_schema." + tableName + "(time DateTime,warming text);";

                    MySqlCommand sqlCommand = getSqlCommand(command);
                    int ret = sqlCommand.ExecuteNonQuery();

                    //sqlCommand.EndExecuteNonQuery();

                    Console.WriteLine(ret.ToString());
                    return ret > 0;
                }
                
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public bool InsertWarmTables(string tableName,string msg)
        {
            try
            {             
                string command = "insert into new_schema." + tableName +" "+msg;
                MySqlCommand sqlCommand = getSqlCommand(command);
                sqlCommand.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {              
                return false; 
            }
    
            
        }






    }
}
