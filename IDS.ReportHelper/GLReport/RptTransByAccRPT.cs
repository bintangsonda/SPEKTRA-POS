using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptTransByAccRPT
    {
        public static DataTable GetData(string pFacc, string pTacc, DateTime pFromDate, DateTime pToDate, string branchcode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepTransByAcc";
                db.AddParameter("@pFacc", System.Data.SqlDbType.VarChar, pFacc);
                db.AddParameter("@pTacc", System.Data.SqlDbType.VarChar, pTacc);
                db.AddParameter("@pFromDate", System.Data.SqlDbType.DateTime, pFromDate);
                db.AddParameter("@pToDate", System.Data.SqlDbType.DateTime, pToDate);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
