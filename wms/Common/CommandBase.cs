using System;
using System.Windows.Input;

namespace wms.Common
{
    public class CommandBase : ICommand
    {
        public Action<object> DoExecute { get; set; }
        //参数为object无返回的委托，对应接口中的Excute
        public Func<object, bool> DoCanExecute { get; set; }
        //参数为object，返回bool型的委托，对应接口中CanExecute
        //上面两个委托在实例化对象时根据需要实例化（属性赋值）
        public event EventHandler CanExecuteChanged;//接口中定义的事件
        //下面用实例化时传来的上面两个委托实现接口中定义的方法
        //判断是否实例化后直接调用

        /// <summary>
        /// 命令是否可执行
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            //实例化了该委托（该属性赋值）
            //NULL检查运算符（?.） 判断一下这个委托是不是为null；如果是则不执行委托，如果不是则执行该委托；
            return DoCanExecute?.Invoke(parameter) == true;
        }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            DoExecute?.Invoke(parameter);
        }
        /// <summary>
        /// 引起允许执行变化事件
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            //NULL检查运算符（?.） 判断一下这个委托是不是为null；如果是则不执行委托，如果不是则执行该委托；
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
