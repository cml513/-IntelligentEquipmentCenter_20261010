using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    internal static class ImportData
    {

        /// <summary>
        /// 导入表格数据到DataTable
        /// </summary>
        /// <param name="fileName">导入的文件名</param>
        /// <param name="sheetNum">导入的第几个sheet</param>
        /// <param name="maxcolumns">导入列数</param>
        /// <param name="exportColumnsName">是否导入列名</param>
        /// <returns>返回一个DataTable</returns>
        public static DataTable ImportToDG(string fileName,int sheetNum,int maxcolumns,bool exportColumnsName)
        {
            DataTable result = new DataTable();
            Workbook wb = new Workbook();
            wb.LoadFromFile(@fileName);
            Worksheet sheet = wb.Worksheets[sheetNum];
            //result = sheet.ExportDataTable(sheet.AllocatedRange, true, false);           
            result = sheet.ExportDataTable(1,1, sheet.LastDataRow, maxcolumns,exportColumnsName);
            return result;
        }

        /// <summary>
        /// 打开打开excel\txt\dll等文件
        /// </summary>
        /// <param name="path">文件路径+文件名（包括扩展名）</param>
        public static void OpenFile(string path)
        {
            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(path);
            process.StartInfo = psi;

            try
            {
                process.Start();
                try
                {
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                    process = null;

                }

            }
        }
    }
}
