using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptSOMScheduleRPT
    {
        public static DataTable GetData(string pPeriod, string pCurr, string branchcode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepSOMaturitySchedule";
                db.AddParameter("@pPeriod", System.Data.SqlDbType.VarChar, pPeriod);
                db.AddParameter("@pCurr", System.Data.SqlDbType.VarChar, pCurr);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
