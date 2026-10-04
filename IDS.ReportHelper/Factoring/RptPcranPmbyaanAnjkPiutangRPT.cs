using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDS.ReportHelper.Factoring
{
    public class RptPcranPmbyaanAnjkPiutangRPT
    {
        public static DataTable GetData(DateTime From, DateTime To, int Type)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "Sp_GetRptPencairanAnjakPiutang";
                db.AddParameter("@From", System.Data.SqlDbType.DateTime, From);
                db.AddParameter("@To", System.Data.SqlDbType.DateTime, To);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, Type);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
