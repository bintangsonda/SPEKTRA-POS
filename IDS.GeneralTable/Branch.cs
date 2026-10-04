using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using IDS.Tool;
using System.Data;
using System.Diagnostics.Metrics;

namespace IDS.GeneralTable
{
    public class Branch
    {
        #region Properties
        [Display(Name = "Branch Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch code is required")]
        [MaxLength(5), StringLength(5)]
        public string BranchCode { get; set; }
        public int NUPExpiredDay { get; set; }

        [Display(Name = "Branch Name")]
        [MaxLength(30), StringLength(30)]
        public string BranchName { get; set; }

        [Display(Name = "Branch Manager Name")]
        [MaxLength(100), StringLength(100)]
        /// <summary>
        /// Nama Branch Manager Cabang
        /// </summary>
        public string BranchManagerName { get; set; }

        [Display(Name = "Financial Officer")]
        [MaxLength(30), StringLength(30)]
        /// <summary>
        /// Nama Financial Account Officer Cabang
        /// </summary>
        public string FinAccOfficer { get; set; }

        [Display(Name = "NPWP")]
        [MaxLength(20), StringLength(20)]
        /// <summary>
        /// Nomor NPWP Cabang
        /// </summary>
        public string NPWP { get; set; }

        [Display(Name = "HO Status")]
        /// <summary>
        /// Status cabang atau kantor pusat.
        /// True = Kantor Pusat, False = cabang
        /// </summary>
        public bool HOStatus { get; set; }

        public BranchLeaseDefaultValue BranchLease { get; set; }

        // Branch Contact
        [Display(Name = "Address 1")]
        [MaxLength(50), StringLength(50)]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        [MaxLength(50), StringLength(50)]
        public string Address2 { get; set; }

        [Display(Name = "Address 3")]
        [MaxLength(50), StringLength(50)]
        public string Address3 { get; set; }

        [Display(Name = "Branch City")]
        public City BranchCity { get; set; }

        [Display(Name = "Branch Country")]
        public Country BranchCountry { get; set; }

        [NotMapped]
        public string FullAddress
        {
            get
            {
                return (Address1 ?? "" + Address2 ?? "" + Address3 ?? "" + BranchCity?.CityCode ?? "" + BranchCountry?.CountryCode ?? "");
            }
        }

        [Display(Name = "Postal Code")]
        [MaxLength(5), StringLength(5)]
        public string PostalCode { get; set; }

        [Display(Name = "Phone 1")]
        [MaxLength(15), StringLength(15)]
        public string Phone1 { get; set; }

        [Display(Name = "Phone 2")]
        [MaxLength(15), StringLength(15)]
        public string Phone2 { get; set; }

        [Display(Name = "Phone 3")]
        [MaxLength(15), StringLength(15)]
        public string Phone3 { get; set; }

        [Display(Name = "Fax")]
        [MaxLength(15), StringLength(15)]
        public string Fax { get; set; }

        public string Telex { get; set; }
        public string ZipPostal { get; set; }
        //public string HoStatus { get; set; }
        public bool PrintAddress { get; set; }
        public bool PrintCity { get; set; }
        public bool PrintCountry { get; set; }
        public string BranchManager { get; set; }
        public string OfficeDesc { get; set; }
        // Report Print Setting
        [Display(Name = "Print Branch Name")]
        public bool PrintBranchName { get; set; }
        [Display(Name = "Print Branch Address")]
        public bool PrintBranchAddress { get; set; }
        [Display(Name = "Print Branch City")]
        public bool PrintBranchCity { get; set; }
        [Display(Name = "Print Branch Country")]
        public bool PrintBranchCountry { get; set; } // Old
        [Display(Name = "print Page Number")]
        public bool PrintPage { get; set; }
        [Display(Name = "Print Date")]
        public bool PrintDate { get; set; }
        [Display(Name = "Print Time")]
        public bool PrintTime { get; set; }
        [Display(Name = "Language")]
        public bool Language { get; set; } //OptIndex


        // Log
        [Display(Name = "Created By")]
        public string EntryUser { get; set; }
        [Display(Name = "Created Date")]
        public DateTime EntryDate { get; set; }
        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }
        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }
        #endregion // Properties


        public Branch()
        {
            BranchLease = new BranchLeaseDefaultValue();
        }

        public Branch(string branchCode)
        {
            this.BranchCode = branchCode;
            this.BranchName = "";
        }

        public Branch(string branchCode, string branchName)
        {
            this.BranchCode = branchCode;
            this.BranchName = BranchName;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetRefBranchForApplication()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "SELECT tblBranch.BranchCode AS Code, tblBranch.BranchName AS [Name] FROM LeKaRep LEFT JOIN tblBranch ON tblBranch.BranchCode = LeKaRep.BranchCode UNION SELECT LeKaRep.KaRepCode AS Code, LeKaRep.KarepName AS [Name] FROM LeKaRep LEFT JOIN tblBranch ON tblBranch.BranchCode = LeKaRep.BranchCode;";
                //db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(); //<string, string>(dr["BranchCode"].ToString(), dr["BranchName"].ToString());
                            branch.Text = dr["Name"].ToString() + " - " + dr["Code"].ToString(); ;
                            branch.Value = dr["Code"].ToString();
                            branches.Add(branch);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            //if (branches.Count > 0)
            //    branches.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "All", Value = null });

            //branches = branches.OrderBy(x => x.Text).ToList();

            return branches;
        }
        public int InsUpDel(int FormState)
        {
            string OFFDESC = "Hapus stlh dta fix"; //ini hanya pelengkap Variable
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from tblBranch Where BranchCode  ='" + BranchCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    db.CommandText = "GTUpdateBranch";
                    db.CommandType = System.Data.CommandType.StoredProcedure;
                    db.Open();

                    db.BeginTransaction();

                    db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                    db.AddParameter("@BranchName", System.Data.SqlDbType.VarChar, BranchName);

                    db.AddParameter("@Addr1", System.Data.SqlDbType.VarChar, Address1);
                    db.AddParameter("@Phone1", System.Data.SqlDbType.VarChar, Phone1);

                    db.AddParameter("@NPWP", System.Data.SqlDbType.VarChar, NPWP);
                    db.AddParameter("@BranchManager", System.Data.SqlDbType.VarChar, BranchManager);

                    db.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, FormState);

                    db.ExecuteNonQuery();
                    db.CommitTransaction();
                  

                    return 1;
                }
                catch (Microsoft.Data.SqlClient.SqlException sex)
                {
                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Branch code is already exists. Please choose other branch code");
                        default:
                            throw;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public int InsUpDel(int ExecCode, string[] data)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";

                    oldData = log.GenLogArray("SELECT * from tblBranch Where BranchCode  ='" + BranchCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTUpdateBranch";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUpdateBranch";
                        cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", "tblBranch", BranchCode, OperatorID, "DELETE");
                }
                catch (Microsoft.Data.SqlClient.SqlException sex)
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

        /// <summary>
        /// Retrieve data branch berdasarkan branchCode
        /// </summary>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        public static Branch GetBranch(string branchCode)
        {
            Branch branch = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "GTSelBranch";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();


                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        branch = new Branch();
                        branch.BranchCode = dr["branchcode"] as string;
                        branch.BranchName = dr["branchname"] as string;
                        branch.NPWP = dr["NPWP"] as string;

                        branch.BranchManager = dr["BranchManager"] as string;

                        // Contact
                        branch.Address1 = Tool.GeneralHelper.NullToString(dr["Addr1"]);

                        branch.Phone1 = Tool.GeneralHelper.NullToString(dr["Phone1"]);

                        branch.PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
                        branch.PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
                        branch.PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
                        branch.PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
                        branch.PrintPage = Convert.ToBoolean(dr["ChkPage"]);
                        branch.PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
                        branch.PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
                        branch.Language = Convert.ToBoolean(dr["OptIndex"]);
                        // Log
                        branch.EntryUser = dr["EntryUser"] as string;
                        branch.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        branch.OperatorID = dr["OperatorID"] as string;
                        branch.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }
                }

                db.Close();
            }

            return branch;
        }

        /// <summary>
        /// Retrieve semua data branch
        /// </summary>
        /// <returns></returns>
        public static List<Branch> GetBranch()
        {
            List<Branch> branches = new List<Branch>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBranch";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();


                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Branch branch = new Branch();
                            branch = new Branch();
                            branch.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["branchcode"]);
                            branch.BranchName = IDS.Tool.GeneralHelper.NullToString(dr["branchname"]);
                            branch.NPWP = IDS.Tool.GeneralHelper.NullToString(dr["NPWP"]);

                            branch.BranchManager = IDS.Tool.GeneralHelper.NullToString(dr["BranchManager"]);

                            branch.Address1 = IDS.Tool.GeneralHelper.NullToString(dr["Addr1"]);
                            
                            branch.Phone1 = IDS.Tool.GeneralHelper.NullToString(dr["Phone1"]);

                            // Print Option
                            branch.PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
                            branch.PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
                            branch.PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
                            branch.PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
                            branch.PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
                            branch.PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
                            branch.PrintPage = Convert.ToBoolean(dr["ChkPage"]);
                            branch.Language = Convert.ToBoolean(dr["OptIndex"]);

                            // Log
                            branch.EntryUser = dr["EntryUser"] as string;
                            branch.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            branch.OperatorID = dr["OperatorID"] as string;
                            branch.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            branches.Add(branch);
                        }
                    }
                }

                db.Close();
            }

            return branches;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetBranchForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "GTSelBranch";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(); //<string, string>(dr["BranchCode"].ToString(), dr["BranchName"].ToString());
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]) +" - "+IDS.Tool.GeneralHelper.NullToString(dr["BranchName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            branches.Add(branch);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            branches = branches.OrderBy(x => x.Text).ToList();

            return branches;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetBranchForDatasourceWithAll()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "GTSelBranch";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(); //<string, string>(dr["BranchCode"].ToString(), dr["BranchName"].ToString());
                            branch.Text = dr["BranchName"].ToString();
                            branch.Value = dr["BranchCode"].ToString();
                            branches.Add(branch);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            if (branches.Count > 0)
                branches.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "All", Value = null });

            branches = branches.OrderBy(x => x.Text).ToList();

            return branches;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCountryForBranch(string BranchCode)
        {

            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBranch";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 5);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem kec = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            kec.Text = Tool.GeneralHelper.NullToString(dr["CountryName"]);
                            kec.Value = Tool.GeneralHelper.NullToString(dr["CountryCode"]);
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

        //public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetBranchForDatasource(bool WithEmptyOption)
        //{
        //    List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

        //    using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
        //    {
        //        db.CommandText = "GTSelBranch";
        //        db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, DBNull.Value);
        //        db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
        //        db.CommandType = System.Data.CommandType.StoredProcedure;
        //        db.Open();

        //        db.ExecuteReader();

        //        using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
        //        {
        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(); //<string, string>(dr["BranchCode"].ToString(), dr["BranchName"].ToString());
        //                    branch.Text = dr["BranchName"].ToString();
        //                    branch.Value = dr["BranchCode"].ToString();
        //                    branches.Add(branch);
        //                }
        //            }

        //            if (!dr.IsClosed)
        //                dr.Close();
        //        }

        //        db.Close();
        //    }

        //    branches = branches.OrderBy(x => x.Text).ToList();

        //    if (WithEmptyOption)
        //    {
        //        branches.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "", Text = "" });
        //    }

        //    return branches;
        //}

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetBranchForDatasource(string branchCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "GTSelBranch";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, branchCode);
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
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(); //<string, string>(dr["BranchCode"].ToString(), dr["BranchName"].ToString());
                            branch.Text = dr["BranchName"].ToString();
                            branch.Value = dr["BranchCode"].ToString();
                            branches.Add(branch);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            branches = branches.OrderBy(x => x.Text).ToList();

            return branches;
        }
        public static DataTable GetBranchDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBranch";
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

    }








    /// <summary>
    /// Class untuk default value leasing per branch
    /// </summary>
    public class BranchLeaseDefaultValue
    {
        [Display(Name = "Contract Limit")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Contract Limit is required")]
        public decimal ContractLimit { get; set; }

        [Display(Name = "NUP Approval Limit")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "NUP Approval Limit is required")]
        public decimal NUPApprovalLimit { get; set; }

        [Display(Name = "NUP Expired Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "NUP Expired Days is required")]
        public int NUPExpiredDays { get; set; }

        [Display(Name = "Penalty Rate")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Penalty Rate for corporate funding is required")]
        public decimal PenaltyRate { get; set; } // Untuk Pembiayaan Corporate

        [Display(Name = "Penalty Rate Retail")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Penalty Rate for retail funding is required")]
        public decimal PenaltyRateRetail { get; set; } // Untuk Pembiayaan Retail

        [Display(Name = "Over Rate")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Over rate for corporate funding is required")]
        public decimal OverRate { get; set; } // Untuk Pembiayaan Corporate

        [Display(Name = "Over Rate Retail")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Contract Limit is required")]
        public decimal OverRateRetail { get; set; } // Untuk Pembiayaan Retail

        [NotMapped]
        [Display(Name = "Last Receipt No")]
        [Required(AllowEmptyStrings = false)]
        [DefaultValue(0)]
        public int LastReceiptNo { get; set; }

        [NotMapped]
        [Display(Name = "Last Contract Count")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Contract Count is required")]
        [DefaultValue(0)]
        public int LastCount { get; set; } // Nomor Urut terakhir nomor kontrak 

        [Display(Name = "SLIK Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "SLIK code is required")]
        [MaxLength(100), StringLength(100)]
        /// <summary>
        /// Kode Cabang untuk pelaporan SLIK
        /// </summary>
        public string SLIKCode { get; set; }

        public BranchLeaseDefaultValue()
        {
        }

        public void GetBranchLeaseDefaultValue(string branchCode)
        {
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelBranchLeaseDefaultValue";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ContractLimit = Convert.ToDecimal(dr["ContractLimit"]);
                            NUPApprovalLimit = Convert.ToDecimal(dr["NUPApprovalLimit"]);
                            NUPExpiredDays = Convert.ToInt32(dr["NUPExpiredDay"]);
                            OverRate = Convert.ToDecimal(dr["OverRate"]);
                            OverRateRetail = Convert.ToDecimal(dr["OverRateRetail"]);
                            PenaltyRate = Convert.ToDecimal(dr["PenaltyRate"]);
                            PenaltyRateRetail = Convert.ToDecimal(dr["PenaltyRateRetail"]);
                            LastCount = Convert.ToInt32(dr["LastCount"]);
                            LastReceiptNo = Convert.ToInt32(dr["LastReceiptNo"]);
                            SLIKCode = Tool.GeneralHelper.NullToString(dr["SLIKCode"]);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
        }
    }

    public class GL
    {
        // COA untuk perpindahan Aset Pembiayaan antar cabang
        //[Display(Name = "HOP No")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "HOP No is required")]
        //public IDS.GLTable.ChartOfAccount HopNo { get; set; }

        //[Display(Name = "Rep No")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Rep No is required")]
        //public IDS.GLTable.ChartOfAccount RepNo { get; set; }

        public GL()
        {
        }

        public void GetBranchAsetMovementAccount()
        {
            //using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            //{
            //    db.CommandText = "GTSelBranchAccount";
            //    db.CommandType = System.Data.CommandType.StoredProcedure;
            //    db.AddParameter("@code", System.Data.SqlDbType.VarChar, DBNull.Value);
            //    db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
            //    db.Open();

            //    db.ExecuteReader();

            //    using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
            //    {
            //        if (dr.HasRows)
            //        {
            //            items = new List<IDS.GL.ChartOfAccount>();

            //            while (dr.Read())
            //            {
            //                IDS.GL.ChartOfAccount item = new IDS.GL.ChartOfAccount();

            //                items.Add(item);
            //            }
            //        }

            //        if (!dr.IsClosed)
            //            dr.Close();
            //    }
            //}

            //return items;
        }

    }

    ///// <summary>
    ///// Class untuk print setting pada branch
    ///// </summary>
    //public class BranchReportSetting
    //{
    //    public BranchReportSetting()
    //    {
    //    }

    //    public BranchReportSetting(bool printName, bool printAddress, bool printCity, bool printCountry, bool printPage, bool printDate, bool printTime, bool language) : this()
    //    {
    //        PrintBranchName = printName;
    //        PrintBranchAddress = printAddress;
    //        PrintBranchCity = printCity;
    //        PrintBranchCountry = printCountry;
    //        PrintPage = printPage;
    //        PrintDate = printDate;
    //        PrintTime = printTime;
    //        Language = language;
    //    }

    //    public void GetBranchReportSetting()
    //    {
    //        List<BranchReportSetting> items = null;

    //        using (DataAccess.SqlServer db = new DataAccess.SqlServer())
    //        {
    //            db.CommandText = "GTSelBranchReportSetting";
    //            db.CommandType = System.Data.CommandType.StoredProcedure;
    //            db.AddParameter("@code", System.Data.SqlDbType.VarChar, DBNull.Value);
    //            db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
    //            db.Open();

    //            db.ExecuteReader();

    //            using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
    //            {
    //                if (dr.HasRows)
    //                {
    //                    items = new List<BranchReportSetting>();

    //                    while (dr.Read())
    //                    {
    //                        PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
    //                        PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
    //                        PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
    //                        PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
    //                        PrintPage = Convert.ToBoolean(dr["ChkPage"]);
    //                        PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
    //                        PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
    //                        Language = Convert.ToBoolean(dr["OptIndex"]);
    //                    }
    //                }

    //                if (!dr.IsClosed)
    //                    dr.Close();
    //            }

    //            db.Close();
    //        }
    //    }
    //}

    //public class BrachContact
    //{

    //    public BrachContact()
    //    {
    //    }

    //    public void GetBranchReportSetting()
    //    {
    //        List<BranchReportSetting> items = null;

    //        using (DataAccess.SqlServer db = new DataAccess.SqlServer())
    //        {
    //            db.CommandText = "GTSelBranchReportSetting";
    //            db.CommandType = System.Data.CommandType.StoredProcedure;
    //            db.AddParameter("@code", System.Data.SqlDbType.VarChar, DBNull.Value);
    //            db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
    //            db.Open();

    //            db.ExecuteReader();

    //            using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
    //            {
    //                if (dr.HasRows)
    //                {
    //                    items = new List<BranchReportSetting>();

    //                    while (dr.Read())
    //                    {
    //                        Address1 = Tool.GeneralHelper.NullToString(dr["Addr1"]);
    //                        Address2 = Tool.GeneralHelper.NullToString(dr["Addr2"]);
    //                        Address3 = Tool.GeneralHelper.NullToString(dr["Addr3"]);

    //                        BranchCountry = new Country();
    //                        BranchCountry.CountryCode = Tool.GeneralHelper.NullToString(dr["CountryCode"]);
    //                        BranchCountry.CountryName = Tool.GeneralHelper.NullToString(dr["CountryName"]);

    //                        BranchCity = new City();
    //                        BranchCity.CityCode = Tool.GeneralHelper.NullToString(dr["CityCode"]);
    //                        BranchCity.CityName = Tool.GeneralHelper.NullToString(dr["CityName"]);
    //                        BranchCity.Country = BranchCountry;

    //                        PostalCode = Tool.GeneralHelper.NullToString(dr["PostalCode"]);

    //                        Phone1 = Tool.GeneralHelper.NullToString(dr["Phone1"]);
    //                        Phone2 = Tool.GeneralHelper.NullToString(dr["Phone2"]);
    //                        Phone3 = Tool.GeneralHelper.NullToString(dr["Phone3"]);
    //                        Fax = Tool.GeneralHelper.NullToString(dr["Fax"]);
    //                    }
    //                }

    //                if (!dr.IsClosed)
    //                    dr.Close();
    //            }

    //            db.Close();
    //        }
    //    }
    //}
}