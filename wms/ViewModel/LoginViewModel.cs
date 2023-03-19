using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wms.Common;
using wms.DataAccess;
using wms.Model;

namespace wms.ViewModel
{
    //用到了除了Model里的其他属性
    public class LoginViewModel : NotifyBase
    {
        public LoginModel LoginModel { get; set; } = new LoginModel();
        public CommandBase CloseWindowCommand { get; set; }
        public CommandBase MoveWindowCommand { get; set; }
        public CommandBase LoginCommand { get; set; }
        public CommandBase RefreshVerificationCodeCommand { get; set; }


        //定义model以外的属性
        private string validCodeStr;
        private string _verificationCode;

        public string VerificationCode
        {
            get { return _verificationCode; }
            set
            {
                _verificationCode = value;
                this.DoNotify();
            }
        }

        private ImageSource _source;
        public ImageSource Source 
        { 
            get { return _source; } 
            set { _source = value; this.DoNotify(); }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; this.DoNotify(); }
        }

        private Visibility _showProgress = Visibility.Collapsed;

        public Visibility ShowProgress
        {
            get { return _showProgress; }
            set
            {
                _showProgress = value; this.DoNotify();
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public LoginViewModel()
        {
            LoginModel.LoginName = "admin";
            LoginModel.LoginPassword = "123456";
            GetImages();
            VerificationCode = validCodeStr;
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
            //登录命令
            this.LoginCommand = new CommandBase();
            this.LoginCommand.DoExecute = new Action<object>(DoLogin);
            //登录按钮可用与显示进度条互斥
            this.LoginCommand.DoCanExecute = new Func<object, bool>((o) => { return ShowProgress == Visibility.Collapsed; });
            //刷新验证码命令
            this.RefreshVerificationCodeCommand = new CommandBase();
            this.RefreshVerificationCodeCommand.DoExecute = new Action<object>((o) =>
            {
                GetImages();
            });
            this.RefreshVerificationCodeCommand.DoCanExecute = new Func<object, bool>((o) => { return true; });
        }

        /// <summary>
        /// 执行登录
        /// </summary>
        /// <param name="o"></param>
        private void DoLogin(object o)
        {
            this.ShowProgress = Visibility.Visible;
            this.ErrorMessage = "";

            if (string.IsNullOrEmpty(LoginModel.LoginName))
            {
                this.ErrorMessage = "请输入用户名";
                this.ShowProgress = Visibility.Collapsed;
                return;
            }
            if (string.IsNullOrEmpty(LoginModel.LoginPassword))
            {
                this.ErrorMessage = "请输入密码";
                this.ShowProgress = Visibility.Collapsed;
                return;
            }
            if (string.IsNullOrEmpty(VerificationCode))
            {
                this.ErrorMessage = "请输入验证码";
                this.ShowProgress = Visibility.Collapsed;
                return;
            }
            if (VerificationCode.ToLower() != validCodeStr)
            {
                this.ErrorMessage = "验证码输入不正确";
                this.ShowProgress = Visibility.Collapsed;
                return;
            }
            //异步多线程控制
            Task.Run(new Action(() =>
            {
                try
                {
                    var userInfo = ServerDataAccess.GetInstance().CheckUserInfo(LoginModel.LoginName, LoginModel.LoginPassword);
                    if (userInfo == null)
                    {
                        throw new Exception("登录失败!用户名或密码错误");
                    }

                    GlobalValues.UserInfo = userInfo;

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        (o as Window).DialogResult = true;
                    }));
                }
                catch (Exception ex)
                {
                    this.ErrorMessage = ex.Message;
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.ShowProgress = Visibility.Collapsed;
                    }));
                }
            }));
        }

        private void GetImages()
        {

            ValidCode validCode = new ValidCode(4, ValidCode.CodeType.Alphas);
            Source = BitmapFrame.Create(validCode.CreateCheckCodeImage());
            validCodeStr = validCode.CheckCode.ToLower();
        }
    }
}
