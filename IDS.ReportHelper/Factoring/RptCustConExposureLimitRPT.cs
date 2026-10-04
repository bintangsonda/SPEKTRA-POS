using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptCustConExposureLimitRPT
    {
        public static DataTable GetData(string Period, string Branch)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FTRepCustControlExLimit";
                db.AddParameter("@Today", System.Data.SqlDbType.VarChar, Period);
                db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, Branch);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
