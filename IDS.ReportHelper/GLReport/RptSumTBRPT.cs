using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptSumTBRPT
    {
        public static DataTable GetData(string pMonth, string pMonthTo, int type, string branchcode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepSummTrialBal";
                db.AddParameter("@pMonth", System.Data.SqlDbType.VarChar, pMonth);
                db.AddParameter("@pMonthTo", System.Data.SqlDbType.VarChar, pMonthTo);
                db.AddParameter("@type", System.Data.SqlDbType.Int, type);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
