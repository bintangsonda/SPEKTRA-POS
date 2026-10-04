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
    public class Supplier
    {
        [Display(Name = "Sup Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Supplier code is required")]
        [MaxLength(4), StringLength(4)]
        public string SupCode { get; set; }

        [Display(Name = "Sup Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Supplier name is required")]
        [MaxLength(50), StringLength(50)]
        public string SupName { get; set; }

        [Display(Name = "Contact Person")]
        [MaxLength(50), StringLength(50)]
        public string ContactPerson { get; set; }

        [Display(Name = "Address")]
        [MaxLength(100), StringLength(100)]
        public string Address1 { get; set; }

        [Display(Name = "Country")]
        //[MaxLength(3), StringLength(3)]
        public IDS.GeneralTable.Country SupCountry { get; set; }

        [Display(Name = "City")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GeneralTable.City SupCity { get; set; }

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

        [Display(Name = "Charts of Account Supplier")]
        //[MaxLength(10), StringLength(10)]
        public IDS.GLTable.ChartOfAccount SupAcc { get; set; }

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
        //public IDS.GLTable.ChartOfAccount BenBankAcc { get; set; }
        public string BenBankAcc { get; set; }


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
        [MaxLength(15), StringLength(15)]
        public IDS.GLTable.ChartOfAccount VATAcc { get; set; }

        [Display(Name = "Sales Acc")]
        [MaxLength(15), StringLength(15)]
        public string SalesAcc { get; set; }

        [Display(Name = "Outstanding")]
        public decimal Outstanding { get; set; }

        [Display(Name = "Gov Private")]
        public bool GovPrivate { get; set; }

        [Display(Name = "Customer Type")]
        [MaxLength(7), StringLength(7)]
        public string CustType { get; set; }

        [Display(Name = "BUMN Tax")]
        [MaxLength(12), StringLength(12)]
        public string BUMNTax { get; set; }

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

        public Supplier()
        {
        }

        public Supplier(string supCode, string supName)
        {
            SupCode = supCode;
            SupName = supName;
        }

        public static List<Supplier> GetSupplier()
        {
            List<IDS.GeneralTable.Supplier> list = new List<Supplier>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLSelACFVEND";
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
                            Supplier sup = new Supplier();
                            sup.SupCode = dr["VEND"] as string;
                            sup.SupName = dr["NAME"] as string;
                            sup.ContactPerson = dr["ContactPerson"] as string;
                            sup.Address1 = dr["ADDR_1"] as string;

                            sup.SupCountry = new Country();
                            sup.SupCountry.CountryCode = Tool.GeneralHelper.NullToString(dr["CountryCode"]);
                            sup.SupCountry.CountryName = Tool.GeneralHelper.NullToString(dr["CountryName"]);

                            sup.SupCity = new City();
                            sup.SupCity.CityCode = Tool.GeneralHelper.NullToString(dr["CityCode"]);
                            sup.SupCity.CityName = dr["CityName"] as string;
                            //Sup.SupCity.BICode = dr["BICode"] as string;
                            //Sup.SupCity.OJKCode = dr["OJKCode"] as string;
                            //Sup.SupCity.SLIKCode = dr["SLIKCode"] as string;
                            //Sup.SupCity.Remark = dr["Remark"] as string;
                            sup.SupCity.Country = sup.SupCountry;

                            sup.Phone = dr["PHONE"] as string;
                            sup.Fax = dr["FAX"] as string;
                            sup.NPWP = dr["NPWP"] as string;
                            sup.NPPKP = dr["NPPKP"] as string;
                            
                            sup.SupAcc = new GLTable.ChartOfAccount();
                            sup.SupAcc.Account = Tool.GeneralHelper.NullToString(dr["SupAcc"]);

                            sup.Acc = new GLTable.ChartOfAccount();
                            sup.Acc.Account = Tool.GeneralHelper.NullToString(dr["ACC"]);

                            sup.CCy = new GeneralTable.Currency();
                            sup.CCy.CurrencyCode = Tool.GeneralHelper.NullToString(dr["CCY"]);

                            sup.Type = Tool.GeneralHelper.NullToInt(dr["Type"], 0);
                            sup.EntryUser = dr["EntryUser"] as string;
                            sup.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            sup.OperatorID = dr["OperatorID"] as string;
                            sup.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            list.Add(sup);
                        }

                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static Supplier GetSupplier(string supCode)
        {
            Supplier sup = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLSelACFVEND";
                db.AddParameter("@VENDCODE", System.Data.SqlDbType.VarChar, supCode);
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        sup = new Supplier();
                        sup.SupCode = dr["VEND"] as string;
                        sup.SupName = dr["NAME"] as string;
                        sup.ContactPerson = dr["ContactPerson"] as string;
                        sup.Address1 = dr["ADDR_1"] as string;

                        sup.SupCountry = new Country();
                        sup.SupCountry.CountryCode = Tool.GeneralHelper.NullToString(dr["CountryCode"]);
                        //sup.supCountry.CountryName = Tool.GeneralHelper.NullToString(dr["CountryName"]);

                        sup.SupCity = new City();
                        sup.SupCity.CityCode = Tool.GeneralHelper.NullToString(dr["CityCode"]);
                        //sup.supCity.CityName = dr["CityName"] as string;
                        //sup.supCity.BICode = dr["BICode"] as string;
                        //sup.supCity.OJKCode = dr["OJKCode"] as string;
                        //sup.supCity.SLIKCode = dr["SLIKCode"] as string;
                        //sup.supCity.Remark = dr["Remark"] as string;
                        sup.SupCity.Country = sup.SupCountry;

                        sup.Phone = dr["PHONE"] as string;
                        sup.Fax = dr["FAX"] as string;
                        sup.NPWP = dr["NPWP"] as string;
                        sup.NPPKP = dr["NPPKP"] as string;
                        
                        sup.SupAcc = new GLTable.ChartOfAccount();
                        sup.SupAcc.Account = Tool.GeneralHelper.NullToString(dr["SupAcc"]);

                        sup.Acc = new GLTable.ChartOfAccount();
                        sup.Acc.Account = Tool.GeneralHelper.NullToString(dr["ACC"]);

                        sup.CCy = new GeneralTable.Currency();
                        sup.CCy.CurrencyCode = Tool.GeneralHelper.NullToString(dr["CCY"]);

                        sup.Type = Tool.GeneralHelper.NullToInt(dr["Type"], 0);
                        sup.EntryUser = dr["EntryUser"] as string;
                        sup.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        sup.OperatorID = dr["OperatorID"] as string;
                        sup.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return sup;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetSupplierForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GLSelACFVEND";
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
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            item.Value = dr["VEND"] as string;
                            item.Text = dr["NAME"] as string;

                            list.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public int InsUpDelSupplier(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.CommandText = "GTUpdateSupplier";
                    cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@VENDCODE", System.Data.SqlDbType.VarChar, SupCode);
                    cmd.AddParameter("@SUPNAME", System.Data.SqlDbType.VarChar, SupName);
                    cmd.AddParameter("@CONTACT", System.Data.SqlDbType.VarChar, ContactPerson);
                    cmd.AddParameter("@ADD1", System.Data.SqlDbType.VarChar, Address1);
                    cmd.AddParameter("@CITY", System.Data.SqlDbType.VarChar, SupCity.CityCode);
                    cmd.AddParameter("@COUNTRY", System.Data.SqlDbType.VarChar, SupCountry.CountryCode);
                    cmd.AddParameter("@MOBILE", System.Data.SqlDbType.VarChar, Phone);
                    cmd.AddParameter("@FAX", System.Data.SqlDbType.VarChar, Fax);
                    cmd.AddParameter("@NPWP", System.Data.SqlDbType.VarChar, NPWP);
                    cmd.AddParameter("@NPPKP", System.Data.SqlDbType.VarChar, NPPKP);
                    cmd.AddParameter("@SupAcc", System.Data.SqlDbType.VarChar, SupAcc.Account);
                    //cmd.AddParameter("@Acc", System.Data.SqlDbType.VarChar, IDS.Tool.GeneralHelper.StringToDBNull(Acc.Account));
                    //cmd.AddParameter("@Ccy", System.Data.SqlDbType.VarChar, IDS.Tool.GeneralHelper.StringToDBNull(CCy.CurrencyCode));
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, Type);
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
                            throw new Exception("Supplier code is already exists. Please choose other Supplier code.");
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

        public int InsUpDelSupplier(Tool.PageActivity ExecCode, string[] data)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.CommandText = "GTUpdateSupplier";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUpdateSupplier";
                        cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, (int)ExecCode);
                        cmd.AddParameter("@VENDCODE", System.Data.SqlDbType.VarChar, data[i]);
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
                            throw new Exception("Supplier Code is already exists. Please choose other Supplier Code.");
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
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetACFVENDForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelACFVEND";
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 6);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem acfvend = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            acfvend.Value = IDS.Tool.GeneralHelper.NullToString(dr["VEND"]);
                            acfvend.Text = acfvend.Value + " - " + IDS.Tool.GeneralHelper.NullToString(dr["NAME"]);

                            list.Add(acfvend);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetACFVENDForDataSourceWithEmpty()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelACFVEND";
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 6);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem acfvend = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            acfvend.Value = IDS.Tool.GeneralHelper.NullToString(dr["VEND"]);
                            acfvend.Text = acfvend.Value + " - " + IDS.Tool.GeneralHelper.NullToString(dr["NAME"]);

                            list.Add(acfvend);
                        }
                    }
                }

                db.Close();
            }

            list.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "", Value = "" });

            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetACFVENDForDataSource(string vend)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelACFVEND";
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 5);
                db.AddParameter("@VENDCODE", System.Data.SqlDbType.VarChar, vend);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem acfvend = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            acfvend.Value = IDS.Tool.GeneralHelper.NullToString(dr["SUPACC"]);
                            acfvend.Text = IDS.Tool.GeneralHelper.NullToString(dr["NAME"]);

                            list.Add(acfvend);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetACFVENDForDataSource(bool withAll)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelACFVEND";
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 6);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem acfvend = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            acfvend.Value = IDS.Tool.GeneralHelper.NullToString(dr["VEND"]);
                            acfvend.Text = IDS.Tool.GeneralHelper.NullToString(dr["NAME"]);

                            list.Add(acfvend);
                        }
                    }
                }

                db.Close();
            }
            if (withAll)
            {
                list.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "All", Value = "" });
            }

            return list;
        }
    }
}
