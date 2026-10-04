using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Catalog
{
    public class VariantDetail
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string VariantCode { get; set; }
        public int SeqNo { get; set; }
        public decimal Harga { get; set; }
        public VariantDetail()
        {
        }   
    }
}
