using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptBalanceSheetRPT
    {
        public static DataTable GetData(int type, string pPeriod, string pCode, string pBranch)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepBalS";
                db.AddParameter("@type", System.Data.SqlDbType.Int, type);
                db.AddParameter("@pPeriod", System.Data.SqlDbType.VarChar, pPeriod);
                db.AddParameter("@pCode", System.Data.SqlDbType.VarChar, pCode);
                db.AddParameter("@pBranch", System.Data.SqlDbType.VarChar, pBranch);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
