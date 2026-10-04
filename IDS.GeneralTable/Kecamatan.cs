using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using IDS.Tool;
using System.Data;

namespace IDS.GeneralTable
{
    public class Kecamatan
    {
        [Display(Name = "Kecamatan Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kecamatan code is required")]
        [MaxLength(10), StringLength(10)]
        public string KecamatanCode { get; set; }

        [Display(Name = "Kecamatan Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kecamatan name is required")]
        [MaxLength(50)]
        public string KecamatanName { get; set; }

        [Display(Name = "Country Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required")]
        public Province ProvinceKecamatan { get; set; }

        [Display(Name = "City Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City Name is required")]
        public Location LocationKecamatan { get; set; }

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

        public Kecamatan()
        {
        }

        public Kecamatan(string kecamatanCode, string kecamatanName)
        {
            KecamatanCode = kecamatanCode;
            KecamatanName = kecamatanName;
        }

        public static List<Kecamatan> GetKecamatan()
        {
            List<IDS.GeneralTable.Kecamatan> list = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKecamatan";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<Kecamatan>();

                        while (dr.Read())
                        {
                            Kecamatan kecamatan = new Kecamatan();
                            kecamatan.KecamatanCode = dr["KecamatanCode"] as string;
                            kecamatan.KecamatanName = dr["KecamatanName"] as string;
                            //GET Provinsi
                            kecamatan.ProvinceKecamatan = new Province();
                            kecamatan.ProvinceKecamatan.Code = dr["ProvinceCode"] as string;
                            kecamatan.ProvinceKecamatan.Name = dr["ProvinceName"] as string;
                            //GET Location atau Kabupaten
                            kecamatan.LocationKecamatan = new Location();
                            kecamatan.LocationKecamatan.LocationCode = dr["CityCode"] as string;
                            kecamatan.LocationKecamatan.LocationName = dr["LocationName"] as string;
                            kecamatan.LocationKecamatan.Province = kecamatan.ProvinceKecamatan;
                            kecamatan.EntryUser = dr["CreatedBy"] as string;
                            kecamatan.EntryDate = Convert.ToDateTime(dr["CreatedDate"]);
                            kecamatan.OperatorID = dr["UpdatedBy"] as string;
                            kecamatan.LastUpdate = Convert.ToDateTime(dr["UpdatedDate"]);

                            list.Add(kecamatan);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static Kecamatan GetKecamatan(string kecamatanCode)
        {
            Kecamatan kecamatan = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKecamatan";
                db.AddParameter("@KecamatanCode", System.Data.SqlDbType.VarChar, kecamatanCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 4);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        kecamatan = new Kecamatan();
                        kecamatan.KecamatanCode = dr["KecamatanCode"] as string;
                        kecamatan.KecamatanName = dr["KecamatanName"] as string;

                        kecamatan.ProvinceKecamatan = new Province();
                        kecamatan.ProvinceKecamatan.Code = dr["ProvinceCode"] as string;
                        kecamatan.ProvinceKecamatan.Name = dr["ProvinceName"] as string;

                        kecamatan.LocationKecamatan = new Location();
                        kecamatan.LocationKecamatan.LocationCode = dr["CityCode"] as string;
                        kecamatan.LocationKecamatan.LocationName = dr["LocationName"] as string;
                        kecamatan.LocationKecamatan.Province = kecamatan.ProvinceKecamatan;

                        //LOG
                        kecamatan.EntryUser = dr["CreatedBy"] as string;
                        kecamatan.EntryDate = Convert.ToDateTime(dr["CreatedDate"]);
                        kecamatan.OperatorID = dr["UpdatedBy"] as string;
                        kecamatan.LastUpdate = Convert.ToDateTime(dr["UpdatedDate"]);

                        //kecamatan.EntryUser = dr["EntryUser"] as string;
                        //kecamatan.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        //kecamatan.OperatorID = dr["OperatorID"] as string;
                        //kecamatan.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return kecamatan;
        }

        public static Kecamatan GetKecamatan(string kecamatanCode, string countryCode, string cityCode)
        {
            Kecamatan kecamatan = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKecamatan";
                db.AddParameter("@ProvinceCode", System.Data.SqlDbType.VarChar, countryCode);
                db.AddParameter("@LocationCode", System.Data.SqlDbType.VarChar, cityCode);
                db.AddParameter("@KecamatanCode", System.Data.SqlDbType.VarChar, kecamatanCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        kecamatan = new Kecamatan();
                        kecamatan.KecamatanCode = dr["KecamatanCode"] as string;
                        kecamatan.KecamatanName = dr["KecamatanName"] as string;

                        kecamatan.ProvinceKecamatan = new Province();
                        kecamatan.ProvinceKecamatan.Code = dr["ProvinceCode"] as string;
                        kecamatan.ProvinceKecamatan.Name = dr["ProvinceName"] as string;

                        kecamatan.LocationKecamatan = new Location();
                        kecamatan.LocationKecamatan.LocationCode = dr["CityCode"] as string;
                        kecamatan.LocationKecamatan.LocationName = dr["LocationName"] as string;
                        kecamatan.LocationKecamatan.Province = kecamatan.ProvinceKecamatan;
                        kecamatan.EntryUser = dr["EntryUser"] as string;
                        kecamatan.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        kecamatan.OperatorID = dr["OperatorID"] as string;
                        kecamatan.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                        kecamatan.EntryUser = dr["EntryUser"] as string;
                        kecamatan.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        kecamatan.OperatorID = dr["OperatorID"] as string;
                        kecamatan.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return kecamatan;
        }

        public int InsUpDelKecamatan(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from GTKecamatan where KecamatanCode ='" + KecamatanCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    cmd.CommandText = "GTUpdKecamatan";
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, KecamatanCode);
                    cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, KecamatanName);
                    //cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, ProvinceKecamatan.Code);
                    cmd.AddParameter("@City", System.Data.SqlDbType.VarChar, LocationKecamatan.LocationCode);

                    if (!string.IsNullOrEmpty(EntryUser))
                    {
                        cmd.AddParameter("@EntryUser", System.Data.SqlDbType.VarChar, EntryUser);
                    }

                    cmd.AddParameter("@Operator", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * from GTKecamatan where KecamatanCode ='" + KecamatanCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "GTKecamatan", KecamatanCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "GTKecamatan", KecamatanCode, OperatorID, "UPDATE");
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
                            throw new Exception("Kecamatan code is already exists. Please choose other kecamatan code.");
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

        public int InsUpDelKecamatan(int ExecCode, string[] data, string[] dataCountry, string[] dataCity)
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
                    oldData = log.GenLogArray("SELECT * from GTKecamatan where KecamatanCode ='" + KecamatanCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    cmd.CommandText = "GTUPDKecamatan";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUPDKecamatan";
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, data[i]);
                        //cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, dataCountry[i]);
                        cmd.AddParameter("@City", System.Data.SqlDbType.VarChar, dataCity[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "GTKecamatan", KecamatanCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Kecamatan Code is already exists. Please choose other Kecamatan Code.");
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

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetKecamatanForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKecamatan";
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 5);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string kecamatanCode = dr["KecamatanCode"].ToString();
                            string kecamatanName = dr["KecamatanName"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = kecamatanName, Value = kecamatanCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetKecamatanForDataSource(string KecamatanCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT KecamatanCode,KecamatanName from GTKecamatan WHERE KecamatanCode = '" + KecamatanCode +"'";


                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string kecamatanCode = dr["KecamatanCode"].ToString();
                            string kecamatanName = dr["KecamatanName"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = kecamatanName, Value = kecamatanCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

   
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetLocationFromProvince(string Province)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * from tblLocation WHERE ProvinceCode = @Province";
                db.AddParameter("@Province", System.Data.SqlDbType.VarChar,Province);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem country = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            country.Value = dr["LocationCode"] as string;
                            country.Text = dr["LocationName"] as string;
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
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetKecamatanForlocation(string Loc)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT kecamatanCode, KecamatanName from GTKecamatan WHERE CityCode = '" + Loc + "' order by KecamatanName asc";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem kec = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            kec.Text = Tool.GeneralHelper.NullToString(dr["KecamatanName"]);
                            kec.Value = Tool.GeneralHelper.NullToString(dr["kecamatanCode"]);
                            list.Add(kec);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        public static DataTable GetKecamatanDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKecamatan";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();


                dt = db.GetDataTable();
            }

            return dt;
        }

    }
}
