using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;
using System.Data;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class Location
    {
        [Display(Name = "Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Location code is required")]
        [MaxLength(10), StringLength(10)]
        public string LocationCode { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Location name is required")]
        [MaxLength(50)]
        public string LocationName { get; set; }

        [Display(Name = "Province name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Province name is required")]
        public Province Province { get; set; }

        [Display(Name = "Operator")]
        public string OperatorID { get; set; }

        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "Remark")]
        [MaxLength(500)]
        public string Remark { get; set; }

        [Display(Name = "BI Code")]
        [MaxLength(10)]
        public string BICode { get; set; }

        [Display(Name = "OJK Code")]
        [MaxLength(10)]
        public string OJKCode { get; set; }

        [Display(Name = "SLIK Code")]
        [MaxLength(10)]
        public string SLIKCode { get; set; }

        [Display(Name = "GNS Code")]
        [MaxLength(10)]
        public string GNSCode { get; set; }

        public Location()
        {
        }

        public Location(string locationCode, string locationName)
        {
            LocationCode = locationCode;
            LocationName = locationName;
        }

        public static List<Location> GetLocation()
        {
            List<IDS.GeneralTable.Location> list = new List<Location>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "Select H.* from tbllocation H inner join GTProvince pro on H.ProvinceCode = Pro.ProvinceCode";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.AddParameter("@code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<Location>();

                        while (dr.Read())
                        {
                            Location location = new Location();
                            location.LocationCode = IDS.Tool.GeneralHelper.NullToString(dr["LocationCode"]);
                            location.LocationName = IDS.Tool.GeneralHelper.NullToString(dr["LocationName"]);
                            location.Province = new Province();
                            location.Province.Code= IDS.Tool.GeneralHelper.NullToString(dr["ProvinceCode"]);
                            location.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            location.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"],DateTime.Now);
                            location.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remark"]);
                            location.BICode = IDS.Tool.GeneralHelper.NullToString(dr["BICode"]);
                            location.OJKCode = IDS.Tool.GeneralHelper.NullToString(dr["OJKCode"]);
                            location.SLIKCode = IDS.Tool.GeneralHelper.NullToString(dr["SLIKCode"]);
                            location.GNSCode = IDS.Tool.GeneralHelper.NullToString(dr["GNSCode"]);
                            list.Add(location);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static Location GetLocation(string locationCode)
        {
            Location location = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelLocation";
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, locationCode);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        location = new Location();
                        location.LocationCode = dr["LocationCode"] as string;
                        location.LocationName = dr["LocationName"] as string;
                        location.Province = IDS.GeneralTable.Province.GetProvince(dr["ProvinceCode"] as string);
                        location.OperatorID = dr["OperatorID"] as string;
                        location.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                        //location.Remark = dr["Remark"] as string;
                        //location.BICode = dr["BICode"] as string;
                        //location.OJKCode = dr["OJKCode"] as string;
                        //location.SLIKCode = dr["SLIKCode"] as string;
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return location;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLocationDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> location = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelLocation";
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        location = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem loc = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            loc.Value = dr["LocationCode"] as string;
                            loc.Text = dr["LocationName"] as string;

                            location.Add(loc);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return location;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLocationDatasource(string LocationCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> location = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT LocationCode,LocationName from tblLocation WHERE LocationCode ='"+ LocationCode +"'";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        location = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem loc = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            loc.Value = dr["LocationCode"] as string;
                            loc.Text = dr["LocationName"] as string;

                            location.Add(loc);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return location;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExecCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int InsUpDelLocation(string ExecCode)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("select * from tblLocation where LocationCode = '" + LocationCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTLocation";
                    cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, LocationCode);
                    cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, LocationName);
                    cmd.AddParameter("@User", System.Data.SqlDbType.VarChar, OperatorID.ToString());
                    cmd.AddParameter("@ProvinceCode ", System.Data.SqlDbType.VarChar, Province.Code);
                    cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, Convert.ToInt32(ExecCode));
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (Convert.ToInt32(ExecCode) != 3)
                    {
                        newData = log.GenLogArray("select * from tblLocation where LocationCode ='" + LocationCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (Convert.ToInt32(ExecCode) == 1)
                        {
                            log.SaveSysLog1("", newData, "tblLocation", LocationCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "tblLocation", LocationCode, OperatorID, "UPDATE");
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
                            throw new Exception("Location code is already exists. Please choose other location code.");
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
        public int InsDelLocation(int ExecCode, string[] data)
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
                    oldData = log.GenLogArray("select * from tblLocation where LocationCode = '" + LocationCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    cmd.CommandText = "GTLocation";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.AddParameter("@init", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "tblLocation", LocationCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Location code is already exists. Please choose other location code.");
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
        public static DataTable GetLocationDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelLocation";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.AddParameter("@code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }


        #region Untuk Sementara, Karena aplikasi lama masih baca dari Location. Untuk kedepannya diganti ke City
        //[Obsolete("Untuk Sementara, Karena aplikasi lama masih baca dari Location. Untuk kedepannya diganti ke City")]
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLocationDatasourceForOldData(string provinceCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> location = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT LocationCode, LocationName, ProvinceCode FROM tblLocation WHERE ISNULL(ProvinceCode, '') LIKE ISNULL(@provinceCode, '%') ORDER BY LocationName ASC;";
                db.AddParameter("@provinceCode", System.Data.SqlDbType.VarChar, (object)provinceCode ?? DBNull.Value);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem loc = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            loc.Value = dr["LocationCode"] as string;
                            loc.Text = dr["LocationName"] as string;

                            location.Add(loc);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return location;
        }
        #endregion
    }
}
