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
    public class SerialNo
    {
        [Display(Name = "Code")]
        public int SerialNoCode { get; set; }

        [Display(Name = "Transaction Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Trans is required")]
        [MaxLength(20)]
        public string Trans { get; set; }

        [Display(Name = "Serial No")]
        [MaxLength(4)]
        public string Serialno { get; set; }

        [Display(Name = "Serial Number")]
        [MaxLength(4)]
        public string SerialNumber { get; set; }

        [Display(Name = "Manual")]
        public bool Manual { get; set; }

        [Display(Name = "Additional")]
        [MaxLength(20)]
        public string Additional { get; set; }

        [Display(Name = "dt")]
        [MaxLength(20)]
        public string dt { get; set; }

        [Display(Name = "Reset")]
        public int Reset { get; set; }

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

        public SerialNo()
        {
        }

        public SerialNo(int code, string trans)
        {
            SerialNoCode = code;
            Trans = trans;
        }

        public static SerialNo GetSerialNo(string serialNoCode)
        {
            SerialNo serialNo = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                
                db.CommandText = "GTSelSerialNo";
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, serialNoCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        serialNo = new SerialNo();
                        serialNo.SerialNoCode = Convert.ToInt16(dr["kode"]);
                        serialNo.Trans = dr["Trans"] as string;
                        if (dr["SerialNo"] != DBNull.Value)
                        {
                            serialNo.Serialno = dr["SerialNo"] as string;
                        }
                        else
                        {
                            serialNo.Serialno = "";
                        }
                        serialNo.SerialNumber = dr["SerialNo"] as string;
                        serialNo.Manual = Convert.ToBoolean(dr["Manual"]);
                        if (dr["Additional"] != DBNull.Value)
                        {
                            serialNo.Additional = dr["Additional"] as string;
                        }
                        else
                        {
                            serialNo.Additional = "";
                        }
                        if (dr["dt"] != DBNull.Value)
                        {
                            serialNo.dt = dr["dt"] as string;
                        }
                        else
                        {
                            serialNo.dt = "";
                        }
                        if (dr["Reset"] != DBNull.Value)
                        {
                            serialNo.Reset = Convert.ToInt16(dr["Reset"]);
                        }
                        else
                        {
                            serialNo.Reset = 0;
                        }
                        ///DICOMENT KARENA DITABLENYA TIDAK ADA KOLUMNYA.
                        //serialNo.EntryUser = dr["EntryUser"] as string;
                        //serialNo.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        //serialNo.OperatorID = dr["OperatorID"] as string;
                        //serialNo.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return serialNo;
        }

        /// <summary>
        /// Retrieve semua daftar Country
        /// </summary>
        /// <returns></returns>
        public static List<SerialNo> GetSerialNo()
        {
            List<IDS.GeneralTable.SerialNo> list = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                
                db.CommandText = "GTSelSerialNo";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.AddParameter("@TransName", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<SerialNo>();

                        while (dr.Read())
                        {
                            SerialNo serialNo = new SerialNo();
                            serialNo.SerialNoCode = IDS.Tool.GeneralHelper.NullToInt(dr["Kode"],0);
                            serialNo.Trans = IDS.Tool.GeneralHelper.NullToString(dr["Trans"]);

                            serialNo.SerialNumber = IDS.Tool.GeneralHelper.NullToString(dr["SerialNo"]);
                            serialNo.Manual = IDS.Tool.GeneralHelper.NullToBool(dr["Manual"],false);
                            if (dr["Additional"] != DBNull.Value)
                            {
                                serialNo.Additional = IDS.Tool.GeneralHelper.NullToString(dr["Additional"]);
                            }
                            else
                            {
                                serialNo.Additional = "";
                            }
                            if (dr["dt"] != DBNull.Value)
                            {
                                serialNo.dt = IDS.Tool.GeneralHelper.NullToString(dr["dt"]);
                            }
                            else
                            {
                                serialNo.dt = "";
                            }
                            if (dr["Reset"] != DBNull.Value)
                            {
                                serialNo.Reset = IDS.Tool.GeneralHelper.NullToInt(dr["Reset"],0);
                            }
                            else
                            {
                                serialNo.Reset = 0;
                            }
                            if (dr["tanggal"] != DBNull.Value) 
                            {
                                serialNo.EntryDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["tanggal"],DateTime.Now);
                            }
                            else 
                            {
                                serialNo.EntryDate = DateTime.Now;
                            }
                            serialNo.EntryUser = IDS.Tool.GeneralHelper.NullToString(dr["EntryUser"]);
                            serialNo.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            serialNo.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"],DateTime.Now);
                            list.Add(serialNo);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetResetForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Reset = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            Reset.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Month", Value = "0" });
            Reset.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Year", Value = "1" });
            Reset.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Auto", Value = "2" });


            return Reset;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDTForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> dt = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            dt.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "mmYY", Value = "mmYY" });
            dt.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "mmmYY", Value = "mmmYY" });
            dt.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "mmmmYY", Value = "mmmmYY" });
            dt.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "mmYYYY", Value = "mmYYYY" });
            dt.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "mmmYYYY", Value = "mmmYYYY" });
            dt.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "mmmmYYYY", Value = "mmmmYYYY" });
            dt.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "YYYYmm", Value = "YYYYmm" });

            return dt;
        }

        /// <summary>
        /// Retrieve semua daftar country untuk datasource
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetSerialNoForDatasource()
        {
            List<KeyValuePair<string, string>> serialnos = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSerialNo";
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        serialnos = new List<KeyValuePair<string, string>>();

                        while (dr.Read())
                        {
                            KeyValuePair<string, string> serialno = new KeyValuePair<string, string>(dr["Code"] as string, dr["Trans"] as string);
                            serialnos.Add(serialno);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return serialnos;
        }

        /// <summary>
        /// Untuk cek apakah kode transaksi yang di cari automatic atau manual
        /// </summary>
        /// <param name="transName">Nama Transaksi contohnya Voucher dan sebagainya</param>
        /// <returns>true = Manual, false = Automatic</returns>
        public static bool IsAutomatic(string transName)
        {
            bool isAutomatic = true;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelSerialNo";
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.AddParameter("@Code", System.Data.SqlDbType.Int, 0);
                db.AddParameter("@TransName", System.Data.SqlDbType.VarChar, transName);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                isAutomatic = !(Convert.ToBoolean(db.ExecuteScalar()));
            }

            return isAutomatic;
        }

        public int InsUpDelSerialNo(int ExecCode)
        {
            string StringSERNO = Convert.ToString(SerialNoCode);
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("Select * from tblSerialNo Where Kode =  '" + SerialNoCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTSerialNo";
                    cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@kode", System.Data.SqlDbType.Int, SerialNoCode);
                    cmd.AddParameter("@Trans", System.Data.SqlDbType.VarChar, Trans);
                    cmd.AddParameter("@Serialno", System.Data.SqlDbType.VarChar, SerialNumber);
                    cmd.AddParameter("@Manual", System.Data.SqlDbType.Bit, Manual);
                    cmd.AddParameter("@Additional", System.Data.SqlDbType.VarChar, Additional);
                    cmd.AddParameter("@dt", System.Data.SqlDbType.VarChar, dt);
                    cmd.AddParameter("@reset", System.Data.SqlDbType.TinyInt, Reset);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT * from tblSerialNo Where kode ='" + SerialNoCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "tblSerialNo", StringSERNO, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "tblSerialNo", StringSERNO, OperatorID, "UPDATE");
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
                            throw new Exception("Serial No code is already exists. Please choose other Serial No code.");
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
        /// Untuk Delete data
        /// </summary>
        /// <param name="ExecCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int InsUpDelSerialNo(int ExecCode, string[] data)
        {
            int result = 0;
            string SerNO = Convert.ToString(SerialNoCode);
            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    oldData = log.GenLogArray("Select * from tblSerialNo Where Kode =  '" + SerialNoCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    cmd.CommandText = "GTSerialNo";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTSerialNo";
                        cmd.AddParameter("@init", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@kode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "GTKelurahan", SerNO, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Serial No Code is already exists. Please choose other Serial No Code.");
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
        public static DataTable GetSerialNoDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelSerialNo";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.AddParameter("@TransName", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.Open();


                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}