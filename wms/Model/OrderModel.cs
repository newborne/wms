using System;
using wms.Common;

namespace wms.Model
{
    public class OrderModel : NotifyBase
    {
        private string _customerId;
        public string CustomerId
        {
            get { return _customerId; }
            set
            {
                _customerId = value;
                this.DoNotify();
            }
        }
        private string _Id;
        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
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
        private string _price;
        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                this.DoNotify();
            }
        }
        private string _quantity;
        public string Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                this.DoNotify();
            }
        }
        private string _total;
        public string Total
        {
            get { return _total; }
            set
            {
                _total = value;
                this.DoNotify();
            }
        }
        private int _orderStatus;

        public int OrderStatus
        {
            get { return _orderStatus; }
            set { _orderStatus = value; this.DoNotify(); }
        }

    }
}
