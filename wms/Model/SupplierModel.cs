using wms.Common;

namespace wms.Model
{
    public class SupplierModel : NotifyBase
    {
        private string _supplierId;

        public string SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; this.DoNotify(); }
        }
        private string _supplierInfoId;

        public string SupplierInfoId
        {
            get { return _supplierInfoId; }
            set { _supplierInfoId = value; this.DoNotify(); }
        }
        private string _supplierName;

        public string SupplierName
        {
            get { return _supplierName; }
            set { _supplierName = value; this.DoNotify(); }
        }
    }
}
