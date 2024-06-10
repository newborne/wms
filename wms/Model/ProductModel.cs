using System;
using wms.Common;

namespace wms.Model
{
    public class ProductModel : NotifyBase
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                this.DoNotify();
            }
        }
        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                this.DoNotify();
            }
        }
        private int _pid;
        public int Pid
        {
            get { return _pid; }
            set
            {
                _pid = value;
                this.DoNotify();
            }
        }
        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                this.DoNotify();
            }
        }
        public int Amt { get; set; }
    }
}
