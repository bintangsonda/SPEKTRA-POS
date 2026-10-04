using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using IDS.Tool;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IDS.GeneralTable
{
    public class Department
    {
        [Display(Name = "Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Department code is required")]
        [MaxLength(3), StringLength(3)]
        public string DepartmentCode { get; set; }

        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Department name is required")]
        [MaxLength(40)]
        public string DepartmentName { get; set; }

        [Display(Name = "Branch Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch Name is required")]
        public Branch BranchDepartment { get; set; }

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

        //public SelectList SelListBranch { get; set; }

        public Department()
        {
        }

        public Department(string departmentCode, string departmentName)
        {
            DepartmentCode = departmentCode;
            DepartmentName = departmentName;
        }

        public static Department GetDepartment(string departmentCode)
        {
            Department department = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, departmentCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        department = new Department();
                        department.DepartmentCode = dr["CODE"] as string;
                        department.DepartmentName = dr["Name"] as string;

                        department.BranchDepartment = new Branch();
                        department.BranchDepartment.BranchCode = dr["branchcode"] as string;
                        department.BranchDepartment.BranchName = dr["branchname"] as string;
                        department.BranchDepartment.HOStatus = dr["HOStatus"] is DBNull ? false : Convert.ToBoolean(dr["HOStatus"]);
                        //department.BranchDepartment.NPWP = dr["NPWP"] as string;

                        //department.BranchDepartment.BranchManagerName = dr["BranchManager"] as string;
                        //department.BranchDepartment.FinAccOfficer = dr["FinAccOfficer"] as string;

                        //department.BranchDepartment.Address1 = dr["Addr1"] as string;
                        //department.BranchDepartment.Address2 = dr["Addr2"] as string;
                        //department.BranchDepartment.Address3 = dr["Addr3"] as string;
                        //department.BranchDepartment.Country = Country.GetCountry(Tool.GeneralHelper.NullToString(dr["CountryCode"]));
                        //department.BranchDepartment.City = City.GetCity(Tool.GeneralHelper.NullToString(dr["CityCode"]), Tool.GeneralHelper.NullToString(dr["CountryCode"]));

                        //department.BranchDepartment.PostalCode = dr["PostalCode"] as string;
                        //department.BranchDepartment.Phone1 = dr["Phone1"] as string;
                        //department.BranchDepartment.Phone2 = dr["Phone2"] as string;
                        //department.BranchDepartment.Phone3 = dr["Phone3"] as string;
                        //department.BranchDepartment.Fax = dr["Fax"] as string;

                        //department.BranchDepartment.FullAddress =
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr1"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr2"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr3"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(department.BranchDepartment.BranchCity?.CityName);

                        //// Print Option
                        //department.BranchDepartment.PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
                        //department.BranchDepartment.PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
                        //department.BranchDepartment.PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
                        //department.BranchDepartment.PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
                        //department.BranchDepartment.PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
                        //department.BranchDepartment.PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
                        //department.BranchDepartment.PrintPage = Convert.ToBoolean(dr["ChkPage"]);
                        //department.BranchDepartment.Language = Convert.ToBoolean(dr["OptIndex"]);

                        // GL
                        //department.BranchDepartment.RepNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["RepNo"]));
                        //department.BranchDepartment.HopNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["HopNo"]));

                        //// Leasing
                        //department.BranchDepartment.ContractLimit = Convert.ToDecimal(dr["ContractLimit"]);
                        //department.BranchDepartment.NUPApprovalLimit = Convert.ToDecimal(dr["NUPApprovalLimit"]);
                        //department.BranchDepartment.NUPExpiredDays = Convert.ToInt32(dr["NUPExpiredDay"]);
                        //department.BranchDepartment.OverRate = Convert.ToDecimal(dr["OverRate"]);
                        //department.BranchDepartment.OverRateRetail = Convert.ToDecimal(dr["OverRateRetail"]);
                        //department.BranchDepartment.PenaltyRate = Convert.ToDecimal(dr["PenaltyRate"]);
                        //department.BranchDepartment.PenaltyRateRetail = Convert.ToDecimal(dr["PenaltyRateRetail"]);
                        ////department.BranchDepartment.LastCount = Convert.ToInt32(dr["LastCount"]);
                        //department.BranchDepartment.LastReceiptNo = Convert.ToInt32(dr["LastReceiptNo"]);

                        // SLIK
                        //department.BranchDepartment.SLIKCode = dr["SLIKCode"] as string;

                        // Log
                        //department.BranchDepartment.EntryUser = dr["EntryUser"] as string;
                        //department.BranchDepartment.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        department.BranchDepartment.OperatorID = dr["OperatorID"] as string;
                        department.BranchDepartment.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                        //department.BranchDepartment.BranchCode = dr["BranchCode"] as string;
                        //department.BranchDepartment.BranchName = dr["BranchName"] as string;
                        //department.BranchDepartment.ContractLimit = Convert.ToDecimal(dr["ContractLimit"]);
                        //department.BranchDepartment.Address1 = dr["Address1"] as string;
                        //department.BranchDepartment.Address2 = dr["Address2"] as string;
                        //department.BranchDepartment.Address3 = dr["Address3"] as string;
                        //department.BranchDepartment.City = new City();

                        //department.EntryUser = dr["EntryUser"] as string;
                        //department.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        department.OperatorID = dr["OperatorID"] as string;
                        department.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return department;
        }

        public static Department GetDepartment(string departmentCode, string branchCode)
        {
            Department department = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, departmentCode);
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        department = new Department();
                        department.DepartmentCode = dr["CODE"] as string;
                        department.DepartmentName = dr["Name"] as string;


                        department.BranchDepartment = new Branch();
                        department.BranchDepartment.BranchCode = dr["branchcode"] as string;
                        department.BranchDepartment.BranchName = dr["branchname"] as string;
                        department.BranchDepartment.HOStatus = dr["HOStatus"] is DBNull ? false : Convert.ToBoolean(dr["HOStatus"]);
                        department.BranchDepartment.NPWP = dr["NPWP"] as string;

                        department.BranchDepartment.BranchManagerName = dr["BranchManager"] as string;
                        department.BranchDepartment.FinAccOfficer = dr["FinAccOfficer"] as string;

                        //department.BranchDepartment.Address1 = dr["Addr1"] as string;
                        //department.BranchDepartment.Address2 = dr["Addr2"] as string;
                        //department.BranchDepartment.Address3 = dr["Addr3"] as string;
                        ////department.BranchDepartment.Country = Country.GetCountry(Tool.GeneralHelper.NullToString(dr["CountryCode"]));
                        ////department.BranchDepartment.City = City.GetCity(Tool.GeneralHelper.NullToString(dr["CityCode"]), Tool.GeneralHelper.NullToString(dr["CountryCode"]));

                        //department.BranchDepartment.PostalCode = dr["PostalCode"] as string;
                        //department.BranchDepartment.Phone1 = dr["Phone1"] as string;
                        //department.BranchDepartment.Phone2 = dr["Phone2"] as string;
                        //department.BranchDepartment.Phone3 = dr["Phone3"] as string;
                        //department.BranchDepartment.Fax = dr["Fax"] as string;

                        //department.BranchDepartment.FullAddress =
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr1"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr2"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr3"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(department.BranchDepartment.BranchCity?.CityName);

                        //// Print Option
                        //department.BranchDepartment.PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
                        //department.BranchDepartment.PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
                        //department.BranchDepartment.PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
                        //department.BranchDepartment.PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
                        //department.BranchDepartment.PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
                        //department.BranchDepartment.PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
                        //department.BranchDepartment.PrintPage = Convert.ToBoolean(dr["ChkPage"]);
                        //department.BranchDepartment.Language = Convert.ToBoolean(dr["OptIndex"]);

                        //// GL
                        ////department.BranchDepartment.RepNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["RepNo"]));
                        ////department.BranchDepartment.HopNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["HopNo"]));

                        //// Leasing
                        //department.BranchDepartment.ContractLimit = Convert.ToDecimal(dr["ContractLimit"]);
                        //department.BranchDepartment.NUPApprovalLimit = Convert.ToDecimal(dr["NUPApprovalLimit"]);
                        //department.BranchDepartment.NUPExpiredDays = Convert.ToInt32(dr["NUPExpiredDay"]);
                        //department.BranchDepartment.OverRate = Convert.ToDecimal(dr["OverRate"]);
                        //department.BranchDepartment.OverRateRetail = Convert.ToDecimal(dr["OverRateRetail"]);
                        //department.BranchDepartment.PenaltyRate = Convert.ToDecimal(dr["PenaltyRate"]);
                        //department.BranchDepartment.PenaltyRateRetail = Convert.ToDecimal(dr["PenaltyRateRetail"]);
                        ////department.BranchDepartment.LastCount = Convert.ToInt32(dr["LastCount"]);
                        //department.BranchDepartment.LastReceiptNo = Convert.ToInt32(dr["LastReceiptNo"]);

                        // SLIK
                        //department.BranchDepartment.SLIKCode = dr["SLIKCode"] as string;

                        department.EntryUser = dr["EntryUser"] as string;
                        department.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        department.OperatorID = dr["OperatorID"] as string;
                        department.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return department;
        }

        public static Department GetDepartmentDistinct()
        {
            Department department = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 5);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        department = new Department();
                        department.DepartmentCode = dr["CODE"] as string;
                        department.DepartmentName = dr["Name"] as string;
                        //department.BranchDepartment = IDS.GeneralTable.Branch.GetBranch(dr["BranchCode"] as string);
                        department.EntryUser = dr["EntryUser"] as string;
                        department.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        department.OperatorID = dr["OperatorID"] as string;
                        department.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return department;
        }

        public static Department GetDepartmentWithBranchCode(string branchCode)
        {
            Department department = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@branchCode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        department = new Department();
                        department.DepartmentCode = dr["CODE"] as string;
                        department.DepartmentName = dr["Name"] as string;
                        department.BranchDepartment = new Branch();
                        department.BranchDepartment.BranchCode = dr["branchcode"] as string;
                        department.BranchDepartment.BranchName = dr["branchname"] as string;
                        department.BranchDepartment.HOStatus = dr["HOStatus"] is DBNull ? false : Convert.ToBoolean(dr["HOStatus"]);
                        department.BranchDepartment.NPWP = dr["NPWP"] as string;

                        department.BranchDepartment.BranchManagerName = dr["BranchManager"] as string;
                        department.BranchDepartment.FinAccOfficer = dr["FinAccOfficer"] as string;

                        //department.BranchDepartment.Address1 = dr["Addr1"] as string;
                        //department.BranchDepartment.Address2 = dr["Addr2"] as string;
                        //department.BranchDepartment.Address3 = dr["Addr3"] as string;
                        ////department.BranchDepartment.Country = Country.GetCountry(Tool.GeneralHelper.NullToString(dr["CountryCode"]));
                        ////department.BranchDepartment.City = City.GetCity(Tool.GeneralHelper.NullToString(dr["CityCode"]), Tool.GeneralHelper.NullToString(dr["CountryCode"]));

                        //department.BranchDepartment.PostalCode = dr["PostalCode"] as string;
                        //department.BranchDepartment.Phone1 = dr["Phone1"] as string;
                        //department.BranchDepartment.Phone2 = dr["Phone2"] as string;
                        //department.BranchDepartment.Phone3 = dr["Phone3"] as string;
                        //department.BranchDepartment.Fax = dr["Fax"] as string;

                        //department.BranchDepartment.FullAddress =
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr1"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr2"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(dr["Addr3"]) +
                        //        IDS.Tool.GeneralHelper.NullToString(department.BranchDepartment.BranchCity?.CityName);

                        //// Print Option
                        //department.BranchDepartment.PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
                        //department.BranchDepartment.PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
                        //department.BranchDepartment.PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
                        //department.BranchDepartment.PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
                        //department.BranchDepartment.PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
                        //department.BranchDepartment.PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
                        //department.BranchDepartment.PrintPage = Convert.ToBoolean(dr["ChkPage"]);
                        //department.BranchDepartment.Language = Convert.ToBoolean(dr["OptIndex"]);

                        //// GL
                        ////department.BranchDepartment.RepNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["RepNo"]));
                        ////department.BranchDepartment.HopNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["HopNo"]));

                        //// Leasing
                        //department.BranchDepartment.ContractLimit = Convert.ToDecimal(dr["ContractLimit"]);
                        //department.BranchDepartment.NUPApprovalLimit = Convert.ToDecimal(dr["NUPApprovalLimit"]);
                        //department.BranchDepartment.NUPExpiredDays = Convert.ToInt32(dr["NUPExpiredDay"]);
                        //department.BranchDepartment.OverRate = Convert.ToDecimal(dr["OverRate"]);
                        //department.BranchDepartment.OverRateRetail = Convert.ToDecimal(dr["OverRateRetail"]);
                        //department.BranchDepartment.PenaltyRate = Convert.ToDecimal(dr["PenaltyRate"]);
                        //department.BranchDepartment.PenaltyRateRetail = Convert.ToDecimal(dr["PenaltyRateRetail"]);
                        ////department.BranchDepartment.LastCount = Convert.ToInt32(dr["LastCount"]);
                        //department.BranchDepartment.LastReceiptNo = Convert.ToInt32(dr["LastReceiptNo"]);

                        // SLIK
                        //department.BranchDepartment.SLIKCode = dr["SLIKCode"] as string;

                        department.EntryUser = dr["EntryUser"] as string;
                        department.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        department.OperatorID = dr["OperatorID"] as string;
                        department.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return department;
        }

        /// <summary>
        /// Retrieve semua daftar Department
        /// </summary>
        /// <returns></returns>
        public static List<Department> GetDepartment()
        {
            List<IDS.GeneralTable.Department> list = new List<Department>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            Department department = new Department();
                            department = new Department();
                            department.DepartmentCode = dr["Code"] as string;
                            department.DepartmentName = dr["Name"] as string;
                            
                            department.BranchDepartment = new Branch();
                            department.BranchDepartment.BranchCode = dr["branchcode"] as string;
                            department.BranchDepartment.BranchName = dr["branchname"] as string;
                            department.BranchDepartment.HOStatus = dr["HOStatus"] is DBNull ? false : Convert.ToBoolean(dr["HOStatus"]);
                            department.BranchDepartment.NPWP = dr["NPWP"] as string;

                            department.BranchDepartment.BranchManagerName = dr["BranchManager"] as string;
                            department.BranchDepartment.FinAccOfficer = dr["FinAccOfficer"] as string;

                            //department.BranchDepartment.Address1 = dr["Addr1"] as string;
                            //department.BranchDepartment.Address2 = dr["Addr2"] as string;
                            //department.BranchDepartment.Address3 = dr["Addr3"] as string;
                            //department.BranchDepartment.Country = Country.GetCountry(Tool.GeneralHelper.NullToString(dr["CountryCode"]));
                            //department.BranchDepartment.City = City.GetCity(Tool.GeneralHelper.NullToString(dr["CityCode"]), Tool.GeneralHelper.NullToString(dr["CountryCode"]));

                            //department.BranchDepartment.PostalCode = dr["PostalCode"] as string;
                            //department.BranchDepartment.Phone1 = dr["Phone1"] as string;
                            //department.BranchDepartment.Phone2 = dr["Phone2"] as string;
                            //department.BranchDepartment.Phone3 = dr["Phone3"] as string;
                            //department.BranchDepartment.Fax = dr["Fax"] as string;

                            //department.BranchDepartment.FullAddress =
                            //        IDS.Tool.GeneralHelper.NullToString(dr["Addr1"]) +
                            //        IDS.Tool.GeneralHelper.NullToString(dr["Addr2"]) +
                            //        IDS.Tool.GeneralHelper.NullToString(dr["Addr3"]) +
                            //        IDS.Tool.GeneralHelper.NullToString(department.BranchDepartment.BranchCity?.CityName);

                            //// Print Option
                            //department.BranchDepartment.PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
                            //department.BranchDepartment.PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
                            //department.BranchDepartment.PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
                            //department.BranchDepartment.PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
                            //department.BranchDepartment.PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
                            //department.BranchDepartment.PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
                            //department.BranchDepartment.PrintPage = Convert.ToBoolean(dr["ChkPage"]);
                            //department.BranchDepartment.Language = Convert.ToBoolean(dr["OptIndex"]);

                            // GL
                            //department.BranchDepartment.RepNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["RepNo"]));
                            //department.BranchDepartment.HopNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["HopNo"]));

                            //// Leasing
                            //department.BranchDepartment.ContractLimit = Convert.ToDecimal(dr["ContractLimit"]);
                            //department.BranchDepartment.NUPApprovalLimit = Convert.ToDecimal(dr["NUPApprovalLimit"]);
                            //department.BranchDepartment.NUPExpiredDays = Convert.ToInt32(dr["NUPExpiredDay"]);
                            //department.BranchDepartment.OverRate = Convert.ToDecimal(dr["OverRate"]);
                            //department.BranchDepartment.OverRateRetail = Convert.ToDecimal(dr["OverRateRetail"]);
                            //department.BranchDepartment.PenaltyRate = Convert.ToDecimal(dr["PenaltyRate"]);
                            //department.BranchDepartment.PenaltyRateRetail = Convert.ToDecimal(dr["PenaltyRateRetail"]);
                            ////department.BranchDepartment.LastCount = Convert.ToInt32(dr["LastCount"]);
                            //department.BranchDepartment.LastReceiptNo = Convert.ToInt32(dr["LastReceiptNo"]);

                            // SLIK
                            //department.BranchDepartment.SLIKCode = dr["SLIKCode"] as string;

                            department.EntryUser = dr["EntryUser"] as string;
                            department.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            department.OperatorID = dr["OperatorID"] as string;
                            department.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            list.Add(department);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static List<Department> GetDepartmentList(string branchCode)
        {
            List<IDS.GeneralTable.Department> list = new List<Department>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 3);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Department department = new Department();
                            department.DepartmentCode = dr["Code"] as string;
                            department.DepartmentName = dr["Name"] as string;


                            department.BranchDepartment = new Branch();
                            department.BranchDepartment.BranchCode = dr["branchcode"] as string;
                            department.BranchDepartment.BranchName = dr["branchname"] as string;
                            department.BranchDepartment.HOStatus = dr["HOStatus"] is DBNull ? false : Convert.ToBoolean(dr["HOStatus"]);
                            department.BranchDepartment.NPWP = dr["NPWP"] as string;

                            department.BranchDepartment.BranchManagerName = dr["BranchManager"] as string;
                            department.BranchDepartment.FinAccOfficer = dr["FinAccOfficer"] as string;

                            //department.BranchDepartment.Address1 = dr["Addr1"] as string;
                            //department.BranchDepartment.Address2 = dr["Addr2"] as string;
                            //department.BranchDepartment.Address3 = dr["Addr3"] as string;
                            ////department.BranchDepartment.Country = Country.GetCountry(Tool.GeneralHelper.NullToString(dr["CountryCode"]));
                            ////department.BranchDepartment.City = City.GetCity(Tool.GeneralHelper.NullToString(dr["CityCode"]), Tool.GeneralHelper.NullToString(dr["CountryCode"]));

                            //department.BranchDepartment.PostalCode = dr["PostalCode"] as string;
                            //department.BranchDepartment.Phone1 = dr["Phone1"] as string;
                            //department.BranchDepartment.Phone2 = dr["Phone2"] as string;
                            //department.BranchDepartment.Phone3 = dr["Phone3"] as string;
                            //department.BranchDepartment.Fax = dr["Fax"] as string;

                            //department.BranchDepartment.FullAddress =
                            //        IDS.Tool.GeneralHelper.NullToString(dr["Addr1"]) +
                            //        IDS.Tool.GeneralHelper.NullToString(dr["Addr2"]) +
                            //        IDS.Tool.GeneralHelper.NullToString(dr["Addr3"]) +
                            //        IDS.Tool.GeneralHelper.NullToString(department.BranchDepartment.BranchCity?.CityName);

                            //// Print Option
                            //department.BranchDepartment.PrintBranchName = Convert.ToBoolean(dr["Chknama"]);
                            //department.BranchDepartment.PrintBranchAddress = Convert.ToBoolean(dr["Chkaddress"]);
                            //department.BranchDepartment.PrintBranchCity = Convert.ToBoolean(dr["Chkcity"]);
                            //department.BranchDepartment.PrintBranchCountry = Convert.ToBoolean(dr["Chkcountry"]);
                            //department.BranchDepartment.PrintDate = Convert.ToBoolean(dr["ChkPrintDate"]);
                            //department.BranchDepartment.PrintTime = Convert.ToBoolean(dr["ChkPrintTime"]);
                            //department.BranchDepartment.PrintPage = Convert.ToBoolean(dr["ChkPage"]);
                            //department.BranchDepartment.Language = Convert.ToBoolean(dr["OptIndex"]);

                            //// GL
                            ////department.BranchDepartment.RepNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["RepNo"]));
                            ////department.BranchDepartment.HopNo = IDS.GL.ChartOfAccount.GetCOA(IDS.Tool.GeneralHelper.NullToString(dr["HopNo"]));

                            //// Leasing
                            //department.BranchDepartment.ContractLimit = Convert.ToDecimal(dr["ContractLimit"]);
                            //department.BranchDepartment.NUPApprovalLimit = Convert.ToDecimal(dr["NUPApprovalLimit"]);
                            //department.BranchDepartment.NUPExpiredDays = Convert.ToInt32(dr["NUPExpiredDay"]);
                            //department.BranchDepartment.OverRate = Convert.ToDecimal(dr["OverRate"]);
                            //department.BranchDepartment.OverRateRetail = Convert.ToDecimal(dr["OverRateRetail"]);
                            //department.BranchDepartment.PenaltyRate = Convert.ToDecimal(dr["PenaltyRate"]);
                            //department.BranchDepartment.PenaltyRateRetail = Convert.ToDecimal(dr["PenaltyRateRetail"]);
                            ////department.BranchDepartment.LastCount = Convert.ToInt32(dr["LastCount"]);
                            //department.BranchDepartment.LastReceiptNo = Convert.ToInt32(dr["LastReceiptNo"]);

                            // SLIK
                            //department.BranchDepartment.SLIKCode = dr["SLIKCode"] as string;

                            // Log
                            department.BranchDepartment.EntryUser = dr["EntryUser"] as string;
                            department.BranchDepartment.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            department.BranchDepartment.OperatorID = dr["OperatorID"] as string;
                            department.BranchDepartment.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);


                            department.EntryUser = dr["EntryUser"] as string;
                            department.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            department.OperatorID = dr["OperatorID"] as string;
                            department.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            list.Add(department);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public int InsUpDelDepartment(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from acfdept WHERE CODE = '" + DepartmentCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "GTUpdateDept";
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@Code", System.Data.SqlDbType.VarChar, DepartmentCode.ToString());
                    cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchDepartment.BranchCode.ToString());
                    cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, DepartmentName.ToString());
                    cmd.AddParameter("@operatorID", System.Data.SqlDbType.VarChar, OperatorID.ToString());
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    if (ExecCode != 3)
                    {
                        newData = log.GenLogArray("SELECT *  from acfdept WHERE CODE = '" + DepartmentCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                        if (ExecCode == 1)
                        {
                            log.SaveSysLog1("", newData, "acfdept", DepartmentCode, OperatorID, "INSERT");
                        }
                        else
                        {
                            log.SaveSysLog1(oldData, newData, "acfdept", DepartmentCode, OperatorID, "UPDATE");
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
                            throw new Exception("Department code is already exists. Please choose other Department code.");
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

        public int InsUpDelDepartment(int ExecCode, string[] data,string dataBranch)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {   //LOG
                    Log log = new Log();
                    string oldData = "";
                    oldData = log.GenLogArray("SELECT * from acfdept WHERE CODE = '" + DepartmentCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");

                    cmd.CommandText = "GTUpdateDept";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "GTUpdateDept";
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@code", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, dataBranch);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    log.SaveSysLog1(oldData, "", " ", DepartmentCode, OperatorID, "DELETE");
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Department Code is already exists. Please choose other Department Code.");
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

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDepartmentForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
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
                            string departmentCode = dr["CODE"].ToString();
                            string departmentName = dr["Name"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = departmentName, Value = departmentCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDepartmentForDataSource(string branchCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 6);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string departmentCode = dr["CODE"].ToString();
                            string departmentName = dr["Name"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = departmentName, Value = departmentCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDepartmentForDataSources(string branchCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 7);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string departmentCode = dr["CODE"].ToString();
                            string departmentName = dr["Name"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = departmentName, Value = departmentCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDepartmentForFAAsset(string branchCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 7);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string departmentCode = dr["CODE"].ToString();
                            string departmentName = dr["Name"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = departmentName, Value = departmentCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDepartmentForDataSourceFromBranch(string branchCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 7);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string departmentCode = dr["CODE"].ToString();
                            string departmentName = dr["Name"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = departmentName, Value = departmentCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDepartmentForDataSourceBRCode(string branchCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT CODE,Name from ACFDEPT WHERE BranchCode = @branchcode";
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string departmentCode = dr["CODE"].ToString();
                            string departmentName = dr["Name"].ToString();
                            list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = departmentName, Value = departmentCode });
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }
        public static DataTable GetDepartmentDataForReport()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelDept";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();


                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
