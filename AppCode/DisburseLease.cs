using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace AppCode
{
    public class DisburseLease
    {
        clsModule mdl = new clsModule();
        
        private static DisburseLease INSTANCE;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly clsModule _mdl;
        //public static DisburseLease GetINSTANCE()
        //{
        //    if (INSTANCE == null)
        //    {
        //        INSTANCE = new DisburseLease();
        //    }
        //    return INSTANCE;
        //}
        private DisburseLease()
        {
            _mdl = new clsModule();
            _httpContextAccessor = new HttpContextAccessor(); // manual ambil accessor
        }

        public static DisburseLease GetINSTANCE()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new DisburseLease();
            }
            return INSTANCE;
        }
        public void Le_SPWSaveTrans(
          string LeaseNo,
          string BranchCode,
          string LeStatusNew,
          string EntryDate,
          bool TransType,
          string TransCode,
          string JCode,
          string ValueDate,
          string CurrCode,
          string Amount,
          string BaseAmount,
          string StatusBefore,
          string StatusAfter,
          string DescBefore1,
          string DescAfter1,
          string BankCode,
          string LogID,
          string Remarks,
          ref long Nilai,
          ref int msg
          )
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    string query = "Le_SPWSaveTrans ";
                    query += "@LeaseNo, ";
                    query += "@BranchCode, ";
                    query += "@LeStatusNew, ";
                    query += "@EntryDate, ";
                    query += "@TransType, ";
                    query += "@TransCode, ";
                    query += "@JCode, ";
                    query += "@ValueDate, ";
                    query += "@CurrCode, ";
                    query += "@Amount, ";
                    query += "@BaseAmount, ";
                    query += "@StatusBefore, ";
                    query += "@StatusAfter,	";
                    query += "@DescBefore1,	";
                    query += "@DescAfter1, ";
                    query += "@BankCode, ";
                    query += "@LogID, ";
                    query += "@Remarks ";

                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = mdl.setNullParam(LeaseNo).Value;
                    cmd.Parameters.Add("@BranchCode", SqlDbType.VarChar).Value = mdl.setNullParam(BranchCode).Value;
                    cmd.Parameters.Add("@LeStatusNew", SqlDbType.SmallInt).Value = mdl.setNullParam(LeStatusNew).Value;
                    cmd.Parameters.Add("@EntryDate", SqlDbType.DateTime).Value = mdl.setNullParam(EntryDate).Value;
                    cmd.Parameters.Add("@TransType", SqlDbType.Bit).Value = mdl.setNullParam(Convert.ToString(TransType)).Value;
                    cmd.Parameters.Add("@TransCode", SqlDbType.VarChar).Value = mdl.setNullParam(TransCode).Value;
                    cmd.Parameters.Add("@JCode", SqlDbType.VarChar).Value = mdl.setNullParam(JCode).Value;
                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = mdl.setNullParam(ValueDate).Value;
                    cmd.Parameters.Add("@CurrCode", SqlDbType.VarChar).Value = mdl.setNullParam(CurrCode).Value;
                    cmd.Parameters.Add("@Amount", SqlDbType.Money).Value = mdl.setNullParam(Amount).Value;
                    cmd.Parameters.Add("@BaseAmount", SqlDbType.Money).Value = mdl.setNullParam(BaseAmount).Value;
                    cmd.Parameters.Add("@StatusBefore", SqlDbType.SmallInt).Value = mdl.setNullParam(StatusBefore).Value;
                    cmd.Parameters.Add("@StatusAfter", SqlDbType.SmallInt).Value = mdl.setNullParam(StatusAfter).Value;
                    cmd.Parameters.Add("@DescBefore1", SqlDbType.VarChar).Value = mdl.setNullParam(DescBefore1).Value;
                    cmd.Parameters.Add("@DescAfter1", SqlDbType.VarChar).Value = mdl.setNullParam(DescAfter1).Value;
                    cmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = mdl.setNullParam(BankCode).Value;
                    cmd.Parameters.Add("@LogID", SqlDbType.VarChar).Value = mdl.setNullParam(LogID).Value;
                    cmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = mdl.setNullParam(Remarks).Value;

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    cmd.CommandText = "SELECT @@IDENTITY";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    Nilai = Convert.ToInt64(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();

                    msg = 0;
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        //Add by Jeremi 13 Juni 2025
        public void Le_SPWSaveTransLoan(
          string LeaseNo,
          string BranchCode,
          string LeStatusNew,
          string EntryDate,
          bool TransType,
          string TransCode,
          string JCode,
          string ValueDate,
          string CurrCode,
          string Amount,
          string BaseAmount,
          string StatusBefore,
          string StatusAfter,
          string DescBefore1,
          string DescAfter1,
          string BankCode,
          string LogID,
          string Remarks,
          ref long Nilai,
          ref int msg
          )
        {
            //Add By Renaldi 25 February 2025 FOR LOG
            string operatorID = string.Empty;
            string myIp = string.Empty;
            if (HttpContextHelper.Current != null && HttpContextHelper.Current.Session != null && HttpContextHelper.Current.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID) != null)
            {
                operatorID = HttpContextHelper.Current.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID).ToString();
                myIp = HttpContextHelper.GetIp();
            }
            //End Add

            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    //Add By Renaldi 25 February 2025 FOR LOG
                    if (!string.IsNullOrEmpty(operatorID))
                    {
                        cmd.CommandText = "EXEC sp_set_session_context 'AppUser', @User";
                        cmd.Parameters.Add("@User", SqlDbType.VarChar).Value = mdl.setNullParam(operatorID).Value;
                        cmd.CommandType = System.Data.CommandType.Text;

                        trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    if (!string.IsNullOrEmpty(myIp))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "EXEC sp_set_session_context 'IPUser', @IPUser";
                        cmd.Parameters.Add("@IPUser", SqlDbType.VarChar).Value = mdl.setNullParam(myIp).Value;
                        cmd.CommandType = System.Data.CommandType.Text;

                        trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                        cmd.Parameters.Clear();
                    }
                    //End Add

                    cmd.CommandType = CommandType.Text;

                    string query = "Le_SPWSaveTransLoan ";
                    query += "@LeaseNo, ";
                    query += "@BranchCode, ";
                    query += "@LeStatusNew, ";
                    query += "@EntryDate, ";
                    query += "@TransType, ";
                    query += "@TransCode, ";
                    query += "@JCode, ";
                    query += "@ValueDate, ";
                    query += "@CurrCode, ";
                    query += "@Amount, ";
                    query += "@BaseAmount, ";
                    query += "@StatusBefore, ";
                    query += "@StatusAfter,	";
                    query += "@DescBefore1,	";
                    query += "@DescAfter1, ";
                    query += "@BankCode, ";
                    query += "@LogID, ";
                    query += "@Remarks ";

                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = mdl.setNullParam(LeaseNo).Value;
                    cmd.Parameters.Add("@BranchCode", SqlDbType.VarChar).Value = mdl.setNullParam(BranchCode).Value;
                    cmd.Parameters.Add("@LeStatusNew", SqlDbType.SmallInt).Value = mdl.setNullParam(LeStatusNew).Value;
                    cmd.Parameters.Add("@EntryDate", SqlDbType.DateTime).Value = mdl.setNullParam(EntryDate).Value;
                    cmd.Parameters.Add("@TransType", SqlDbType.Bit).Value = mdl.setNullParam(Convert.ToString(TransType)).Value;
                    cmd.Parameters.Add("@TransCode", SqlDbType.VarChar).Value = mdl.setNullParam(TransCode).Value;
                    cmd.Parameters.Add("@JCode", SqlDbType.VarChar).Value = mdl.setNullParam(JCode).Value;
                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = mdl.setNullParam(ValueDate).Value;
                    cmd.Parameters.Add("@CurrCode", SqlDbType.VarChar).Value = mdl.setNullParam(CurrCode).Value;
                    cmd.Parameters.Add("@Amount", SqlDbType.Money).Value = mdl.setNullParam(Amount).Value;
                    cmd.Parameters.Add("@BaseAmount", SqlDbType.Money).Value = mdl.setNullParam(BaseAmount).Value;
                    cmd.Parameters.Add("@StatusBefore", SqlDbType.SmallInt).Value = mdl.setNullParam(StatusBefore).Value;
                    cmd.Parameters.Add("@StatusAfter", SqlDbType.SmallInt).Value = mdl.setNullParam(StatusAfter).Value;
                    cmd.Parameters.Add("@DescBefore1", SqlDbType.VarChar).Value = mdl.setNullParam(DescBefore1).Value;
                    cmd.Parameters.Add("@DescAfter1", SqlDbType.VarChar).Value = mdl.setNullParam(DescAfter1).Value;
                    cmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = mdl.setNullParam(BankCode).Value;
                    cmd.Parameters.Add("@LogID", SqlDbType.VarChar).Value = mdl.setNullParam(LogID).Value;
                    cmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = mdl.setNullParam(Remarks).Value;

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    //Edited By Renaldi 25 February 2025 FOR LOG
                    //cmd.CommandText = "SELECT @@IDENTITY";
                    cmd.CommandText = "select top 1 TransNo from LoanTrans order by TransNo desc";
                    //End Edited
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    Nilai = Convert.ToInt64(cmd.ExecuteScalar());
                    msg = 1;

                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();

                    msg = 0;
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void RA_SPWSaveTrans(
          string LeaseNo,
          string BranchCode,
          string LeStatusNew,
          string EntryDate,
          bool TransType,
          string TransCode,
          string JCode,
          string ValueDate,
          string CurrCode,
          string Amount,
          string BaseAmount,
          string StatusBefore,
          string StatusAfter,
          string DescBefore1,
          string DescAfter1,
          string BankCode,
          string LogID,
          string Remarks,
          ref long Nilai,
          ref int msg
          )
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    string query = "RA_SPWSaveTransLoan ";
                    query += "@LeaseNo, ";
                    query += "@BranchCode, ";
                    query += "@LeStatusNew, ";
                    query += "@EntryDate, ";
                    query += "@TransType, ";
                    query += "@TransCode, ";
                    query += "@JCode, ";
                    query += "@ValueDate, ";
                    query += "@CurrCode, ";
                    query += "@Amount, ";
                    query += "@BaseAmount, ";
                    query += "@StatusBefore, ";
                    query += "@StatusAfter,	";
                    query += "@DescBefore1,	";
                    query += "@DescAfter1, ";
                    query += "@BankCode, ";
                    query += "@LogID, ";
                    query += "@Remarks ";

                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = mdl.setNullParam(LeaseNo).Value;
                    cmd.Parameters.Add("@BranchCode", SqlDbType.VarChar).Value = mdl.setNullParam(BranchCode).Value;
                    cmd.Parameters.Add("@LeStatusNew", SqlDbType.SmallInt).Value = mdl.setNullParam(LeStatusNew).Value;
                    cmd.Parameters.Add("@EntryDate", SqlDbType.DateTime).Value = mdl.setNullParam(EntryDate).Value;
                    cmd.Parameters.Add("@TransType", SqlDbType.Bit).Value = mdl.setNullParam(Convert.ToString(TransType)).Value;
                    cmd.Parameters.Add("@TransCode", SqlDbType.VarChar).Value = mdl.setNullParam(TransCode).Value;
                    cmd.Parameters.Add("@JCode", SqlDbType.VarChar).Value = mdl.setNullParam(JCode).Value;
                    cmd.Parameters.Add("@ValueDate", SqlDbType.DateTime).Value = mdl.setNullParam(ValueDate).Value;
                    cmd.Parameters.Add("@CurrCode", SqlDbType.VarChar).Value = mdl.setNullParam(CurrCode).Value;
                    cmd.Parameters.Add("@Amount", SqlDbType.Money).Value = mdl.setNullParam(Amount).Value;
                    cmd.Parameters.Add("@BaseAmount", SqlDbType.Money).Value = mdl.setNullParam(BaseAmount).Value;
                    cmd.Parameters.Add("@StatusBefore", SqlDbType.SmallInt).Value = mdl.setNullParam(StatusBefore).Value;
                    cmd.Parameters.Add("@StatusAfter", SqlDbType.SmallInt).Value = mdl.setNullParam(StatusAfter).Value;
                    cmd.Parameters.Add("@DescBefore1", SqlDbType.VarChar).Value = mdl.setNullParam(DescBefore1).Value;
                    cmd.Parameters.Add("@DescAfter1", SqlDbType.VarChar).Value = mdl.setNullParam(DescAfter1).Value;
                    cmd.Parameters.Add("@BankCode", SqlDbType.VarChar).Value = mdl.setNullParam(BankCode).Value;
                    cmd.Parameters.Add("@LogID", SqlDbType.VarChar).Value = mdl.setNullParam(LogID).Value;
                    cmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = mdl.setNullParam(Remarks).Value;

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    //Edited By Renaldi 25 February 2025 FOR LOG
                    //cmd.CommandText = "SELECT @@IDENTITY";
                    cmd.CommandText = "select top 1 TransNo from LoanTrans order by TransNo desc";
                    //End Edited
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Clear();
                    Nilai = Convert.ToInt64(cmd.ExecuteScalar());
                    msg = 1;
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();

                    msg = 0;
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        //End Jeremi
        public void Le_SPUpdatedOSLessee(
       string LesseeNo,
       string BranchCode,
       string CurrCode,
       string UsedCreditLimit,
       string CrdLmtType,
       int Type,
       ref int msg
       )
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    string query = "Le_SPUpdatedOSLessee ";
                    query += "@LesseeNo, ";
                    query += "@BranchCode, ";
                    query += "@CurrCode, ";
                    query += "@UsedCreditLimit, ";
                    query += "@CrdLmtType, ";
                    query += "@Utype";

                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LesseeNo", SqlDbType.VarChar).Value = mdl.setNullParam(LesseeNo).Value;
                    cmd.Parameters.Add("@BranchCode", SqlDbType.VarChar).Value = mdl.setNullParam(BranchCode).Value;
                    cmd.Parameters.Add("@CurrCode", SqlDbType.VarChar).Value = mdl.setNullParam(CurrCode).Value;
                    cmd.Parameters.Add("@UsedCreditLimit", SqlDbType.Money).Value = mdl.setNullParam(UsedCreditLimit).Value;
                    cmd.Parameters.Add("@CrdLmtType", SqlDbType.TinyInt).Value = mdl.setNullParam(CrdLmtType).Value;
                    cmd.Parameters.Add("@Utype", SqlDbType.VarChar).Value = mdl.setNullParam(Convert.ToString(Type)).Value;

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();

                    msg = 0;
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        public void Le_SPUpdatedToLease(
      string LeaseNo,
      string BankCode,
      string InstType,
      string CurrCode
      )
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                SqlTransaction trans = null;
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "Le_SPUpdateToLease ";
                    cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
                    cmd.Parameters.AddWithValue("@BankCode", BankCode);
                    cmd.Parameters.AddWithValue("@InstType", InstType);
                    cmd.Parameters.AddWithValue("@CurrCode", CurrCode);

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();

                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }

        }
    }
}
