using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace   IntelligentEquipmentCenter
{

    //
    public class F_TRIG
    {

        public bool Last { get; private set; }

        public bool CLK

        {
            set
            {
                Q = !value && Last;//1变0 上次为真本次为假
                Last = value;
            }
        }

        public bool Q { get; private set; }

    }

}

