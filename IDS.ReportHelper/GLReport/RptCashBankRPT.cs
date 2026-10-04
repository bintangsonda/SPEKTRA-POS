using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptCashBankRPT
    {
        public static DataTable GetData(string SPType, DateTime Trans, DateTime Trans2, int Init, bool CHK, string ACC, string ACCTo, string branchcode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLRepKasBank";
                db.AddParameter("@SPType", System.Data.SqlDbType.VarChar, SPType);
                db.AddParameter("@Trans", System.Data.SqlDbType.DateTime, Trans);
                db.AddParameter("@Trans2", System.Data.SqlDbType.DateTime, Trans2);
                db.AddParameter("@Init", System.Data.SqlDbType.Int, Init);
                db.AddParameter("@CHK", System.Data.SqlDbType.Bit, CHK);
                db.AddParameter("@ACC", System.Data.SqlDbType.VarChar, ACC);
                db.AddParameter("@ACCTo", System.Data.SqlDbType.VarChar, ACCTo);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
