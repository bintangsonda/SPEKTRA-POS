using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class Area
    {
        [Display(Name = "Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Area code is required")]
        [MaxLength(5), StringLength(5)]
        public string AreaCode { get; set; }

        [Display(Name = "Area")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Area name is required")]
        [MaxLength(30)]
        public string AreaName { get; set; }

        [Display(Name = "Description")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Country Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required")]
        public Country CountryArea { get; set; }

        [Display(Name = "City Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City Name is required")]
        public City CityArea { get; set; }

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

        public Area()
        {

        }

        public Area(string areaCode, string areaName)
        {
            AreaCode = areaCode;
            AreaName = areaName;
        }

        public static Area GetArea(string areaCode)
        {
            Area area = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                //Edited By Renaldi 9 July 2024
                //db.CommandText = "GTSelArea";
                //db.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, areaCode);
                //db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                //db.CommandType = System.Data.CommandType.StoredProcedure;
                //db.Open();
                db.CommandText = "SELECT * from Area LEFT JOIN tblCity on Area.CityCode = tblCity.CityCode LEFT JOIN tblCountry on tblCity.CountryCode = tblCountry.CountryCode Where AreaCode = @AreaCode";
                db.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, areaCode);
                //db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        area = new Area();
                        area.AreaCode = dr["AreaCode"] as string;
                        area.AreaName = dr["AreaName"] as string;

                        area.CountryArea = new Country();
                        area.CountryArea.CountryCode = dr["CountryCode"] as string;
                        area.CountryArea.CountryName = dr["CountryName"] as string;

                        area.CityArea = new City();
                        area.CityArea.CityCode = dr["CityCode"] as string;
                        area.CityArea.CityName = dr["CityName"] as string;
                        area.CityArea.Country = area.CountryArea;

                        area.Description = dr["Description"] as string;
                        //area.EntryUser = dr["EntryUser"] as string;
                        //area.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        area.OperatorID = dr["OperatorID"] as string;
                        area.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return area;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetAreaForDS()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM Area";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        cities = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem city = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            city.Value = dr["AreaCode"] as string;
                            city.Text = dr["AreaName"] as string;

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
        /// Retrieve semua daftar Area
        /// </summary>
        /// <returns></returns>
        public static List<Area> GetArea()
        {
            List<IDS.GeneralTable.Area> list = new List<Area>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelArea";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Area area = new Area();
                            area.AreaCode = dr["AreaCode"] as string;
                            area.AreaName = dr["AreaName"] as string;

                            area.CountryArea = new Country();
                            area.CountryArea.CountryCode = dr["CountryCode"] as string;
                            area.CountryArea.CountryName = dr["CountryName"] as string;

                            area.CityArea = new City();
                            area.CityArea.CityCode = dr["CityCode"] as string;
                            area.CityArea.CityName = dr["CityName"] as string;
                            area.CityArea.Country = area.CountryArea;

                            area.Description = dr["Description"] as string;
                            //area.EntryUser = dr["EntryUser"] as string;
                           // area.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            area.OperatorID = dr["OperatorID"] as string;
                            area.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            list.Add(area);
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
        /// Retrieve semua daftar area untuk datasource
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetAreaForDatasource()
        {
            List<KeyValuePair<string, string>> areas = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTArea";
                db.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        areas = new List<KeyValuePair<string, string>>();

                        while (dr.Read())
                        {
                            KeyValuePair<string, string> area = new KeyValuePair<string, string>(dr["AreaCode"] as string, dr["AreaName"] as string);
                            areas.Add(area);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return areas;
        }

        public int InsUpDelArea(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * FROM Area where AreaCode ='" + AreaCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTArea";
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                    cmd.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, CityArea.CityCode);
                    //cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, CountryArea.CountryCode);
                    cmd.AddParameter("@AreaName", System.Data.SqlDbType.VarChar, AreaName);
                    cmd.AddParameter("@Description", System.Data.SqlDbType.VarChar, Description);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * FROM Area where AreaCode = '" + AreaCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "Area", AreaCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "Area", AreaCode, OperatorID, "UPDATE");
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
                            throw new Exception("Area code is already exists. Please choose other area code.");
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

        public int InsUpDelArea(int ExecCode, string[] data)
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
                    oldData = log.GenLogArray("SELECT * FROM Area where AreaCode ='" + AreaCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTArea";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTArea";
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "Area", AreaCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Area Code is already exists. Please choose other Area Code.");
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
    }
}
