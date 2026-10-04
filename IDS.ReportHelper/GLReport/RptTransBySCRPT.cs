using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptTransBySCRPT
    {
        public static DataTable GetData(DateTime pFromDate, DateTime pToDate, string branchcode, string pSCode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepTransBySC";
                db.AddParameter("@pFromDate", System.Data.SqlDbType.DateTime, pFromDate);
                db.AddParameter("@pToDate", System.Data.SqlDbType.DateTime, pToDate);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.AddParameter("@pSCode", System.Data.SqlDbType.VarChar, pSCode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
