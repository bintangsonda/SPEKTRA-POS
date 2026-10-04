using IDS.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class DashboardNew
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string BranchCode { get; set; }
        public string Department { get; set; }
        public string COAName { get; set; }
        public string ACC { get; set; }
        public string MN { get; set; }
        public decimal NilaiFinancingReceivableJan { get; set; }
        public decimal NilaiFinancingReceivableFeb { get; set; }
        public decimal NilaiFinancingReceivableMar { get; set; }
        public decimal NilaiFinancingReceivableApr { get; set; }
        public decimal NilaiFinancingReceivableMei { get; set; }
        public decimal NilaiFinancingReceivableJun { get; set; }
        public decimal NilaiFinancingReceivableJul { get; set; }
        public decimal NilaiFinancingReceivableAug { get; set; }
        public decimal NilaiFinancingReceivableSep { get; set; }
        public decimal NilaiFinancingReceivableOkt { get; set; }
        public decimal NilaiFinancingReceivableNov { get; set; }
        public decimal NilaiFinancingReceivableDes { get; set; }
        public decimal NilaiTotalAssetJan { get; set; }
        public decimal NilaiTotalAssetFeb { get; set; }
        public decimal NilaiTotalAssetMar { get; set; }
        public decimal NilaiTotalAssetApr { get; set; }
        public decimal NilaiTotalAssetMei { get; set; }
        public decimal NilaiTotalAssetJun { get; set; }
        public decimal NilaiTotalAssetJul { get; set; }
        public decimal NilaiTotalAssetAug { get; set; }
        public decimal NilaiTotalAssetSep { get; set; }
        public decimal NilaiTotalAssetOkt { get; set; }
        public decimal NilaiTotalAssetNov { get; set; }
        public decimal NilaiTotalAssetDes { get; set; }
        public decimal NilaiBankLoanJan { get; set; }
        public decimal NilaiBankLoanFeb { get; set; }
        public decimal NilaiBankLoanMar { get; set; }
        public decimal NilaiBankLoanApr { get; set; }
        public decimal NilaiBankLoanMei { get; set; }
        public decimal NilaiBankLoanJun { get; set; }
        public decimal NilaiBankLoanJul { get; set; }
        public decimal NilaiBankLoanAug { get; set; }
        public decimal NilaiBankLoanSep { get; set; }
        public decimal NilaiBankLoanOkt { get; set; }
        public decimal NilaiBankLoanNov { get; set; }
        public decimal NilaiBankLoanDes { get; set; }
        public decimal NilaiIncomeJan { get; set; }
        public decimal NilaiIncomeFeb { get; set; }
        public decimal NilaiIncomeMar { get; set; }
        public decimal NilaiIncomeApr { get; set; }
        public decimal NilaiIncomeMei { get; set; }
        public decimal NilaiIncomeJun { get; set; }
        public decimal NilaiIncomeJul { get; set; }
        public decimal NilaiIncomeAug { get; set; }
        public decimal NilaiIncomeSep { get; set; }
        public decimal NilaiIncomeOkt { get; set; }
        public decimal NilaiIncomeNov { get; set; }
        public decimal NilaiIncomeDes { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxJan { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxFeb { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxMar { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxApr { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxMei { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxJun { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxJul { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxAug { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxSep { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxOkt { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxNov { get; set; }
        public decimal NilaiTotalIncomeBeforeTaxDes { get; set; }
        public decimal TotalFinancingReceivable { get; set; }
        public decimal AverageFinancingReceivable { get; set; }
        public decimal TotalAsset { get; set; }
        public decimal AverageTotalAsset { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal AverageTotalIncome { get; set; }
        public decimal TotalBankLoan { get; set; }
        public decimal AverageBankLoan { get; set; }
        public decimal NilaiTotalAsset { get; set; }
        public decimal NilaiTotalFinancingWorkingCapital { get; set; }
        public decimal NilaiTotalFinancingInvestment { get; set; }
        public decimal NilaiTotalFinancingMultipurpose { get; set; }
        public decimal NilaiTotalIncomeWorkingCapital { get; set; }
        public decimal NilaiTotalIncomeInvestment { get; set; }
        public decimal NilaiTotalIncomeMultipurpose { get; set; }
        public decimal NilaiBankLoan { get; set; }
        public decimal NilaiIncomeBeforeTax { get; set; }
        public decimal TotalIncomeBeforeTax { get; set; }
        public decimal AverageIncomeBeforeTax { get; set; }
        public decimal NilaiWorkingCapital { get; set; }
        public decimal NilaiInvestment { get; set; }
        public decimal NilaiMultipurpose { get; set; }
        public decimal TotalOperatorIncomeThisYear { get; set; }
        public decimal TotalOtherIncomeThisYear { get; set; }
        public decimal TotalOperatorIncomeLastYear { get; set; }
        public decimal TotalOtherIncomeLastYear { get; set; }
        public decimal PersentaseOperatingIncome { get; set; }
        public decimal PersentaseOtherIncome { get; set; }
        public decimal NilaiInvestasi { get; set; }
        public decimal NilaiModalUsaha { get; set; }
        public decimal NilaiMultiguna { get; set; }
        public decimal AdminIncome { get; set; }
        public decimal ProvisionIncome { get; set; }
        public decimal LatePaymentIncome { get; set; }
        public decimal AdminInsuranceIncome { get; set; }

        public DashboardNew()
        {

        }

        public static List<DashboardNew> GetDataTotalFinancing(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> list = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadFinancingReceivabletForNewDashboard";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew dash = new DashboardNew();
                            dash.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            if (dash.COAName == "FIN REC - WORKING CAPITAL")
                            {
                                dash.NilaiFinancingReceivableJan = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalJan"], 0);
                                dash.NilaiFinancingReceivableFeb = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalFeb"], 0);
                                dash.NilaiFinancingReceivableMar = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalMar"], 0);
                                dash.NilaiFinancingReceivableApr = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalApr"], 0);
                                dash.NilaiFinancingReceivableMei = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalMei"], 0);
                                dash.NilaiFinancingReceivableJun = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalJun"], 0);
                                dash.NilaiFinancingReceivableJul = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalJul"], 0);
                                dash.NilaiFinancingReceivableAug = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalAug"], 0);
                                dash.NilaiFinancingReceivableSep = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalSep"], 0);
                                dash.NilaiFinancingReceivableOkt = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalOkt"], 0);
                                dash.NilaiFinancingReceivableNov = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalNov"], 0);
                                dash.NilaiFinancingReceivableDes = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableWorkingCapitalDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiFinancingReceivableFeb = 0;
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                    dash.AverageFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiTotalAssetFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes) / 12;
                                }
                                else
                                {
                                    dash.TotalFinancingReceivable = 0;
                                    dash.AverageFinancingReceivable = 0;
                                }
                            }
                            if (dash.COAName == "FIN REC - INVESTMENT")
                            {
                                dash.NilaiFinancingReceivableJan = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentJan"], 0);
                                dash.NilaiFinancingReceivableFeb = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentFeb"], 0);
                                dash.NilaiFinancingReceivableMar = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentMar"], 0);
                                dash.NilaiFinancingReceivableApr = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentApr"], 0);
                                dash.NilaiFinancingReceivableMei = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentMei"], 0);
                                dash.NilaiFinancingReceivableJun = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentJun"], 0);
                                dash.NilaiFinancingReceivableJul = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentJul"], 0);
                                dash.NilaiFinancingReceivableAug = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentAug"], 0);
                                dash.NilaiFinancingReceivableSep = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentSep"], 0);
                                dash.NilaiFinancingReceivableOkt = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentOkt"], 0);
                                dash.NilaiFinancingReceivableNov = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentNov"], 0);
                                dash.NilaiFinancingReceivableDes = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableInvestmentDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiFinancingReceivableFeb = 0;
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                    dash.AverageFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiTotalAssetFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes) / 12;
                                }
                                else
                                {
                                    dash.TotalFinancingReceivable = 0;
                                    dash.AverageFinancingReceivable = 0;
                                }
                            }
                            if (dash.COAName == "FIN REC - MULTIPURPOSE")
                            {
                                dash.NilaiFinancingReceivableJan = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeJan"], 0);
                                dash.NilaiFinancingReceivableFeb = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeFeb"], 0);
                                dash.NilaiFinancingReceivableMar = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeMar"], 0);
                                dash.NilaiFinancingReceivableApr = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeApr"], 0);
                                dash.NilaiFinancingReceivableMei = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeMei"], 0);
                                dash.NilaiFinancingReceivableJun = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeJun"], 0);
                                dash.NilaiFinancingReceivableJul = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeJul"], 0);
                                dash.NilaiFinancingReceivableAug = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeAug"], 0);
                                dash.NilaiFinancingReceivableSep = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeSep"], 0);
                                dash.NilaiFinancingReceivableOkt = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeOkt"], 0);
                                dash.NilaiFinancingReceivableNov = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeNov"], 0);
                                dash.NilaiFinancingReceivableDes = Tool.GeneralHelper.NullToDecimal(dr["FinancingReceivableMultipurposeDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiFinancingReceivableFeb = 0;
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                    dash.AverageFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiTotalAssetFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes) / 12;
                                }
                                else
                                {
                                    dash.TotalFinancingReceivable = 0;
                                    dash.AverageFinancingReceivable = 0;
                                }
                            }
                            if (dash.COAName == "TOTAL FINANCING")
                            {
                                dash.NilaiFinancingReceivableJan = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableJan"], 0);
                                dash.NilaiFinancingReceivableFeb = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableFeb"], 0);
                                dash.NilaiFinancingReceivableMar = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableMar"], 0);
                                dash.NilaiFinancingReceivableApr = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableApr"], 0);
                                dash.NilaiFinancingReceivableMei = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableMei"], 0);
                                dash.NilaiFinancingReceivableJun = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableJun"], 0);
                                dash.NilaiFinancingReceivableJul = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableJul"], 0);
                                dash.NilaiFinancingReceivableAug = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableAug"], 0);
                                dash.NilaiFinancingReceivableSep = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableSep"], 0);
                                dash.NilaiFinancingReceivableOkt = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableOkt"], 0);
                                dash.NilaiFinancingReceivableNov = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableNov"], 0);
                                dash.NilaiFinancingReceivableDes = Tool.GeneralHelper.NullToDecimal(dr["TotalFinancingReceivableDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiFinancingReceivableFeb = 0;
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                    dash.AverageFinancingReceivable = dash.NilaiFinancingReceivableJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiFinancingReceivableMar = 0;
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiFinancingReceivableApr = 0;
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiFinancingReceivableMei = 0;
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiFinancingReceivableJun = 0;
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiFinancingReceivableJul = 0;
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiFinancingReceivableAug = 0;
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiFinancingReceivableSep = 0;
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiFinancingReceivableOkt = 0;
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiFinancingReceivableNov = 0;
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiFinancingReceivableDes = 0;
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalFinancingReceivable = dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes;
                                    dash.AverageFinancingReceivable = (dash.NilaiFinancingReceivableJan + dash.NilaiFinancingReceivableFeb + dash.NilaiFinancingReceivableMar + dash.NilaiFinancingReceivableApr + dash.NilaiFinancingReceivableMei + dash.NilaiFinancingReceivableJun + dash.NilaiFinancingReceivableJul + dash.NilaiFinancingReceivableAug + dash.NilaiFinancingReceivableSep + dash.NilaiFinancingReceivableOkt + dash.NilaiFinancingReceivableNov + dash.NilaiFinancingReceivableDes) / 12;
                                }
                                else
                                {
                                    dash.TotalFinancingReceivable = 0;
                                    dash.AverageFinancingReceivable = 0;
                                }
                            }
                            list.Add(dash);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }
        public static List<DashboardNew> GetDataBankLoan(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> list = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadBankLoanForNewDashboard";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew dash = new DashboardNew();
                            dash.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            dash.NilaiBankLoanJan = Tool.GeneralHelper.NullToDecimal(dr["BankLoanJan"], 0);
                            dash.NilaiBankLoanFeb = Tool.GeneralHelper.NullToDecimal(dr["BankLoanFeb"], 0);
                            dash.NilaiBankLoanMar = Tool.GeneralHelper.NullToDecimal(dr["BankLoanMar"], 0);
                            dash.NilaiBankLoanApr = Tool.GeneralHelper.NullToDecimal(dr["BankLoanApr"], 0);
                            dash.NilaiBankLoanMei = Tool.GeneralHelper.NullToDecimal(dr["BankLoanMei"], 0);
                            dash.NilaiBankLoanJun = Tool.GeneralHelper.NullToDecimal(dr["BankLoanJun"], 0);
                            dash.NilaiBankLoanJul = Tool.GeneralHelper.NullToDecimal(dr["BankLoanJul"], 0);
                            dash.NilaiBankLoanAug = Tool.GeneralHelper.NullToDecimal(dr["BankLoanAug"], 0);
                            dash.NilaiBankLoanSep = Tool.GeneralHelper.NullToDecimal(dr["BankLoanSep"], 0);
                            dash.NilaiBankLoanOkt = Tool.GeneralHelper.NullToDecimal(dr["BankLoanOkt"], 0);
                            dash.NilaiBankLoanNov = Tool.GeneralHelper.NullToDecimal(dr["BankLoanNov"], 0);
                            dash.NilaiBankLoanDes = Tool.GeneralHelper.NullToDecimal(dr["BankLoanDes"], 0);
                            if (month == "01")
                            {
                                dash.NilaiBankLoanFeb = 0;
                                dash.NilaiBankLoanMar = 0;
                                dash.NilaiBankLoanApr = 0;
                                dash.NilaiBankLoanMei = 0;
                                dash.NilaiBankLoanJun = 0;
                                dash.NilaiBankLoanJul = 0;
                                dash.NilaiBankLoanAug = 0;
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan;
                                dash.AverageBankLoan = dash.NilaiBankLoanJan;
                            }
                            else if (month == "02")
                            {
                                dash.NilaiBankLoanMar = 0;
                                dash.NilaiBankLoanApr = 0;
                                dash.NilaiBankLoanMei = 0;
                                dash.NilaiBankLoanJun = 0;
                                dash.NilaiBankLoanJul = 0;
                                dash.NilaiBankLoanAug = 0;
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb) / 2;
                            }
                            else if (month == "03")
                            {
                                dash.NilaiBankLoanApr = 0;
                                dash.NilaiBankLoanMei = 0;
                                dash.NilaiBankLoanJun = 0;
                                dash.NilaiBankLoanJul = 0;
                                dash.NilaiBankLoanAug = 0;
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar) / 3;
                            }
                            else if (month == "04")
                            {
                                dash.NilaiBankLoanMei = 0;
                                dash.NilaiBankLoanJun = 0;
                                dash.NilaiBankLoanJul = 0;
                                dash.NilaiBankLoanAug = 0;
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr) / 4;
                            }
                            else if (month == "05")
                            {
                                dash.NilaiBankLoanJun = 0;
                                dash.NilaiBankLoanJul = 0;
                                dash.NilaiBankLoanAug = 0;
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei) / 5;
                            }
                            else if (month == "06")
                            {
                                dash.NilaiBankLoanJul = 0;
                                dash.NilaiBankLoanAug = 0;
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun) / 6;
                            }
                            else if (month == "07")
                            {
                                dash.NilaiBankLoanAug = 0;
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul) / 7;
                            }
                            else if (month == "08")
                            {
                                dash.NilaiBankLoanSep = 0;
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug) / 8;
                            }
                            else if (month == "09")
                            {
                                dash.NilaiBankLoanOkt = 0;
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep) / 9;
                            }
                            else if (month == "10")
                            {
                                dash.NilaiBankLoanNov = 0;
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep + dash.NilaiBankLoanOkt;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep + dash.NilaiBankLoanOkt) / 10;
                            }
                            else if (month == "11")
                            {
                                dash.NilaiBankLoanDes = 0;
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep + dash.NilaiBankLoanOkt + dash.NilaiBankLoanNov;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep + dash.NilaiBankLoanOkt + dash.NilaiBankLoanNov) / 11;
                            }
                            else if (month == "12")
                            {
                                dash.TotalBankLoan = dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep + dash.NilaiBankLoanOkt + dash.NilaiBankLoanNov + dash.NilaiBankLoanDes;
                                dash.AverageBankLoan = (dash.NilaiBankLoanJan + dash.NilaiBankLoanFeb + dash.NilaiBankLoanMar + dash.NilaiBankLoanApr + dash.NilaiBankLoanMei + dash.NilaiBankLoanJun + dash.NilaiBankLoanJul + dash.NilaiBankLoanAug + dash.NilaiBankLoanSep + dash.NilaiBankLoanOkt + dash.NilaiBankLoanNov + dash.NilaiBankLoanDes) / 12;
                            }
                            else
                            {
                                dash.TotalBankLoan = 0;
                                dash.AverageBankLoan = 0;
                            }
                            list.Add(dash);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }
        public static List<DashboardNew> GetDataTotalAsset(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> list = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadTotalAssetForNewDashboard";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew dash = new DashboardNew();
                            dash.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            dash.NilaiTotalAssetJan = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetJan"], 0);
                            dash.NilaiTotalAssetFeb = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetFeb"], 0);
                            dash.NilaiTotalAssetMar = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetMar"], 0);
                            dash.NilaiTotalAssetApr = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetApr"], 0);
                            dash.NilaiTotalAssetMei = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetMei"], 0);
                            dash.NilaiTotalAssetJun = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetJun"], 0);
                            dash.NilaiTotalAssetJul = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetJul"], 0);
                            dash.NilaiTotalAssetAug = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetAug"], 0);
                            dash.NilaiTotalAssetSep = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetSep"], 0);
                            dash.NilaiTotalAssetOkt = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetOkt"], 0);
                            dash.NilaiTotalAssetNov = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetNov"], 0);
                            dash.NilaiTotalAssetDes = Tool.GeneralHelper.NullToDecimal(dr["TotalAssetDes"], 0);
                            if (month == "01")
                            {
                                dash.NilaiTotalAssetFeb = 0;
                                dash.NilaiTotalAssetMar = 0;
                                dash.NilaiTotalAssetApr = 0;
                                dash.NilaiTotalAssetMei = 0;
                                dash.NilaiTotalAssetJun = 0;
                                dash.NilaiTotalAssetJul = 0;
                                dash.NilaiTotalAssetAug = 0;
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan;
                                dash.AverageTotalAsset = dash.NilaiTotalAssetJan;
                            }
                            else if (month == "02")
                            {
                                dash.NilaiTotalAssetMar = 0;
                                dash.NilaiTotalAssetApr = 0;
                                dash.NilaiTotalAssetMei = 0;
                                dash.NilaiTotalAssetJun = 0;
                                dash.NilaiTotalAssetJul = 0;
                                dash.NilaiTotalAssetAug = 0;
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb) / 2;
                            }
                            else if (month == "03")
                            {
                                dash.NilaiTotalAssetApr = 0;
                                dash.NilaiTotalAssetMei = 0;
                                dash.NilaiTotalAssetJun = 0;
                                dash.NilaiTotalAssetJul = 0;
                                dash.NilaiTotalAssetAug = 0;
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar) / 3;
                            }
                            else if (month == "04")
                            {
                                dash.NilaiTotalAssetMei = 0;
                                dash.NilaiTotalAssetJun = 0;
                                dash.NilaiTotalAssetJul = 0;
                                dash.NilaiTotalAssetAug = 0;
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr) / 4;
                            }
                            else if (month == "05")
                            {
                                dash.NilaiTotalAssetJun = 0;
                                dash.NilaiTotalAssetJul = 0;
                                dash.NilaiTotalAssetAug = 0;
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei) / 5;
                            }
                            else if (month == "06")
                            {
                                dash.NilaiTotalAssetJul = 0;
                                dash.NilaiTotalAssetAug = 0;
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun) / 6;
                            }
                            else if (month == "07")
                            {
                                dash.NilaiTotalAssetAug = 0;
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul) / 7;
                            }
                            else if (month == "08")
                            {
                                dash.NilaiTotalAssetSep = 0;
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug) / 8;
                            }
                            else if (month == "09")
                            {
                                dash.NilaiTotalAssetOkt = 0;
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep) / 9;
                            }
                            else if (month == "10")
                            {
                                dash.NilaiTotalAssetNov = 0;
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep + dash.NilaiTotalAssetOkt;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep + dash.NilaiTotalAssetOkt) / 10;
                            }
                            else if (month == "11")
                            {
                                dash.NilaiTotalAssetDes = 0;
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep + dash.NilaiTotalAssetOkt + dash.NilaiTotalAssetNov;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep + dash.NilaiTotalAssetOkt + dash.NilaiTotalAssetNov) / 11;
                            }
                            else if (month == "12")
                            {
                                dash.TotalAsset = dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep + dash.NilaiTotalAssetOkt + dash.NilaiTotalAssetNov + dash.NilaiTotalAssetDes;
                                dash.AverageTotalAsset = (dash.NilaiTotalAssetJan + dash.NilaiTotalAssetFeb + dash.NilaiTotalAssetMar + dash.NilaiTotalAssetApr + dash.NilaiTotalAssetMei + dash.NilaiTotalAssetJun + dash.NilaiTotalAssetJul + dash.NilaiTotalAssetAug + dash.NilaiTotalAssetSep + dash.NilaiTotalAssetOkt + dash.NilaiTotalAssetNov + dash.NilaiTotalAssetDes) / 12;
                            }
                            else
                            {
                                dash.TotalAsset = 0;
                                dash.AverageTotalAsset = 0;
                            }
                            list.Add(dash);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }
        public static List<DashboardNew> GetDataTotalIncome(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> list = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadIncomeForNewDashboard";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew dash = new DashboardNew();
                            dash.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            if (dash.COAName == "INCOME - WORKING CAPITAL")
                            {
                                dash.NilaiIncomeJan = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalJan"], 0);
                                dash.NilaiIncomeFeb = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalFeb"], 0);
                                dash.NilaiIncomeMar = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalMar"], 0);
                                dash.NilaiIncomeApr = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalApr"], 0);
                                dash.NilaiIncomeMei = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalMei"], 0);
                                dash.NilaiIncomeJun = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalJun"], 0);
                                dash.NilaiIncomeJul = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalJul"], 0);
                                dash.NilaiIncomeAug = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalAug"], 0);
                                dash.NilaiIncomeSep = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalSep"], 0);
                                dash.NilaiIncomeOkt = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalOkt"], 0);
                                dash.NilaiIncomeNov = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalNov"], 0);
                                dash.NilaiIncomeDes = Tool.GeneralHelper.NullToDecimal(dr["IncomeWorkingCapitalDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiIncomeFeb = 0;
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan;
                                    dash.AverageTotalIncome = dash.NilaiIncomeJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes) / 12;
                                }
                                else
                                {
                                    dash.TotalIncome = 0;
                                    dash.AverageTotalIncome = 0;
                                }
                            }
                            if (dash.COAName == "INCOME - INVESTMENT")
                            {
                                dash.NilaiIncomeJan = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentJan"], 0);
                                dash.NilaiIncomeFeb = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentFeb"], 0);
                                dash.NilaiIncomeMar = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentMar"], 0);
                                dash.NilaiIncomeApr = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentApr"], 0);
                                dash.NilaiIncomeMei = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentMei"], 0);
                                dash.NilaiIncomeJun = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentJun"], 0);
                                dash.NilaiIncomeJul = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentJul"], 0);
                                dash.NilaiIncomeAug = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentAug"], 0);
                                dash.NilaiIncomeSep = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentSep"], 0);
                                dash.NilaiIncomeOkt = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentOkt"], 0);
                                dash.NilaiIncomeNov = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentNov"], 0);
                                dash.NilaiIncomeDes = Tool.GeneralHelper.NullToDecimal(dr["IncomeInvestmentDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiIncomeFeb = 0;
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan;
                                    dash.AverageTotalIncome = dash.NilaiIncomeJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes) / 12;
                                }
                                else
                                {
                                    dash.TotalIncome = 0;
                                    dash.AverageTotalIncome = 0;
                                }
                            }
                            if (dash.COAName == "INCOME - MULTIPURPOSE")
                            {
                                dash.NilaiIncomeJan = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeJan"], 0);
                                dash.NilaiIncomeFeb = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeFeb"], 0);
                                dash.NilaiIncomeMar = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeMar"], 0);
                                dash.NilaiIncomeApr = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeApr"], 0);
                                dash.NilaiIncomeMei = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeMei"], 0);
                                dash.NilaiIncomeJun = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeJun"], 0);
                                dash.NilaiIncomeJul = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeJul"], 0);
                                dash.NilaiIncomeAug = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeAug"], 0);
                                dash.NilaiIncomeSep = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeSep"], 0);
                                dash.NilaiIncomeOkt = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeOkt"], 0);
                                dash.NilaiIncomeNov = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeNov"], 0);
                                dash.NilaiIncomeDes = Tool.GeneralHelper.NullToDecimal(dr["IncomeMultipurposeDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiIncomeFeb = 0;
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan;
                                    dash.AverageTotalIncome = dash.NilaiIncomeJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes) / 12;
                                }
                                else
                                {
                                    dash.TotalIncome = 0;
                                    dash.AverageTotalIncome = 0;
                                }
                            }
                            if (dash.COAName == "TOTAL INCOME")
                            {
                                dash.NilaiIncomeJan = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeJan"], 0);
                                dash.NilaiIncomeFeb = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeFeb"], 0);
                                dash.NilaiIncomeMar = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeMar"], 0);
                                dash.NilaiIncomeApr = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeApr"], 0);
                                dash.NilaiIncomeMei = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeMei"], 0);
                                dash.NilaiIncomeJun = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeJun"], 0);
                                dash.NilaiIncomeJul = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeJul"], 0);
                                dash.NilaiIncomeAug = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeAug"], 0);
                                dash.NilaiIncomeSep = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeSep"], 0);
                                dash.NilaiIncomeOkt = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeOkt"], 0);
                                dash.NilaiIncomeNov = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeNov"], 0);
                                dash.NilaiIncomeDes = Tool.GeneralHelper.NullToDecimal(dr["TotalIncomeDes"], 0);
                                if (month == "01")
                                {
                                    dash.NilaiIncomeFeb = 0;
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan;
                                    dash.AverageTotalIncome = dash.NilaiIncomeJan;
                                }
                                else if (month == "02")
                                {
                                    dash.NilaiIncomeMar = 0;
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb) / 2;
                                }
                                else if (month == "03")
                                {
                                    dash.NilaiIncomeApr = 0;
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar) / 3;
                                }
                                else if (month == "04")
                                {
                                    dash.NilaiIncomeMei = 0;
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr) / 4;
                                }
                                else if (month == "05")
                                {
                                    dash.NilaiIncomeJun = 0;
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei) / 5;
                                }
                                else if (month == "06")
                                {
                                    dash.NilaiIncomeJul = 0;
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun) / 6;
                                }
                                else if (month == "07")
                                {
                                    dash.NilaiIncomeAug = 0;
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul) / 7;
                                }
                                else if (month == "08")
                                {
                                    dash.NilaiIncomeSep = 0;
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug) / 8;
                                }
                                else if (month == "09")
                                {
                                    dash.NilaiIncomeOkt = 0;
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep) / 9;
                                }
                                else if (month == "10")
                                {
                                    dash.NilaiIncomeNov = 0;
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt) / 10;
                                }
                                else if (month == "11")
                                {
                                    dash.NilaiIncomeDes = 0;
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov) / 11;
                                }
                                else if (month == "12")
                                {
                                    dash.TotalIncome = dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes;
                                    dash.AverageTotalIncome = (dash.NilaiIncomeJan + dash.NilaiIncomeFeb + dash.NilaiIncomeMar + dash.NilaiIncomeApr + dash.NilaiIncomeMei + dash.NilaiIncomeJun + dash.NilaiIncomeJul + dash.NilaiIncomeAug + dash.NilaiIncomeSep + dash.NilaiIncomeOkt + dash.NilaiIncomeNov + dash.NilaiIncomeDes) / 12;
                                }
                                else
                                {
                                    dash.TotalIncome = 0;
                                    dash.AverageTotalIncome = 0;
                                }
                            }
                            list.Add(dash);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }
        public static List<DashboardNew> GetDataTotalIncomeBeforeTax(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> list = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadTotalIncomeBeforeTaxForNewDashboard";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew dash = new DashboardNew();
                            dash.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            dash.NilaiTotalIncomeBeforeTaxJan = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxJan"], 0);
                            dash.NilaiTotalIncomeBeforeTaxFeb = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxFeb"], 0);
                            dash.NilaiTotalIncomeBeforeTaxMar = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxMar"], 0);
                            dash.NilaiTotalIncomeBeforeTaxApr = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxApr"], 0);
                            dash.NilaiTotalIncomeBeforeTaxMei = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxMei"], 0);
                            dash.NilaiTotalIncomeBeforeTaxJun = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxJun"], 0);
                            dash.NilaiTotalIncomeBeforeTaxJul = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxJul"], 0);
                            dash.NilaiTotalIncomeBeforeTaxAug = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxAug"], 0);
                            dash.NilaiTotalIncomeBeforeTaxSep = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxSep"], 0);
                            dash.NilaiTotalIncomeBeforeTaxOkt = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxOkt"], 0);
                            dash.NilaiTotalIncomeBeforeTaxNov = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxNov"], 0);
                            dash.NilaiTotalIncomeBeforeTaxDes = Tool.GeneralHelper.NullToDecimal(dr["IncomeBeforeTaxDes"], 0);
                            if (month == "01")
                            {
                                dash.NilaiTotalIncomeBeforeTaxFeb = 0;
                                dash.NilaiTotalIncomeBeforeTaxMar = 0;
                                dash.NilaiTotalIncomeBeforeTaxApr = 0;
                                dash.NilaiTotalIncomeBeforeTaxMei = 0;
                                dash.NilaiTotalIncomeBeforeTaxJun = 0;
                                dash.NilaiTotalIncomeBeforeTaxJul = 0;
                                dash.NilaiTotalIncomeBeforeTaxAug = 0;
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan;
                                dash.AverageIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan;
                            }
                            else if (month == "02")
                            {
                                dash.NilaiTotalIncomeBeforeTaxMar = 0;
                                dash.NilaiTotalIncomeBeforeTaxApr = 0;
                                dash.NilaiTotalIncomeBeforeTaxMei = 0;
                                dash.NilaiTotalIncomeBeforeTaxJun = 0;
                                dash.NilaiTotalIncomeBeforeTaxJul = 0;
                                dash.NilaiTotalIncomeBeforeTaxAug = 0;
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb) / 2;
                            }
                            else if (month == "03")
                            {
                                dash.NilaiTotalIncomeBeforeTaxApr = 0;
                                dash.NilaiTotalIncomeBeforeTaxMei = 0;
                                dash.NilaiTotalIncomeBeforeTaxJun = 0;
                                dash.NilaiTotalIncomeBeforeTaxJul = 0;
                                dash.NilaiTotalIncomeBeforeTaxAug = 0;
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar) / 3;
                            }
                            else if (month == "04")
                            {
                                dash.NilaiTotalIncomeBeforeTaxMei = 0;
                                dash.NilaiTotalIncomeBeforeTaxJun = 0;
                                dash.NilaiTotalIncomeBeforeTaxJul = 0;
                                dash.NilaiTotalIncomeBeforeTaxAug = 0;
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr) / 4;
                            }
                            else if (month == "05")
                            {
                                dash.NilaiTotalIncomeBeforeTaxJun = 0;
                                dash.NilaiTotalIncomeBeforeTaxJul = 0;
                                dash.NilaiTotalIncomeBeforeTaxAug = 0;
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei) / 5;
                            }
                            else if (month == "06")
                            {
                                dash.NilaiTotalIncomeBeforeTaxJul = 0;
                                dash.NilaiTotalIncomeBeforeTaxAug = 0;
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun) / 6;
                            }
                            else if (month == "07")
                            {
                                dash.NilaiTotalIncomeBeforeTaxAug = 0;
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul) / 7;
                            }
                            else if (month == "08")
                            {
                                dash.NilaiTotalIncomeBeforeTaxSep = 0;
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug) / 8;
                            }
                            else if (month == "09")
                            {
                                dash.NilaiTotalIncomeBeforeTaxOkt = 0;
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep) / 9;
                            }
                            else if (month == "10")
                            {
                                dash.NilaiTotalIncomeBeforeTaxNov = 0;
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep + dash.NilaiTotalIncomeBeforeTaxOkt;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep + dash.NilaiTotalIncomeBeforeTaxOkt) / 10;
                            }
                            else if (month == "11")
                            {
                                dash.NilaiTotalIncomeBeforeTaxDes = 0;
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep + dash.NilaiTotalIncomeBeforeTaxOkt + dash.NilaiTotalIncomeBeforeTaxNov;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep + dash.NilaiTotalIncomeBeforeTaxOkt + dash.NilaiTotalIncomeBeforeTaxNov) / 11;
                            }
                            else if (month == "12")
                            {
                                dash.TotalIncomeBeforeTax = dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep + dash.NilaiTotalIncomeBeforeTaxOkt + dash.NilaiTotalIncomeBeforeTaxNov + dash.NilaiTotalIncomeBeforeTaxDes;
                                dash.AverageIncomeBeforeTax = (dash.NilaiTotalIncomeBeforeTaxJan + dash.NilaiTotalIncomeBeforeTaxFeb + dash.NilaiTotalIncomeBeforeTaxMar + dash.NilaiTotalIncomeBeforeTaxApr + dash.NilaiTotalIncomeBeforeTaxMei + dash.NilaiTotalIncomeBeforeTaxJun + dash.NilaiTotalIncomeBeforeTaxJul + dash.NilaiTotalIncomeBeforeTaxAug + dash.NilaiTotalIncomeBeforeTaxSep + dash.NilaiTotalIncomeBeforeTaxOkt + dash.NilaiTotalIncomeBeforeTaxNov + dash.NilaiTotalIncomeBeforeTaxDes) / 12;
                            }
                            else
                            {
                                dash.TotalIncomeBeforeTax = 0;
                                dash.AverageIncomeBeforeTax = 0;
                            }
                            list.Add(dash);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }
        public static List<DashboardNew> GetTotalAssetForChart(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> listTotalAsset = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadTotalAssetForChart";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew totalasset = new DashboardNew();
                            totalasset.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            totalasset.NilaiTotalAsset = Tool.GeneralHelper.NullToDecimal(dr["NilaiTotalAsset"], 0);
                            listTotalAsset.Add(totalasset);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return listTotalAsset;
        }
        public static List<DashboardNew> GetTotalFinancingForChart(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> listFinRec = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadTotalFinancingForChart";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew finrec = new DashboardNew();
                            finrec.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            finrec.NilaiTotalFinancingWorkingCapital = Tool.GeneralHelper.NullToDecimal(dr["NilaiWorkingCapital"], 0);
                            finrec.NilaiTotalFinancingInvestment = Tool.GeneralHelper.NullToDecimal(dr["NilaiInvestment"], 0);
                            finrec.NilaiTotalFinancingMultipurpose = Tool.GeneralHelper.NullToDecimal(dr["NilaiMultipurpose"], 0);
                            listFinRec.Add(finrec);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return listFinRec;
        }
        public static List<DashboardNew> GetBankLoanForChart(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> listBankLoan = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadBankLoanForChart";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew bankloan = new DashboardNew();
                            bankloan.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            bankloan.NilaiBankLoan = Tool.GeneralHelper.NullToDecimal(dr["NilaiBankLoan"], 0);
                            listBankLoan.Add(bankloan);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return listBankLoan;
        }
        public static List<DashboardNew> GetIncomeBeforeTaxForChart(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> listIncomeBeforeTax = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadIncomeBeforeTaxForChart";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew incomebeforetax = new DashboardNew();
                            incomebeforetax.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            incomebeforetax.NilaiIncomeBeforeTax = Tool.GeneralHelper.NullToDecimal(dr["NilaiIncomeBeforeTax"], 0);
                            listIncomeBeforeTax.Add(incomebeforetax);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return listIncomeBeforeTax;
        }
        public static List<DashboardNew> GetTotalIncomeForChart(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> listTotalIncome = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadTotalIncomeForChart";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew totalIncome = new DashboardNew();
                            totalIncome.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            totalIncome.NilaiTotalIncomeWorkingCapital = Tool.GeneralHelper.NullToDecimal(dr["NilaiWorkingCapital"], 0);
                            totalIncome.NilaiTotalIncomeInvestment = Tool.GeneralHelper.NullToDecimal(dr["NilaiInvestment"], 0);
                            totalIncome.NilaiTotalIncomeMultipurpose = Tool.GeneralHelper.NullToDecimal(dr["NilaiMultipurpose"], 0);
                            listTotalIncome.Add(totalIncome);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return listTotalIncome;
        }
        public static List<DashboardNew> GetFinancingForPieChart(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> listFinancing = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadFinancingForPieChart";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew financing = new DashboardNew();
                            financing.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            financing.NilaiWorkingCapital = Tool.GeneralHelper.NullToDecimal(dr["NilaiWorkingCapital"], 0);
                            financing.NilaiInvestment = Tool.GeneralHelper.NullToDecimal(dr["NilaiInvestment"], 0);
                            financing.NilaiMultipurpose = Tool.GeneralHelper.NullToDecimal(dr["NilaiMultipurpose"], 0);
                            listFinancing.Add(financing);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return listFinancing;
        }
        public static List<DashboardNew> GetIncomeForPieChart(string year, string month)
        {
            List<IDS.GeneralTable.DashboardNew> listIncome = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadIncomeForPieChart";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            DashboardNew income = new DashboardNew();
                            income.COAName = Tool.GeneralHelper.NullToString(dr["ACCName"]);
                            income.NilaiWorkingCapital = Tool.GeneralHelper.NullToDecimal(dr["NilaiWorkingCapital"], 0);
                            income.NilaiInvestment = Tool.GeneralHelper.NullToDecimal(dr["NilaiInvestment"], 0);
                            income.NilaiMultipurpose = Tool.GeneralHelper.NullToDecimal(dr["NilaiMultipurpose"], 0);
                            listIncome.Add(income);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return listIncome;
        }
        public static List<DashboardNew> GetDataAccounting(string year, string month)
        {
            List<DashboardNew> dashboard = new List<DashboardNew>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "LoadDashboard";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);
                db.AddParameter("@UserGroup", System.Data.SqlDbType.VarChar, "SuAd");
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        DashboardNew dash = new DashboardNew();
                        dash.TotalOperatorIncomeThisYear = Convert.ToDecimal(dr["TotalOperatorIncomeThisYear"]);
                        dash.TotalOperatorIncomeLastYear = Convert.ToDecimal(dr["TotalOperatorIncomeLastYear"]);
                        //dash.TotalOperatorIncomeLastYear = 4000000000;
                        dash.TotalOtherIncomeThisYear = Convert.ToDecimal(dr["TotalOtherIncomeThisYear"]);
                        //dash.TotalOtherIncomeLastYear = 800000000;
                        dash.TotalOtherIncomeLastYear = Convert.ToDecimal(dr["TotalOtherIncomeLastYear"]);
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
                        dash.NilaiInvestasi = Convert.ToDecimal(dr["TotalNilaiInvestasi"]);
                        dash.NilaiModalUsaha = Convert.ToDecimal(dr["TotalNilaiModalKerja"]);
                        dash.NilaiMultiguna = Convert.ToDecimal(dr["TotalNilaiMultiguna"]);
                        dash.AdminIncome = Convert.ToDecimal(dr["AdminIncome"]);
                        dash.ProvisionIncome = Convert.ToDecimal(dr["ProvisionIncome"]);
                        dash.LatePaymentIncome = Convert.ToDecimal(dr["LatePaymentIncome"]);
                        dash.AdminInsuranceIncome = Convert.ToDecimal(dr["AdminInsuranceIncome"]);
                        dashboard.Add(dash);
                    }

                    if (!dr.IsClosed)
                        dr.Close();

                }

            }
            return dashboard;
        }
    }
}
