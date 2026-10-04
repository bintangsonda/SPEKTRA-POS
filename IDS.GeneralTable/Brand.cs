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
    public class Brand
    {
        [Display(Name = "ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Brand id is required")]
        [MaxLength(10), StringLength(10)]
        public string BrandID { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Brand name is required")]
        [MaxLength(50)]
        public string BrandName { get; set; }

        [Display(Name = "Created By")]
        public string EntryUser { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Created Date")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "", DataFormatString = Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT)]
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }

        public Brand()
        {
        }

        public Brand(string brandID, string brandName)
        {
            BrandID = brandID;
            BrandName = brandName;
        }

        public static Brand GetBrand(string brandID)
        {
            Brand brand = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select * from tblBrand where BrandID = @ID";
                db.AddParameter("@ID", System.Data.SqlDbType.VarChar, brandID);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        brand = new Brand();
                        brand.BrandID = dr["BrandID"] as string;
                        brand.BrandName = dr["BrandName"] as string;
                       // br//and.EntryUser = dr["EntryUser"] as string;
                        //brand.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        brand.OperatorID = dr["LogUser"] as string;
                        brand.LastUpdate = Convert.ToDateTime(dr["LogDate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return brand;
        }

        /// <summary>
        /// Retrieve semua daftar Brand
        /// </summary>
        /// <returns></returns>
        public static List<Brand> GetBrand()
        {
            List<IDS.GeneralTable.Brand> list = new List<Brand>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblBrand";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Brand brand = new Brand();
                            brand = new Brand();
                            brand.BrandID = dr["BrandID"] as string;
                            brand.BrandName = dr["BrandName"] as string;
                            //brand.EntryUser = dr["EntryUser"] as string;
                            //brand.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            brand.OperatorID = dr["LogUser"] as string;
                            brand.LastUpdate = Convert.ToDateTime(dr["LogDate"]);

                            list.Add(brand);
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
        /// Retrieve semua daftar Brand untuk datasource
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetBrandForDatasource()
        {
            List<KeyValuePair<string, string>> brands = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBrand";
                db.AddParameter("@ID", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        brands = new List<KeyValuePair<string, string>>();

                        while (dr.Read())
                        {
                            KeyValuePair<string, string> brand = new KeyValuePair<string, string>(dr["BrandID"] as string, dr["BrandName"] as string);
                            brands.Add(brand);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return brands;
        }

        public int InsUpDelBrand(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from tblBrand Where BrandID ='" + BrandID + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTBrand";
                    cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@ID", System.Data.SqlDbType.VarChar, BrandID);
                    cmd.AddParameter("@BrandName", System.Data.SqlDbType.VarChar, BrandName);
                    cmd.AddParameter("@LogUser", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * from tblBrand Where BrandID ='" + BrandID + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "tblBrand", BrandID, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "tblBrand", BrandID, OperatorID, "UPDATE");
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
                            throw new Exception("Brand id is already exists. Please choose other brand id.");
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

        public int InsUpDelBrand(int ExecCode, string[] data)
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
                 
                    oldData = log.GenLogArray("SELECT * from tblBrand Where BrandID ='" + BrandID + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTBrand";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTBrand";
                        cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@ID", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "tblBrand", BrandID, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Brand ID is already exists. Please choose other Brand ID.");
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
        public static DataTable GetBrandDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBrand";
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 1);
                db.AddParameter("@ID", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();


                dt = db.GetDataTable();
            }

            return dt;
        }

    }
}
