using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using IDS.Tool;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace IDS.GeneralTable
{
    public class Country
    {
        [Display(Name ="Country Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country code is required")]
        [MaxLength(3), StringLength(3)]
        public string CountryCode { get; set; }

        [Display(Name = "Country Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country name is required")]
        [MaxLength(30)]
        public string CountryName { get; set; }

        [Display(Name = "SLIK Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "SLIK Code is required")]
        public string SLIKCode { get; set; }

        [Display(Name = "Created User", ShortName = "Creator")]
        public string EntryUser { get; set; }

        [Display(Name = "Created Date", ShortName = "Created Date")]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "", DataFormatString = "dd/MMM/yyyy HH:mm:ss")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID", ShortName = "Opr. ID")]
        public string OperatorID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = "dd/MMM/yyyy HH:mm:ss")]        
        [Display(Name = "Last Upd.")]
        public DateTime LastUpdate { get; set; }

        private readonly Log? _log;

        public Country()
        {
        }

        public Country(string countryCode, string countryName)
        {
            CountryCode = countryCode;
            CountryName = countryName;
        }

        /// <summary>
        /// Retrieve Daftar Country berdasarkan kode country
        /// </summary>
        /// <param name="countryCode">Kode Country</param>
        /// <returns></returns>
        public static Country GetCountry(string countryCode)
        {
            Country country = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCountry";
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, countryCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();
                
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        country = new Country();
                        country.CountryCode = dr["CountryCode"] as string;
                        country.CountryName = dr["CountryName"] as string;
                        country.SLIKCode = dr["SLIKCode"] as string;
                       // country.EntryUser = dr["EntryUser"] as string;
                       // country.EntryDate = Convert.ToDateTime(dr["EntryDate"]);                                              
                        country.OperatorID = dr["OperatorID"] as string;
                        country.LastUpdate = Convert.ToDateTime(dr["LastUpd"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return country;
        }

        /// <summary>
        /// Retrieve semua daftar Country
        /// </summary>
        /// <returns></returns>
        public static List<Country> GetCountry()
        {
            List<IDS.GeneralTable.Country> list = new List<Country>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCountry";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<Country>();

                        while (dr.Read())
                        {
                            Country country = new Country();
                            country.CountryCode = dr["CountryCode"] as string;
                            country.CountryName = dr["CountryName"] as string;
                            //country.SLIKCode = dr["SLIKCode"] as string;
                            //country.EntryUser = dr["EntryUser"] as string;
                            //country.EntryDate = Convert.ToDateTime(dr["EntryDate"]);                            
                            country.OperatorID = dr["OperatorID"] as string;
                            country.LastUpdate = Convert.ToDateTime(dr["LastUpd"]);
                            country.SLIKCode = dr["SLIKCode"] as string;

                            list.Add(country);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCountryFromCityForDatasource(string cityCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> countries = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT co.CountryCode, co.CountryName FROM tblCountry co LEFT JOIN tblCity ci ON co.CountryCode = ci. CountryCode WHERE ci.cityCode = @cityCode";
                db.AddParameter("@cityCode", System.Data.SqlDbType.VarChar, cityCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem country = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            country.Value = dr["CountryCode"] as string;
                            country.Text = dr["CountryName"] as string;

                            countries.Add(country);
                        }
                    }
                }

                db.Close();
            }

            return countries;
        }

        /// <summary>
        /// Retrieve semua daftar country untuk datasource
        /// </summary>
        /// <returns></returns>
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCountryForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> countries = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCountry";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem country = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            country.Value = dr["CountryCode"] as string;
                            country.Text = dr["CountryName"] as string;
                            countries.Add(country);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return countries;
        }

        public int InsUpDel(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from tblCountry WHERE CountryCode ='" + CountryCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GLInsertUpdateCountry";
                    cmd.AddParameter("@ExecCode", System.Data.SqlDbType.Int, ExecCode);
                    cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, CountryCode.ToString());
                    cmd.AddParameter("@CountryName", System.Data.SqlDbType.VarChar, CountryName.ToString());
                    cmd.AddParameter("@LastUpd", System.Data.SqlDbType.DateTime, LastUpdate);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, HttpContextHelper.Current?.Session?.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID) ?? string.Empty);
                    cmd.AddParameter("@SLIKCode", System.Data.SqlDbType.VarChar, SLIKCode.ToString());
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * from tblCountry WHERE CountryCode ='" + CountryCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "tblCountry", CountryCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "tblCountry", CountryCode, OperatorID, "UPDATE");
                        }
                    }
                    else
                    {
                        log.SaveSysLog1(oldData, "", "tblCountry", CountryCode, OperatorID, "DELETE");
                    }
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Country code is already exists. Please choose other country code.");
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

        /// <summary>
        /// Untuk menghapus data country lebih dari satu kode
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int DeleteCountries(string[] data)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    //LOG
                    Log log = new Log();
                    string oldData = "";
                    oldData = log.GenLogArray("SELECT * from tblCountry WHERE CountryCode ='" + CountryCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    cmd.CommandText = "GLInsertUpdateCountry";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.AddParameter("@ExecCode", System.Data.SqlDbType.Int, IDS.Tool.PageActivity.Delete);
                        cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.AddParameter("@CountryName", System.Data.SqlDbType.VarChar, DBNull.Value);
                        cmd.AddParameter("@LastUpd", System.Data.SqlDbType.DateTime, DBNull.Value);
                        cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, HttpContextHelper.Current?.Session?.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID) ?? string.Empty);
                        //cmd.AddParameter("@SLIKCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "tblCountry", CountryCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Country code is already exists. Please choose other country code.");
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
        public static DataTable GetCountryDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCountry";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();


                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}