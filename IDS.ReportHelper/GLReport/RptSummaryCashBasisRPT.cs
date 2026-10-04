using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptSummaryCashBasisRPT
    {
        public static DataTable GetData(string Period, string PeriodTo, int tipe, string BranchCode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepCashBases";
                db.AddParameter("@Period", System.Data.SqlDbType.VarChar, Period);
                db.AddParameter("@PeriodTo", System.Data.SqlDbType.VarChar, PeriodTo);
                db.AddParameter("@tipe", System.Data.SqlDbType.Int, tipe);
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
