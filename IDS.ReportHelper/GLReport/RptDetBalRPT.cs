using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptDetBalRPT
    {
        public static DataTable GetData(string pFPeriod, string pTPeriod, string pFAcc, string pTAcc, string pFCcy, string pTCcy, string branchcode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepDTB";
                db.AddParameter("@pFPeriod", System.Data.SqlDbType.VarChar, pFPeriod);
                db.AddParameter("@pTPeriod", System.Data.SqlDbType.VarChar, pTPeriod);
                db.AddParameter("@pFAcc", System.Data.SqlDbType.VarChar, pFAcc);
                db.AddParameter("@pTAcc", System.Data.SqlDbType.VarChar, pTAcc);
                db.AddParameter("@pFCcy", System.Data.SqlDbType.VarChar, pFCcy);
                db.AddParameter("@pTCcy", System.Data.SqlDbType.VarChar, pTCcy);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
