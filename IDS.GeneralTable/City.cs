using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Linq;
using IDS.Tool;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace IDS.GeneralTable
{
    public class City : Tool.ILastLog
    {
        [Display(Name = "City Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City code is required")]
        [MaxLength(10), StringLength(10)]
        public string CityCode { get; set; }

        [Display(Name = "City Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City name is required")]
        [MaxLength(50)]
        public string CityName { get; set; }

        [Display(Name = "Country Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required")]
        public Country Country { get; set; }
        
        [Display(Name = "Created By")]
        [MaxLength(20), StringLength(20)]
        public string EntryUser { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        [MaxLength(20), StringLength(20)]
        public string OperatorID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }

        public City()
        {
            this.Country = new Country();
        }

        public City(string cityCode, string cityName) : this()
        {
            CityCode = cityCode;
            CityName = cityName;
        }
        
        /// <summary>
        /// Retrieve semua daftar City
        /// </summary>
        /// <returns></returns>
        public static List<City> GetCity()
        {
            List<IDS.GeneralTable.City> list = new List<City>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCity";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<City>();

                        while (dr.Read())
                        {
                            City city = new City();
                            city.CityCode = IDS.Tool.GeneralHelper.NullToString(dr["CityCode"]);
                            city.CityName = IDS.Tool.GeneralHelper.NullToString(dr["CityName"]);
                            city.Country = new Country(IDS.Tool.GeneralHelper.NullToString(dr["CountryCode"]), IDS.Tool.GeneralHelper.NullToString(dr["CountryName"]));
                            city.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["LogUser"]);
                            city.LastUpdate = Convert.ToDateTime(dr["LogDate"]);

                            list.Add(city);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            if (list.Count > 0)
                list = list.OrderBy(x => x.Country.CountryName).ThenBy(x => x.CityName).ToList();

            return list;
        }

        /// <summary>
        /// Retrieve City berdasarkan kode parameter City
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        public static List<City> GetCity(string cityCode)
        {
            List<City> list = new List<City>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCity";
                db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, cityCode);
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while(dr.Read())
                        {
                            City city = new City();
                            city.CityCode = IDS.Tool.GeneralHelper.NullToString(dr["CityCode"]);
                            city.CityName = IDS.Tool.GeneralHelper.NullToString(dr["CityName"]);
                            city.Country = new Country(IDS.Tool.GeneralHelper.NullToString(dr["CountryCode"]), IDS.Tool.GeneralHelper.NullToString(dr["CountryName"]));
                            city.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            city.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            list.Add(city);
                        }
                        
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            if (list.Count > 0)
                list = list.OrderBy(x => x.Country.CountryName).ThenBy(x => x.CityName).ToList();

            return list;
        }

        /// <summary>
        /// Retrieve kota berdasarkan kode City dan kode Country
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public static City GetCity(string cityCode, string countryCode)
        {
            City city = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCity";
                db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, cityCode);
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, countryCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 4);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            city = new City();
                            city.CityCode = IDS.Tool.GeneralHelper.NullToString(dr["CityCode"]);
                            city.CityName = IDS.Tool.GeneralHelper.NullToString(dr["CityName"]);
                            city.Country = new Country(IDS.Tool.GeneralHelper.NullToString(dr["CountryCode"]), IDS.Tool.GeneralHelper.NullToString(dr["CountryName"]));
                            city.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["LogUser"]);
                            city.LastUpdate = Convert.ToDateTime(dr["LogDate"]);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return city;
        }

        /// <summary>
        /// Retrieve daftar kota untuk datasource
        /// </summary>
        /// <returns></returns>
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCityForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCity";
                db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 6);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            SelectListItem city = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            city.Value = dr["CityCode"] as string;
                            city.Text = dr["CityName"] as string;

                            cities.Add(city);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return cities;
        }

        /// <summary>
        /// Retrieve daftar kota dari suatu negara
        /// </summary>
        /// <param name="countryCode">Kode negara</param>
        /// <returns></returns>
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCityForDatasource(string countryCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCity";
                db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, countryCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 7);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            SelectListItem city = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            city.Value = dr["CityCode"] as string;
                            city.Text = dr["CityName"] as string;

                            cities.Add(city);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return cities;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCityForCountry(string countryCode,string CityCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCity";
                db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, CityCode);
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, countryCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 7);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            SelectListItem city = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            city.Value = dr["CityCode"] as string;
                            city.Text = dr["CityName"] as string;

                            cities.Add(city);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return cities;
        }
        /// <summary>
        /// Untuk Insert, Update, Delete Data
        /// </summary>
        /// <param name="ExecCode"></param>
        /// <returns></returns>
        public int InsUpDelCity(Tool.PageActivity ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from tblCity WHERE CityCode ='" + CityCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTCity";
                    cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, Country.CountryCode);
                    cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, CityCode);
                    cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, CityName);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID.ToString());
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, (int)ExecCode);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if ((int)ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * from tblCity WHERE CityCode = '" + CityCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if ((int)ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "tblCity", CityCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "tblCity", CityCode, OperatorID, "UPDATE");
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
                            throw new Exception("City code is already exists. Please choose other city code.");
                        case 547:
                            throw new Exception("Data can not be delete while data used for reference.");
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
        /// Untuk delete data
        /// </summary>
        /// <param name="ExecCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int InsUpDelCity(Tool.PageActivity ExecCode, string[] data)
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
                 
                    oldData = log.GenLogArray("SELECT * from tblCity WHERE CityCode ='" + CityCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTCity";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTCity";
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, (int) ExecCode);
                        cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "tblCity", CityCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("City Code is already exists. Please choose other City Code.");
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
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCityFromCountry(string countryCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * From tblcity WHERE CountryCode = '" + countryCode + "' Order by CityName";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            SelectListItem city = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            city.Value = dr["CityCode"] as string;
                            city.Text = dr["CityName"] as string;

                            cities.Add(city);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return cities;
        }
        public static DataTable GetCityDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelCity";
                db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();


                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}