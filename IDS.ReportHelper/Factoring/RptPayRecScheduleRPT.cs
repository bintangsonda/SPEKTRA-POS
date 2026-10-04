using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptPayRecScheduleRPT
    {
        public static DataTable GetData(DateTime asDate, string CustNo)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FTRepPayRecSchedule";
                db.AddParameter("@Today", System.Data.SqlDbType.DateTime, asDate);
                db.AddParameter("@AgreementNo", System.Data.SqlDbType.VarChar, CustNo);

                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetCustNo()
        {
            List<SelectListItem> agr = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select distinct ClientID, ClientID + ' - ' + Lessee.LesseeFullName as LesseeFullName from FTTransH inner join Lessee on Lessee.LesseeNo=FTTransH.ClientID";
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
                            facility.Text = dr["LesseeFullName"].ToString();
                            facility.Value = dr["ClientID"].ToString();
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
