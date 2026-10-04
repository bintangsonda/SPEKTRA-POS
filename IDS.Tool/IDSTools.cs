using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;
using System.Web;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using IDS.Tool;
using Microsoft.Data.SqlClient;
using System.Reflection;
using IDS.DataAccess;
using System.Text.RegularExpressions;
using System.Net;


namespace AppCode
{
    public enum FormAction : byte
    {
        Insert = 1,
        Update = 2,
        Delete = 3
    }

    public static class IDSTools
    {
        public static decimal GetAccrResh(string leaseno, int period)
        {
            decimal result = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {


                db.CommandText = "SELECT Accrual2 as Hasil FROM Schedule where period = @period and leaseno = @leaseno";
                db.CommandType = System.Data.CommandType.Text;

                db.AddParameter("@period", SqlDbType.TinyInt, period-1);
                db.AddParameter("@leaseno", SqlDbType.VarChar, leaseno);

                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result = Convert.ToDecimal(dr["Hasil"]);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return result;
        }
        public static int CheckClosing(string period)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                db.CommandText = "SELECT Closing FROM ACFMCTR where period = @period";
                db.CommandType = System.Data.CommandType.Text;
                db.ClearParameter();
                db.AddParameter("@period", SqlDbType.VarChar, period);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result = Convert.ToInt32(dr["Closing"]);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return result;

        }
        public static int CheckJCode(string Jcode)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {


                db.CommandText = "SELECT COUNT(Jcode) as Hasil FROM LeJournalTD where Jcode = @Jcode";
                db.CommandType = System.Data.CommandType.Text;

                db.AddParameter("@Jcode", SqlDbType.VarChar, Jcode);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            result = IDS.Tool.GeneralHelper.NullToInt(dr["Hasil"], 0);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return result;
        }
        public static List<SelectListItem> GetLeaseForPaymentLunasDatasource(string clientID, string clientBranch)
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select LeaseNo from Lease where LesseeNo = @clientID and LesseeBranch = @clientBranch";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@clientID", System.Data.SqlDbType.VarChar, clientID);
                db.AddParameter("@clientBranch", System.Data.SqlDbType.VarChar, clientBranch);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["LeaseNo"].ToString();
                            l.Value = dr["LeaseNo"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return RP;
        }
        public static int UpdateAccrResch(string leaseno)
        {
            int hasil = 0;
            using (IDS.DataAccess.SqlServer db = new SqlServer())
            {
                try
                {
                    db.CommandText = "LeUpdAccrual";
                    db.CommandType = CommandType.StoredProcedure;
                    db.ClearParameter();
                    db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                    db.Open();
                    db.BeginTransaction();
                    hasil = db.ExecuteNonQuery();
                    db.CommitTransaction();
                }
                catch (SqlException ex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
            }
            return hasil;
        }
        public static int UpdateAdjResch(string leaseno, string reschno)
        {
            int hasil = 0;
            using (IDS.DataAccess.SqlServer db = new SqlServer())
            {
                try
                {
                    db.CommandText = "LeReschAdjustUpd";
                    db.CommandType = CommandType.StoredProcedure;
                    db.ClearParameter();
                    db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                    db.AddParameter("@ReschNo", SqlDbType.VarChar, reschno);
                    db.Open();
                    db.BeginTransaction();
                    hasil = db.ExecuteNonQuery();
                   

                    db.CommitTransaction();
                }
                catch (SqlException ex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
            }
            return hasil;
        }
        public static int UpdateAccrualResch(string leaseno, double amt, int period)
        {
            int hasil = 0;
            using (IDS.DataAccess.SqlServer db = new SqlServer())
            {
                try
                {
                    db.CommandText = "Update schedule set accrual2 = @amt where leaseno = @leaseno and period = @period";
                    db.CommandType = CommandType.Text;
                    db.ClearParameter();
                    db.AddParameter("@amt", SqlDbType.Money, amt);
                    db.AddParameter("@leaseno", SqlDbType.VarChar, leaseno);
                    db.AddParameter("@period", SqlDbType.TinyInt, period - 1);

                    db.Open();
                    db.BeginTransaction();
                    hasil = db.ExecuteNonQuery();
                    db.CommandText = "Update schedule set accrual1 = leincome - @amt where leaseno = @leaseno and period = @period";
                    db.CommandType = CommandType.Text;
                    db.ClearParameter();

                    db.AddParameter("@amt", SqlDbType.Money, amt);
                    db.AddParameter("@leaseno", SqlDbType.VarChar, leaseno);
                    db.AddParameter("@period", SqlDbType.TinyInt, period);
                    hasil = db.ExecuteNonQuery();

                    db.CommitTransaction();
                }
                catch (SqlException ex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
            }
            return hasil;
        }
        public static int UpdateCustomerDepositLease(string leaseno, double amt)
        {
            int hasil = 0;
            using (IDS.DataAccess.SqlServer db = new SqlServer())
            {
                try
                {
                    db.CommandText = "Update Lease SET DebtorDeposit = @amt where leaseno = @leaseno";
                    db.CommandType = CommandType.Text;
                    db.AddParameter("@amt", SqlDbType.Money, amt);
                    db.AddParameter("@leaseno", SqlDbType.VarChar, leaseno);
                    db.Open();
                    db.BeginTransaction();
                    hasil = db.ExecuteNonQuery();
                    db.CommitTransaction();
                }
                catch (SqlException ex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
            }
            return hasil;
        }
        public static int UpdateReverseLeasePay(int period, string leaseno, double amt)
        {
            int hasil = 0;
            using(IDS.DataAccess.SqlServer db = new SqlServer())
            {
                try
                {
                    db.CommandText = "Update Schedule SET Payment -= @amt where leaseno = @leaseno and period = @period";
                    db.CommandType = CommandType.Text;
                    db.AddParameter("@amt", SqlDbType.Money, amt);
                    db.AddParameter("@period", SqlDbType.Int, period);
                    db.AddParameter("@leaseno", SqlDbType.VarChar, leaseno);
                    db.Open();
                    db.BeginTransaction();
                    hasil = db.ExecuteNonQuery();
                    db.CommitTransaction();
                }
                catch(SqlException ex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
            }
            return hasil;
        }
        public static List<SelectListItem> GetWriteOffStatus()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Dihapusbukukan", Value = "03" });
            category.Add(new SelectListItem() { Text = "Hapus Tagih", Value = "04" });

            return category;
        }
        public static List<SelectListItem> GetFundingAgreementType()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Dibawah Tangan", Value = "0" });
            category.Add(new SelectListItem() { Text = "Legalisasi", Value = "1" });
            category.Add(new SelectListItem() { Text = "Notarial", Value = "2" });

            return category;
        }
        public static List<SelectListItem> GetSOAStatusFactoring()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Waiting Approval", Value = "0" });
            category.Add(new SelectListItem() { Text = "Approved", Value = "1" });
            category.Add(new SelectListItem() { Text = "Disburst", Value = "2" });


            return category;
        }
        public static List<SelectListItem> GetTerminationStatusFactoring()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Waiting Approval", Value = "0" });
            category.Add(new SelectListItem() { Text = "Approved", Value = "1" });

            return category;
        }
        public static List<SelectListItem> GetTerminationTypeFactoring()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Pelunasan Dipercepat", Value = "0" });
            category.Add(new SelectListItem() { Text = "Pembayaran Pokok", Value = "1" });

            return category;
        }
        public static List<SelectListItem> GetActiveProductContract(int corpRetail, int financeMethod, int goodsService, int leaseType, bool withNull)
        {
            List<SelectListItem> category = new List<SelectListItem>();

            string strSql = " ";

            //#region LeaseMethod
            //switch (LeaseMethod)
            //{
            //    case 1:
            //        strSql += " AND (LeaseMethod IN (1, 3)) ";
            //        break;
            //    case 2:
            //        strSql += " AND (LeaseMethod IN (2, 3)) ";
            //        break;
            //}
            //#endregion

            #region FinanceMethod
            switch (financeMethod)
            {
                case 1:
                    strSql += " AND (FinanceMethod IN (1, 3, 5, 7)) ";
                    break;
                case 2:
                    strSql += " AND (FinanceMethod IN (2, 3, 5, 7)) ";
                    break;
                case 3:
                    strSql += " AND (FinanceMethod IN (4, 5, 6, 7)) ";
                    break;
            }
            #endregion

            #region GoodsServices
            switch (goodsService)
            {
                case 1:
                    strSql += " AND GoodsService IN (1, 3) ";
                    break;
                case 2:
                    strSql += " AND GoodsService IN (2, 3) ";
                    break;
            }
            #endregion

            #region LeaseType
            switch (leaseType)
            {
                case 1:
                    strSql += " AND LeaseType IN (1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31) ";
                    break;
                case 2:
                    strSql += " AND LeaseType IN (2, 3, 6, 7, 10, 11, 14, 15, 18, 19, 22, 23, 26, 27, 30, 31) ";
                    break;
                case 3:
                    strSql += " AND LeaseType IN (4, 5, 6, 7, 12, 13, 14, 15, 20, 21, 22, 23, 28, 29, 30, 31) ";
                    break;
                case 4:
                    strSql += " AND LeaseType IN (8, 9, 10, 11, 12, 13, 14, 15, 24, 25, 26, 27, 28, 29, 30, 31) ";
                    break;
                case 5:
                    strSql += " AND LeaseType IN (16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31) ";
                    break;
            }
            #endregion

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                if (withNull)
                    db.CommandText = "SELECT NULL AS ProdCode, '' AS ProdName UNION ";
                else
                    db.CommandText = "";

                db.CommandText += "SELECT ProdCode, ProdName FROM LeProduct WHERE Status = 1 " + strSql;
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static string GetCustNameFromAgreementNo(string id)
        {
            string custName = "";
       
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select LesseeFullName as A FROM FTTransH H INNER JOIN Lessee L ON H.ClientID = L.LesseeNo AND H.ClientBranch = L.BranchCode WHERE AgreementNo = @id";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@id", System.Data.SqlDbType.VarChar, id);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            custName = IDS.Tool.GeneralHelper.NullToString(dr["A"]);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
            }
            return custName;
        }
        public static int GetLeaseTypeLease(string leaseno)
        {
            int custName = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select LeaseType as A FROM lease where leaseno = @id";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@id", System.Data.SqlDbType.VarChar, leaseno);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            custName = IDS.Tool.GeneralHelper.NullToInt(dr["A"], 1);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
            }
            return custName;
        }
        public static int GetFinanceMethodLease(string leaseno)
        {
            int custName = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select FinanceMethod as A FROM lease where leaseno = @id";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@id", System.Data.SqlDbType.VarChar, leaseno);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            custName = IDS.Tool.GeneralHelper.NullToInt(dr["A"],1);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
            }
            return custName;
        }
        public static string GetCustNameFromSOA(string id)
        {
            string custName = "";

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select LesseeFullName as A from fttransd d inner join fttransh h on d.AgreementNo = h.AgreementNo inner join lessee l on l.LesseeNo = h.ClientID where d.InvNo = @id";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@id", System.Data.SqlDbType.VarChar, id);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            custName = IDS.Tool.GeneralHelper.NullToString(dr["A"]);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
            }
            return custName;
        }
        public static string GetCustNameFromLeaseNo(string id)
        {
            string custName = "";

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select LesseeFullName as A FROM Lease le INNER JOIN Lessee L ON le.lesseeno = L.LesseeNo AND le.lesseebranch = L.BranchCode WHERE leaseno = @id";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@id", System.Data.SqlDbType.VarChar, id);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            custName = IDS.Tool.GeneralHelper.NullToString(dr["A"]);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
            }
            return custName;
        }
        public static string GetCustIDFromAgreementNo(string id)
        {
            string custName = "";

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select LesseeNo as A FROM FTTransH H INNER JOIN Lessee L ON H.ClientID = L.LesseeNo AND H.ClientBranch = L.BranchCode WHERE AgreementNo = @id";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@id", System.Data.SqlDbType.VarChar, id);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            custName = IDS.Tool.GeneralHelper.NullToString(dr["A"]);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
            }
            return custName;
        }
        public static double GetLeAmt(string leaseno)
        {
            double res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                //db.CommandText = "LeGetLeAmt";
                db.CommandText = "LeGetLeAmtAdvFee";
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            res = IDS.Tool.GeneralHelper.NullToDouble(dr["Result"],0);
                            }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return res;
        }
        public static double GetAmtProvisionLeasing(string leaseno)
        {
            double res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                //db.CommandText = "LeGetLeAmt";
                db.CommandText = "LeGetLeProvisionLease";
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            res = IDS.Tool.GeneralHelper.NullToDouble(dr["Result"], 0);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return res;
        }
        public static double GetAmtTerminationBunga(string leaseno)
        {
            double res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                //db.CommandText = "LeGetLeAmt";
                db.CommandText = "LeGetTermIntAmt";
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            res = IDS.Tool.GeneralHelper.NullToDouble(dr["Result"], 0);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return res;
        }
        public static double GetAmtTerminationPrincipal(string leaseno)
        {
            double res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                //db.CommandText = "LeGetLeAmt";
                db.CommandText = "LeGetTermPrincAmt";
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            res = IDS.Tool.GeneralHelper.NullToDouble(dr["Result"], 0);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return res;
        }
        public static double GetAmtNonAdvanceFee(string leaseno)
        {
            double res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                //db.CommandText = "LeGetLeAmt";
                db.CommandText = "LeGetLeAmtFee";
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            res = IDS.Tool.GeneralHelper.NullToDouble(dr["Result"], 0);
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return res;
        }
        public static string GetLesseeName(string lesseeNo, string branch)
        {
            string res = "";
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                db.CommandText = "select LesseeName from Lessee where LesseeNo = @lesseeNo AND BranchCode = @branch";
                db.AddParameter("@lesseeNo", SqlDbType.VarChar, lesseeNo);
                db.AddParameter("@branch", SqlDbType.VarChar, branch);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            res = dr["LesseeName"].ToString();
                        }

                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return res;
        }
        public static int GetRequestValid(string url, string key)
        {
            int res = 0;

            string[] List = key.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string Sql1 = "SELECT * FROM MntMenuRequestApproval WHERE MenuUrl = @MenuUrl AND ";

            for (int i = 0; i < List.Length; i++)
            {
                Sql1 += "KeyField" + (i + 1) + " = '" + List[i] + "'";
            }
            Sql1 += " AND RequestStatus = 0";
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                db.CommandText = Sql1;
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@MenuUrl", SqlDbType.VarChar, url);
                db.Open();
                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        res = 1;
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return res;
        }
        public static string GetUserEmailGroup(string branch, string group)
        {
            string user = "";
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                db.CommandText = "SELECT EmailAddress FROM MntUser WHERE BranchCode = @branch AND GroupCode = @group and Status = 1";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@branch", SqlDbType.VarChar, branch);
                db.AddParameter("@group", SqlDbType.VarChar, group);
                db.Open();
                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        string regexPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,6})+)$";

                        while (dr.Read())
                        {
                            if (dr["EmailAddress"] == DBNull.Value || dr["EmailAddress"] == "" || !Regex.Match(dr["EmailAddress"] as string, regexPattern).Success)
                                continue;

                            user += Convert.ToString(dr["EmailAddress"]) + ";";

                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return user;
        }
        /// <summary>
        /// Cek apakah user group parent memiliki list user pada cabang atau tidak.
        /// Jika tidak ada, asumsi bahwa group tersebut merupakan group user pusat / KPNO, sehingga yang akan melakukan approval menu adalah orang pusat / KPNO.
        /// Jika ada, maka yang akan melakukan approval adalah user cabang dari group parent.
        /// </summary>
        /// <param name="userGroupCode">User Group Parent</param>
        /// <returns></returns>
        public static bool IsUserListGroupExistsForBranch(string parentUserGroupCode, string userBranchCode)
        {
            bool result = false;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "IF EXISTS(SELECT GroupCode FROM MntUser WHERE Status = 1 AND GroupCode = @parentGroupCode AND BranchCode = @userBranchCode AND BranchCode <> (SELECT TOP 1 BranchCode FROM tblBranch WHERE (ISNULL(HOStatus, 0) = 1 OR BranchCode = 'KPNO'))) SELECT 1 ELSE SELECT 0";
                db.AddParameter("@parentGroupCode", SqlDbType.VarChar, parentUserGroupCode);
                db.AddParameter("@userBranchCode", SqlDbType.VarChar, userBranchCode);
                db.CommandType = CommandType.Text;
                db.Open();

                result = Convert.ToBoolean(db.ExecuteScalar());

                db.Close();
            }

            return result;
        }

        /// <summary>
        /// Cek apakah user group parent memiliki list user pada kantor pusat atau tidak.
        /// Jika ada, email akan dikirimkan ke user kantor pusat.
        /// Jika tidak ada, akan notifikasi ke user bahwa tidak ada user yang akan diemail untuk approval.
        /// </summary>
        /// <param name="userGroupCode">User Group Parent</param>
        /// <returns></returns>
        public static bool IsUserListGroupExistsForHO(string parentUserGroupCode)
        {
            bool result = false;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "IF EXISTS(SELECT GroupCode FROM MntUser WHERE Status = 1 AND GroupCode = @parentGroupCode AND BranchCode = (SELECT TOP 1 BranchCode FROM tblBranch WHERE (ISNULL(HOStatus, 0) = 1 OR BranchCode = 'KPNO'))) SELECT 1 ELSE SELECT 0";
                db.AddParameter("@parentGroupCode", SqlDbType.VarChar, parentUserGroupCode);
                db.CommandType = CommandType.Text;
                db.Open();

                result = Convert.ToBoolean(db.ExecuteScalar());

                db.Close();
            }

            return result;
        }

        /// <summary>
        /// Mengambil daftar email user dari group user yang akan diemail. 
        /// Group user bisa group user cabang atau group user pusat bergantung pada group parent user.
        /// </summary>
        /// <param name="userGroupCode">User Group Parent</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetUserEmailListByUserGroup(string userGroupCode, string branchCode)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT UserName, EmailAddress FROM MntUser WHERE BranchCode = @branchCode AND GroupCode = @groupCode AND Status = 1";
                db.AddParameter("@groupCode", SqlDbType.VarChar, userGroupCode);
                db.AddParameter("@branchCode", SqlDbType.VarChar, branchCode);
                db.CommandType = CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        string regexPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,6})+)$";

                        while (dr.Read())
                        {
                            if (dr["EmailAddress"] == DBNull.Value || dr["EmailAddress"] == "" || !Regex.Match(dr["EmailAddress"] as string, regexPattern).Success)
                                continue;

                            KeyValuePair<string, string> userEmail = new KeyValuePair<string, string>(Convert.ToString(dr["Username"]), Convert.ToString(dr["EmailAddress"]));
                            result.Add(userEmail);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return result;
        }
        public static string GetUserGroupParent(string userGroupCode)
        {
            if (string.IsNullOrEmpty(userGroupCode))
                return null;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT GroupParent FROM MntGroupUser WHERE GroupCode = @groupcode";
                db.AddParameter("@groupcode", SqlDbType.VarChar, userGroupCode);
                db.CommandType = CommandType.Text;
                db.Open();

                object result = db.ExecuteScalar();

                db.Close();

                if (result == DBNull.Value || result == null || string.IsNullOrEmpty(result.ToString()) || result.ToString() == "")
                    return null;
                else
                    return Convert.ToString(result);
            }
        }
        public static System.Data.DataTable ToDataTable<T>(List<T> items)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        // Add - Roby - 20210614
        public static System.Data.DataTable GetLesseeType(bool withAll)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name", typeof(string));
            DataColumn dcValue = new DataColumn("Value", typeof(string));

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);
            DataRow dr = dt.NewRow();
            if (withAll)
            {

                dr["Name"] = "All";
                dr["Value"] = DBNull.Value;
                dt.Rows.Add(dr);
            }

            dr = dt.NewRow();
            dr["Name"] = "Personal";
            dr["Value"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Company";
            dr["Value"] = "1";
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }


        //public static string getPostBackControlName(System.Web.UI.Page Page)
        //{
        //    System.Web.UI.Control control = null;
        //    //first we will check the "__EVENTTARGET" because if post back made by       the controls
        //    //which used "_doPostBack" function also available in Request.Form collection.

        //    string ctrlname = Page.Request.Params["__EVENTTARGET"];
        //    if (ctrlname != null && ctrlname != String.Empty)
        //    {
        //        control = Page.FindControl(ctrlname);
        //    }

        //    // if __EVENTTARGET is null, the control is a button type and we need to
        //    // iterate over the form collection to find it
        //    else
        //    {
        //        string ctrlStr = String.Empty;
        //        System.Web.UI.Control c = null;
        //        foreach (string ctl in Page.Request.Form)
        //        {
        //            //handle ImageButton they having an additional "quasi-property" in their Id which identifies
        //            //mouse x and y coordinates
        //            if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
        //            {
        //                ctrlStr = ctl.Substring(0, ctl.Length - 2);
        //                c = Page.FindControl(ctrlStr);
        //            }
        //            else
        //            {
        //                c = Page.FindControl(ctl);
        //            }
        //            if (c is System.Web.UI.WebControls.Button ||
        //                     c is System.Web.UI.WebControls.ImageButton)
        //            {
        //                control = c;
        //                break;
        //            }
        //        }

        //    }

        //    if (control != null)
        //        return control.ID;
        //    else
        //        return string.Empty;
        //}
        public static string GetPostBackControlName(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            // cek tombol submit biasa
            foreach (var key in request.Form.Keys)
            {
                // tombol submit dikirim dengan name=value
                if (key != "__RequestVerificationToken" &&
                    !key.StartsWith("__") &&
                    request.Form[key] == "Submit")
                {
                    return key;
                }

                // kalau pakai <button name="btnSave" value="Save">Save</button>
                if (request.Form[key].Count > 0 && !key.StartsWith("__"))
                {
                    return key;
                }
            }

            // fallback: kalau form nggak punya tombol bernama unik
            return string.Empty;
        }


        public static System.Data.DataTable ProjectNonProjectList;

        public static System.Data.DataTable GetLesseeStatus()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name", typeof(string));
            DataColumn dcValue = new DataColumn("Value", typeof(string));

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "All";
            dr["Value"] = DBNull.Value;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Active";
            dr["Value"] = "A";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Close";
            dr["Value"] = "C";
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

        //public static List<int> GetPageData()
        //{
        //    return new List<int>(new System.Int32[] { 1, 5, 10, 20, 50, 100, 1000 });
        //}

        //public static DataTable GetBranchListWithAll()
        //{
        //    DataTable dt = new DataTable();

        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        try
        //        {
        //            db.CommandText = "SELECT NULL BranchCode, 'ALL' BranchName UNION All SELECT BranchCode, BranchName FROM tblBranch ORDER BY BranchCode ASC;";
        //            db.CommandType = CommandType.Text;
        //            db.Open();

        //            dt = db.GetDataTable();
        //        }
        //        catch
        //        {
        //        }
        //        finally
        //        {
        //            db.Close();
        //        }
        //    }

        //    return dt;
        //}

        public static System.Data.DataTable GetBranchList()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            //using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
            //{
            //    try
            //    {
            //        db.CommandText = "SELECT BranchCode, BranchName FROM tblBranch ORDER BY BranchCode ASC;";
            //        db.CommandType = CommandType.Text;
            //        db.Open();

            //        dt = db.GetDataTable();
            //    }
            //    catch
            //    {
            //        throw;
            //    }
            //    finally
            //    {
            //        db.Close();
            //    }
            //}

            return dt;
        }

        public static System.Data.DataTable GetBranchListWithEmpty()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            //using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
            //{
            //    try
            //    {
            //        db.CommandText = "SELECT NULL BranchCode, '' BranchName UNION All SELECT BranchCode, BranchName FROM tblBranch ORDER BY BranchCode ASC;";
            //        db.CommandType = CommandType.Text;
            //        db.Open();

            //        dt = db.GetDataTable();
            //    }
            //    catch
            //    {
            //    }
            //    finally
            //    {
            //        db.Close();
            //    }
            //}

            return dt;
        }

        public static List<string> GetBranchCodeList()
        {
            List<string> result = null;

            //using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
            //{
            //    db.CommandText = "SELECT BranchCode FROM tblBranch;";
            //    db.CommandType = CommandType.Text;
            //    db.Open();

            //    db.ExecuteReader();

            //    using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
            //    {
            //        if (dr.HasRows)
            //        {
            //            result = new List<string>();

            //            while (dr.Read())
            //            {
            //                result.Add(dr["BranchCode"].ToString());
            //            }
            //        }

            //        if (!dr.IsClosed)
            //            dr.Close();
            //    }
            //}

            return result.OrderBy(x => x).ToList();
        }
        public static List<SelectListItem> GetFinanceMethodFT()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Investasi", Value = "1" });
            category.Add(new SelectListItem() { Text = "Modal Kerja", Value = "2" });

            return category;
        }
        //public static System.Data.DataTable GetFinanceMethodFT()
        //{
        //    System.Data.DataTable dt = new System.Data.DataTable();

        //    DataColumn dcName = new DataColumn("Name");
        //    DataColumn dcValue = new DataColumn("Value");

        //    dt.Columns.Add(dcName);
        //    dt.Columns.Add(dcValue);

        //    DataRow dr = dt.NewRow();
        //    dr["Name"] = "Investasi";
        //    dr["Value"] = 1;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Modal Kerja";
        //    dr["Value"] = 2;
        //    dt.Rows.Add(dr);

          

        //    dt.AcceptChanges();

        //    return dt;
        //}
        public static System.Data.DataTable GetFinanceMethod()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "Investasi";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Modal Kerja";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Multiguna";
            dr["Value"] = 3;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }
        
        public static string GetFinanceMethodName(int code)
        {
            string result = string.Empty;

            switch (code)
            {
                case 1:
                    result = "Investasi";
                    break;
                case 2:
                    result = "Modal Kerja";
                    break;
                case 3:
                    result = "Multiguna";
                    break;
            }

            return result;
        }

        //public static DataTable GetLeaseType()
        //{
        //    DataTable dt = new DataTable();

        //    DataColumn dcName = new DataColumn("Name");
        //    DataColumn dcValue = new DataColumn("Value");

        //    dt.Columns.Add(dcName);
        //    dt.Columns.Add(dcValue);

        //    DataRow dr = dt.NewRow();
        //    dr["Name"] = "Finance Lease";
        //    dr["Value"] = 1;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Sale and Leaseback";
        //    dr["Value"] = 2;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Installment Financing";
        //    dr["Value"] = 3;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Modal Usaha";
        //    dr["Value"] = 4;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Fasilitas Dana";
        //    dr["Value"] = 5;
        //    dt.Rows.Add(dr);

        //    dt.AcceptChanges();

        //    return dt;
        //}
  
        //public static DataTable GetGoodsService()
        //{
        //    DataTable dt = new DataTable();

        //    DataColumn dcName = new DataColumn("Name");
        //    DataColumn dcValue = new DataColumn("Value");

        //    dt.Columns.Add(dcName);
        //    dt.Columns.Add(dcValue);

        //    DataRow dr = dt.NewRow();
        //    dr["Name"] = "Goods";
        //    dr["Value"] = 1;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Services";
        //    dr["Value"] = 2;
        //    dt.Rows.Add(dr);

        //    dt.AcceptChanges();

        //    return dt;
        //}

        //public static DataTable GetGoodsServiceWithAll()
        //{
        //    DataTable dt = new DataTable();

        //    DataColumn dcName = new DataColumn("Name");
        //    DataColumn dcValue = new DataColumn("Value");

        //    dt.Columns.Add(dcName);
        //    dt.Columns.Add(dcValue);

        //    DataRow dr = dt.NewRow();
        //    dr["Name"] = "All";
        //    dr["Value"] = 0;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Goods";
        //    dr["Value"] = 1;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Services";
        //    dr["Value"] = 2;
        //    dt.Rows.Add(dr);

        //    dt.AcceptChanges();

        //    return dt;
        //}

        public static string GetGoodsServiceName(int code)
        {
            string result = string.Empty;

            switch (code)
            {
                case 1:
                    result = "Goods";
                    break;
                case 2:
                    result = "Service";
                    break;
            }

            return result;
        }

        //public static DataTable GetLeaseMethod()
        //{
        //    DataTable dt = new DataTable();

        //    DataColumn dcName = new DataColumn("Name");
        //    DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

        //    dt.Columns.Add(dcName);
        //    dt.Columns.Add(dcValue);

        //    DataRow dr = dt.NewRow();
        //    dr["Name"] = "Leasing";
        //    dr["Value"] = 1;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Consumer Finance";
        //    dr["Value"] = 2;
        //    dt.Rows.Add(dr);

        //    dt.AcceptChanges();

        //    return dt;
        //}


        public static bool IsMenuMustBeRequest(string menuUrl)
        {
            bool result = false;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SP_CheckMenuMustBeRequest";
                db.AddParameter("@MenuURL", SqlDbType.VarChar, menuUrl);
                db.CommandType = CommandType.StoredProcedure;
                db.Open();

                result = Convert.ToBoolean(db.ExecuteScalar());
            }

            return result;
        }
        public static string GetLeaseMethodName(int code)
        {
            string result = string.Empty;

            switch (code)
            {
                case 1:
                    result = "Sewa Guna Usaha";
                    break;
                case 2:
                    result = "Consumer Finance";
                    break;
            }

            return result;
        }

        public static string GetLeaseTypeName(int code)
        {
            string result = string.Empty;

            switch (code)
            {
                case 1:
                    result = "Finance Lease";
                    break;
                case 2:
                    result = "Sale and Leaseback";
                    break;
                case 3:
                    result = "Installment Financing";
                    break;
                case 4:
                    result = "Modal Usaha";
                    break;
                case 5:
                    result = "Fasilitas Dana";
                    break;
            }

            return result;
        }
        public static int GetLeaseStatus(string leaseno)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ISNULL(LeStatus, 0) FROM Lease WHERE LeaseNo = @leaseno";
                db.AddParameter("@leaseNo", SqlDbType.VarChar, leaseno);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                result = Convert.ToInt32(db.ExecuteScalar());

                db.Close();
            }

            return result;
        }
        public static System.Data.DataTable GetLeaseCurrOutstanding(string leaseno, string period)
        {
            System.Data.DataTable result = null;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "Le_SPCalcCurrentLeaseOutstanding";
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                db.AddParameter("@xPeriod", SqlDbType.VarChar, period);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.DbCommand.CommandTimeout = 0;
                db.Open();

                result = db.GetDataTable();

                db.Close();
            }

            return result;
        }
        //public static DataTable GetLeaseContractType()
        //{
        //    DataTable dt = new DataTable();

        //    DataColumn dcName = new DataColumn("Name");
        //    DataColumn dcValue = new DataColumn("Value");

        //    dt.Columns.Add(dcName);
        //    dt.Columns.Add(dcValue);

        //    DataRow dr = dt.NewRow();
        //    dr["Name"] = "Normal";
        //    dr["Value"] = 1;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "PAKAR";
        //    dr["Value"] = 2;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Syndication";
        //    dr["Value"] = 3;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Joint Financing";
        //    dr["Value"] = 4;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Channeling";
        //    dr["Value"] = 5;
        //    dt.Rows.Add(dr);

        //    dr = dt.NewRow();
        //    dr["Name"] = "Receivable Assignment";
        //    dr["Value"] = 6;
        //    dt.Rows.Add(dr);

        //    dt.AcceptChanges();

        //    return dt;
        //}



        public static System.Data.DataTable GetLeaseContractTypeWithAll()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "All";
            dr["Value"] = 0;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Normal";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "PAKAR";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Syndication";
            dr["Value"] = 3;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Joint Financing";
            dr["Value"] = 4;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Channeling";
            dr["Value"] = 5;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Receivable Assignment";
            dr["Value"] = 6;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }
        
        //public static DataTable GetCollateralGroup()
        //{
        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        db.CommandText = "select Code,Name from LeCollateralCode ORDER BY Name ASC;";
        //        db.CommandType = CommandType.Text;
        //        db.Open();

        //        return db.GetDataTable();
        //    }
        //}

        //public static DataTable GetCollateralGroupWithAll()
        //{
        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        db.CommandText = "select NULL Code,NULL Name UNION ALL select Code,Name from LeCollateralCode ORDER BY Name;";
        //        db.CommandType = CommandType.Text;
        //        db.Open();

        //        return db.GetDataTable();
        //    }
        //}

        //public static DataTable GetContractObject()
        //{
        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        db.CommandText = "select DISTINCT ObjectClassCode,ObjectClassName from LeObjectClass ORDER BY ObjectClassName ASC;";
        //        db.CommandType = CommandType.Text;
        //        db.Open();

        //        return db.GetDataTable();
        //    }
        //}

        //public static DataTable GetContractObjectWithAll()
        //{
        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        db.CommandText = "SELECT 'ALL' ObjectClassCode,'ALL' ObjectClassName UNION ALL SELECT DISTINCT ObjectClassCode,ObjectClassName from LeObjectClass ORDER BY ObjectClassName ASC;";
        //        db.CommandType = CommandType.Text;
        //        db.Open();

        //        return db.GetDataTable();
        //    }
        //}

        //public static DataTable GetIndustryGroup()
        //{
        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        db.CommandText = "select IndusGrpCode, IndusGrpName from IndustryGroup ORDER BY IndusGrpName ASC;";
        //        db.CommandType = CommandType.Text;
        //        db.Open();

        //        return db.GetDataTable();
        //    }
        //}

        //public static DataTable GetIndustryGroupWithAll()
        //{
        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        db.CommandText = "SELECT 'ALL' IndusGrpCode,'ALL' IndusGrpName,0 AA UNION select IndusGrpCode,IndusGrpName,1 AA from IndustryGroup ORDER BY AA,IndusGrpName ASC;";
        //        db.CommandType = CommandType.Text;
        //        db.Open();

        //        return db.GetDataTable();
        //    }
        //}

        //public static DataTable GetIndustry()
        //{
        //    DataTable dt = null;
        //    using (IDSDataAccess.DBDataAccessLibrary.SqlServer db = new IDSDataAccess.DBDataAccessLibrary.SqlServer())
        //    {
        //        db.CommandText = "SELECT IndusCode, IndusName FROM Industry ORDER BY IndusCode ASC;";
        //        db.CommandType = System.Data.CommandType.Text;
        //        db.Open();

        //        dt = db.GetDataTable();

        //        db.Close();
        //    }

        //    return dt;

        //}

        

        public static System.Data.DataTable GetObjectFunctionList()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "Produktif";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Konsumtif";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

    
        public static System.Data.DataTable GetProjectNonProjectList()
        {
            if (ProjectNonProjectList == null)
            {
                ProjectNonProjectList = new System.Data.DataTable();

                ProjectNonProjectList.Columns.Add(new DataColumn("Name", typeof(System.String)));
                ProjectNonProjectList.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

                DataRow dr = ProjectNonProjectList.NewRow();
                dr[0] = "All";
                dr[1] = -1;
                ProjectNonProjectList.Rows.Add(dr);
                ProjectNonProjectList.AcceptChanges();

                dr = ProjectNonProjectList.NewRow();
                dr[0] = "Non Project";
                dr[1] = 0;
                ProjectNonProjectList.Rows.Add(dr);
                ProjectNonProjectList.AcceptChanges();

                dr = ProjectNonProjectList.NewRow();
                dr[0] = "Project";
                dr[1] = 1;
                ProjectNonProjectList.Rows.Add(dr);
                ProjectNonProjectList.AcceptChanges();
            }

            return ProjectNonProjectList;
        }

        public static object GetProjectNonProjectListWithoutAll()
        {
            DataTable dtProject = new DataTable();

            dtProject.Columns.Add(new DataColumn("Name", typeof(System.String)));
            dtProject.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = dtProject.NewRow();

            dr = dtProject.NewRow();
            dr[0] = "Non Project";
            dr[1] = 0;
            dtProject.Rows.Add(dr);
            dtProject.AcceptChanges();

            dr = dtProject.NewRow();
            dr[0] = "Project";
            dr[1] = 1;
            dtProject.Rows.Add(dr);
            dtProject.AcceptChanges();

            return dtProject;
        }

        public static string GetFinanceMethodNameByCode(object type, object projectCode)
        {
            if (type == DBNull.Value || type == null)
            {
                return "";
            }
            else
            {
                switch(Convert.ToInt32(type))
                {
                    case 1:
                        if (projectCode != DBNull.Value && projectCode != null && Convert.ToString(projectCode) != "")
                            return "Pembiayaan Investasi - Proyek";
                        else
                            return "Pembiayaan Investasi";
                    case 2:
                        if (projectCode != DBNull.Value || projectCode != null || Convert.ToString(projectCode) != "")
                            return "Pembiayaan Modal Kerja - Proyek";
                        else
                            return "Pembiayaan Modal Kerja";
                    case 3:
                        return  "Pembiayaan Multiguna";
                    default:
                        return "";
                }
            }
        }

        public static string GetNewLeaseTypeNameByCode(object type)
        {
            if (type == DBNull.Value || type == null)
            {
                return "";
            }
            else
            {
                switch (Convert.ToInt32(type))
                {
                    case 1:
                        return "Finance Lease";
                    case 2:
                        return "Sale and Leaseback";
                    case 3:
                        return "Installment Financing";
                    case 4:
                        return "Modal Usaha";
                    default:
                        return "";
                }
            }
        }

        public static string GetNewLeaseTypeNameByCode(object type, object goodsService)
        {
            if (type == DBNull.Value || type == null)
            {
                return "";
            }
            else
            {
                switch (Convert.ToInt32(type))
                {
                    case 1:
                        return "Finance Lease";
                    case 2:
                        return "Sale and Leaseback";
                    case 3:
                        if (goodsService != DBNull.Value || goodsService != null)
                        {
                            switch (Convert.ToInt32(goodsService))
                            {
                                case 1: 
                                    return "Installment Financing - Barang";
                                case 2:
                                    return "Installment Financing - Jasa";
                                default:
                                    return "Installment Financing";
                            }
                        }
                        else
                        {
                            return "Installment Financing";
                        }
                    case 4:
                        return "Modal Usaha";
                    case 5:
                        if (goodsService != DBNull.Value || goodsService != null)
                        {
                            switch (Convert.ToInt32(goodsService))
                            {
                                case 1:
                                    return "Fasilitas Dana - Barang";
                                case 2:
                                    return "Fasilitas Dana - Jasa";
                                default:
                                    return "Fasilitas Dana";
                            }
                        }
                        else
                        {
                            return "Fasilitas Dana";
                        }
                    default:
                        return "";
                }
            }
        }

        public static System.Data.DataTable GetProjectInfrastructureList()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            DataColumn dcName = new DataColumn("Name", typeof(System.String));
            DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "";
            dr["Value"] = 0;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Proyek";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Infrastruktur";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

        public static System.Data.DataTable GetInsuranceStatus()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "Active";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Expired";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Renewer";
            dr["Value"] = 3;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

        public static System.Data.DataTable GetInsuranceStatusWithAll()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "All";
            dr["Value"] = 0;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Active";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Expired";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Renewer";
            dr["Value"] = 3;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

        private static System.Data.DataTable _dtApplicationType;
        public static System.Data.DataTable GetApplicationType()
        {
            if (_dtApplicationType != null)
                return _dtApplicationType;

            _dtApplicationType = new System.Data.DataTable();
            DataColumn dcName = new DataColumn("Name", typeof(System.String));
            DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

            _dtApplicationType.Columns.Add(dcName);
            _dtApplicationType.Columns.Add(dcValue);

            DataRow dr = _dtApplicationType.NewRow();
            dr = _dtApplicationType.NewRow();
            dr["Name"] = "Personal";
            dr["Value"] = 0;
            _dtApplicationType.Rows.Add(dr);

            dr = _dtApplicationType.NewRow();
            dr["Name"] = "Company";
            dr["Value"] = 1;
            _dtApplicationType.Rows.Add(dr);

            _dtApplicationType.AcceptChanges();

            return _dtApplicationType;
        }

        // ASSET
        public static System.Data.DataTable GetAssetKe()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            DataColumn dcName = new DataColumn("Name", typeof(System.String));
            DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr = dt.NewRow();
            dr["Name"] = "Pertama";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Kedua dan seterusnya";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

        public static System.Data.DataTable GetAssetKeWithEmpty()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            DataColumn dcName = new DataColumn("Name", typeof(System.String));
            DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr = dt.NewRow();
            dr["Name"] = "";
            dr["Value"] = DBNull.Value;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Pertama";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Kedua dan seterusnya";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }
        // END ASSET

        // Metode Penilaian Agunan
        public static System.Data.DataTable GetAssessmentMethod()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "Fixed (100%)";
            dr["Value"] = 0;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Internal";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Independent";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }

        private static System.Data.DataTable _dtPaymentMethod;
        public static System.Data.DataTable GetPaymentMethod()
        {
            if (_dtPaymentMethod != null)
                return _dtPaymentMethod;

            _dtPaymentMethod = new System.Data.DataTable();
            DataColumn dcName = new DataColumn("Name", typeof(System.String));
            DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

            _dtPaymentMethod.Columns.Add(dcName);
            _dtPaymentMethod.Columns.Add(dcValue);

            DataRow dr = _dtPaymentMethod.NewRow();
            dr = _dtPaymentMethod.NewRow();
            dr["Name"] = "Fixed Payment";
            dr["Value"] = 0;
            _dtPaymentMethod.Rows.Add(dr);

            dr = _dtPaymentMethod.NewRow();
            dr["Name"] = "Fixed Principal";
            dr["Value"] = 1;
            _dtPaymentMethod.Rows.Add(dr);

            dr = _dtPaymentMethod.NewRow();
            dr["Name"] = "Step Principal";
            dr["Value"] = 3;
            _dtPaymentMethod.Rows.Add(dr);

            dr = _dtPaymentMethod.NewRow();
            dr["Name"] = "Step Rental";
            dr["Value"] = 4;
            _dtPaymentMethod.Rows.Add(dr);

            dr = _dtPaymentMethod.NewRow();
            dr["Name"] = "Irregular";
            dr["Value"] = 5;
            _dtPaymentMethod.Rows.Add(dr);

            _dtPaymentMethod.AcceptChanges();

            return _dtPaymentMethod;
        }

        public static string GetPaymentMethodName(int code)
        {
            if (_dtPaymentMethod != null && _dtPaymentMethod.Rows.Count > 0)
            {
                return Convert.ToString(_dtPaymentMethod.Rows.Cast<DataRow>().Where(x => Convert.ToInt32(x["Value"]) == code).FirstOrDefault()["Name"]);
            }
            else
            {
                string result = string.Empty;

                switch (code)
                {
                    case 0:
                        result = "Fixed Payment";
                        break;
                    case 1:
                        result = "Fixed Principal";
                        break;
                    case 3:
                        result = "Step Principal";
                        break;
                    case 4:
                        result = "Step Rental";
                        break;
                    case 5:
                        result = "Irregular";
                        break;
                }

                return result;
            }
        }

        private static System.Data.DataTable _dtFixedFloat;
        public static System.Data.DataTable GetFixedFloat()
        {
            if (_dtFixedFloat != null)
                return _dtFixedFloat;

            _dtFixedFloat = new System.Data.DataTable();
            DataColumn dcName = new DataColumn("Name", typeof(System.String));
            DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

            _dtFixedFloat.Columns.Add(dcName);
            _dtFixedFloat.Columns.Add(dcValue);

            DataRow dr = _dtFixedFloat.NewRow();
            dr = _dtFixedFloat.NewRow();
            dr["Name"] = "Fixed";
            dr["Value"] = 1;
            _dtFixedFloat.Rows.Add(dr);

            dr = _dtFixedFloat.NewRow();
            dr["Name"] = "Floating";
            dr["Value"] = 2;
            _dtFixedFloat.Rows.Add(dr);

            _dtFixedFloat.AcceptChanges();

            return _dtFixedFloat;
        }

        public static string GetFixedFloatName(int code)
        {
            if (_dtFixedFloat != null && _dtFixedFloat.Rows.Count > 0)
            {
                return Convert.ToString(_dtFixedFloat.Rows.Cast<DataRow>().Where(x => Convert.ToInt32(x["Value"]) == code).FirstOrDefault()["Name"]);
            }
            else
            {
                string result = string.Empty;

                switch (code)
                {
                    case 1:
                        result = "Fixed";
                        break;
                    case 2:
                        result = "Floating";
                        break;
                }

                return result;
            }
        }

        private static System.Data.DataTable _dtAdvanceArrear;
        public static List<SelectListItem> GetAdvanceArrear()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Advance", Value = "1" });
            category.Add(new SelectListItem() { Text = "Arrear", Value = "2" });

            return category;
        }
        //public static System.Data.DataTable GetAdvanceArrear()
        //{
        //    if (_dtAdvanceArrear != null)
        //        return _dtAdvanceArrear;

        //    _dtAdvanceArrear = new System.Data.DataTable();
        //    DataColumn dcName = new DataColumn("Name", typeof(System.String));
        //    DataColumn dcValue = new DataColumn("Value", typeof(System.Int32));

        //    _dtAdvanceArrear.Columns.Add(dcName);
        //    _dtAdvanceArrear.Columns.Add(dcValue);

        //    DataRow dr = _dtAdvanceArrear.NewRow();
        //    dr = _dtAdvanceArrear.NewRow();
        //    dr["Name"] = "Advance";
        //    dr["Value"] = 1;
        //    _dtAdvanceArrear.Rows.Add(dr);

        //    dr = _dtAdvanceArrear.NewRow();
        //    dr["Name"] = "Arrear";
        //    dr["Value"] = 2;
        //    _dtAdvanceArrear.Rows.Add(dr);

        //    _dtAdvanceArrear.AcceptChanges();

        //    return _dtAdvanceArrear;
        //}


        #region Modifikasi SLIK
        private static System.Data.DataTable _dtMaritalStatus = null;
        public static System.Data.DataTable GetMaritalStatus()
        {
            if (_dtMaritalStatus != null)
                return _dtMaritalStatus;

            _dtMaritalStatus = new System.Data.DataTable();
            _dtMaritalStatus.Columns.Add(new DataColumn("Name", typeof(string)));
            _dtMaritalStatus.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = _dtMaritalStatus.NewRow();
            dr["Name"] = "Kawin";
            dr["Value"] = 1;
            _dtMaritalStatus.Rows.Add(dr);

            dr = _dtMaritalStatus.NewRow();
            dr["Name"] = "Belum Kawin";
            dr["Value"] = 2;
            _dtMaritalStatus.Rows.Add(dr);

            dr = _dtMaritalStatus.NewRow();
            dr["Name"] = "Cerai";
            dr["Value"] = 3;
            _dtMaritalStatus.Rows.Add(dr);

            return _dtMaritalStatus;
        }

        private static System.Data.DataTable _dtLesseeResidenceStatus = null;
        public static System.Data.DataTable GetLesseeResidenceStatus()
        {
            if (_dtLesseeResidenceStatus != null)
                return _dtLesseeResidenceStatus;

            _dtLesseeResidenceStatus = new System.Data.DataTable();
            _dtLesseeResidenceStatus.Columns.Add(new DataColumn("Name", typeof(string)));
            _dtLesseeResidenceStatus.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = _dtLesseeResidenceStatus.NewRow();
            dr["Name"] = "Milik Sendiri";
            dr["Value"] = 1;
            _dtLesseeResidenceStatus.Rows.Add(dr);

            dr = _dtLesseeResidenceStatus.NewRow();
            dr["Name"] = "Milik Keluarga";
            dr["Value"] = 2;
            _dtLesseeResidenceStatus.Rows.Add(dr);

            dr = _dtLesseeResidenceStatus.NewRow();
            dr["Name"] = "Sewa / Kontrak";
            dr["Value"] = 3;
            _dtLesseeResidenceStatus.Rows.Add(dr);

            dr = _dtLesseeResidenceStatus.NewRow();
            dr["Name"] = "Rumah Dinas";
            dr["Value"] = 4;
            _dtLesseeResidenceStatus.Rows.Add(dr);

            return _dtLesseeResidenceStatus;
        }

        public static System.Data.DataTable GetPersonalLesseeIDType()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = dt.NewRow();
            dr["Name"] = "KTP";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Passport";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            return dt;
        }

        public static System.Data.DataTable GetCompanyLesseeIDType()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new DataColumn("Name", typeof(System.String)));
            dt.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = dt.NewRow();
            dr["Name"] = "NPWP";
            dr["Value"] = 3;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Public Agency";
            dr["Value"] = 9;
            dt.Rows.Add(dr);

            return dt;
        }

        public static System.Data.DataTable GetAllGuarantorIDType()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = dt.NewRow();
            dr["Name"] = "KTP";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Passport";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "NPWP";
            dr["Value"] = 3;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Public Agency";
            dr["Value"] = 9;
            dt.Rows.Add(dr);

            return dt;
        }
                
        private static System.Data.DataTable _dtIsPromo;
        public static System.Data.DataTable GetIsPromo()
        {
            if (_dtIsPromo != null)
                return _dtIsPromo;

            _dtIsPromo = new DataTable();
            _dtIsPromo.Columns.Add(new DataColumn("Name", typeof(string)));
            _dtIsPromo.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = _dtIsPromo.NewRow();
            dr["Name"] = "Normal";
            dr["Value"] = 0;
            _dtIsPromo.Rows.Add(dr);

            dr = _dtIsPromo.NewRow();
            dr["Name"] = "Promo";
            dr["Value"] = 1;
            _dtIsPromo.Rows.Add(dr);

            return _dtIsPromo;
        }

        private static DataTable _dtIsPromoAll;
        public static List<SelectListItem> GetIsPromoWithAll()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "ALL", Value = "255" });
            RP.Add(new SelectListItem() { Text = "Normal", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Promo", Value = "1" });
            return RP;
          }

        public static List<SelectListItem> GetStatusInsurance()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Active", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Expired", Value = "1" });
            RP.Add(new SelectListItem() { Text = "Renewer", Value = "2" });
            return RP;
        }
        public static List<SelectListItem> GetInsPayType()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            //RP.Add(new SelectListItem() { Text = "Covered Through LS", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Covered By Customer", Value = "1" });
            RP.Add(new SelectListItem() { Text = "Covered By Company", Value = "0" });
            return RP;
        }
        public static List<SelectListItem> GetListCorporateOrRetail()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Corporate", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Retail", Value = "1" });
            return RP;
        }
        private static DataTable _dtCorpRetail;
        

        private static DataTable _dtCorpRetailAll;
        public static List<SelectListItem> GetListCorporateOrRetailwithAll()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "ALL", Value = "255" });
            RP.Add(new SelectListItem() { Text = "Corporate", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Retail", Value = "1" });
            return RP;
        }

        private static DataTable _dtCorpRetailApp;
        public static DataTable GetListCorporateOrRetailForApp()
        {
            if (_dtCorpRetailApp != null)
                return _dtCorpRetailApp;

            _dtCorpRetailApp = new DataTable();
            _dtCorpRetailApp.Columns.Add(new DataColumn("Name", typeof(string)));
            _dtCorpRetailApp.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = _dtCorpRetailApp.NewRow();
            dr["Name"] = "Corporate";
            dr["Value"] = 0;
            _dtCorpRetailApp.Rows.Add(dr);

            dr = _dtCorpRetailApp.NewRow();
            dr["Name"] = "Retail - SME";
            dr["Value"] = 1;
            _dtCorpRetailApp.Rows.Add(dr);

            dr = _dtCorpRetailApp.NewRow();
            dr["Name"] = "Retail - PKK";
            dr["Value"] = 2;
            _dtCorpRetailApp.Rows.Add(dr);

            return _dtCorpRetailApp;
        }

        private static DataTable _dtSMEPKK;
        public static DataTable GetListSMEPKK()
        {
            if (_dtSMEPKK != null)
                return _dtSMEPKK;

            _dtSMEPKK = new DataTable();
            _dtSMEPKK.Columns.Add(new DataColumn("Name", typeof(string)));
            _dtSMEPKK.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr; 

            dr = _dtSMEPKK.NewRow();
            dr["Name"] = "SME";
            dr["Value"] = 1;
            _dtSMEPKK.Rows.Add(dr);

            dr = _dtSMEPKK.NewRow();
            dr["Name"] = "PKK";
            dr["Value"] = 2;
            _dtSMEPKK.Rows.Add(dr);

            return _dtSMEPKK;
        }



        private static DataTable _dtSMEPKKAll;
        public static List<SelectListItem> GetListSMEPKKWithAll()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "ALL", Value = "255" });
            RP.Add(new SelectListItem() { Text = "SME", Value = "1" });
            RP.Add(new SelectListItem() { Text = "PKK", Value = "2" });
            return RP;
        }
        public static List<SelectListItem> GetRevolveNew()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "New", Value = "New" });
            RP.Add(new SelectListItem() { Text = "Revolving", Value = "Revolving" });
            return RP;
        }
        public static List<SelectListItem> GetIntType()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Daily", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Monthly", Value = "1" });
            return RP;
        }
        public static List<SelectListItem> GetOverDueType()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Daily", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Monthly", Value = "1" });
            RP.Add(new SelectListItem() { Text = "Yearly", Value = "2" });
            return RP;
        }
        public static List<SelectListItem> LeaseFromBranch(string branch)
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT DISTINCT Schedule.LeaseNo FROM Schedule  INNER JOIN Lease  ON Schedule.LeaseNo=Lease.LeaseNo  WHERE Lease.BranchCode='"+branch+"';";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["LeaseNo"].ToString();
                           l.Value= dr["LeaseNo"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        }
        public static List<SelectListItem> GetFacilityForSOADisburst()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM FTTransH INNER JOIN Lessee ON Lessee.LesseeNo = FTTransH.ClientID AND Lessee.BranchCode = FTTransH.ClientBranch WHERE FactStatus = 2";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["AgreementNo"].ToString() + " - " + dr["LesseeFullName"].ToString();
                            l.Value = dr["AgreementNo"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        }
        public static List<SelectListItem> GetCustomerFacility()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT LesseeNo,LesseeFullName FROM Lessee where statuscode = 'A'";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["LesseeNo"].ToString() + " - " + dr["LesseeFullName"].ToString();
                            l.Value = dr["LesseeNo"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        }
        public static List<SelectListItem> GetFacilityForSOA()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM FTTransH INNER JOIN Lessee ON Lessee.LesseeNo = FTTransH.ClientID AND Lessee.BranchCode = FTTransH.ClientBranch WHERE FactStatus >= 1";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["AgreementNo"].ToString() + " - " + dr["LesseeFullName"].ToString();
                            l.Value = dr["AgreementNo"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        }

        public static List<SelectListItem> GetFacilityForSOAWithLimit()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT *, FORMAT(CreditLimit, '##,##0') AS 'FormattedMoney' FROM FTTransH INNER JOIN Lessee ON Lessee.LesseeNo = FTTransH.ClientID AND Lessee.BranchCode = FTTransH.ClientBranch WHERE FactStatus >= 1";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = IDS.Tool.GeneralHelper.NullToString(dr["AgreementNo"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["LesseeFullName"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["FormattedMoney"]);
                              
                            l.Value = IDS.Tool.GeneralHelper.NullToString(dr["AgreementNo"]);
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        } 
        public static List<SelectListItem> GetFacilityForSOACreate()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT distinct FTTransH.AgreementNo,Lessee.Lesseefullname FROM FTTransH INNER JOIN Lessee ON Lessee.LesseeNo = FTTransH.ClientID AND Lessee.BranchCode = FTTransH.ClientBranch LEFT JOIN FTTransD ON FTTransH.AgreementNo = FTTransD.AgreementNo WHERE FTTransH.FactStatus > 0";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["AgreementNo"].ToString() + " - " + dr["Lesseefullname"].ToString();
                            l.Value = dr["AgreementNo"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        }
        public static string GetBouwheerForDatasource()
        {
            StringBuilder sb = new StringBuilder();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT CustID, CustName FROM FTCustomer";
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);

                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    //if (dr.HasRows)
                    //{
                    //    sb.Append("<option value=\"\">" +
                    //              "");
                    //    while (dr.Read())
                    //    {
                    //        sb.Append("<option value=\"" + System.Web.HttpContext.Current.WebUtility.HtmlEncode(IDS.Tool.GeneralHelper.NullToString(dr["CustID"])) + "\">" +
                    //            System.Web.HttpContext.Current.WebUtility.HtmlEncode(IDS.Tool.GeneralHelper.NullToString(dr["CustID"]) +" - "+ IDS.Tool.GeneralHelper.NullToString(dr["CustName"])));
                    //    }
                    //}
                    if (dr.HasRows)
                    {
                        sb.Append("<option value=\"\"></option>");
                        while (dr.Read())
                        {
                            var custID = IDS.Tool.GeneralHelper.NullToString(dr["CustID"]);
                            var custName = IDS.Tool.GeneralHelper.NullToString(dr["CustName"]);

                            sb.Append("<option value=\"")
                              .Append(WebUtility.HtmlEncode(custID))
                              .Append("\">")
                              .Append(WebUtility.HtmlEncode($"{custID} - {custName}"))
                              .Append("</option>");
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }


            return sb.ToString();
        }


        public static List<SelectListItem> LeaseFromContType(string branch,string contractType)
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                string S = "";
                if (!string.IsNullOrEmpty(contractType))
                {
                    S = "SELECT DISTINCT Schedule.LeaseNo FROM Schedule  INNER JOIN Lease  ON Schedule.LeaseNo=Lease.LeaseNo  WHERE Lease.BranchCode=@branch AND ContractType LIKE ISNULL('"+contractType+"','%')";
                }
                else
                {
                    S = "SELECT DISTINCT Schedule.LeaseNo FROM Schedule  INNER JOIN Lease  ON Schedule.LeaseNo=Lease.LeaseNo  WHERE Lease.BranchCode=@branch AND ContractType LIKE ISNULL(NULL,'%')";
                }
                db.CommandText = S;
                db.AddParameter("@branch", SqlDbType.VarChar, branch);
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["LeaseNo"].ToString();
                            l.Value = dr["LeaseNo"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        }

        public static bool IsFloating(string leaseNo)
        {
            bool return_ = false;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                string sql = "SELECT FixFloat from Lease WHERE LeaseNo=@leaseNo";
                db.CommandText = sql;
                db.AddParameter("@leaseNo", SqlDbType.VarChar, leaseNo);
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        if (dr["FixFloat"].ToString()=="2")
                        {
                            return_ = true;
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return return_;
        }

        //public static List<SelectListItem> LeaseFromContTypeAndYear(string ctype, string branch, string period)
        //{
        //    List<SelectListItem> RP = new List<SelectListItem>();

        //    string contract_date = "";
        //    if (!string.IsNullOrEmpty(period))
        //    {
        //        DateTime d = System.Convert.ToDateTime(period);
        //        contract_date = d.ToString("yyyyMM");
        //    }else
        //    {
        //        DateTime d = DateTime.Today;
        //        contract_date = d.ToString("yyyyMM");
        //    }

        //    //string sql = "SELECT * FROM Lease where BranchCode=@Branch AND ContractType LIKE ISNULL(@Ctype,'%') AND CONVERT(VARCHAR(6),Lease.ContractDate, 112)='" + contract_date + "'";
        //    string sql = "SELECT * FROM Lease where BranchCode='"+branch+"' AND ContractType LIKE ISNULL("+ctype+",'%') AND CONVERT(VARCHAR(6),Lease.ContractDate, 112)='" + contract_date + "'";
        //    using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
        //    {
        //        db.CommandText = sql;
        //        db.CommandType = CommandType.Text;
        //        db.Open();
        //        db.ExecuteReader();
        //        using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
        //        {
        //            if (dr.HasRows)
        //            {
        //                while (dr.Read())
        //                {
        //                    SelectListItem l = new SelectListItem();
        //                    l.Text = dr["LeaseNo"].ToString();
        //                    l.Value = dr["LeaseNo"].ToString();
        //                    RP.Add(l);
        //                }
        //            }

        //            if (!dr.IsClosed)
        //                dr.Close();
        //        }
        //        db.Close();
        //    }
        //    return RP;
        //}

        public static List<SelectListItem> GetLeaseLeaseFromContTypeAndYear(string lease)
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            string sql = "SELECT Period FROM Schedule WHERE LeaseNo='"+ lease + "' AND Period>0";
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = sql;
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem l = new SelectListItem();
                            l.Text = dr["Period"].ToString();
                            l.Value = dr["Period"].ToString();
                            RP.Add(l);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return RP;
        }

        public static object CheckFloating(string leaseNo)
        {
            object return_ = null;
            System.Text.StringBuilder b = new StringBuilder();
            b.AppendLine("DECLARE @FixFloat AS VARCHAR(100)='0',@TotalRow AS Tinyint");
            b.AppendLine("SET @FixFloat=(SELECT FixFloat FROM Lease where LeaseNo='"+ leaseNo + "')");
            b.AppendLine("SET @TotalRow =(SELECT MAX(Period) FROM Schedule WHERE LeaseNo='"+ leaseNo + "')");
            b.AppendLine("select  @FixFloat as fixfloat, @TotalRow as totalrow");
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = b.ToString();
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        string fixfloat = dr["fixfloat"].ToString();
                        string totalcount = dr["totalrow"].ToString();
                        //return_ = "{fixfloat:"+ fixfloat + ",totalcount:"+ totalcount + "}";
                        return_ = new { fix = fixfloat, tot = totalcount };
                        //return_ = configsing();// JsonConvert.SerializeObject(configs);
                      }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return return_;
        }

        private static DataTable _dtOJKRisk;
        public static DataTable GetOJKRisk()
        {
            if (_dtOJKRisk != null)
                return _dtOJKRisk;

            _dtOJKRisk = new DataTable();
            _dtOJKRisk.Columns.Add(new DataColumn("Name", typeof(string)));
            _dtOJKRisk.Columns.Add(new DataColumn("Value", typeof(System.Int32)));

            DataRow dr = _dtOJKRisk.NewRow();
            dr["Name"] = "Rendah";
            dr["Value"] = 0;
            _dtOJKRisk.Rows.Add(dr);

            dr = _dtOJKRisk.NewRow();
            dr["Name"] = "Tinggi";
            dr["Value"] = 1;
            _dtOJKRisk.Rows.Add(dr);

            return _dtOJKRisk;
        }
        #endregion
             
        public static string GetJCodeName(string code)
        {
            if (string.IsNullOrEmpty(code)) return "";

            string result = "";

            switch (code)
            {
                case "1LIF1":
                    result = "Investasi - Finance Lease";
                    break;
                case "1LPF1":
                    result = "Proyek - Finance Lease";
                    break;
                case "1LGF":
                    result = "Multiguna - Finance Lease";
                    break;
                case "1LIS1":
                    result = "Investasi - Sale & Leaseback";
                    break;
                case "1LPS1":
                    result = "Proyek - Sale & Leaseback";
                    break;
                case "1LMS1":
                    result = "Modal kerja - Sale & Leaseback ";
                    break;
                case "1CIB1":
                    result = "Investasi - Inst. Financing Barang";
                    break;
                case "1CPB1":
                    result = "Proyek - Inst. Financing Barang";
                    break;
                case "1CGB1":
                    result = "Multiguna - Inst. Financing Barang";
                    break;
                case "1CPJ1":
                    result = "Proyek - Inst. Financing - Jasa";
                    break;
                case "1CGJ1":
                    result = "Multiguna - Inst. Financing - Jasa";
                    break;
                case "1CMU1":
                    result = "Modal Kerja - F. Modal Usaha";
                    break;
                case "1CGG1":
                    result = "Multiguna - Fasilitas Dana - Barang";
                    break;
                case "1CGS1":
                    result = "Multiguna - Fasilitas Dana - Jasa";
                    break;
            }

            return result;
        }

        public static DataTable GetFinancingPurposeDataSource()
        {
            DataTable dt = new DataTable();

            DataColumn dcName = new DataColumn("Name");
            DataColumn dcValue = new DataColumn("Value");

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "Productive";
            dr["Value"] = 1;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Consumptive";
            dr["Value"] = 2;
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }
             
        public static DataTable GetApplicationStatus()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Value", typeof(int)));

            DataRow row = dt.NewRow();
            row[0] = "New Application";
            row[1] = 0;
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "Surveyed";
            row[1] = 5;
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "Approved";
            row[1] = 2;
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "Rejected";
            row[1] = 1;
            dt.Rows.Add(row);
            

            return dt;
        }
        // End add - Anthony - 20200812

          public static bool PublicObjectPropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                Type type = typeof(T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return self == to;
        }
        // End add - Anthony - 20210306

        // Add - Roby - 20210614
        public static System.Data.DataTable GetLesseeType()
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            DataColumn dcName = new DataColumn("Name", typeof(string));
            DataColumn dcValue = new DataColumn("Value", typeof(string));

            dt.Columns.Add(dcName);
            dt.Columns.Add(dcValue);

            DataRow dr = dt.NewRow();
            dr["Name"] = "All";
            dr["Value"] = DBNull.Value;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Personal";
            dr["Value"] = "0";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["Name"] = "Company";
            dr["Value"] = "1";
            dt.Rows.Add(dr);

            dt.AcceptChanges();

            return dt;
        }
        // End Add - Roby - 20210614
        public static List<SelectListItem> GetFinancingPurpose()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Productive", Value = "1" });
            category.Add(new SelectListItem() { Text = "Consumptive", Value = "2" });
            return category;
        }
   
        public static string GetAlloTypeForDataSource()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<option value=\"\">" +
                                 "");
            //sb.Append("<option value=\"1\">" + "Invoice Payment");
            //sb.Append("<option value=\"2\">" + "Other Income");
            sb.Append("<option value=\"3\">" + "Penalty");
            sb.Append("<option value=\"4\">" + "Interest");
            sb.Append("<option value=\"5\">" + "Factoring Fee");
            sb.Append("<option value=\"6\">" + "Legal Fee");
            sb.Append("<option value=\"7\">" + "VAT Fee");
            sb.Append("<option value=\"8\">" + "Provision Fee");
            sb.Append("<option value=\"9\">" + "Other Fee");
            sb.Append("<option value=\"10\">" + "Other Inc/Exp");
            sb.Append("<option value=\"11\">" + "Schedule Principal");



            return sb.ToString();
        }
        public static string GetAlloTypeLeaseForDataSource()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<option value=\"\">" +
                                 "");
            //sb.Append("<option value=\"1\">" + "Invoice Payment");
            //sb.Append("<option value=\"2\">" + "Other Income");
            sb.Append("<option value=\"3\">" + "Rental");
            sb.Append("<option value=\"4\">" + "Other Income");
            sb.Append("<option value=\"9\">" + "Late Charges");
            sb.Append("<option value=\"28\">" + "Customer Deposit");



            return sb.ToString();
        }
        public static List<SelectListItem> GetAllowanceLoss()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Individual", Value = "1" });
            category.Add(new SelectListItem() { Text = "Collective", Value = "2" });
            return category;
        }
        public static List<SelectListItem> GetStatusAvailabilityCollateral()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            //Edited By Renaldi 6 July 2024
            //category.Add(new SelectListItem() { Text = "Indent", Value = "0" });
            category.Add(new SelectListItem() { Text = "Indent", Value = "2" });
            category.Add(new SelectListItem() { Text = "Tersedia", Value = "1" });
            return category;
        }
        public static List<SelectListItem> GetAlloType()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            //category.Add(new SelectListItem() { Text = "Invoice Payment", Value = "1" });
            //category.Add(new SelectListItem() { Text = "Other Income", Value = "2" });
            category.Add(new SelectListItem() { Text = "Penalty", Value = "3" });
            category.Add(new SelectListItem() { Text = "Interest", Value = "4" });
            category.Add(new SelectListItem() { Text = "Factoring Fee", Value = "5" });
            category.Add(new SelectListItem() { Text = "Legal Fee", Value = "6" });
            category.Add(new SelectListItem() { Text = "VAT Fee", Value = "7" });
            category.Add(new SelectListItem() { Text = "Provision Fee", Value = "8" });
            category.Add(new SelectListItem() { Text = "Other Fee", Value = "9" });
            category.Add(new SelectListItem() { Text = "Other Inc/Exp", Value = "10" });
            category.Add(new SelectListItem() { Text = "Schedule Principal", Value = "11" });


            return category;
        }
        public static List<SelectListItem> GetCollectStatus()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            //category.Add(new SelectListItem() { Text = "Invoice Payment", Value = "1" });
            //category.Add(new SelectListItem() { Text = "Other Income", Value = "2" });
            category.Add(new SelectListItem() { Text = "Lancar", Value = "1" });
            category.Add(new SelectListItem() { Text = "Dalam Perhatian Khusus", Value = "2" });
            category.Add(new SelectListItem() { Text = "Kurang Lancar", Value = "3" });
            category.Add(new SelectListItem() { Text = "Diragukan", Value = "4" });
            category.Add(new SelectListItem() { Text = "Macet", Value = "5" });
            return category;
        }
        public static List<SelectListItem> GetAlloTypeLease()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            //category.Add(new SelectListItem() { Text = "Invoice Payment", Value = "1" });
            //category.Add(new SelectListItem() { Text = "Other Income", Value = "2" });
            category.Add(new SelectListItem() { Text = "Rental", Value = "3" });
            category.Add(new SelectListItem() { Text = "Other Income", Value = "4" });
            category.Add(new SelectListItem() { Text = "Late Charges", Value = "9" });
            category.Add(new SelectListItem() { Text = "Customer Deposit", Value = "28" });
            return category;
        }
        public static List<SelectListItem> GetCategoryCustomerGrp()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Internal", Value = "1" });
            category.Add(new SelectListItem() { Text = "External", Value = "2" });
            return category;
        }
        public static List<SelectListItem> GetOJKRiskForDatasource()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Tinggi", Value = "1" });
            category.Add(new SelectListItem() { Text = "Rendah", Value = "0" });
            return category;
        }
        public static List<SelectListItem> GetCustomerTypeNUP()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Ex Customer", Value = "2" });
            category.Add(new SelectListItem() { Text = "Repeat Order", Value = "1" });
            category.Add(new SelectListItem() { Text = "New Customer", Value = "0" });
            return category;
        }
        public static List<SelectListItem> GetFacilityTypeNUP()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Pembiayaan Konsumen", Value = "2" });
            category.Add(new SelectListItem() { Text = "Sewa Guna Usaha", Value = "1" });
            return category;
        }
        public static List<SelectListItem> GetTerminationType()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Pembayaran Pokok", Value = "1" });
            category.Add(new SelectListItem() { Text = "Pembayaran Dipercepat", Value = "0" });
            return category;
        }

        public static List<SelectListItem> GetLeaseTypeDS()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Finance Lease", Value = "1" });
            category.Add(new SelectListItem() { Text = "Sale and Leaseback", Value = "2" });
            category.Add(new SelectListItem() { Text = "Installment Financing", Value = "3" });
            category.Add(new SelectListItem() { Text = "Modal Usaha", Value = "4" });
            category.Add(new SelectListItem() { Text = "Fasilitas Dana", Value = "5" });
            return category;

        }
        public static List<SelectListItem> GetRescOrRest()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            category.Add(new SelectListItem() { Text = "Restructure", Value = "0" });
            category.Add(new SelectListItem() { Text = "Reschedule", Value = "1" });
            return category;
        }
        public static List<LesseeSelect2> GetLesseeDatasource()
        {
            List<LesseeSelect2> list = new List<LesseeSelect2>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT LesseeNo,BranchCode , LesseeName as text FROM Lessee where statuscode = 'A'";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            LesseeSelect2 item = new LesseeSelect2();
                            item.id = IDS.Tool.GeneralHelper.NullToString(dr["LesseeNo"]);
                            item.text = IDS.Tool.GeneralHelper.NullToString(dr["LesseeNo"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["text"]);
                            list.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return list;
        }
        public static List<SelectListItem> GetLeaseForParipasu(string branch)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT LeaseNo, LesseeName FROM Lease INNER JOIN Lessee on Lease.LesseeNo = Lessee.LesseeNo and Lease.LesseeBranch = Lessee.BranchCode where lease.BranchCode = @Branch";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@Branch", SqlDbType.VarChar, branch);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["LeaseNo"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["LeaseNo"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["LesseeName"]);
                            list.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return list;
        }
        public static List<SelectListItem> GetBRP()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT BRPCode,BRPName FROM tblBRP";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["BRPCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["BRPName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static DataTable GetItemsForDataSource(bool withNull)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                if (withNull)
                {
                    db.CommandText = "SELECT NULL AS RescheduleMethodCode, '' AS RescheduleMethodName, -1 IsReschedule UNION SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                }
                else
                {
                    db.CommandText = "SELECT RescheduleMethodCode, RescheduleMethodName FROM LeRescheduleMethod ORDER BY IsReschedule ASC, RescheduleMethodCode;";
                }

                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                dt = db.GetDataTable();

                db.Close();
            }

            return dt;
        }

        public static DataTable GetItemsForDataSource(bool withNull, int restructureOrReschedule)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                if (withNull)
                {
                    if (restructureOrReschedule == 1) // Reschedule
                    {
                        db.CommandText = "SELECT NULL AS RescheduleMethodCode, '' AS RescheduleMethodName, -1 IsReschedule UNION SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 1 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                    else // Restructure
                    {
                        db.CommandText = "SELECT NULL AS RescheduleMethodCode, '' AS RescheduleMethodName, -1 IsReschedule UNION SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 0 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                }
                else
                {
                    if (restructureOrReschedule == 1) // Reschedule
                    {
                        db.CommandText = "SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 1 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                    else // Restructure
                    {
                        db.CommandText = "SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 0 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                }

                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                dt = db.GetDataTable();

                db.Close();
            }

            return dt;
        }
        public static List<SelectListItem> GetRescMethod(bool withNull,int restructureOrReschedule)
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                if (withNull)
                {
                    if (restructureOrReschedule == 1) // Reschedule
                    {
                        db.CommandText = "SELECT NULL AS RescheduleMethodCode, '' AS RescheduleMethodName, -1 IsReschedule UNION SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 1 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                    else // Restructure
                    {
                        db.CommandText = "SELECT NULL AS RescheduleMethodCode, '' AS RescheduleMethodName, -1 IsReschedule UNION SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 0 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                }
                else
                {
                    if (restructureOrReschedule == 1) // Reschedule
                    {
                        db.CommandText = "SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 1 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                    else // Restructure
                    {
                        db.CommandText = "SELECT RescheduleMethodCode, RescheduleMethodName, IsReschedule FROM LeRescheduleMethod WHERE IsReschedule = 0 ORDER BY IsReschedule ASC, RescheduleMethodCode ASC;";
                    }
                }

                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["RescheduleMethodCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["RescheduleMethodName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static double GetOverRateResc(string branch)
        {
            double OverDueRate = 0;
            using (IDS.DataAccess.SqlServer db = new SqlServer())
            {
                db.CommandText = "SELECT ISNULL(OverRate, 0) OverRate FROM tblBranch WHERE BranchCode = @branchCode;";
                db.AddParameter("@branchCode", SqlDbType.VarChar, branch);
                db.CommandType = CommandType.Text;
                db.Open();

                OverDueRate = Convert.ToDouble(db.ExecuteScalar());
                db.Close();
            }
            return OverDueRate;
        }
        public static List<SelectListItem> GetLeaseResc(string branch, string lesseeNo, string lesseeBranch)
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT LeaseNo FROM Lease WHERE LeStatus IN (1,3,6) AND BranchCode=@branch AND LesseeNo=@lesseeNo AND LesseeBranch = @lesseeBranch";
                db.CommandType = CommandType.Text;
                db.AddParameter("@branch", SqlDbType.VarChar, branch);
                db.AddParameter("@lesseeNo", SqlDbType.VarChar, lesseeNo);
                db.AddParameter("@lesseeBranch", SqlDbType.VarChar, lesseeBranch);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["LeaseNo"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["LeaseNo"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetCustResc(string branch)
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "LeSelRescDatasource";
                db.CommandType = CommandType.StoredProcedure;
                db.AddParameter("@Branch", SqlDbType.VarChar, branch);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["CustNo"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["CustName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetLeaseForDSWithProd(string branch, string prod)
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select leaseno,lesseename from lease le inner join lessee l on le.LesseeNo = l.LesseeNo and le.LesseeBranch = l.BranchCode where le.BranchCode = @branch and  ProdCode = @prod and le.lestatus IN(0,1)";
                db.CommandType = CommandType.Text;
                db.AddParameter("@branch", SqlDbType.VarChar, branch);
                db.AddParameter("prod", SqlDbType.VarChar, prod);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["LeaseNo"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["LeaseNo"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["LesseeName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetBRO()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using(IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT BROCode,BROName FROM tblBRO";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while(dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["BROCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["BROName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            
            return category;
        }
        public static List<SelectListItem> GetAreaDS()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT AreaCode,AreaName FROM Area";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["AreaCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["AreaName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }

        public static List<SelectListItem> GetInternalReference()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM View_InternalReference ORDER BY Status,BranchCode ASC";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["BranchName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
      
        public static List<SelectListItem> GetCustomerPaymentLease()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "  select distinct le.lesseeno, le.lesseename, le.branchcode from lessee le inner join lease la on le.LesseeNo = la.LesseeNo and le.BranchCode = la.LesseeBranch";
                db.CommandType = CommandType.Text;

                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["lesseeno"]) + "," + IDS.Tool.GeneralHelper.NullToString(dr["branchcode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["lesseeno"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["lesseename"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetCustomerPayment()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select DISTINCT LesseeNo,L.BranchCode, LesseeFullName from Lessee L INNER JOIN FTTransH H ON L.BranchCode = H.ClientBranch AND H.ClientID = L.LesseeNo INNER JOIN FTTransD D ON D.AgreementNo = H.AgreementNo WHERE H.FactStatus = 2";
                db.CommandType = CommandType.Text;

                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["LesseeNo"]) + ","+IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["LesseeNo"]) +" - "+IDS.Tool.GeneralHelper.NullToString(dr["LesseeFullName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }

        public static List<SelectListItem> FillBankDisbFT()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblBank";
                db.CommandType = CommandType.Text;
           
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["BankCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["BankName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> FillProject(string branch, string lesseeBranch, string lesseeNo)
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT NULL AS Code , NULL AS Name UNION ALL SELECT LeProject.Code, LeProject.Name FROM LeProject LEFT JOIN LeNUP on LeProject.Code = LeNUP.ProjectNo and LeNUP.BranchCode = LeProject.BranchCode WHERE LeProject.BranchCode = @branch AND (LeNUP.NUPNo is null OR (LeNUP.LesseeNo = @lesseeNo AND LeNUP.LesseeBranch = @lesseeBranch)) ORDER BY NAME ASC";
                db.CommandType = CommandType.Text;
                db.AddParameter("@branch", SqlDbType.VarChar, branch);
                db.AddParameter("@lesseeBranch", SqlDbType.VarChar, lesseeBranch);
                db.AddParameter("@lesseeNo", SqlDbType.VarChar, lesseeNo);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["Code"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["Name"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetActiveProductByFinanceCriteriaDataTable(int corpRetail, int LeaseMethod, int financeMethod, int goodsService, int leaseType, bool withNull)
        {
            List<SelectListItem> category = new List<SelectListItem>();

            string strSql = " ";

            #region LeaseMethod
            switch (LeaseMethod)
            {
                case 1:
                    strSql += " AND (LeaseMethod IN (1, 3)) ";
                    break;
                case 2:
                    strSql += " AND (LeaseMethod IN (2, 3)) ";
                    break;
            }
            #endregion

            #region FinanceMethod
            switch (financeMethod)
            {
                case 1:
                    strSql += " AND (FinanceMethod IN (1, 3, 5, 7)) ";
                    break;
                case 2:
                    strSql += " AND (FinanceMethod IN (2, 3, 5, 7)) ";
                    break;
                case 3:
                    strSql += " AND (FinanceMethod IN (4, 5, 6, 7)) ";
                    break;
            }
            #endregion

            #region GoodsServices
            switch (goodsService)
            {
                case 1:
                    strSql += " AND GoodsService IN (1, 3) ";
                    break;
                case 2:
                    strSql += " AND GoodsService IN (2, 3) ";
                    break;
            }
            #endregion

            #region LeaseType
            switch (leaseType)
            {
                case 1:
                    strSql += " AND LeaseType IN (1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31) ";
                    break;
                case 2:
                    strSql += " AND LeaseType IN (2, 3, 6, 7, 10, 11, 14, 15, 18, 19, 22, 23, 26, 27, 30, 31) ";
                    break;
                case 3:
                    strSql += " AND LeaseType IN (4, 5, 6, 7, 12, 13, 14, 15, 20, 21, 22, 23, 28, 29, 30, 31) ";
                    break;
                case 4:
                    strSql += " AND LeaseType IN (8, 9, 10, 11, 12, 13, 14, 15, 24, 25, 26, 27, 28, 29, 30, 31) ";
                    break;
                case 5:
                    strSql += " AND LeaseType IN (16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31) ";
                    break;
            }
            #endregion

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                if (withNull)
                    db.CommandText = "SELECT NULL AS ProdCode, '' AS ProdName UNION ";
                else
                    db.CommandText = "";

                db.CommandText += "SELECT ProdCode, ProdName FROM LeProduct WHERE Status = 1 " + strSql;
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetActiveProductByFinanceCriteriaDataTableForLease(int corpRetail, int financeMethod, int goodsService, int leaseType, bool withNull)
        {
            List<SelectListItem> category = new List<SelectListItem>();

            string strSql = " ";

            //#region LeaseMethod
            //switch (LeaseMethod)
            //{
            //    case 1:
            //        strSql += " AND (LeaseMethod IN (1, 3)) ";
            //        break;
            //    case 2:
            //        strSql += " AND (LeaseMethod IN (2, 3)) ";
            //        break;
            //}
            //#endregion

            #region FinanceMethod
            switch (financeMethod)
            {
                case 1:
                    strSql += " AND (FinanceMethod IN (1, 3, 5, 7)) ";
                    break;
                case 2:
                    strSql += " AND (FinanceMethod IN (2, 3, 5, 7)) ";
                    break;
                case 3:
                    strSql += " AND (FinanceMethod IN (4, 5, 6, 7)) ";
                    break;
            }
            #endregion

            #region GoodsServices
            switch (goodsService)
            {
                case 1:
                    strSql += " AND GoodsService IN (1, 3) ";
                    break;
                case 2:
                    strSql += " AND GoodsService IN (2, 3) ";
                    break;
            }
            #endregion

            #region LeaseType
            switch (leaseType)
            {
                case 1:
                    strSql += " AND LeaseType IN (1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31) ";
                    break;
                case 2:
                    strSql += " AND LeaseType IN (2, 3, 6, 7, 10, 11, 14, 15, 18, 19, 22, 23, 26, 27, 30, 31) ";
                    break;
                case 3:
                    strSql += " AND LeaseType IN (4, 5, 6, 7, 12, 13, 14, 15, 20, 21, 22, 23, 28, 29, 30, 31) ";
                    break;
                case 4:
                    strSql += " AND LeaseType IN (8, 9, 10, 11, 12, 13, 14, 15, 24, 25, 26, 27, 28, 29, 30, 31) ";
                    break;
                case 5:
                    strSql += " AND LeaseType IN (16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31) ";
                    break;
            }
            #endregion

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                if (withNull)
                    db.CommandText = "SELECT NULL AS ProdCode, '' AS ProdName UNION ";
                else
                    db.CommandText = "";

                db.CommandText += "SELECT ProdCode, ProdName FROM LeProduct WHERE Status = 1 " + strSql;
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetRevolving()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Not Revolving", Value = "false" });
            RP.Add(new SelectListItem() { Text = "Revolving", Value = "true" });
            return RP;
        }
        public static List<SelectListItem> GetFactType()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Factoring without Recourses Basis", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Factoring with Recourses Basis", Value = "1" });
            return RP;
        }
        public static List<SelectListItem> GetFactFeeType()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "First Advance Payment", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Every Disburse Payment", Value = "1" });
            return RP;
        }
        public static List<SelectListItem> GetVATList()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "No Include", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Include", Value = "1" });
            return RP;
        }
        public static List<SelectListItem> GetPelunasanStatusListForGrid()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "All", Value = "" },
                new SelectListItem() { Text = "Open", Value = "0" },
                new SelectListItem() { Text = "Lunas", Value = "1"},
                new SelectListItem() { Text = "Cancel", Value = "2"},
                new SelectListItem() { Text = "Giro Tolak", Value = "3"},
            };
        }
    
        public static List<SelectListItem> GetGuaranteeType()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "From Personal", Value = "0" });
            RP.Add(new SelectListItem() { Text = "From Corporate", Value = "1" });
            RP.Add(new SelectListItem() { Text = "From Payment Guarantee", Value = "2" });
            RP.Add(new SelectListItem() { Text = "Hipotik Land and Building", Value = "3" });
            return RP;
        }
        public static List<SelectListItem> GetInstType()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Giro Bilyet", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Cheque", Value = "1" });
            RP.Add(new SelectListItem() { Text = "Cash", Value = "2" });
            RP.Add(new SelectListItem() { Text = "Inkaso", Value = "3" });
            RP.Add(new SelectListItem() { Text = "Standing Order", Value = "4" });
            RP.Add(new SelectListItem() { Text = "Cummulative", Value = "5" });
            RP.Add(new SelectListItem() { Text = "Transfer", Value = "6" });
            return RP;
        }
        public static List<SelectListItem> FillPeriodResc(string leaseno)
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "LeSelReschPeriod";
                db.CommandType = CommandType.StoredProcedure;
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, leaseno);
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["HSL"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["Period"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        public static List<SelectListItem> GetSyncList()
        {
            List<SelectListItem> RP = new List<SelectListItem>();
            RP.Add(new SelectListItem() { Text = "Customer", Value = "0" });
            RP.Add(new SelectListItem() { Text = "Contract", Value = "1" });
            RP.Add(new SelectListItem() { Text = "Payment", Value = "2" });
            return RP;
        }
        public static List<SelectListItem> GetPayMtdListForGrid()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "Cheque", Value = "1"},
                new SelectListItem() { Text = "Giro", Value = "0"},
                new SelectListItem() { Text = "Transfer", Value = "6"},
            };
        }
        public static List<SelectListItem> GetPayMtdListForGridLease()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "Cheque", Value = "1"},
                new SelectListItem() { Text = "Giro", Value = "0"},
                new SelectListItem() { Text = "Transfer", Value = "6"},
            };
        }
        //SOA untuk disburse datasource
        public static int SyncCustomerCredinex(DateTime date)
        {
            int res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    db.CommandText = "LeInsCustomerCredinex";
                    db.CommandType = CommandType.StoredProcedure;
                    db.Open();
                    db.BeginTransaction();
                    res = db.ExecuteNonQuery();
                    db.CommitTransaction();
                }
                catch(SqlException sex)
                {
                    res = -1;
                    if(db.Transaction != null)
                        db.RollbackTransaction();
                }
                finally
                {
                    db.Close();
                }
                
            }
            return res;
        }

        public static int SyncContractCredinex(DateTime date)
        {
            int res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    db.CommandText = "LeInsCrediDisb";
                    db.CommandType = CommandType.StoredProcedure;
                    db.AddParameter("@Date", SqlDbType.DateTime, date.Date);
                    db.Open();
                    db.BeginTransaction();
                    res = db.ExecuteNonQuery();
                    db.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
                finally
                {
                    db.Close();
                }

            }
            return res;
        }

        public static int SyncPaymentCredinex(DateTime date)
        {
            int res = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    db.CommandText = "LeInsKrediPayment";
                    db.CommandType = CommandType.StoredProcedure;
                    db.AddParameter("@date", SqlDbType.DateTime, date.Date);
                    db.Open();
                    db.BeginTransaction();
                    res = db.ExecuteNonQuery();
                    db.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
                finally
                {
                    db.Close();
                }

            }
            return res;
        }
        public static List<SelectListItem> GetApprovalStatus()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "All", Value = "-1"},
                new SelectListItem() { Text = "Need Approve", Value = "0"},
                new SelectListItem() { Text = "Approved", Value = "1"},
                new SelectListItem() { Text = "Rejected", Value = "2"},
            };
        }
        public static List<SelectListItem> GetRequestStatus()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "All", Value = "-1"},
                new SelectListItem() { Text = "Open", Value = "0"},
                new SelectListItem() { Text = "Closed", Value = "1"},
            };
        }
        public static List<SelectListItem> GetGoodsServicesDS()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "Barang", Value = "1"},
                new SelectListItem() { Text = "Jasa", Value = "2"}
            };
        }

        public static List<SelectListItem> GetTypeKPR()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "Tanda Terima Nasabah", Value = "1"},
                new SelectListItem() { Text = "Instruksi Pencairan", Value = "2"},
                new SelectListItem() { Text = "Surat Perjanjian", Value = "3"},
                new SelectListItem() { Text = "All", Value = "4"},
            };
        }
        public static List<SelectListItem> GetTypeJITU()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "Checklist", Value = "1"},
                new SelectListItem() { Text = "Instruksi Pencairan", Value = "2"},
                new SelectListItem() { Text = "Perjanjian Pembiayaan", Value = "3"},
                new SelectListItem() { Text = "Surat Kuasa", Value = "4"},
                new SelectListItem() { Text = "All", Value = "5"},

            };
        }
        public static List<SelectListItem> GetTypePINTAR()
        {
            return new List<SelectListItem> {
                new SelectListItem() { Text = "Checklist", Value = "1"},
                new SelectListItem() { Text = "Instruksi Pencairan", Value = "2"},
                new SelectListItem() { Text = "Perjanjian Pembiayaan", Value = "3"},
                new SelectListItem() { Text = "Surat Kuasa", Value = "4"},
                new SelectListItem() { Text = "Surat Pernyataan", Value = "5"},
                new SelectListItem() { Text = "All", Value = "6"},

            };
        }

        //Add By Renaldi 17 December 2024
        public static List<SelectListItem> GetCustomerPaymentForEdit()
        {
            List<SelectListItem> category = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select DISTINCT LesseeNo,L.BranchCode, LesseeFullName from Lessee L INNER JOIN FTTransH H ON L.BranchCode = H.ClientBranch AND H.ClientID = L.LesseeNo INNER JOIN FTTransD D ON D.AgreementNo = H.AgreementNo";
                db.CommandType = CommandType.Text;

                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new SelectListItem();
                            item.Value = IDS.Tool.GeneralHelper.NullToString(dr["LesseeNo"]) + "," + IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            item.Text = IDS.Tool.GeneralHelper.NullToString(dr["LesseeNo"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["LesseeFullName"]);
                            category.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }

            return category;
        }
        //End Add

        //Add by Jeremi 12 Juni 2025
        public static int CekMenuRequest(string controllerName)
        {
            int flag = 0;
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select MenuRequest from MntWebMenu where Controller='" + controllerName + "'";
                db.CommandType = CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            flag = IDS.Tool.GeneralHelper.NullToInt(dr["MenuRequest"], 0);
                        }
                    }
                }
            }
            return flag;
        }
        //End Jeremi
    }
}