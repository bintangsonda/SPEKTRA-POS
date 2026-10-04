using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptForexRevRPT
    {
        public static DataTable GetData(string pTPeriod, string branchcode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepFXReval";
                db.AddParameter("@pTPeriod", System.Data.SqlDbType.VarChar, pTPeriod);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
