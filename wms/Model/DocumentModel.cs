using System;
using wms.Common;

namespace wms.Model
{
    public class DocumentModel : NotifyBase
    {
        private string _documentId;

        public string DocumentId
        {
            get { return _documentId; }
            set { _documentId = value; this.DoNotify(); }
        }

        private DateTime _documentDate;
        public DateTime DocumentDate
        {
            get { return _documentDate; }
            set { _documentDate = value; this.DoNotify(); }
        }
    }
}
