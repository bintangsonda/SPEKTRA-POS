using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptAccrualPerClientRPT
    {
        public static DataTable GetData(string CustNo)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FT_prrAccrualPerFTClient";
                db.AddParameter("@ClientID", System.Data.SqlDbType.VarChar, CustNo);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetCustNoList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select distinct FTTransH.ClientID, FTTransH.ClientID + ' - ' + Lessee.LesseeFullName as Name from FTTransH inner join FTTransD on FTTransH.AgreementNo=FTTransD.AgreementNo inner join Lessee on Lessee.LesseeNo=FTTransH.ClientID where FTTransD.FactStatus in(1)";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    SelectListItem a = new SelectListItem();
                    a.Value = "%";
                    a.Text = "ALL";
                    list.Add(a);

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem coa = new SelectListItem();
                            coa.Value = IDS.Tool.GeneralHelper.NullToString(dr["ClientID"]);
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["Name"]);

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
