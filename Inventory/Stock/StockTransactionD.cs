using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOS.Inventory.Stock
{
    public class StockTransactionD
    {
        string TransNo { get; set; }    
        int SeqNo { get; set; }
        string ProdCode { get; set; }
        decimal Qty { get; set; }
        string Remark { get; set; }


    }
}
