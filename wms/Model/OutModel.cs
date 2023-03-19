using System;
using wms.Common;

namespace wms.Model
{
    public class OutModel : NotifyBase
    {
        private int _outStatus;

        public int OutStatus
        {
            get { return _outStatus; }
            set { _outStatus = value; this.DoNotify(); }
        }
    }
}
