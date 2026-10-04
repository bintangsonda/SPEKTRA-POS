using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptCashBankListRPT
    {
        public static DataTable GetData(string branchCode, DateTime from, DateTime to, int status, int inOut, string ccy, string acc, string cb)
        {
            System.Data.DataTable dt = null;
            to = to.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "ReportCBTranListHeader";
                db.CommandType = System.Data.CommandType.StoredProcedure;

                db.AddParameter("@branch", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@from", System.Data.SqlDbType.DateTime, from);
                db.AddParameter("@to", System.Data.SqlDbType.DateTime, to);
                db.AddParameter("@status", System.Data.SqlDbType.TinyInt, status);
                db.AddParameter("@inout", System.Data.SqlDbType.TinyInt, inOut);
                db.AddParameter("@ccy", System.Data.SqlDbType.VarChar, ccy);
                db.AddParameter("@acc", System.Data.SqlDbType.VarChar, acc);
                db.AddParameter("@CashBankType", System.Data.SqlDbType.TinyInt, Convert.ToInt32(cb));
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static DataTable GetDataDetail(string branchCode, DateTime from, DateTime to, int status, int inOut, string ccy, string acc, string cb)
        {
            System.Data.DataTable dt = null;
            to = to.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "ReportCBTranListDetail";
                db.CommandType = System.Data.CommandType.StoredProcedure;

                db.AddParameter("@branch", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@from", System.Data.SqlDbType.DateTime, from);
                db.AddParameter("@to", System.Data.SqlDbType.DateTime, to);
                db.AddParameter("@status", System.Data.SqlDbType.TinyInt, status);
                db.AddParameter("@inout", System.Data.SqlDbType.TinyInt, inOut);
                db.AddParameter("@ccy", System.Data.SqlDbType.VarChar, ccy);
                db.AddParameter("@acc", System.Data.SqlDbType.VarChar, acc);
                db.AddParameter("@CashBankType", System.Data.SqlDbType.TinyInt, Convert.ToInt32(cb));
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetAccList(string ccy, string cb)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ACC, NAME FROM ACFGLMH  LEFT JOIN ACFSPACC on ACFSPACC.FR_ACC=ACFGLMH.ACC and ACFSPACC.FR_CCY=ACFGLMH.CCY WHERE AT=0 and CCY=@Ccy and TYPE_ACC=@TypeAcc";
                db.AddParameter("@CCy", System.Data.SqlDbType.VarChar, ccy);
                if (cb == "1")
                {
                    db.AddParameter("@TypeAcc", System.Data.SqlDbType.VarChar, "BN");
                }
                else
                {
                    db.AddParameter("@TypeAcc", System.Data.SqlDbType.VarChar, "KS");
                }
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem coa = new SelectListItem();
                            coa.Value = IDS.Tool.GeneralHelper.NullToString(dr["ACC"]);
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["ACC"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["NAME"]);

                            list.Add(coa);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }
    }
}
