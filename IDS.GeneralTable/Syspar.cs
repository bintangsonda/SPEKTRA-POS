using IDS.GeneralTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;
using System.Data;

namespace IDS.GeneralTable
{
    public sealed class Syspar
    {
        private static Syspar _instance = null;

        public int Code { get; set; }
        public string Name { get; set; }
        public string BaseCCy { get; set; }
        public string Department { get; set; }
        public string Version { get; set; }
        public int CollDate { get; set; }
        public bool AdditionalVoucher { get; set; }
        public bool DefferedIncome { get; set; }

        #region Contact
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CountryCode { get; set; }        
        public string Phone { get; set; }
        public string Fax { get; set; }
        #endregion

        #region Leeasing
        //public string OJK_SLIK_ID { get; set; }        
        //public int FundSource { get; set; }
        //public string FundSourceNo { get; set; }
        //public bool RetailContractAMApprovalNeeds { get; set; }
        #endregion

        #region GL Setting
        public DateTime? StartFiscalYear { get; set; }
        #endregion

        #region Print Report Setting
        public bool PrintName { get; set; }
        public bool PrintAddress { get; set; }
        public bool PrintCity { get; set; }
        public bool PrintCountry { get; set; }
        public bool PrintDate { get; set; }
        public bool PrintTime { get; set; }
        public bool PrintPageNumber { get; set; }
        public string Language { get; set; }
        #endregion
        
        private Syspar()
        {
            LoadData();
        }

        public static Syspar GetInstance()
        {
            if (_instance == null)
                _instance = new Syspar();
            return _instance;
        }

        private void LoadData()
        {
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "GTSelSyspar";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        Code = Convert.ToInt32(dr["Code"]);
                        Name = IDS.Tool.GeneralHelper.NullToString(dr["Name"]);
                        BaseCCy = IDS.Tool.GeneralHelper.NullToString(dr["BaseCcy"]);
                        Version = IDS.Tool.GeneralHelper.NullToString(dr["version"]);

                        Address1 = IDS.Tool.GeneralHelper.NullToString(dr["Address-1"]);
                        Address2 = IDS.Tool.GeneralHelper.NullToString(dr["Address-2"]);
                        Address3 = IDS.Tool.GeneralHelper.NullToString(dr["Address-3"]);
                        CountryCode = IDS.Tool.GeneralHelper.NullToString(dr["CountryCode"]);
                        Phone = IDS.Tool.GeneralHelper.NullToString(dr["telp"]);
                        Fax = IDS.Tool.GeneralHelper.NullToString(dr["fax"]);
                        //OJK_SLIK_ID = IDS.Tool.GeneralHelper.NullToString(dr["OJK_SLIK_ID"]);

                        // GL
                        StartFiscalYear = dr["StartFiscalYear"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["StartFiscalYear"]);

                        // Print Setting
                        PrintName = IDS.Tool.GeneralHelper.NullToBool(dr["chknama"], false);
                        PrintAddress = IDS.Tool.GeneralHelper.NullToBool(dr["chkaddress"], false);
                        PrintCity = IDS.Tool.GeneralHelper.NullToBool(dr["chkcity"], false);
                        PrintCountry = IDS.Tool.GeneralHelper.NullToBool(dr["chkcountry"], false);
                        PrintDate = IDS.Tool.GeneralHelper.NullToBool(dr["chkPrintDate"], false);
                        PrintTime = IDS.Tool.GeneralHelper.NullToBool(dr["chkPrintTime"], false);
                        PrintPageNumber = IDS.Tool.GeneralHelper.NullToBool(dr["chkPage"], false);
                        Language = IDS.Tool.GeneralHelper.NullToString(dr["optIndex"]);
                        CollDate = IDS.Tool.GeneralHelper.NullToInt(dr["CollectionDate"], 1);

                        //Add by Jeremi 20 September 2024
                        //DefferedIncome = IDS.Tool.GeneralHelper.NullToBool(dr["DefferedIncome"], false);
                        //End Jeremi

                        // Leasing
                        //FundSource = IDS.Tool.GeneralHelper.NullToInt(dr["FundSource"], 0);
                        //FundSourceNo = IDS.Tool.GeneralHelper.NullToString(dr["FundSourceNo"]);
                        // TODO: Check apakah kepake atau tidak. Jika tidak hapus dan di DB hapus juga
                        //RetailContractAMApprovalNeeds = IDS.Tool.GeneralHelper.NullToBool(dr["RetailContractAMApprovalNeeds"]);

                    }
                }
            }
        }

        public void RefreshData()
        {
            this.LoadData();
        }

        public static DataTable GetRptSaldoAccr(string period)
        {
            DateTime B = Convert.ToDateTime(period);
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "FT_RptSaldoPembiayaan";
                db.AddParameter("@Period", System.Data.SqlDbType.DateTime, B);
                db.AddParameter("@ExDate", System.Data.SqlDbType.DateTime, B);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
        public int UpdateSyspar()
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    string Kode = Convert.ToString(Code);
                    Log log = new Log();
                    string oldData = "";
                    string newData = "";
                    oldData = log.GenLogArray("SELECT * from SYSPAR Where Code = '" + Code + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    cmd.CommandText = "SYSPARUpdate";
                    cmd.AddParameter("@ver", System.Data.SqlDbType.VarChar, Version);
                    cmd.AddParameter("@info",System.Data.SqlDbType.VarChar, Version);
                    cmd.AddParameter("@FundSource", System.Data.SqlDbType.VarChar, 1);
                    cmd.AddParameter("@FundSourceID", System.Data.SqlDbType.VarChar, Version);
                    cmd.AddParameter("@Name", System.Data.SqlDbType.VarChar, Name);
                    cmd.AddParameter("@address1", System.Data.SqlDbType.VarChar, Address1);
                    cmd.AddParameter("@address2", System.Data.SqlDbType.VarChar, Address2);
                    cmd.AddParameter("@address3", System.Data.SqlDbType.VarChar, Address3);
                    //cmd.AddParameter("@info", System.Data.SqlDbType.VarChar, IDS.Tool.GeneralHelper.StringToDBNull(InfoListDir));
                    cmd.AddParameter("@countrycode", System.Data.SqlDbType.VarChar, CountryCode);
                    cmd.AddParameter("@baseccy", System.Data.SqlDbType.VarChar, BaseCCy);
                    cmd.AddParameter("@dept", System.Data.SqlDbType.VarChar, Department);
                    cmd.AddParameter("@startfiscal", System.Data.SqlDbType.DateTime, StartFiscalYear);
                    cmd.AddParameter("@telp", System.Data.SqlDbType.VarChar, Phone);
                    cmd.AddParameter("@fax", System.Data.SqlDbType.VarChar, Fax);
                    //cmd.AddParameter("@mobile", System.Data.SqlDbType.VarChar, Mobile);
                    cmd.AddParameter("@chknama", System.Data.SqlDbType.VarChar, PrintName == true ? "True" : "false");
                    cmd.AddParameter("@chkaddress", System.Data.SqlDbType.VarChar, PrintAddress == true ? "True" : "false");
                    cmd.AddParameter("@chkcity", System.Data.SqlDbType.VarChar, PrintCity == true ? "True" : "false");
                    cmd.AddParameter("@chkcountry", System.Data.SqlDbType.VarChar, PrintCountry == true ? "True" : "false");
                    cmd.AddParameter("@chkprintdate", System.Data.SqlDbType.VarChar, PrintDate == true ? "True" : "false");
                    cmd.AddParameter("@chkprinttime", System.Data.SqlDbType.VarChar, PrintTime == true ? "True" : "false");
                    cmd.AddParameter("@chkpage", System.Data.SqlDbType.VarChar, PrintPageNumber == true ? "true" : "false");
                    cmd.AddParameter("@chkAdditionalVoucher", System.Data.SqlDbType.VarChar, AdditionalVoucher == true ? "true" : "false");
                    cmd.AddParameter("@optindex", System.Data.SqlDbType.VarChar, Language);
                    //Add by Jeremi 20 September 2024
                    //cmd.AddParameter("@chkDefferedIncome", System.Data.SqlDbType.VarChar, DefferedIncome == true ? "true" : "false");
                    //End Jeremi

                    //cmd.AddParameter("@FundSource", System.Data.SqlDbType.VarChar, FundSource);
                    //cmd.AddParameter("@FundSourceId", System.Data.SqlDbType.VarChar, FundSourceNo);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    newData = log.GenLogArray("SELECT * from SYSPAR Where Code ='" + Code + "' FOR JSON AUTO, INCLUDE_NULL_VALUES");
                    log.SaveSysLog1(oldData, newData, "SYSPAR", Kode, Name, "UPDATE");

                    Syspar.GetInstance().RefreshData();
                }
                catch (Microsoft.Data.SqlClient.SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Syspar id is already exists. Please choose other Tax id.");
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

        //Add By Renaldi 4 September 2024
        public static DateTime GetFiscalYear()
        {
            DateTime fiscalYear = DateTime.Now;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT StartFiscalYear FROM SYSPAR where Code=1";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                fiscalYear = (DateTime)db.ExecuteScalar();
            }

            return fiscalYear;
        }
        //End Add
    }
}