using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptIncDisbRPT
    {
        public static DataTable GetData(DateTime from, DateTime to)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "rptIncomeDisb";
                db.AddParameter("@FROM", System.Data.SqlDbType.DateTime, from);
                db.AddParameter("@TO", System.Data.SqlDbType.DateTime, to);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
