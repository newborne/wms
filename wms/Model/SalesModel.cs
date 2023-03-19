using System;
using wms.Common;

namespace wms.Model
{
    public class SalesModel
    {
        public string DocumentId { get; set; }
        public DateTime DocumentDate { get; set; }
        public string CustomerId { get; set; }
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

    }
}
