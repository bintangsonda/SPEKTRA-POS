using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IDS.ReportHelper.Factoring
{
    public class RptSaldoPembiayaanRPT
    {
        public static DataTable GetDataDPD(DateTime period)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SP_GetSaldoPembiayaan";
                db.AddParameter("@Period", System.Data.SqlDbType.DateTime, period);
                db.AddParameter("@ExDate", System.Data.SqlDbType.DateTime, period);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static DataTable GetDataSaldo(DateTime period)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FT_RptSaldoPembiayaan";
                db.AddParameter("@Period", System.Data.SqlDbType.DateTime, period);
                db.AddParameter("@ExDate", System.Data.SqlDbType.DateTime, period);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

    }
}
