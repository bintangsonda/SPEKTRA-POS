using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using IDS.Tool;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;


namespace IDS.GeneralTable
{
    public class Currency
    {
        [Display(Name = "Currency Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Currency code is required")]
        [MaxLength(3), StringLength(3)]
        public string CurrencyCode { get; set; }

        [Display(Name = "Currency Name")]
        [MaxLength(20)]
        public string CurrencyName { get; set; }

        [Display(Name = "Country Name")]
        public Country CountryCurrency { get; set; }

        [Display(Name = "Decimal Places")]
        [Range(0,10)]
        public int DecimalPlaces { get; set; }

        [Display(Name = "Rounding Up")]
        public bool RoundingUp { get; set; }

        [Display(Name = "Multiply")]
        [Range(typeof(bool), "False", "True")]
        public bool MultiplyDivided { get; set; }

        [Display(Name = "Variance Limit")]
        [Range(0,100)]
        public decimal VarianceLimit { get; set; }

        [Display(Name = "Created By")]
        public string EntryUser { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Created Date")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "Rounding")]
        public int Rounding { get; set; }

        public Currency()
        {

        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCurrencyForApplicationDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCurrency";
                db.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem currency = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            currency.Value = dr["CurrencyCode"] as string;
                            currency.Text = dr["CurrencyName"] as string + " - " + currency.Value;

                            currencies.Add(currency);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return currencies;
        }
        public static decimal SetMidRateFormat(decimal midRate, string ccy)
        {
            if (!String.IsNullOrEmpty(ccy))
            {
                Currency Ccy = GetCurrency(ccy);
                if (Ccy.Rounding == 1)
                {
                    if (Ccy.RoundingUp == true)
                    {
                        midRate = Math.Ceiling(midRate);
                    }
                    else
                    {
                        midRate = Math.Round(midRate, Ccy.DecimalPlaces);
                    }
                }

            }



            return midRate;
        }
        public Currency(string currencyCode, string currencyName)
        {
            CurrencyCode = currencyCode;
            CurrencyName = currencyName;
        }

        public static Currency GetCurrency(string currencyCode)
        {
            Currency currency = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCurrency";
                db.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, currencyCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        currency = new Currency();
                        currency.CurrencyCode = dr["CurrencyCode"] as string;
                        currency.CurrencyName = dr["CurrencyName"] as string;
                        currency.CountryCurrency = IDS.GeneralTable.Country.GetCountry(dr["CountryCode"] as string);
                        currency.DecimalPlaces = Convert.ToInt16(dr["DecimalPlaces"]);
                        currency.RoundingUp = Convert.ToBoolean(dr["RoundingUp"]);
                        currency.MultiplyDivided = Convert.ToBoolean(dr["MultiplyDivided"]);
                        currency.VarianceLimit = Convert.ToDecimal(dr["VarianceLimit"]);
                        //currency.EntryUser = dr["EntryUser"] as string;
                        //currency.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        currency.OperatorID = dr["OperatorID"] as string;
                        currency.LastUpdate = Convert.ToDateTime(dr["LastUpD"]);
                        currency.Rounding = Convert.ToInt16(dr["Rounding"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return currency;
        }

        /// <summary>
        /// Retrieve semua daftar Country
        /// </summary>
        /// <returns></returns>
        public static List<Currency> GetCurrencyList()
        {
            List<IDS.GeneralTable.Currency> list = new List<Currency>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCurrency";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Currency currency = new Currency();
                            currency = new Currency();
                            currency.CurrencyCode = dr["CurrencyCode"] as string;
                            currency.CurrencyName = dr["CurrencyName"] as string;
                            currency.CountryCurrency = IDS.GeneralTable.Country.GetCountry(dr["CountryCode"] as string);
                            currency.DecimalPlaces = Convert.ToInt16(dr["DecimalPlaces"]);
                            currency.RoundingUp = Convert.ToBoolean(dr["RoundingUp"]);
                            currency.MultiplyDivided = Convert.ToBoolean(dr["MultiplyDivided"]);
                            currency.VarianceLimit = Convert.ToDecimal(dr["VarianceLimit"]);
                            //currency.EntryUser = dr["EntryUser"] as string;
                            //currency.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            currency.OperatorID = dr["OperatorID"] as string;
                            currency.LastUpdate = Convert.ToDateTime(dr["LastUpd"]);
                            currency.Rounding = Convert.ToInt16(dr["Rounding"]);

                            list.Add(currency);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        
        public static string GetbaseCCY()
        {
            string hasil = "";
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select BaseCcy from SYSPAR";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            hasil = Tool.GeneralHelper.NullToString(dr["BaseCcy"]);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return hasil;
        }
        

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCurrencyNotBaseCcy()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblCurrency WHERE CurrencyCode NOT IN(SELECT BASECCY FROM SYSPAR)";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem currency = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            currency.Value = Tool.GeneralHelper.NullToString(dr["CurrencyCode"]);
                            currency.Text = Tool.GeneralHelper.NullToString(dr["CurrencyCode"]);

                            currencies.Add(currency);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return currencies;
        }


        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCurrencyForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCurrency";
                db.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem currency = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            currency.Value = dr["CurrencyCode"] as string;
                            currency.Text = dr["CurrencyCode"] as string;

                            currencies.Add(currency);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return currencies;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCurrencyForDataSource(string currencyCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCurrency";
                db.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, currencyCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 4);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem currency = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            currency.Value = dr["CurrencyCode"] as string;
                            currency.Text = dr["CurrencyCode"] as string;

                            currencies.Add(currency);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return currencies;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCurrencyBaseOnChartOfAccountForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCurrency";
                db.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 5);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        currencies = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem currency = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            currency.Value = Tool.GeneralHelper.NullToString(dr["CCY"]);
                            currency.Text = Tool.GeneralHelper.NullToString(dr["CCY"]);

                            currencies.Add(currency);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return currencies;
        }

        public int InsUpDelCurrency(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from tblCurrency Where CurrencyCode = '" + CurrencyCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    cmd.CommandText = "GTUpdateCurrency";
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, CurrencyCode.ToString());
                    cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, CountryCurrency.CountryCode.ToString());
                    cmd.AddParameter("@CurrencyName", System.Data.SqlDbType.VarChar, CurrencyName.ToString());
                    cmd.AddParameter("@DecimalPlaces", System.Data.SqlDbType.Money, DecimalPlaces);
                    cmd.AddParameter("@RoundingUp", System.Data.SqlDbType.Bit, RoundingUp);
                    cmd.AddParameter("@Rounding", System.Data.SqlDbType.Int, Rounding);
                    cmd.AddParameter("@MultiplyDivided", System.Data.SqlDbType.TinyInt, MultiplyDivided);
                    cmd.AddParameter("@VarianceLimit", System.Data.SqlDbType.Float, VarianceLimit);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID.ToString());
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * from tblCurrency Where CurrencyCode =  '" + CurrencyCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "tblCurrency", CurrencyCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "tblCurrency", CurrencyCode, OperatorID, "UPDATE");
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
                            throw new Exception("Currency code is already exists. Please choose other Currency code.");
                        default:
                            throw;
                    }
                }
                catch (Exception ex)
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

        public int InsUpDelCurrency(int ExecCode, string[] data)
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
                   
                    oldData = log.GenLogArray("SELECT * from tblCurrency Where CurrencyCode = '" + CurrencyCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTUpdateCurrency";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUpdateCurrency";
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "tblCurrency", CurrencyCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Currency Code is already exists. Please choose other Currency Code.");
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
        public static DataTable GetCurrencyDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCurrency";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@CurrencyCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        //Edited By Renaldi 2 October 2024
        public static List<Currency> GetCurrencyListVariance()
        {
            List<IDS.GeneralTable.Currency> list = new List<Currency>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT CurrencyCode,VarianceLimit FROM tblCurrency";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Currency currency = new Currency();
                            currency = new Currency();
                            currency.CurrencyCode = dr["CurrencyCode"] as string;
                            currency.VarianceLimit = Convert.ToDecimal(dr["VarianceLimit"]);

                            list.Add(currency);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        //End Edited
    }
}
