using wms.Common;

namespace wms.Model
{
    public class CustomerModel : NotifyBase
    {
        private string _customerId;

        public string CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; this.DoNotify(); }
        }
        private string _customerInfoId;

        public string CustomerInfoId
        {
            get { return _customerInfoId; }
            set { _customerInfoId = value; this.DoNotify(); }
        }
        private string _customerName;

        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; this.DoNotify(); }
        }
    }
}
