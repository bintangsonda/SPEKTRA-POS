using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.Employee
{
    public class PurchaseOrderD
    {
        public string POHNo { get; set; }
        public int SeqNo { get; set; }
        public string ProdCode { get; set; }
        public string ProdName { get; set; }
        public string UOM { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
    }
}
