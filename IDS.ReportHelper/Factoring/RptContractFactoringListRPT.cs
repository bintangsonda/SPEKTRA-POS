using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptContractFactoringListRPT
    {
        public static DataTable GetData(string Period, string Status)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GETConFactAPPList";
                db.AddParameter("@Period", System.Data.SqlDbType.VarChar, Period);
                db.AddParameter("@Status", System.Data.SqlDbType.VarChar, Status);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
