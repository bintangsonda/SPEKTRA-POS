using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDS.Maintenance
{
    public class CustomViewModel
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public DateTime DateUpdated { get; set; }
        public string UserName { get; set; }
        public string[] oldValue { get; set; }
        public string[] newValue { get; set; }
        public string Status { get; set; }
    }
}