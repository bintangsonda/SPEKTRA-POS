using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptExchangeRateRPT
    {
        public static DataTable GetData(DateTime FromDate, DateTime ToDate, string Ccy1, bool IsLastDayOfMonth)
        {
            System.Data.DataTable dt = null;
           
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "Rpt_ExchangeRate";
                db.CommandType = System.Data.CommandType.StoredProcedure;

                db.AddParameter("@FromDate", System.Data.SqlDbType.DateTime, FromDate);
                db.AddParameter("@ToDate", System.Data.SqlDbType.DateTime, ToDate);
                db.AddParameter("@Ccy1", System.Data.SqlDbType.VarChar, Ccy1);
                db.AddParameter("@IsLastDayOfMonth", System.Data.SqlDbType.Bit, IsLastDayOfMonth);
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
