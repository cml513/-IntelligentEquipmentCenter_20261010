using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentEquipmentCenter
{
    /// <summary>
    /// 封装单例模式类，调用该类可使当前类只产生一个实例（此处为UserControl，可以设置成继承Form，此时多次show的时候都是这一个实例）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CGenericSingletonU<T> where T : UserControl, new()
    {
        private static T t = null;
        public static T CreateInstrance()
        {
            
            //判断是否已创建或是被回收释放
            if (null == t || t.IsDisposed)
            {             
                t = new T();
            }
            return t;
        }
    } 
    public class CGenericSingletonF<T> where T : Form, new()
    {
        private static T t = null;
        public static T CreateInstrance()
        {
            
            //判断是否已创建或是被回收释放
            if (null == t || t.IsDisposed)
            {             
                t = new T();
            }
            return t;
        }
    }
}
