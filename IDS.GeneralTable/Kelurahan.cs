using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;
using System.Data;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class Kelurahan
    {
        [Display(Name = "Kelurahan Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kelurahan code is required")]
        [MaxLength(10), StringLength(10)]
        public string KelurahanCode { get; set; }

        [Display(Name = "Kelurahan Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kelurahan name is required")]
        [MaxLength(50)]
        public string KelurahanName { get; set; }

        [Display(Name = "Country Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required")]
        public Province ProvinceKelurahan { get; set; }

        [Display(Name = "City Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City Name is required")]
        public Location LocationKelurahan { get; set; }

        [Display(Name = "Kecamatan Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kecamatan Name is required")]
        public Kecamatan KecamatanKelurahan { get; set; }

        [Display(Name = "Zip Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Zip Code is required")]
        [MaxLength(5)]
        public string ZipCode { get; set; }

        [Display(Name = "Created By")]
        public string EntryUser { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Created Date")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        public DateTime LastUpdate { get; set; }

        public Kelurahan()
        {
        }

        public Kelurahan(string kelurahanCode, string kelurahanName)
        {
            KelurahanCode = kelurahanCode;
            KelurahanName = kelurahanName;
        }

        public static List<Kelurahan> GetKelurahan()
        {
            List<IDS.GeneralTable.Kelurahan> list = new List<Kelurahan>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKelurahan";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Kelurahan kelurahan = new Kelurahan();
                            kelurahan.KelurahanCode = dr["KelurahanCode"] as string;
                            kelurahan.KelurahanName = dr["KelurahanName"] as string;

                            kelurahan.ProvinceKelurahan = new Province();
                            kelurahan.ProvinceKelurahan.Code = dr["ProvinceCode"] as string;
                            kelurahan.ProvinceKelurahan.Name = dr["ProvinceName"] as string;

                            kelurahan.LocationKelurahan = new Location();
                            kelurahan.LocationKelurahan.LocationCode = dr["CityCode"] as string;
                            kelurahan.LocationKelurahan.LocationName = dr["LocationName"] as string;
                            kelurahan.LocationKelurahan.Province = kelurahan.ProvinceKelurahan;

                            kelurahan.KecamatanKelurahan = new Kecamatan();
                            kelurahan.KecamatanKelurahan.KecamatanCode = dr["KecamatanCode"] as string;
                            kelurahan.KecamatanKelurahan.KecamatanName = dr["KecamatanName"] as string;
                            kelurahan.KecamatanKelurahan.LocationKecamatan = kelurahan.LocationKelurahan;
                            kelurahan.KecamatanKelurahan.LocationKecamatan.Province = kelurahan.ProvinceKelurahan;

                            kelurahan.ZipCode = dr["ZipCode"] as string;
                            kelurahan.EntryUser = dr["CreatedBy"] as string;
                            kelurahan.EntryDate = Convert.ToDateTime(dr["CreatedDate"]);
                            kelurahan.OperatorID = dr["UpdatedBy"] as string;
                            kelurahan.LastUpdate = Convert.ToDateTime(dr["UpdatedDate"]);

                            list.Add(kelurahan);
                        }

                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static Kelurahan GetKelurahan(string kelurahanCode)
        {
            Kelurahan kelurahan = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKelurahan";
                db.AddParameter("@KelurahanCode", System.Data.SqlDbType.VarChar, kelurahanCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 4);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                            kelurahan = new Kelurahan();
                            kelurahan.KelurahanCode = dr["KelurahanCode"] as string;
                            kelurahan.KelurahanName = dr["KelurahanName"] as string;

                            kelurahan.ProvinceKelurahan = new Province();
                            kelurahan.ProvinceKelurahan.Code = dr["ProvinceCode"] as string;
                            kelurahan.ProvinceKelurahan.Name = dr["ProvinceName"] as string;

                            kelurahan.LocationKelurahan = new Location();
                            kelurahan.LocationKelurahan.LocationCode = dr["CityCode"] as string;
                            kelurahan.LocationKelurahan.LocationName = dr["LocationName"] as string;
                            kelurahan.LocationKelurahan.Province = kelurahan.ProvinceKelurahan;

                            kelurahan.KecamatanKelurahan = new Kecamatan();
                            kelurahan.KecamatanKelurahan.KecamatanCode = dr["KecamatanCode"] as string;
                            kelurahan.KecamatanKelurahan.KecamatanName = dr["KecamatanName"] as string;
                            kelurahan.KecamatanKelurahan.LocationKecamatan = kelurahan.LocationKelurahan;
                            kelurahan.KecamatanKelurahan.LocationKecamatan.Province = kelurahan.ProvinceKelurahan;
                        
                        kelurahan.ZipCode = dr["ZipCode"] as string;
                        kelurahan.OperatorID = dr["OperatorID"] as string;
                        kelurahan.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return kelurahan;
        }

        public static Kelurahan GetKelurahan(string kelurahanCode, string countryCode, string cityCode, string kecamatanCode)
        {
            Kelurahan kelurahan = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKelurahan";
                db.AddParameter("@KelurahanCode", System.Data.SqlDbType.VarChar, kelurahanCode);
                //db.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, countryCode);
                //db.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, cityCode);
                //db.AddParameter("@KecamatanCode", System.Data.SqlDbType.VarChar, kecamatanCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        kelurahan = new Kelurahan();
                        kelurahan.KelurahanCode = dr["KelurahanCode"] as string;
                        kelurahan.KelurahanName = dr["KelurahanName"] as string;

                        kelurahan.ProvinceKelurahan = new Province();
                        kelurahan.ProvinceKelurahan.Code = dr["ProvinceCode"] as string;
                        kelurahan.ProvinceKelurahan.Name = dr["ProvinceName"] as string;

                        kelurahan.LocationKelurahan = new Location();
                        kelurahan.LocationKelurahan.LocationCode = dr["CityCode"] as string;
                        kelurahan.LocationKelurahan.LocationName = dr["LocationName"] as string;
                        kelurahan.LocationKelurahan.Province = kelurahan.ProvinceKelurahan;

                        kelurahan.KecamatanKelurahan = new Kecamatan();
                        kelurahan.KecamatanKelurahan.KecamatanCode = dr["KecamatanCode"] as string;
                        kelurahan.KecamatanKelurahan.KecamatanName = dr["KecamatanName"] as string;
                        kelurahan.KecamatanKelurahan.LocationKecamatan = kelurahan.LocationKelurahan;
                        kelurahan.KecamatanKelurahan.LocationKecamatan.Province = kelurahan.ProvinceKelurahan;

                        kelurahan.ZipCode = dr["ZipCode"] as string;
                        kelurahan.OperatorID = dr["OperatorID"] as string;
                        kelurahan.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return kelurahan;
        }

        public static List<KeyValuePair<string, string>> GetKelurahanForDataSource()
        {
            List<KeyValuePair<string, string>> kelurahans = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKelurahan";
                db.AddParameter("@KelurahanCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 4);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        kelurahans = new List<KeyValuePair<string, string>>();

                        while (dr.Read())
                        {
                            KeyValuePair<string, string> kecamatan = new KeyValuePair<string, string>(dr["KelurahanCode"] as string, dr["KelurahanName"] as string);
                            kelurahans.Add(kecamatan);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return kelurahans;
        }

        public int InsUpDelKelurahan(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from GTKelurahan Where KelurahanCode = '" + KelurahanCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTUpdKelurahan";
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, KelurahanCode);
                    cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, KelurahanName);
                    cmd.AddParameter("@ZipCode", System.Data.SqlDbType.VarChar, ZipCode);
                    cmd.AddParameter("@Kecamatan", System.Data.SqlDbType.VarChar, KecamatanKelurahan.KecamatanCode);
                    ///cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, ProvinceKelurahan.Code);
                    //cmd.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, LocationKelurahan.LocationCode);
                    cmd.AddParameter("@Operator", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * from GTKelurahan Where KelurahanCode ='" + KelurahanCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "GTKelurahan", KelurahanCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "GTKelurahan", KelurahanCode, OperatorID, "UPDATE");
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
                            throw new Exception("Kelurahan code is already exists. Please choose other kecamatan code.");
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

        public int InsUpDelKelurahan(int ExecCode, string[] data, string[] dataCountry, string[] dataCity, string[] dataKecamatan)
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
           
                    oldData = log.GenLogArray("SELECT * from GTKelurahan Where KelurahanCode = '" + KelurahanCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTUpdKelurahan";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUpdKelurahan";
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.AddParameter("@Kecamatan", System.Data.SqlDbType.VarChar, dataKecamatan[i]);
                        //cmd.AddParameter("@CountryCode", System.Data.SqlDbType.VarChar, dataCountry[i]);
                        //cmd.AddParameter("@CityCode", System.Data.SqlDbType.VarChar, dataCity[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "GTKelurahan", KelurahanCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Kelurahan Code is already exists. Please choose other Kelurahan Code.");
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
        public static DataTable GetKelurahanDataForReport()
            {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelKelurahan";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();


                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
