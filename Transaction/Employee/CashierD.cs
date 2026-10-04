using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transaction.Employee
{
    public class CashierD
    {
        public string TransCode { get; set; }
        public string BranchCode { get; set; }
        public int SeqNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public string OrderType { get; set; }
        public string VariantCode { get; set; }
        public string VariantName { get; set; }
        public string VariantSeqNo { get; set; }
        public string VariantHarga { get; set; }
        public string VariantValue { get; set; }
        public string Remark { get; set; }
    }
}
