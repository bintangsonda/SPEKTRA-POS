using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptVoucherWithDetailRPT
    {
        public static DataTable GetData(string pScode, string pVoucher, string pBranchCode, DateTime pFromDate, DateTime pToDate)
        {
            if (string.IsNullOrEmpty(pScode))
            {
                pScode = "ALL";
            }

            if (string.IsNullOrEmpty(pVoucher))
            {
                pVoucher = "ALL";
            }

            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SelVoucherDetail";

                db.AddParameter("@pScode", System.Data.SqlDbType.VarChar, pScode);
                db.AddParameter("@pVoucher", System.Data.SqlDbType.VarChar, pVoucher);
                db.AddParameter("@pBranchCode", System.Data.SqlDbType.VarChar, pBranchCode);
                db.AddParameter("@pFromDate", System.Data.SqlDbType.DateTime, pFromDate);
                db.AddParameter("@pToDate", System.Data.SqlDbType.DateTime, pToDate);

                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

    }
}
