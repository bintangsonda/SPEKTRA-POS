using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOS.Inventory.Stock
{
    public class StockTransactionD
    {
        public string TransNo { get; set; }
        public int SeqNo { get; set; }
        public string ProdCode { get; set; }
        public decimal Qty { get; set; }
        public string Remark { get; set; }


    }
}
