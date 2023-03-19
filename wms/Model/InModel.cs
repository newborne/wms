using System;
using wms.Common;

namespace wms.Model
{
    public class InModel : NotifyBase
    {
        private int _inStatus;

        public int InStatus
        {
            get { return _inStatus; }
            set { _inStatus = value; this.DoNotify(); }
        }
    }
}
