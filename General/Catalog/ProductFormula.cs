using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Catalog
{
    public class ProductFormula
    {
        public string ProductHeaderCode { get; set; }
        public string ProductHeaderBranch { get; set; }
        public int SeqNo { get; set; }
        public string ProdCode { get; set; }
        public decimal Qty { get; set; }
        public string On_Off { get; set; }
    }
}
