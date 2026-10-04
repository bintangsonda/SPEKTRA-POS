using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptDailyVoucherRPT
    {
        
        public static DataTable GetData(int Opt, DateTime FromDate, DateTime ToDate, string Voucher)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FTRepDailyVoucherPmt";
                db.AddParameter("@Opt", System.Data.SqlDbType.TinyInt, Opt);
                db.AddParameter("@FromDate", System.Data.SqlDbType.DateTime, FromDate);
                db.AddParameter("@ToDate", System.Data.SqlDbType.DateTime, ToDate);
                db.AddParameter("@txtvoucher", System.Data.SqlDbType.VarChar, Voucher);

                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetVoucherNo(int Opt, DateTime FromDate, DateTime ToDate)
        {
            List<SelectListItem> agr = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                if (Opt == 2)
                {
                    db.CommandText = "select distinct voucher from FTGLTrans where left(scode,2)='FP' and TransDate >= @FromDate  and TransDate <= @ToDate";
                }
                else if (Opt == 3)
                {
                    db.CommandText = "select distinct voucher from FTGLTrans where left(scode,2)='FT09' and TransDate >= @FromDate  and TransDate <= @ToDate";
                }
                else
                {
                    db.CommandText = "select distinct voucher from FTGLTrans where left(scode,2)='FT' and TransDate>=@FromDate and TransDate<=@ToDate";
                }

                db.AddParameter("@FromDate", System.Data.SqlDbType.DateTime, FromDate);
                db.AddParameter("@ToDate", System.Data.SqlDbType.DateTime, ToDate);

                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem item;

                        item = new SelectListItem();
                        item.Value = "%"; ;
                        item.Text = "ALL";
                        agr.Add(item);

                        SelectListItem a = new SelectListItem();
                        while (dr.Read())
                        {
                            SelectListItem facility = new SelectListItem();
                            facility.Text = dr["voucher"].ToString();
                            facility.Value = dr["voucher"].ToString();
                            agr.Add(facility);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return agr;
        }

    }
}
