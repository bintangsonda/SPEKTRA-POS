using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptCollateralRPT
    {
        public static DataTable GetData(string AgreeNo)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FT_prrCollateral";
                db.AddParameter("@AgreeNo", System.Data.SqlDbType.VarChar, AgreeNo);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetAgreementNoList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select distinct AgreementNo, AgreementNo + ' - ' + OwnerName as name from FTCollateral";
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
                            coa.Value = IDS.Tool.GeneralHelper.NullToString(dr["AgreementNo"]);
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["name"]);

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
