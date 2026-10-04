using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptIncomeStatementRPT
    {
        public static DataTable GetData(string pCode, string Period, string pBranch)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepIncomeStat";
                db.AddParameter("@pCode", System.Data.SqlDbType.VarChar, pCode);
                db.AddParameter("@Period", System.Data.SqlDbType.VarChar, Period);
                db.AddParameter("@pBranch", System.Data.SqlDbType.VarChar, pBranch);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
