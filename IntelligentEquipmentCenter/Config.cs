
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    public class Config
    {
        private Config()
        { 
        }
        public static Config Instance { get; set; }=new Config();
       



        /// <summary>
        /// 通过modbus寄存器获取条码,先获取托盘码，然后通过设备托盘绑定信息上传接口获取SN
        /// </summary>
        public bool GetSNByModbus=false;




    }
}
