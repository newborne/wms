using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wms.Common;

namespace wms.Model
{
    public class DepartModel : NotifyBase
    {
        private string _departName;

        public string DepartName
        {
            get { return _departName; }
            set { _departName = value; this.DoNotify(); }
        }


        private string _departId;

        public string DepartId
        {
            get { return _departId; }
            set { _departId = value; this.DoNotify(); }
        }
    }
}
