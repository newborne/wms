using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace wms.Common
{
    /*把Model需要INotifyPropertyChanged接口和PropertyChanged属性写成一个Base类*/
    public class NotifyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性改变时，调用该方法发出通知
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void DoNotify([CallerMemberName] string propName = "")
        {
            //NULL检查运算符（?.） 判断一下这个委托是不是为null；如果是则不执行委托，如果不是则执行该委托；
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
