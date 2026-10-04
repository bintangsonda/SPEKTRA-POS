using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public static class DataTableExtension
    {
        public static void SortDataTable(this System.Data.DataTable dt, string columnName, string sortDirection)
        {
            if (dt == null || string.IsNullOrEmpty(columnName)) return;

            if (dt.Columns.Contains(columnName))
            {
                System.Data.DataView dv = new System.Data.DataView();
                dv = dt.DefaultView;
                dv.Sort = string.Format("{0} {1}", columnName, sortDirection);
                dt = dv.ToTable();
            }
        }
    }
}