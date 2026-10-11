using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{

    public class InfoForm
    {
        
        public string FormCode { get; set; }
        public string AddressType { get; set; }
        public int Address { get; set; }
        public string FormContext { get; set; }
        public int LeftAddress { get; set; }
        public int RightAddress { get; set; }

        public string LeftText { get; set; }
        public string RightText { get; set; }
        public bool FormEnable { get; set; }
        public int ButtonNumber { get; set; }
    }
}
