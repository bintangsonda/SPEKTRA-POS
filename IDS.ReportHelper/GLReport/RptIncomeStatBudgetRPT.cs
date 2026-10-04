using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptIncomeStatBudgetRPT
    {
        public static DataTable GetData(string pCode, string PeriodFrom, string PeriodTo, string pBranch)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepIncomeStatActualVsBudget";
                db.AddParameter("@pCode", System.Data.SqlDbType.VarChar, pCode);
                db.AddParameter("@PeriodFrom", System.Data.SqlDbType.VarChar, PeriodFrom);
                db.AddParameter("@PeriodTo", System.Data.SqlDbType.VarChar, PeriodTo);
                db.AddParameter("@pBranch", System.Data.SqlDbType.VarChar, pBranch);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

    }
}
