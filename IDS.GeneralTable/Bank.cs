using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using IDS.Tool;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class Bank
    {
        [Display(Name = "Bank Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bank Code is required")]
        [MaxLength(20), StringLength(20)]
        public string BankCode { get; set; }

        [Display(Name = "Bank Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bank name is required")]
        [MaxLength(50)]
        public string BankName { get; set; }
        
        [Display(Name = "Bank Ccy")]
        public IDS.GeneralTable.Currency BankCcy { get; set; }

        [Display(Name = "Gel Account")]
        public IDS.GLTable.ChartOfAccount GelAccount { get; set; }

        [Display(Name = "Beneficiary")]
        [MaxLength(100), StringLength(100)]
        public string Beneficiary { get; set; }

        /// <summary>
        /// Cash Account Leasing
        /// </summary>
        public string CashAccLE { get; set; }
        /// <summary>
        /// Cash Account CF
        /// </summary>
        public string CashAccCF { get; set; }
        /// <summary>
        /// Cash Account JF
        /// </summary>
        public string CashAccJF { get; set; }
        /// <summary>
        /// Cash Account Factoring
        /// </summary>
        public string CashAccFT { get; set; }
        /// <summary>
        /// Cash Account PAKAR
        /// </summary>
        public string CashAccPK { get; set; }
        /// <summary>
        /// Cash Account Investasi Finance Lease
        /// </summary>
        public string CashAccInvFL { get; set; }
        /// <summary>
        /// Cash Account Investatis Sale and Leaseback
        /// </summary>
        public string CashAccInvSL { get; set; }
        /// <summary>
        /// Cash Account Investasi Installment Financing Barang
        /// </summary>
        public string CashAccInvIB { get; set; }
        /// <summary>
        /// Cash Account Proyek - Finance lease
        /// </summary>
        public string CashAccPykFL { get; set; }
        /// <summary>
        /// Cash Account Proyek - Sale & Leaseback
        /// </summary>
        public string CashAccPykSL { get; set; }
        /// <summary>
        /// Cash Account Proyek Installment Financing - Barang
        /// </summary>
        public string CashAccPykIB { get; set; }
        /// <summary>
        /// Cash Account Proyek Installment Financing - Jasa
        /// </summary>
        public string CashAccPykIJ { get; set; }
        /// <summary>
        /// Cash Account Modal Kerja - Sale & Leaseback
        /// </summary>
        public string CashAccMoKSL { get; set; }
        /// <summary>
        /// Cash Account Modal Kerja - Modal Usaha
        /// </summary>
        public string CashAccMoKMU { get; set; }
        /// <summary>
        /// Cash Account Multiguna - Finance Lease
        /// </summary>
        public string CashAccMtgFL { get; set; }
        /// <summary>
        /// Cash Account Multiguna Installment Financing - Barang
        /// </summary>
        public string CashAccMtgIB { get; set; }
        /// <summary>
        /// Cash Account Multiguna Installment Financing - Jasa
        /// </summary>
        public string CashAccMtgIJ { get; set; }
        /// <summary>
        /// Cash Account Multiguna - Fasilitas Dana - Barang
        /// </summary>
        public string CashAccMtgDG { get; set; }
        /// <summary>
        /// Casg Account Multiguna - Fasilitas Dana - Jasa
        /// </summary>
        public string CashAccMtgDS { get; set; }
        /// <summary>
        /// Cash Account Multiguna - Respossessed
        /// </summary>
        public string CashAccRepossessed { get; set; }


        //[Display(Name = "Created By")]
        //public string EntryUser { get; set; }

        //[DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        //[Display(Name = "Created Date")]
        //public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }

        public Bank()
        {

        }

        public Bank(string bankCode)
        {
            BankCode = bankCode;
        }

        public static List<Bank> GetBank()
        {
            List<IDS.GeneralTable.Bank> list = new List<Bank>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBank";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@bankCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Bank bank = new Bank();
                            bank.BankCode = dr["BankCode"] as string;
                            bank.BankName = dr["BankName"] as string;
                            bank.Beneficiary = dr["Beneficiary"] as string;

                            bank.GelAccount = new IDS.GLTable.ChartOfAccount();
                            bank.GelAccount.Account = dr["GelAcc"] as string;

                            bank.BankCcy = new IDS.GeneralTable.Currency();
                            bank.BankCcy.CurrencyCode = dr["Ccy"] as string;

                            bank.CashAccLE = IDS.Tool.GeneralHelper.NullToString(dr["CashAccLE"]);
                            bank.CashAccCF = IDS.Tool.GeneralHelper.NullToString(dr["CashAccCF"]);
                            bank.CashAccJF = IDS.Tool.GeneralHelper.NullToString(dr["CashAccJF"]);
                            bank.CashAccFT = IDS.Tool.GeneralHelper.NullToString(dr["CashAccFT"]);
                            bank.CashAccPK = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPK"]);
                            bank.CashAccInvFL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccInvFL"]);
                            bank.CashAccInvSL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccInvSL"]);
                            bank.CashAccInvIB = IDS.Tool.GeneralHelper.NullToString(dr["CashAccInvIB"]);
                            bank.CashAccPykFL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykFL"]);
                            bank.CashAccPykSL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykSL"]);
                            bank.CashAccPykIB = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykIB"]);
                            bank.CashAccPykIJ = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykIJ"]);
                            bank.CashAccMoKSL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMoKSL"]);
                            bank.CashAccMoKMU = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMoKMU"]);
                            bank.CashAccMtgFL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgFL"]);
                            bank.CashAccMtgIB = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgIB"]);
                            bank.CashAccMtgIJ = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgIJ"]);
                            bank.CashAccMtgDG = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgDG"]);
                            bank.CashAccMtgDS = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgDS"]);
                            bank.CashAccRepossessed = IDS.Tool.GeneralHelper.NullToString(dr["CashAccRepossessed"]);

                            //bank.EntryUser = dr["EntryUser"] as string;
                            //bank.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            bank.OperatorID = dr["LogUser"] as string;
                            bank.LastUpdate = Convert.ToDateTime(dr["LogDate"]);

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

        public static Bank GetBank(string bankCode)
        {
            Bank bank = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBank";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@bankCode", System.Data.SqlDbType.VarChar, bankCode);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 2);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        bank = new Bank();
                        bank.BankCode = dr["BankCode"] as string;
                        bank.BankName = dr["BankName"] as string;
                        bank.Beneficiary = dr["Beneficiary"] as string;

                        bank.GelAccount = new IDS.GLTable.ChartOfAccount();
                        bank.GelAccount.Account = dr["GelAcc"] as string;

                        bank.BankCcy = new IDS.GeneralTable.Currency();
                        bank.BankCcy.CurrencyCode = dr["Ccy"] as string;

                        bank.CashAccLE = IDS.Tool.GeneralHelper.NullToString(dr["CashAccLE"]);
                        bank.CashAccCF = IDS.Tool.GeneralHelper.NullToString(dr["CashAccCF"]);
                        bank.CashAccJF = IDS.Tool.GeneralHelper.NullToString(dr["CashAccJF"]);
                        bank.CashAccFT = IDS.Tool.GeneralHelper.NullToString(dr["CashAccFT"]);
                        bank.CashAccPK = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPK"]);
                        bank.CashAccInvFL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccInvFL"]);
                        bank.CashAccInvSL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccInvSL"]);
                        bank.CashAccInvIB = IDS.Tool.GeneralHelper.NullToString(dr["CashAccInvIB"]);
                        bank.CashAccPykFL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykFL"]);
                        bank.CashAccPykSL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykSL"]);
                        bank.CashAccPykIB = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykIB"]);
                        bank.CashAccPykIJ = IDS.Tool.GeneralHelper.NullToString(dr["CashAccPykIJ"]);
                        bank.CashAccMoKSL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMoKSL"]);
                        bank.CashAccMoKMU = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMoKMU"]);
                        bank.CashAccMtgFL = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgFL"]);
                        bank.CashAccMtgIB = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgIB"]);
                        bank.CashAccMtgIJ = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgIJ"]);
                        bank.CashAccMtgDG = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgDG"]);
                        bank.CashAccMtgDS = IDS.Tool.GeneralHelper.NullToString(dr["CashAccMtgDS"]);
                        bank.CashAccRepossessed = IDS.Tool.GeneralHelper.NullToString(dr["CashAccRepossessed"]);
                        
                        //bank.EntryUser = dr["EntryUser"] as string;
                        //bank.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        bank.OperatorID = dr["LogUser"] as string;
                        bank.LastUpdate = Convert.ToDateTime(dr["LogDate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return bank;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetBankForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBank";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@bankCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem bank = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            bank.Value = IDS.Tool.GeneralHelper.NullToString(dr["BankCode"]);
                            bank.Text = IDS.Tool.GeneralHelper.NullToString(dr["BankName"]);                            

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

        public int InsUpDelBank(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from tblBANK Where BankCode ='" + BankCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTUpdtblBANK";
                    cmd.AddParameter("@type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@BankCode", System.Data.SqlDbType.VarChar, BankCode);
                    cmd.AddParameter("@BankName", System.Data.SqlDbType.VarChar, BankName);
                    cmd.AddParameter("@Ccy", System.Data.SqlDbType.VarChar, BankCcy.CurrencyCode);
                    cmd.AddParameter("@Beneficiary", System.Data.SqlDbType.VarChar, Beneficiary);
                    cmd.AddParameter("@GelAcc", System.Data.SqlDbType.VarChar, GelAccount.Account);
                    cmd.AddParameter("@LogUser", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@CashAccLE", System.Data.SqlDbType.VarChar, CashAccLE);
                    cmd.AddParameter("@CashAccCF", System.Data.SqlDbType.VarChar, CashAccCF);
                    cmd.AddParameter("@CashAccJF", System.Data.SqlDbType.VarChar, CashAccJF);
                    cmd.AddParameter("@CashAccFT", System.Data.SqlDbType.VarChar, CashAccFT);
                    cmd.AddParameter("@CashAccPK", System.Data.SqlDbType.VarChar, CashAccPK);
                    cmd.AddParameter("@CashAccInvFL", System.Data.SqlDbType.VarChar, CashAccInvFL);
                    cmd.AddParameter("@CashAccInvSL", System.Data.SqlDbType.VarChar, CashAccInvSL);
                    cmd.AddParameter("@CashAccInvIB", System.Data.SqlDbType.VarChar, CashAccInvIB);
                    cmd.AddParameter("@CashAccPykFL", System.Data.SqlDbType.VarChar, CashAccPykFL);
                    cmd.AddParameter("@CashAccPykSL", System.Data.SqlDbType.VarChar, CashAccPykSL);
                    cmd.AddParameter("@CashAccPykIB", System.Data.SqlDbType.VarChar, CashAccPykIB);
                    cmd.AddParameter("@CashAccPykIJ", System.Data.SqlDbType.VarChar, CashAccPykIJ);
                    cmd.AddParameter("@CashAccMoKSL", System.Data.SqlDbType.VarChar, CashAccMoKSL);
                    cmd.AddParameter("@CashAccMoKMU", System.Data.SqlDbType.VarChar, CashAccMoKMU);
                    cmd.AddParameter("@CashAccMtgFL", System.Data.SqlDbType.VarChar, CashAccMtgFL);
                    cmd.AddParameter("@CashAccMtgIB", System.Data.SqlDbType.VarChar, CashAccMtgIB);
                    cmd.AddParameter("@CashAccMtgIJ", System.Data.SqlDbType.VarChar, CashAccMtgIJ);
                    cmd.AddParameter("@CashAccMtgDG", System.Data.SqlDbType.VarChar, CashAccMtgDG);
                    cmd.AddParameter("@CashAccMtgDS", System.Data.SqlDbType.VarChar, CashAccMtgDS);
                    cmd.AddParameter("@CashAccRepossessed", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    {
                        newData = log.GenLogArray("SELECT * from tblBANK Where BankCode = '" + BankCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "tblBANK", BankCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "tblBANK", BankCode, OperatorID, "UPDATE");
                        }
                    }
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Bank Code is already exists. Please choose other Bank Code.");
                        default:
                            throw;
                    }
                }
                catch
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
                }
                finally
                {
                    cmd.Close();
                }
            }

            return result;
        }

        public int InsUpDelBank(int ExecCode, string[] data)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    oldData = log.GenLogArray("SELECT * from tblBANK Where BankCode ='" + BankCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTUpdtblBANK";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUpdtblBANK";
                        cmd.AddParameter("@type", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@BankCode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "tblBANK", BankCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Bank Code is already exists. Please choose other Bank Code.");
                        case 547:
                            throw new Exception("One or more data can not be delete while data used for reference.");
                        default:
                            throw;
                    }
                }
                catch
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
                }
                finally
                {
                    cmd.Close();
                }
            }

            return result;
        }
        public static DataTable GetBankDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBank";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@bankCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
