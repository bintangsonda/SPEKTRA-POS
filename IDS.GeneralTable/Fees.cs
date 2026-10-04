using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class Fees
    {
        public int FeeType { get; set; }
        public string FeeName { get; set; }
        public string AccPayable { get; set; }
        public string AccMKFactoring { get; set; }
        public string AccIVFactoring { get; set; }
        public string AccMKModalUsaha { get; set; }
        public string AccIVSaleLeaseback { get; set; }
        public string AccIVInstallment { get; set; }
        public string AccMGInstallment { get; set; }
        public string AccMGFinanceLease { get; set; }
        public string AccMGFasilitasDana { get; set; }
        public string AccIVFinanceLease { get; set; }
        public string AccMKSaleLeaseback { get; set; }
        public string AccIVProject { get; set; }
        public string AccIVInfra { get; set; }
        public string Description { get; set; }
        public string ACCMKInstallment { get; set; }

        public Fees()
        {

        }
        public static List<Fees> GetFees()
        {
            List<Fees> fees = new List<Fees>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblFees";
                db.CommandType = System.Data.CommandType.Text;
          
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Fees fee = new Fees();
                            fee.FeeName = Tool.GeneralHelper.NullToString(dr["FeeName"]);
                            fee.FeeType = Tool.GeneralHelper.NullToInt(dr["FeeType"], 0);
                            fee.AccPayable = Tool.GeneralHelper.NullToString(dr["AccPayable"]);
                            fee.Description = Tool.GeneralHelper.NullToString(dr["Description"]);
                            fee.AccMKFactoring = Tool.GeneralHelper.NullToString(dr["AccMKFactoring"]);
                            fee.AccIVFactoring = Tool.GeneralHelper.NullToString(dr["AccIVFactoring"]);
                            fee.AccMKModalUsaha = Tool.GeneralHelper.NullToString(dr["AccMKModalUsaha"]);
                            fee.AccIVSaleLeaseback = Tool.GeneralHelper.NullToString(dr["AccIVSaleLeaseback"]);
                            fee.AccIVInstallment = Tool.GeneralHelper.NullToString(dr["AccIVInstallment"]);
                            fee.AccMGInstallment = Tool.GeneralHelper.NullToString(dr["AccMGInstallment"]);
                            fee.AccMGFinanceLease = Tool.GeneralHelper.NullToString(dr["AccMGFinanceLease"]);
                            fee.AccMGFasilitasDana = Tool.GeneralHelper.NullToString(dr["AccMGFasilitasDana"]);
                            fee.AccIVFinanceLease = Tool.GeneralHelper.NullToString(dr["AccIVFinanceLease"]);
                            fee.AccMKSaleLeaseback = Tool.GeneralHelper.NullToString(dr["AccMKSaleLeaseback"]);
                            fee.AccIVProject = Tool.GeneralHelper.NullToString(dr["AccIVProject"]);
                            fee.AccIVInfra = Tool.GeneralHelper.NullToString(dr["AccIVInfra"]);
                            fee.ACCMKInstallment = Tool.GeneralHelper.NullToString(dr["ACCMKInstallment"]);
                            fees.Add(fee);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return fees;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetFeesForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblFees";
                db.CommandType = System.Data.CommandType.Text;

                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem bank = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            bank.Value = IDS.Tool.GeneralHelper.NullToString(dr["FeeType"]);
                            bank.Text = IDS.Tool.GeneralHelper.NullToString(dr["FeeName"]);

                            list.Add(bank);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
    }
}
