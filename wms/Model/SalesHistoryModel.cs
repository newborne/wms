using System;
using wms.Common;

namespace wms.Model
{
    public class SalesHistoryModel 
    {
        public int Hyear { get; set; }
        public int Hmon { get; set; }
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Monqty { get; set; }
        public decimal Monsalesroom { get; set; }
        public int Yearqty { get; set; }
        public decimal Yearsalesroom { get; set; }
    }
}
