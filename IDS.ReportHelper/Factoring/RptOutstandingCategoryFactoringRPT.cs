using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptOutstandingCategoryFactoringRPT
    {
        public static DataTable GetDataLeasing(string Period, string Type, string branchCode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "Le_RptOutstandingByCategory";
                db.AddParameter("@Period", System.Data.SqlDbType.VarChar, Period);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, Convert.ToInt32(Type));
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@financeMethod", System.Data.SqlDbType.SmallInt, 0);
                db.AddParameter("@leaseType", System.Data.SqlDbType.SmallInt, 0);
                db.AddParameter("@leaseMethod", System.Data.SqlDbType.SmallInt, 0);
                db.AddParameter("@goodsService", System.Data.SqlDbType.SmallInt, 0);
                db.AddParameter("@contractType", System.Data.SqlDbType.SmallInt, 0);
                db.AddParameter("@group", System.Data.SqlDbType.SmallInt, 0);
                db.AddParameter("@prodCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static DataTable GetDataFactoring(DateTime Period, string Type)
        {
            Period = new DateTime(Period.Year, Period.Month, DateTime.DaysInMonth(Period.Year, Period.Month));

            System.Data.DataTable dt = null;
            if (Type == "2" || Type == "3")
            {

                using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
                {
                    db.CommandText = "SP_GetSaldoPembiayaanReportOutstanding";
                    db.AddParameter("@Period", System.Data.SqlDbType.DateTime, Period);
                    db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, Convert.ToInt32(Type));
                    db.AddParameter("@ExDate", System.Data.SqlDbType.DateTime, Period);

                    db.CommandType = System.Data.CommandType.StoredProcedure;
                    db.Open();

                    dt = db.GetDataTable();
                }
            }
            else
            {
                dt = new System.Data.DataTable();
            }

            return dt;
        }

    }
}
