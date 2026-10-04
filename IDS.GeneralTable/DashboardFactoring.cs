using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class DashboardFactoring
    {
        public string SelectedYear { get; set; }
        public string SelectedMonth { get; set; }
        public int TotalFacility { get; set; }
        public int TotalPayment { get; set; }
        public int TotalTermination { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalAssetContract { get; set; }
        public int TotalAssetCollateral { get; set; }
        public int TotalFixedAsset { get; set; } //Add by Jeremi 31 Juli 2024
        public int TotalRejectedFixedAsset { get; set; } //Add by Jeremi 31 Juli 2024
        public int TotalSO { get; set; }
        public int TotalSOA { get; set; }
        public int TotalContractDisbursement { get; set; }
        public decimal TotalOperatorIncomeThisYear { get; set; }
        public decimal TotalOtherIncomeThisYear { get; set; }
        public decimal TotalOperatorIncomeLastYear { get; set; }
        public decimal TotalOtherIncomeLastYear { get; set; }
        public decimal NilaiInvestasi { get; set; }
        public decimal NilaiModalUsaha { get; set; }
        public decimal NilaiMultiguna { get; set; }
        public decimal InvestasiSaleLeaseBack { get; set; }
        public decimal InvestasiPurchaseInstallment { get; set; }
        public decimal ModalKerjaFactoring { get; set; }
        public decimal ModalKerjaModalUsaha { get; set; }
        public decimal MultigunaPurchaseInstallment { get; set; }
        public decimal AdminIncome { get; set; }
        public decimal ProvisionIncome { get; set; }
        public decimal LatePaymentIncome { get; set; }
        public decimal AdminInsuranceIncome { get; set; }
        public decimal PersentaseOperatingIncome { get; set; }
        public decimal PersentaseOtherIncome { get; set; }
        public decimal AdminModalUsaha { get; set; }
        public decimal AdminInvestasi { get; set; }
        public decimal AdminMultiguna { get; set; }
        public decimal ProvisionModalUsaha { get; set; }
        public decimal ProvisionInvestasi { get; set; }
        public decimal ProvisionMultiguna { get; set; }
        public decimal LatePaymentModalUsaha { get; set; }
        public decimal LatePaymentInvestasi { get; set; }
        public decimal LatePaymentMultiguna { get; set; }
        public decimal AdminInsuranceModalUsaha { get; set; }
        public decimal AdminInsuranceInvestasi { get; set; }
        public decimal AdminInsuranceMultiguna { get; set; }
        //Add by Jeremi 5 Agustus 2024
        public int TotalFactoringFacility { get; set; }
        public int TotalFactoringFacilityAmendment { get; set; }
        //End Jeremi
        public DashboardFactoring()
        {

        }

        public static DashboardFactoring GetData(string SelectedYear,string SelectedMonth,string userGroup)
        {
            IDS.GeneralTable.DashboardFactoring dash = null;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "LoadDashboardFactoring";
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, SelectedYear);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, SelectedMonth);
                db.AddParameter("@UserGroup", System.Data.SqlDbType.VarChar, userGroup);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        dash = new DashboardFactoring();
                        //Tinggal diisi sesuai dengan kebutuhan
                        dash.TotalFacility = Convert.ToInt32(dr["TotalFacility"]);
                        dash.TotalPayment = Convert.ToInt32(dr["TotalPayment"]);
                        dash.TotalTermination = Convert.ToInt32(dr["TotalTermination"]);
                        dash.TotalSO = Convert.ToInt32(dr["TotalSO"]);
                        dash.TotalSOA = Convert.ToInt32(dr["TotalSOA"]);
                        dash.TotalContractDisbursement = Convert.ToInt32(dr["TotalContractDisburse"]);
                        dash.TotalOtherIncomeThisYear = Convert.ToDecimal(dr["TotalOtherIncomeThisYear"]);
                        //dash.TotalOtherIncomeLastYear = 800000000;
                        dash.TotalOtherIncomeLastYear = Convert.ToDecimal(dr["TotalOtherIncomeLastYear"]);
                        dash.NilaiInvestasi = Convert.ToDecimal(dr["TotalNilaiInvestasi"]);
                        dash.NilaiModalUsaha = Convert.ToDecimal(dr["TotalNilaiModalKerja"]);
                        dash.NilaiMultiguna = Convert.ToDecimal(dr["TotalNilaiMultiguna"]);
                        dash.ModalKerjaFactoring = Convert.ToDecimal(dr["ModalKerjaFactoring"]);
                        dash.ModalKerjaModalUsaha = Convert.ToDecimal(dr["ModalKerjaModalUsaha"]);
                        dash.InvestasiSaleLeaseBack = Convert.ToDecimal(dr["InvestasiSaleLeaseBack"]);
                        dash.InvestasiPurchaseInstallment = Convert.ToDecimal(dr["InvestasiPurchaseInstallment"]);
                        dash.MultigunaPurchaseInstallment = Convert.ToDecimal(dr["MultigunaPurchaseInstallment"]);
                        dash.AdminIncome = Convert.ToDecimal(dr["AdminIncome"]);
                        dash.ProvisionIncome = Convert.ToDecimal(dr["ProvisionIncome"]);
                        dash.LatePaymentIncome = Convert.ToDecimal(dr["LatePaymentIncome"]);
                        dash.AdminInsuranceIncome = Convert.ToDecimal(dr["AdminInsuranceIncome"]);
                        dash.NilaiInvestasi = Convert.ToDecimal(dr["TotalNilaiInvestasi"]);
                        dash.NilaiModalUsaha = Convert.ToDecimal(dr["TotalNilaiModalKerja"]);
                        dash.NilaiMultiguna = Convert.ToDecimal(dr["TotalNilaiMultiguna"]);
                        dash.ModalKerjaFactoring = Convert.ToDecimal(dr["ModalKerjaFactoring"]);
                        dash.ModalKerjaModalUsaha = Convert.ToDecimal(dr["ModalKerjaModalUsaha"]);
                        dash.InvestasiSaleLeaseBack = Convert.ToDecimal(dr["InvestasiSaleLeaseBack"]);
                        dash.InvestasiPurchaseInstallment = Convert.ToDecimal(dr["InvestasiPurchaseInstallment"]);
                        dash.MultigunaPurchaseInstallment = Convert.ToDecimal(dr["MultigunaPurchaseInstallment"]);
                        dash.AdminIncome = Convert.ToDecimal(dr["AdminIncome"]);
                        dash.ProvisionIncome = Convert.ToDecimal(dr["ProvisionIncome"]);
                        dash.LatePaymentIncome = Convert.ToDecimal(dr["LatePaymentIncome"]);
                        dash.AdminInsuranceIncome = Convert.ToDecimal(dr["AdminInsuranceIncome"]);
                        dash.AdminModalUsaha = Convert.ToDecimal(dr["AdminModalUsaha"]);
                        dash.AdminInvestasi = Convert.ToDecimal(dr["AdminInvestasi"]);
                        dash.AdminMultiguna = Convert.ToDecimal(dr["AdminMultiguna"]);
                        dash.ProvisionModalUsaha = Convert.ToDecimal(dr["ProvisionModalUsaha"]);
                        dash.ProvisionInvestasi = Convert.ToDecimal(dr["ProvisionInvestasi"]);
                        dash.ProvisionMultiguna = Convert.ToDecimal(dr["ProvisionMultiguna"]);
                        dash.LatePaymentModalUsaha = Convert.ToDecimal(dr["LatePaymentModalUsaha"]);
                        dash.LatePaymentInvestasi = Convert.ToDecimal(dr["LatePaymentInvestasi"]);
                        dash.LatePaymentMultiguna = Convert.ToDecimal(dr["LatePaymentMultiguna"]);
                        dash.AdminInsuranceModalUsaha = Convert.ToDecimal(dr["AdminInsuranceModalUsaha"]);
                        dash.AdminInsuranceInvestasi = Convert.ToDecimal(dr["AdminInsuranceInvestasi"]);
                        dash.AdminInsuranceMultiguna = Convert.ToDecimal(dr["AdminInsuranceMultiguna"]);
                        decimal OperatingIncomeThisYear = dash.TotalOperatorIncomeThisYear;
                        decimal OperatingIncomeLastYear = dash.TotalOperatorIncomeLastYear;
                        decimal OtherIncomeThisYear = dash.TotalOtherIncomeThisYear;
                        decimal OtherIncomeLastYear = dash.TotalOtherIncomeLastYear;
                        if (OperatingIncomeThisYear > OperatingIncomeLastYear && !(OperatingIncomeLastYear == 0))
                        {
                            dash.PersentaseOperatingIncome = ((OperatingIncomeThisYear - OperatingIncomeLastYear) / OperatingIncomeLastYear) * 100;
                        }
                        else if (OperatingIncomeThisYear < OperatingIncomeLastYear && !(OperatingIncomeLastYear == 0))
                        {
                            dash.PersentaseOperatingIncome = ((OperatingIncomeLastYear - OperatingIncomeThisYear) / OperatingIncomeLastYear) * 100;
                        }
                        if (OtherIncomeThisYear > OtherIncomeLastYear && !(OtherIncomeLastYear == 0))
                        {
                            dash.PersentaseOtherIncome = ((OtherIncomeThisYear - OtherIncomeLastYear) / OtherIncomeLastYear) * 100;
                        }
                        else if (OtherIncomeThisYear < OtherIncomeLastYear && !(OtherIncomeLastYear == 0))
                        {
                            dash.PersentaseOtherIncome = ((OtherIncomeLastYear - OtherIncomeThisYear) / OtherIncomeLastYear) * 100;
                        }
                        dash.TotalFixedAsset = Convert.ToInt32(dr["TotalFixedAsset"]);
                        dash.TotalRejectedFixedAsset = Convert.ToInt32(dr["TotalRejectedFixedAsset"]);
                        if (!dr.IsClosed)
                        {
                            dr.Close();
                        }
                    }

                    db.Close();
                }

                return dash;
            }
        }
    }
}
