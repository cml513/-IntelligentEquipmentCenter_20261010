using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentEquipmentCenter
{
    public class R_TRIG
    {

        ///
        /// 这个属性存储上一次的bool状态,get;private set;这个写法相当于PLC的 Output接口
        ///

        public bool Last { get; private set; }

        ///
        /// 这个属性填被检测的bool量,set;相当于PLC的Input接口
        ///

        public bool CLK

        {
            set

            {
                Q = value && !Last;//我们知道上升沿是从0变1的一瞬间，所以本次扫描为真上次为假时就产生了上升沿
                Last = value;//每个扫描周期刷新参考位
            }
        }

        ///
        /// 这个就是检测的状态，外部获取这个变量就知道上升沿有没有产生
        /// 相当于PLC的 Output接口
        public bool Q { get; private set; }

    } 

}


