using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public interface ILastLog
    {
        string EntryUser { get; set; }
        DateTime EntryDate { get; set; }
        string OperatorID { get; set; }
        DateTime LastUpdate { get; set; }
    }
}
