using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    public class Project
    {
        private Project()
        {
        }
        public static Project Instance { get; set; } = new Project();

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MaterialCode = "001-00";
    }
}
