using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptDailyTransRPT
    {
        public static DataTable GetData(DateTime AtDate, string Code, int Type)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SP_GETDAilyTransRPT";
                db.AddParameter("@AtDate", System.Data.SqlDbType.DateTime, AtDate);
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, Code);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, Type);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
