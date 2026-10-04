using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptDailyCashRegisterRPT
    {
        public static DataTable GetData(DateTime pEntDate, string pCurr, string branchcode)
        {
            pEntDate = new DateTime(pEntDate.Year, pEntDate.Month, DateTime.DaysInMonth(pEntDate.Year, pEntDate.Month));

            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepDailyCash";
                db.AddParameter("@pEntDate", System.Data.SqlDbType.DateTime, pEntDate);
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
