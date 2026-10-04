using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptOutstandingRPT
    {
        public static DataTable GetData(string Period, string AgreementNo)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FT_PrrOutsFact";
                db.AddParameter("@Period", System.Data.SqlDbType.VarChar, Period);
                db.AddParameter("@AgreementNo", System.Data.SqlDbType.VarChar, AgreementNo);

                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetAgrNoForOutstandingContractFactoring(string tgl)
        {
            List<SelectListItem> agr = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select distinct FTTransH.AgreementNo, FTTransH.AgreementNo+' - '+Lessee.LesseeFullName as lesseefullname from FTTransH inner join FTTransD on FTTransH.AgreementNo=FTTransD.AgreementNo inner join Lessee on Lessee.LesseeNo=FTTransH.ClientID AND Lessee.BranchCode=FTTransH.BranchCode  where FTTransD.FactStatus in(1) and Convert(varchar(6),FTTransD.FromDue,112)=@Period AND FTTransD.invno not in (select InvNo from FTAllocation A inner join FTPaymentInst P on A.InstNo=P.InstNo where P.Status=2) order by FTTransH.agreementNo";
                db.AddParameter("@Period", System.Data.SqlDbType.VarChar, tgl);
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
                            facility.Text = dr["lesseefullname"].ToString();
                            facility.Value = dr["AgreementNo"].ToString();
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
