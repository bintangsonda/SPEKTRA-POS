using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class Customer
    {
        [Display(Name = "Customer Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Customer code is required")]
        [MaxLength(4), StringLength(4)]
        public string CUSTCode { get; set; }

        [Display(Name = "Customer Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Customer name is required")]
        [MaxLength(50), StringLength(50)]
        public string CUSTName { get; set; }

        [Display(Name = "Contact Person")]
        [MaxLength(50), StringLength(50)]
        public string ContactPerson { get; set; }

        [Display(Name = "Address")]
        [MaxLength(100), StringLength(100)]
        public string Address1 { get; set; }

        [Display(Name = "Country")]
        //[MaxLength(3), StringLength(3)]
        public IDS.GeneralTable.Country CustCountry { get; set; }

        [Display(Name = "City")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GeneralTable.City CustCity { get; set; }

        [Display(Name = "Phone")]
        [MaxLength(20), StringLength(20)]
        public string Phone { get; set; }

        [Display(Name = "FAX")]
        [MaxLength(20), StringLength(20)]
        public string Fax { get; set; }

        [Display(Name = "NPWP")]
        [MaxLength(30), StringLength(30)]
        public string NPWP { get; set; }

        [Display(Name = "NPPKP")]
        [MaxLength(20), StringLength(20)]
        public string NPPKP { get; set; }

        [Display(Name = "Charts of Account Customer")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GLTable.ChartOfAccount CustAcc { get; set; }

        [Display(Name = "Beneficiary Name")]
        [MaxLength(50), StringLength(50)]
        public string BenName { get; set; }

        [Display(Name = "Beneficiary Address 1")]
        [MaxLength(50), StringLength(50)]
        public string BenAddress1 { get; set; }

        [Display(Name = "Beneficiary Address 2")]
        [MaxLength(50), StringLength(50)]
        public string BenAddress2 { get; set; }

        [Display(Name = "Beneficiary Bank")]
        [MaxLength(20), StringLength(20)]
        public string BenBank { get; set; }

        [Display(Name = "Acc No Beneficiary Bank")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GLTable.ChartOfAccount BenBankAcc { get; set; }

        [Display(Name = "Beneficiary Bank Address 1")]
        [MaxLength(50), StringLength(50)]
        public string BenBankAddress1 { get; set; }

        [Display(Name = "Beneficiary Bank Address 2")]
        [MaxLength(50), StringLength(50)]
        public string BenBankAddress2 { get; set; }

        [Display(Name = "Bill Name")]
        [MaxLength(100), StringLength(100)]
        public string BillName { get; set; }

        [Display(Name = "Bill Address")]
        [MaxLength(300), StringLength(300)]
        public string BillAdd { get; set; }

        [Display(Name = "Bill Country")]
        //[MaxLength(3), StringLength(3)]
        public IDS.GeneralTable.Country BillCountry { get; set; }

        [Display(Name = "Bill City")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GeneralTable.City BillCity { get; set; }

        [Display(Name = "Tax Name")]
        [MaxLength(100), StringLength(100)]
        public string TaxName { get; set; }

        [Display(Name = "Tax Address")]
        [MaxLength(300), StringLength(300)]
        public string TaxAdd { get; set; }

        [Display(Name = "Tax Country")]
        //[MaxLength(3), StringLength(3)]
        public IDS.GeneralTable.Country TaxCountry { get; set; }

        [Display(Name = "Tax City")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GeneralTable.City TaxCity { get; set; }

        [Display(Name = "Acc")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GLTable.ChartOfAccount Acc { get; set; }

        [Display(Name = "Currency")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Currency is required")]
        public IDS.GeneralTable.Currency CCy { get; set; }

        [Display(Name = "Type")]
        public int Type { get; set; }

        [Display(Name = "Active / Passive")]
        public bool ActivePassive { get; set; }

        [Display(Name = "Flag Group")]
        public bool FlagGroup { get; set; }

        [Display(Name = "VAT Acc")]
        public IDS.GLTable.ChartOfAccount VATAcc { get; set; }

        [Display(Name = "Sales Acc")]
        public IDS.GLTable.ChartOfAccount SalesAcc { get; set; }

        [Display(Name = "Outstanding")]
        public decimal Outstanding { get; set; }

        [Display(Name = "Gov Private")]
        public bool GovPrivate { get; set; }

        [Display(Name = "Customer Type")]
        [MaxLength(7), StringLength(7)]
        public string CustType { get; set; }

        [Display(Name = "BUMN Tax")]
        public IDS.GLTable.ChartOfAccount BUMNTax { get; set; }

        [Display(Name = "Created By")]
        public string EntryUser { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Created Date")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }

        [Display(Name = "Last Update")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        public DateTime LastUpdate { get; set; }

        public Customer()
        {
        }

        public Customer(string custCode, string custName)
        {
            CUSTCode = custCode;
            CUSTName = custName;
        }

        public static List<Customer> GetCustomer()
        {
            List<IDS.GeneralTable.Customer> list = new List<Customer>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLSelACFCUST";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Customer cust = new Customer();
                            cust.CUSTCode = dr["CUST"] as string;
                            cust.CUSTName = dr["NAME"] as string;
                            cust.ContactPerson = dr["ContactPerson"] as string;
                            cust.Address1 = dr["ADDR_1"] as string;

                            cust.CustCountry = new Country();
                            cust.CustCountry.CountryCode = Tool.GeneralHelper.NullToString(dr["Country"]);
                            cust.CustCountry.CountryName = Tool.GeneralHelper.NullToString(dr["CountryName"]);

                            cust.CustCity = new City();
                            cust.CustCity.CityCode = Tool.GeneralHelper.NullToString(dr["City"]);
                            cust.CustCity.CityName = dr["CityName"] as string;
                            //cust.CustCity.BICode = dr["BICode"] as string;
                            //cust.CustCity.OJKCode = dr["OJKCode"] as string;
                            //cust.CustCity.SLIKCode = dr["SLIKCode"] as string;
                            //cust.CustCity.Remark = dr["Remark"] as string;
                            cust.CustCity.Country = cust.CustCountry;

                            cust.Phone = dr["PHONE"] as string;
                            cust.Fax = dr["FAX"] as string;

                            cust.CustAcc = new GLTable.ChartOfAccount();
                            cust.CustAcc.Account = Tool.GeneralHelper.NullToString(dr["CustAcc"]);

                            cust.BenName = dr["BENNAME"] as string;
                            cust.BenAddress1 = dr["BENADD1"] as string;
                            cust.BenAddress2 = dr["BENADD2"] as string;
                            cust.BenBank = dr["BENBANK"] as string;

                            cust.BenBankAcc = new GLTable.ChartOfAccount();
                            cust.BenBankAcc.Account = Tool.GeneralHelper.NullToString(dr["BENBANKACC"]);

                            cust.BenBankAddress1 = dr["BENBANKADD1"] as string;
                            cust.BenBankAddress2 = dr["BENBANKADD2"] as string;

                            cust.Acc = new GLTable.ChartOfAccount();
                            cust.Acc.Account = Tool.GeneralHelper.NullToString(dr["ACC"]);

                            cust.CCy = new GeneralTable.Currency();
                            cust.CCy.CurrencyCode = Tool.GeneralHelper.NullToString(dr["CCY"]);

                            cust.Type = Tool.GeneralHelper.NullToInt(dr["Type"], 0);
                            cust.EntryUser = dr["EntryUser"] as string;
                            cust.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            cust.OperatorID = dr["OperatorID"] as string;
                            cust.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            list.Add(cust);
                        }

                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static Customer GetCustomer(string custCode)
        {
            Customer cust = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLSelACFCUST";
                db.AddParameter("@CUSTCODE", System.Data.SqlDbType.VarChar, custCode);
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        cust = new Customer();
                        cust.CUSTCode = dr["CUST"] as string;
                        cust.CustType = dr["CustType"] as string;
                        cust.CUSTName = dr["NAME"] as string;
                        cust.ContactPerson = dr["ContactPerson"] as string;
                        cust.Address1 = dr["ADDR_1"] as string;

                        cust.CustCountry = new Country();
                        cust.CustCountry.CountryCode = Tool.GeneralHelper.NullToString(dr["Country"]);
                        //cust.CustCountry.CountryName = Tool.GeneralHelper.NullToString(dr["CountryName"]);

                        cust.CustCity = new City();
                        cust.CustCity.CityCode = Tool.GeneralHelper.NullToString(dr["City"]);
                        //cust.CustCity.CityName = dr["CityName"] as string;
                        //cust.CustCity.BICode = dr["BICode"] as string;
                        //cust.CustCity.OJKCode = dr["OJKCode"] as string;
                        //cust.CustCity.SLIKCode = dr["SLIKCode"] as string;
                        //cust.CustCity.Remark = dr["Remark"] as string;
                        cust.CustCity.Country = cust.CustCountry;

                        cust.Phone = dr["PHONE"] as string;
                        cust.Fax = dr["FAX"] as string;
                        cust.NPWP = dr["NPWP"] as string;
                        cust.NPPKP = dr["NPPKP"] as string;

                        cust.CustAcc = new GLTable.ChartOfAccount();
                        cust.CustAcc.Account = Tool.GeneralHelper.NullToString(dr["CustAcc"]);

                        cust.SalesAcc = new GLTable.ChartOfAccount();
                        cust.SalesAcc.Account = Tool.GeneralHelper.NullToString(dr["SalesAcc"]);

                        cust.VATAcc = new GLTable.ChartOfAccount();
                        cust.VATAcc.Account = Tool.GeneralHelper.NullToString(dr["VATAcc"]);

                        cust.BUMNTax = new GLTable.ChartOfAccount();
                        cust.BUMNTax.Account = Tool.GeneralHelper.NullToString(dr["BUMNTax"]);

                        cust.Outstanding = Convert.ToDecimal(dr["outstanding"]);
                        cust.BenName = dr["BENNAME"] as string;
                        cust.BenAddress1 = dr["BENADD1"] as string;
                        cust.BenAddress2 = dr["BENADD2"] as string;
                        cust.BenBank = dr["BENBANK"] as string;

                        cust.BenBankAcc = new GLTable.ChartOfAccount();
                        cust.BenBankAcc.Account = Tool.GeneralHelper.NullToString(dr["BENBANKACC"]);

                        cust.BenBankAddress1 = dr["BENBANKADD1"] as string;
                        cust.BenBankAddress2 = dr["BENBANKADD2"] as string;

                        cust.Acc = new GLTable.ChartOfAccount();
                        cust.Acc.Account = Tool.GeneralHelper.NullToString(dr["ACC"]);

                        cust.CCy = new GeneralTable.Currency();
                        cust.CCy.CurrencyCode = Tool.GeneralHelper.NullToString(dr["CCY"]);

                        cust.Type = Tool.GeneralHelper.NullToInt(dr["Type"], 0);
                        cust.ActivePassive = Tool.GeneralHelper.NullToBool(dr["ActivePasive"], false);
                        cust.FlagGroup = Tool.GeneralHelper.NullToBool(dr["FlagGroup"], false);
                        cust.EntryUser = dr["EntryUser"] as string;
                        cust.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        cust.OperatorID = dr["OperatorID"] as string;
                        cust.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return cust;
        }

        public int InsUpDelCustomer(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.CommandText = "GTUpdateCustomer";
                    cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@CUSTCODE", System.Data.SqlDbType.VarChar, CUSTCode);
                    cmd.AddParameter("@CustType", System.Data.SqlDbType.Bit, Convert.ToBoolean(Convert.ToInt16(CustType)));
                    cmd.AddParameter("@CUSTNAME", System.Data.SqlDbType.VarChar, CUSTName);
                    cmd.AddParameter("@CONTACT", System.Data.SqlDbType.VarChar, ContactPerson);
                    cmd.AddParameter("@ADD1", System.Data.SqlDbType.VarChar, Address1);
                    cmd.AddParameter("@CITY", System.Data.SqlDbType.VarChar, CustCity.CityCode);
                    cmd.AddParameter("@COUNTRY", System.Data.SqlDbType.VarChar, CustCountry.CountryCode);
                    cmd.AddParameter("@MOBILE", System.Data.SqlDbType.VarChar, Phone);
                    cmd.AddParameter("@FAX", System.Data.SqlDbType.VarChar, Fax);
                    cmd.AddParameter("@NPWP", System.Data.SqlDbType.VarChar, NPWP);
                    cmd.AddParameter("@NPPKP", System.Data.SqlDbType.VarChar, NPPKP);
                    //cmd.AddParameter("@IndusCode", System.Data.SqlDbType.VarChar, IndustryCust.IndusCode);
                    cmd.AddParameter("@CustAcc", System.Data.SqlDbType.VarChar, CustAcc.Account);
                    cmd.AddParameter("@SlsAcc", System.Data.SqlDbType.VarChar, SalesAcc.Account);
                    cmd.AddParameter("@VATAcc", System.Data.SqlDbType.VarChar, VATAcc.Account);
                    cmd.AddParameter("@BUMNAcc", System.Data.SqlDbType.VarChar, BUMNTax.Account);
                    cmd.AddParameter("@Outstanding", System.Data.SqlDbType.Money, Outstanding);
                    cmd.AddParameter("@BenName", System.Data.SqlDbType.VarChar, BenName);
                    cmd.AddParameter("@BenAddr1", System.Data.SqlDbType.VarChar, BenAddress1);
                    cmd.AddParameter("@BenAddr2", System.Data.SqlDbType.VarChar, BenAddress2);
                    cmd.AddParameter("@BenBank", System.Data.SqlDbType.VarChar, BenBank);
                    cmd.AddParameter("@BenBankAcc", System.Data.SqlDbType.VarChar, BenBankAcc.Account);
                    cmd.AddParameter("@BenBankAdd1", System.Data.SqlDbType.VarChar, BenBankAddress1);
                    cmd.AddParameter("@BenBankAdd2", System.Data.SqlDbType.VarChar, BenBankAddress2);
                    //cmd.AddParameter("@Acc", System.Data.SqlDbType.VarChar, IDS.Tool.GeneralHelper.StringToDBNull(Acc.Account));
                    //cmd.AddParameter("@Ccy", System.Data.SqlDbType.VarChar, IDS.Tool.GeneralHelper.StringToDBNull(CCy.CurrencyCode));
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, Type);
                    cmd.AddParameter("@ActivePassive", System.Data.SqlDbType.Bit, Convert.ToBoolean(Convert.ToInt16(ActivePassive)));
                    cmd.AddParameter("@FlagGroup", System.Data.SqlDbType.Bit, Convert.ToBoolean(Convert.ToInt16(FlagGroup)));
                    if (!string.IsNullOrEmpty(EntryUser))
                    {
                        cmd.AddParameter("@EntryUser", System.Data.SqlDbType.VarChar, EntryUser);
                    }
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Customer code is already exists. Please choose other customer code.");
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

        public int InsUpDelCustomer(Tool.PageActivity ExecCode, string[] data)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.CommandText = "GTUpdateCustomer";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUpdateCustomer";
                        cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, (int)ExecCode);
                        cmd.AddParameter("@CUSTCODE", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Customer Code is already exists. Please choose other Customer Code.");
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

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetACFCUSTForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLSelACFCUST";
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem acfcust = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            acfcust.Value = IDS.Tool.GeneralHelper.NullToString(dr["cust"]);
                            acfcust.Text = IDS.Tool.GeneralHelper.NullToString(dr["NAME"]);

                            list.Add(acfcust);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }
    }
}
