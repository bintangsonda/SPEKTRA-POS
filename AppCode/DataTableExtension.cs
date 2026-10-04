using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCode
{
    public static class DataTableExtension
    {
        public static void SortDataTable(this DataTable dt, string columnName, string sortDirection)
        {
            if (dt == null || string.IsNullOrEmpty(columnName)) return;

            if (dt.Columns.Contains(columnName))
            {
                DataView dv = new DataView();
                dv = dt.DefaultView;
                dv.Sort = string.Format("{0} {1}", columnName, sortDirection);
                dt = dv.ToTable();
            }
        }
    }
}
