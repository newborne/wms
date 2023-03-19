using wms.Common;

namespace wms.Model
{
    public class LoginModel : NotifyBase
    {
        private string _loginName;
        public string LoginName
        {
            get { return _loginName; }
            set
            {
                _loginName = value;
                this.DoNotify();
            }
        }

        private string _loginPassword;

        public string LoginPassword
        {
            get { return _loginPassword; }
            set
            {
                _loginPassword = value;
                this.DoNotify();
            }
        }
        private string _logo;

        public string Logo
        {
            get { return _logo; }
            set { _logo = value; this.DoNotify(); }
        }
    }
}
