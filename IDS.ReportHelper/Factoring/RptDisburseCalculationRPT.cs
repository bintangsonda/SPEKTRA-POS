using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.Factoring
{
    public class RptDisburseCalculationRPT
    {
        public static DataTable GetDataAll(string AgreeNo, string BranchCode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FTRepFactFacility";
                db.AddParameter("@AgreeNo", System.Data.SqlDbType.VarChar, AgreeNo);
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static DataTable GetDataOne(string AgreeNo, string InvNo, string BranchCode)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FTRepFFacilityDetail";
                db.AddParameter("@AgreeNo", System.Data.SqlDbType.VarChar, AgreeNo);
                db.AddParameter("@InvNo", System.Data.SqlDbType.VarChar, InvNo);
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetAgrNoForDisbCalc(string branchcode)
        {
            List<SelectListItem> agr = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select distinct FTTransh.AgreementNo, FTTransh.AgreementNo + ' - ' + LesseeFullName as Name from FTTransH inner join FTTransD on FTTransH.AgreementNo=FTTransD.AgreementNo inner join Lessee on Lessee.LesseeNo = FTTransH.ClientID  where FTTransD.FactStatus in(1) AND FTTransH.BranchCode=@branchcode order by FTTransH.agreementNo";
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new SelectListItem();
                        while (dr.Read())
                        {
                            SelectListItem facility = new SelectListItem();
                            facility.Text = dr["Name"].ToString();
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

        public static List<SelectListItem> GetInvNo(string agreementNo, string branchcode)
        {
            List<SelectListItem> agr = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select distinct FTTransD.InvNo from FTTransh inner join FTTRansD on FTTransH.AgreementNo=FTTransD.AgreementNo where FTTransD.FactStatus in(1) and FTtransh.AgreementNo=@agreementNo AND FTTransH.BranchCode=@branchcode order by FTTransD.InvNo";
                db.AddParameter("@agreementNo", System.Data.SqlDbType.VarChar, agreementNo ?? "");
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchcode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new SelectListItem();
                        while (dr.Read())
                        {
                            SelectListItem facility = new SelectListItem(); //<string, string>(dr["BranchCode"].ToString(), dr["BranchName"].ToString());
                            facility.Text = dr["InvNo"].ToString();
                            facility.Value = dr["InvNo"].ToString();
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
