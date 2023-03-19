using System;
using System.Reflection;
using System.Windows;
using wms.Common;
using wms.DataAccess.DataEntity;
using wms.Model;

namespace wms.ViewModel
{
    public class MainViewModel : NotifyBase
    {
        public EmployeeModel EmployeeModel { get; set; } = new EmployeeModel();
        public LoginModel LoginModel { get; set; } = new LoginModel();
        public UserEntity UserEntity { get; set; } = new UserEntity();
        public CommandBase MinWindowCommand { get; set; }
        public CommandBase ZoomWindowCommand { get; set; }
        public CommandBase CloseWindowCommand { get; set; }
        public CommandBase NavChangedCommand { get; set; }
        public CommandBase MoveWindowCommand { get; set; }
        public CommandBase LogoutCommand { get; set; }

        private FrameworkElement _mainContent;

        public FrameworkElement MainContent
        {
            get { return _mainContent; }
            set { _mainContent = value; this.DoNotify(); }
        }

        private int _isNormal;

        public int IsNormal
        {
            get { return _isNormal; }
            set { _isNormal = value; this.DoNotify(); }
        }

        private int _navNumber;

        public int NavNumber
        {
            get { return _navNumber; }
            set { _navNumber = value; this.DoNotify(); }
        }
        
        public MainViewModel()
        {
            //通过全局变量给UserInfo赋值
            EmployeeModel.EmployeeSex = GlobalValues.UserInfo.EmployeeSex;
            EmployeeModel.EmployeeName = GlobalValues.UserInfo.EmployeeName;
            LoginModel.Logo = GlobalValues.UserInfo.Logo;
            //通过全局变量给UserEntity赋值
            this.UserEntity = GlobalValues.UserInfo;
            //默认窗口状态
            IsNormal = 1;
            //默认子用户控件
            DoNavChanged("SalesView");
            //默认导航条目数
            NavNumber = 1;
            //最小化窗口命令
            this.MinWindowCommand = new CommandBase();
            this.MinWindowCommand.DoExecute = new Action<object>((o) =>
            {
                (o as Window).WindowState = WindowState.Minimized;
            });
            this.MinWindowCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //放大缩小窗口命令
            this.ZoomWindowCommand = new CommandBase();
            this.ZoomWindowCommand.DoExecute = new Action<object>((o) =>
            {
                (o as Window).WindowState = (o as Window).WindowState == WindowState.Maximized ?
                WindowState.Normal : WindowState.Maximized;
                IsNormal = (o as Window).WindowState == WindowState.Maximized ? 0 : 1;
            });
            this.ZoomWindowCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //关闭窗口命令
            this.CloseWindowCommand = new CommandBase();
            this.CloseWindowCommand.DoExecute = new Action<object>((o) =>
            {
                (o as Window).Close();
            });
            this.CloseWindowCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
            //移动窗口命令
            this.MoveWindowCommand = new CommandBase();
            this.MoveWindowCommand.DoExecute = new Action<object>((o) =>
            {
                (o as Window).DragMove();
            });
            this.MoveWindowCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });

            //展示子用户控件
            this.NavChangedCommand = new CommandBase();
            this.NavChangedCommand.DoExecute = new Action<object>(DoNavChanged);
            this.NavChangedCommand.DoCanExecute = new Func<object, bool>((o) => true);
            //注销命令
            this.LogoutCommand = new CommandBase();
            this.LogoutCommand.DoExecute = new Action<object>((o) =>
            {
                (o as Window).DialogResult = true;
            });
            this.LogoutCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }


        /// <summary>
        /// 导航栏改变
        /// </summary>
        /// <param name="obj"></param>
        private void DoNavChanged(object obj)
        {
            if (obj.ToString().Equals("SalesView"))
            {
                NavNumber = 1;
            }
            if (obj.ToString().Equals("SalesHistoryView"))
            {
                NavNumber = 2;
            }
            if (obj.ToString().Equals("ProductView"))
            {
                NavNumber = 3;
            }
            if (obj.ToString().Equals("InView"))
            {
                NavNumber = 4;
            }
            if (obj.ToString().Equals("OrderView"))
            {
                NavNumber = 5;
            }
            if (obj.ToString().Equals("OutView"))
            {
                NavNumber = 6;
            }
            if (obj.ToString().Equals("SupplierView"))
            {
                NavNumber = 7;
            }
            if (obj.ToString().Equals("CustomerView"))
            {
                NavNumber = 8;
            }
            if (obj.ToString().Equals("EmployeeView"))
            {
                NavNumber = 9;
            }
            if (obj.ToString().Equals("DepartView"))
            {
                NavNumber = 10;
            }
            Type type = Type.GetType("wms.View." + obj.ToString());
            ConstructorInfo cti = type.GetConstructor(System.Type.EmptyTypes);
            this.MainContent = (FrameworkElement)cti.Invoke(null);
        }

    }
}
