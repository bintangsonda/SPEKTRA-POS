using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptUnpostGLTransRPT
    {
        public static DataTable GetData(DateTime pFromDate, DateTime pToDate, string pCurr, string branchcode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLUnPostGL";
                db.AddParameter("@pFromDate", System.Data.SqlDbType.DateTime, pFromDate);
                db.AddParameter("@pToDate", System.Data.SqlDbType.DateTime, pToDate);
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
