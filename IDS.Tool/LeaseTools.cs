using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Data.SqlClient;

namespace IDS.Tool
{
    public class LeaseTools
    {
        private static LeaseTools INSTANCE;

        public static LeaseTools GetINSTANCE()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new LeaseTools();
            }
            return INSTANCE;
        }
        public void ExecuteLepruCalculateAccrual(string LeaseNo, string CurrCode)
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

                    string query = "exec ";
                    query += "Le_SPCalculateAccrual ";
                    query += "@LeaseNo, @CurrCode";

                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = LeaseNo;
                    cmd.Parameters.Add("@CurrCode", SqlDbType.VarChar).Value = CurrCode;

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void DisbursementContarct(string LeaseNo, int status)
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

                    string query = "exec ";
                    query += "Le_SPUPDATELeStatus ";
                    query += "@LeaseNo, @Status ";

                    cmd.CommandText = query;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LeaseNo", SqlDbType.VarChar).Value = LeaseNo;
                    cmd.Parameters.Add("@Status", SqlDbType.SmallInt).Value = status;

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                  
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public void Le_SPUpdateGLTrans(IDS.DataAccess.SqlServer cmd,
        string strTransNo,
        string Scode,
        string Voucher,
        byte Counter,
        DateTime EntryDate,
        DateTime TransDate,
        bool Reverse,
        string Dept,
        string Account,
        string Ccy,
        double Amount,
        string Description,
        string LeaseNo,
        string DocNo,
        bool TransferToGL,
        int ErrCode,
        DateTime OriTransDate,
        string strBranchCode,
        string strAccCashBasis,
        ref int message
    )
        {
            int Action = 0;

            IDS.DataAccess.SqlServer db = cmd;

            try
            {
                db.CommandText = "SELECT CASE WHEN EXISTS (SELECT * FROM LeGLTRANS WHERE SCODE = @SCode AND Voucher=@Voucher AND BranchCode = @BranchCode AND Counter = @Counter) THEN 1 ELSE 0 END";
                db.AddParameter("@SCode", SqlDbType.VarChar, Scode);
                db.AddParameter("@Voucher", SqlDbType.VarChar, Voucher);
                db.AddParameter("@BranchCode", SqlDbType.VarChar, strBranchCode);
                db.AddParameter("@Counter", SqlDbType.Int, Counter);
                db.CommandType = CommandType.Text;
                db.Open();

                int count = Convert.ToInt32(db.ExecuteScalar());

                if (count == 0)
                    Action = 0;
                else
                    Action = 1;

                db.CommandText = "Le_SPUpdateGLTrans";
                db.AddParameter("@TransNo", SqlDbType.VarChar, strTransNo);
                db.AddParameter("@Action", SqlDbType.Int, Action);
                db.AddParameter("@SCode", SqlDbType.VarChar, Scode);
                db.AddParameter("@Voucher", SqlDbType.VarChar, Voucher);
                db.AddParameter("@Counter", SqlDbType.Int, Counter);
                db.AddParameter("@EntryDate", SqlDbType.DateTime, EntryDate);
                db.AddParameter("@TransDate", SqlDbType.Date, TransDate);
                db.AddParameter("@Reverse", SqlDbType.Bit, Reverse);
                db.AddParameter("@Dept", SqlDbType.VarChar, Dept);
                db.AddParameter("@Account", SqlDbType.VarChar, Account);
                db.AddParameter("@Ccy", SqlDbType.VarChar, Ccy);
                db.AddParameter("@Amount", SqlDbType.Money, Amount);
                db.AddParameter("@Description", SqlDbType.VarChar, Description);
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, LeaseNo);
                db.AddParameter("@DocNo", SqlDbType.VarChar, DocNo);
                db.AddParameter("@TransferToGL", SqlDbType.Bit, TransferToGL);
                db.AddParameter("@ErrCode", SqlDbType.Int, ErrCode);
                db.AddParameter("@OriTransDate", SqlDbType.DateTime, OriTransDate);
                db.AddParameter("@BranchCode", SqlDbType.VarChar, strBranchCode);
                db.AddParameter("@AccCashBasis", SqlDbType.VarChar, string.IsNullOrEmpty(strAccCashBasis) ? "" : strAccCashBasis);

                db.CommandType = CommandType.StoredProcedure;
                db.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                message = 0;
                db.RollbackTransaction();
            }
        }

        public void CalcPenalty(string strLeaseNo, DateTime ProcessDate, double Rate)
        {
            clsModule mdl = new clsModule();
            StringBuilder sb = new StringBuilder();

            DateTime DueDate;
            int Period;
            double Rental;
            double Penalty;
            double vPayment;
            int vDay;
            DateTime vClearDate;

            bool Max = false;
            double MaxResidu = 0;
            double MaxRental = 0;
            int MaxPaidPeriod = 0;
            DateTime MaxDue;

            // Untuk mencari pembayaran terakhir yang dilakukan lessee tetapi belum full
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                sb.Remove(0, sb.ToString().Length);

                sb.Append("SELECT ");
                sb.Append("sc.duedate, sc.Period, sc.Rental, ");
                sb.Append("(sc.rental) - ( ");
                sb.Append("SELECT ISNULL(SUM(ISNULL(la.AlloLeAmt, 0)), 0) ");
                sb.Append("FROM LePaymentInst lp ");
                sb.Append("INNER JOIN LeAllocation la ON lp.InstNo = la.InstNo ");
                sb.Append("WHERE lp.Status = 2 AND la.AlloType = 3 AND la.LeaseNo = sc.LeaseNo AND la.Period = sc.Period ");
                sb.Append("GROUP BY la.LeaseNo, la.Period) AS TotalPaid ");
                sb.Append("FROM Schedule sc ");
                sb.Append("INNER JOIN ");
                sb.Append("( ");
                sb.Append("SELECT la.leaseno, MAX(la.period) MaxPeriod ");
                sb.Append("FROM LePaymentInst lp inner join LeAllocation la ON lp.InstNo = la.InstNo ");
                sb.Append("WHERE lp.Status = 2 and la.AlloType = 3 and la.LeaseNo = @lease ");
                sb.Append("GROUP BY la.LeaseNo ");
                sb.Append(") AS MaxPeriod ON sc.leaseno = MaxPeriod.leaseno ");
                sb.Append("AND sc.period = MaxPeriod.MaxPeriod");

                db.CommandText = sb.ToString();
                db.CommandType = CommandType.Text;
                db.AddParameter("@lease", SqlDbType.VarChar, strLeaseNo);
                db.Open();

                DataTable dtMaxPay = new DataTable();
                dtMaxPay = db.GetDataTable();

                if (dtMaxPay.Rows.Count > 0)
                {
                    Max = true;
                    MaxDue = Convert.ToDateTime(dtMaxPay.Rows[0][0]);
                    MaxPaidPeriod = Convert.ToInt32(dtMaxPay.Rows[0][1]);
                    MaxRental = Convert.ToDouble(dtMaxPay.Rows[0][2]);
                    MaxResidu = Convert.ToDouble(dtMaxPay.Rows[0][3]);
                }

                sb.Remove(0, sb.ToString().Length);

                db.CommandText = "SELECT CurrCode FROM Lease WHERE LeaseNo = @leaseno;";
                db.AddParameter("@leaseno", SqlDbType.VarChar, strLeaseNo);
                db.CommandType = CommandType.Text;
                db.Open();

                string strCurrCode = db.ExecuteScalar().ToString();

                db.CommandText = "SELECT Period, DueDate, Rental FROM Schedule WHERE LeaseNo=@leaseno AND Rental > 0 AND DueDate <= @duedate;";
                db.AddParameter("@leaseno", SqlDbType.VarChar, strLeaseNo);
                db.AddParameter("@DueDate", SqlDbType.DateTime, ProcessDate);
                db.CommandType = CommandType.Text;
                db.Open();

                DataTable dt = db.GetDataTable();

                db.BeginTransaction();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    Period = Convert.ToInt16(row["Period"]);
                    Rental = Convert.ToDouble(row["Rental"]);
                    DueDate = Convert.ToDateTime(row["DueDate"]);
                    Penalty = 0;

                    sb.Remove(0, sb.ToString().Length);
                    sb.Append("SELECT LeAllocation.InstNo, LeAllocation.AlloLeAmt, LePaymentInst.ClearDate ");
                    sb.Append("FROM LeAllocation ");
                    sb.Append("INNER JOIN LePaymentInst ");
                    sb.Append("ON LeAllocation.InstNo=LePaymentInst.InstNo ");
                    sb.Append("WHERE LeAllocation.LeaseNo=@leaseno ");
                    sb.Append("AND AlloType=3 AND LePaymentInst.Status = 2 AND Period=@period");

                    DataTable dtAllo;

                    db.CommandText = sb.ToString();
                    db.AddParameter("@leaseno", SqlDbType.VarChar, strLeaseNo);
                    db.AddParameter("@period", SqlDbType.Int, Period);
                    db.CommandType = CommandType.Text;
                    db.Open();

                    dtAllo = db.GetDataTable();

                    if (Max)
                    {
                        if ((MaxResidu < MaxRental) && (Period == MaxPaidPeriod) && MaxPaidPeriod != 0 && MaxResidu != 0)
                        {
                            DataRow dr = dtAllo.NewRow();
                            dr.BeginEdit();
                            dr[0] = "";
                            dr[1] = MaxResidu;
                            dr[2] = ProcessDate.Date;
                            dr.EndEdit();
                            dtAllo.Rows.Add(dr);
                        }
                    }

                    TimeSpan ts;

                    if (dtAllo.Rows.Count == 0)
                    {
                        ts = ProcessDate - DueDate;
                        vDay = ts.Days;
                        if (vDay > 0)
                        {
                            if (this.CheckIfHoliday(ProcessDate, DueDate))
                                Penalty = Penalty + (vDay * (((Rate * 12) / 360) / 100) * Rental);
                        }
                    }
                    else
                    {
                        for (int c = 0; c < dtAllo.Rows.Count; c++)
                        {
                            vPayment = Convert.ToDouble(dtAllo.Rows[c]["AlloLeAmt"]);
                            vClearDate = Convert.ToDateTime(dtAllo.Rows[c]["ClearDate"]);

                            ts = vClearDate - DueDate;
                            vDay = ts.Days;
                            if (vDay > 0)
                            {
                                if (this.CheckIfHoliday(vClearDate, DueDate))
                                {
                                    //if (dtAllo.Rows.Count == 1)
                                    //    Penalty = Penalty + (vDay * (((Rate * 12) / 360) / 100) * Rental);
                                    //else
                                    Penalty = Penalty + (vDay * (((Rate * 12) / 360) / 100) * vPayment);
                                }
                            }
                        }
                    }

                    Penalty = mdl.RoundNominal(Penalty, strCurrCode);

                    db.CommandText = "Le_SPUpdateLPCAmt";
                    db.CommandType = CommandType.StoredProcedure;
                    db.AddParameter("@LeaseNo", SqlDbType.VarChar, strLeaseNo);
                    db.AddParameter("@Period", SqlDbType.Int, Period);
                    db.AddParameter("@LPCAmt", SqlDbType.Money, Penalty);

                    db.ExecuteNonQuery();
                }

                db.CommandText = "UPDATE Lease SET OverRate=@rate WHERE LeaseNo=@leaseno";
                db.CommandType = CommandType.Text;
                db.AddParameter("@rate", SqlDbType.Float, Rate.ToString());
                db.AddParameter("@leaseno", SqlDbType.VarChar, strLeaseNo);
                db.ExecuteNonQuery();

                db.CommitTransaction();

                db.Close();
            }
        }
        public bool CheckIfHoliday(DateTime Date, DateTime DueDate)
        {
            int x = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT dbo.fnChkIsHoliday(@CurrDate, @DueDate);";
                db.AddParameter("@CurrDate", SqlDbType.DateTime, Date);
                db.AddParameter("@DueDate", SqlDbType.DateTime, DueDate);
                db.CommandType = CommandType.Text;
                db.Open();

                x = Convert.ToInt16(db.ExecuteScalar());

                if (x == 0) return true; // Kena Penalty
                else return false; // tidak kena penalty
            }
        }
        public object GetFieldValue(
        string strFieldName,
        string strTableName,
        string strCondition
    )
        {
            return GetFieldValue(
                strFieldName,
                strTableName,
                strCondition, null
            );
        }

        public object GetFieldValue(
            string strFieldName,
            string strTableName,
            string strCondition,
            SqlTransaction trans
        )
        {
            SqlConnection conn;
            if (trans == null)
            {
                conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString());
                conn.Open();
            }
            else
                conn = trans.Connection;
            object obj;

            // Add - Anthony - 20180913
            StringBuilder sb = new StringBuilder();
            try
            {
                // Alter - Anthony - 20180913
                //string strSQL;
                //strSQL = "SELECT " + strFieldName + " FROM " + strTableName + " WHERE " + strCondition;
                //SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);

                sb.Append("SELECT ")
                    .Append(strFieldName)
                    .Append(" FROM ")
                    .Append(strTableName)
                    .Append(" WHERE ")
                    .Append(strCondition);
                // End alter - Anthony - 20180913

                SqlDataAdapter da = new SqlDataAdapter(sb.ToString(), conn);
                if (trans != null)
                    da.SelectCommand.Transaction = trans;
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                    obj = "";
                else
                    obj = dt.Rows[0][0];
            }
            catch (Exception ex)
            {
                obj = "";
          
            }
            finally
            {
                // Add - Anthony - 20180913
                sb.Remove(0, sb.Length);

                if (trans == null)
                {
                    conn.Close();
                    // Add by Anthony - 20160916
                    conn.Dispose();
                    GC.WaitForPendingFinalizers();
                    // End of add by Anthony - 20160916
                }
            }
            return obj;
        }


        // Get Max Field Value
        public object GetMaxFieldValue(
            string strFieldName,
            string strTableName,
            string strCondition
        )
        {
            return GetMaxFieldValue(
                strFieldName,
                strTableName,
                strCondition,
                null
            );
        }

        public object GetMaxFieldValue(
            string strFieldName,
            string strTableName,
            string strCondition,
            SqlTransaction trans
        )
        {
            SqlConnection conn;
            if (trans == null)
            {
                conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString());
                conn.Open();
            }
            else
                conn = trans.Connection;
            object obj;
            try
            {
                string strSQL;
                strSQL = "SELECT ISNULL(MAX(" + strFieldName + "),0) FROM " + strTableName + " WHERE " + strCondition;
                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                if (trans != null)
                    da.SelectCommand.Transaction = trans;
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                    obj = 0;
                else
                    obj = dt.Rows[0][0];
            }
            catch (Exception ex)
            {
                obj = 0;
            
            }
            finally
            {
                if (trans == null)
                    conn.Close();
            }
            return obj;
        }


        // Get Sum Field Value
        public double GetSumFieldValue(
            string strFieldName,
            string strTableName,
            string strCondition
        )
        {
            return GetSumFieldValue(
                strFieldName,
                strTableName,
                strCondition,
                null
            );
        }


        public double GetSumFieldValue(
            string strFieldName,
            string strTableName,
            string strCondition,
            SqlTransaction trans
        )
        {
            SqlConnection conn;
            if (trans == null)
            {
                conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString());
                conn.Open();
            }
            else
                conn = trans.Connection;
            double d;

            try
            {
                string strSQL;
                strSQL = "SELECT ISNULL(SUM(" + strFieldName + "),0) FROM " + strTableName + " WHERE " + strCondition;
                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                if (trans != null)
                    da.SelectCommand.Transaction = trans;
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                    d = 0;
                else
                    d = Convert.ToDouble(dt.Rows[0][0]);
            }
            catch (Exception ex)
            {
                d = 0;
               
            }
            finally
            {
                if (trans == null)
                    conn.Close();
            }
            return d;
        }


        // End
        public double GetExchangeRate(string strCurrCode,DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                try
                {
                    string strSQL;
                    strSQL = "SELECT ISNULL((SELECT TOP 1 MidRate FROM tblExchangeRate ";
                    strSQL += "WHERE CurrencyCode1 = '" + strCurrCode + "' AND CurrencyCode2=";
                    strSQL += "(SELECT BaseCcy FROM SYSPAR) AND ExchangeDate <= GETDATE() ";
                    strSQL += "ORDER BY ExchangeDate DESC),1) AS CurrentRate";
                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return Convert.ToDouble(dt.Rows[0][0]);
                }
                catch (Exception ex)
                {
                    //WebMsgBox.Show(ex.Message);
                    return 0;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void Le_SPUpdateOPULILR_GAB(
        string LeaseNo,
        string CustNo,
        string BranchCode,
        string ProcessDate,
        ref int message)
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                SqlTransaction trans = null;
                trans = conn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Le_SPUpdateOPULILR_GAB";
                    cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
                    cmd.Parameters.AddWithValue("@LesseeNo", CustNo);
                    cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    cmd.Parameters.AddWithValue("@DateNow", ProcessDate);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    message = 0;
                    trans.Rollback();
                    // Alter by Anthony - 02072012
                    //WebMsgBox.Show(ex.Message);
                    throw ex;
                    // End of alter by Anthony - 02072012

                }
                finally
                {
                    conn.Close();
                }
            }
        }

        // Add by Anthony - 20180227
        public void Le_SPUpdateOPULILR_GAB(
            SqlTransaction sqlTrans,
            string LeaseNo,
            string CustNo,
            string BranchCode,
            string ProcessDate,
            ref int message)
        {
            //using (SqlConnection conn = new SqlConnection(AppTools.GetSQLConnectionString()))
            //{
            //conn.Open();
            SqlTransaction trans = sqlTrans;
            //trans = sqlTrans;

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = trans.Connection;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Le_SPUpdateOPULILR_GAB";
                cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
                cmd.Parameters.AddWithValue("@LesseeNo", CustNo);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@DateNow", ProcessDate);
                cmd.Transaction = trans;
                cmd.ExecuteNonQuery();
                //trans.Commit();
            }
            catch (Exception ex)
            {
                message = 0;
                trans.Rollback();

                throw ex;
            }
            finally
            {
                //conn.Close();
            }
            //}
        }
        public void UpdateOPULILR(
               int ByWhat,
               string strLessee,
               string strLease,
               string strCurrency,
               string strBranchCode,
               ref int message
           )
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                SqlTransaction trans = null;
                trans = conn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Transaction = trans;
                    cmd.Connection = conn;
                    string strSQL;
                    int Counter;
                    string GroupNo = "";
                    string GuarantorNo = "";
                    Counter = 1;

                    string strLesseeBranch;
                    strLesseeBranch = GetFieldValue("LesseeBranch", "Lease", "LeaseNo='" + strLease + "'").ToString();

                    while (Counter <= 3)
                    {
                        if (Counter == 1)
                        {
                            switch (ByWhat)
                            {
                                case 0:
                                    strSQL = "Le_SPpruScheduleforOPULILRAll";
                                    break;
                                case 1:
                                    strSQL = "Le_SPpruScheduleforOPULILRByLease '" + strLease + "'";
                                    break;
                                case 2:
                                    strSQL = "Le_SPpruScheduleforOPULILRByLessee '" + strLessee + "','" + strBranchCode + "'";
                                    break;
                            }
                        }
                        else
                        {
                            if (Counter == 2)
                            {
                                strSQL = "Le_SPpruScheduleInsurforOPULILRAll";
                            }
                            else
                            {
                                //Update Another
                                string strSQL1;
                                string strSQL2;
                                string strSQL3;
                                switch (ByWhat)
                                {
                                    case 0:
                                        strSQL2 = "Le_SPpruLeaseLesseeforOPULILRAll";
                                        strSQL1 = "Le_SPpruUpdLGforOPULILRAll";
                                        break;
                                    default:
                                        //Edit By: Tomy
                                        //strSQL2 = "Le_SPpruLeaseLesseeforOPULILRByLessee '" + strLessee + "'";
                                        //strSQL1 = "Le_SPpruUpdLGforOPULILRByLG '" + strLessee + "'";
                                        strSQL2 = "Le_SPpruLeaseLesseeforOPULILRByLessee '" + strLessee + "' AND BranchCode='" + strLesseeBranch + "'";
                                        strSQL1 = "Le_SPpruUpdLGforOPULILRByLG '" + strLessee + "' AND BranchCode='" + strLesseeBranch + "'";
                                        //End Edit
                                        break;
                                }
                                strSQL = strSQL1;
                                strSQL = strSQL2;

                                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                                DataTable dt = new DataTable();
                                da.SelectCommand.Transaction = trans;
                                da.Fill(dt);

                                //add by bintang 22 Apr 2025
                                SqlDataAdapter daLessee = new SqlDataAdapter("select * from Lessee where LesseeNo='" + strLessee + "' AND BranchCode='" + strLesseeBranch + "'", conn);
                                DataTable dtLessee2 = new DataTable();
                                daLessee.SelectCommand.Transaction = trans;
                                daLessee.Fill(dtLessee2);
                                DataRow rowContract = dtLessee2.Rows[0];//ini header makanya cuman 1 nilainy

                                bool LesseeMatch = dt.AsEnumerable().Any(row => row["Lesseeno"].ToString() == rowContract["Lesseeno"]);
                                //add by bintang 22 Apr 2025

                                if (LesseeMatch)
                                {

                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        SqlDataAdapter da1 = new SqlDataAdapter("select * from Lessee where LesseeNo='" + strLessee + "' AND BranchCode='" + strLesseeBranch + "'", conn);
                                        DataTable dtLessee = new DataTable();
                                        da1.SelectCommand.Transaction = trans;
                                        da1.Fill(dtLessee);
                                        if (dtLessee.Rows.Count > 0)
                                        {
                                            double mOP;
                                            double mULI;
                                            mOP = 0;
                                            mULI = 0;

                                            DataRow row = dt.Rows[i];
                                            if (strCurrency != row["CurrCode"].ToString())
                                            {
                                                mOP = 0;
                                                mULI = 0;
                                            }

                                            //Edit By: Tomy (30 Apr 2011)
                                            //mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                            //mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                            if (row["OutPrincipal"] != DBNull.Value)
                                            {
                                                mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                                mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                            }
                                            //End Edit

                                            strSQL3 = "Le_SPpruUpdOSLessee '" + strLessee + "','" + row["CurrCode"].ToString() + "'," + mOP + "," + mULI + "," + (mOP + mULI) + ",'" + strBranchCode + "','" + strLesseeBranch + "'";
                                            cmd.CommandText = strSQL3;
                                            cmd.ExecuteNonQuery();
                                            if (ByWhat != 0)
                                            {
                                                //Get Group No
                                                GroupNo = dtLessee.Rows[0]["GrpNo"].ToString();

                                                //Get Guarantor No
                                                SqlDataAdapter da2 = new SqlDataAdapter("select * from Lease where LeaseNo='" + strLease + "'", conn);
                                                DataTable dtLease = new DataTable();
                                                da2.SelectCommand.Transaction = trans;
                                                da2.Fill(dtLease);
                                                GuarantorNo = dtLease.Rows[0]["Guarantor1"].ToString();
                                            }
                                        }
                                    }
                                }

                                //Update Guarantor
                                switch (ByWhat)
                                {
                                    case 0:
                                        strSQL = "Le_SPpruLeaseGuarantorforOPULILRAll";
                                        break;
                                    default:
                                        strSQL = "Le_SPpruLeaseGuarantorforOPULILRByGuarantor '" + GuarantorNo + "'";
                                        break;
                                }
                                da = new SqlDataAdapter(strSQL, conn);
                                dt = new DataTable();
                                da.SelectCommand.Transaction = trans;
                                da.Fill(dt);

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    GuarantorNo = dt.Rows[i]["Guarantor1"].ToString();
                                    SqlDataAdapter da1 = new SqlDataAdapter("select * from Lessee where LesseeNo='" + GuarantorNo + "' AND BranchCode='" + strLesseeBranch + "'", conn);
                                    DataTable dtGuarantor = new DataTable();
                                    da1.SelectCommand.Transaction = trans;
                                    da1.Fill(dtGuarantor);

                                    if (dtGuarantor.Rows.Count > 0)
                                    {
                                        double mOP;
                                        double mULI;
                                        mOP = 0;
                                        mULI = 0;

                                        DataRow row = dt.Rows[i];

                                        if (strCurrency != row["CurrCode"].ToString())
                                        {
                                            mOP = 0;
                                            mULI = 0;
                                        }

                                        //Edit By: Tomy (30 Apr 2011)
                                        //mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                        //mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                        if (row["OutPrincipal"] != DBNull.Value)
                                        {
                                            mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                            mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                        }
                                        //End Edit
                                        strSQL3 = "Le_SPpruUpdOSLessee '" + GuarantorNo + "','" + row["CurrCode"].ToString() + "'," + mOP + "," + mULI + "," + (mOP + mULI) + ",'" + strBranchCode + "','" + strLesseeBranch + "'";
                                        cmd.CommandText = strSQL3;
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                //Update Group
                                switch (ByWhat)
                                {
                                    case 0:
                                        strSQL1 = "Le_SPpruLesseeforOPULILRAll";
                                        strSQL2 = "Le_SPpruUpdGroupforOPULILRAll";
                                        break;
                                    default:
                                        strSQL1 = "Le_SPpruLesseeforOPULILRByGroup " + GroupNo;
                                        strSQL2 = "Le_SPpruUpdGroupforOPULILRByGroup " + GroupNo;
                                        break;
                                }
                                strSQL = strSQL2;
                                strSQL = strSQL1;
                                da = new SqlDataAdapter(strSQL, conn);
                                dt = new DataTable();
                                da.SelectCommand.Transaction = trans;
                                da.Fill(dt);

                                //for (int i = 0; i < dt.Rows.Count; i++)
                                //{
                                //    GroupNo = dt.Rows[i]["GrpNo"].ToString();
                                //    SqlDataAdapter da1 = new SqlDataAdapter("select * from tblGroup where GrpNo='" + GroupNo + "'", conn);
                                //    DataTable dtGroup = new DataTable();
                                //    da1.SelectCommand.Transaction = trans;
                                //    da1.Fill(dtGroup);

                                //    if (dtGroup.Rows.Count > 0)
                                //    {
                                //        double mOP;
                                //        double mULI;
                                //        mOP = 0;
                                //        mULI = 0;

                                //        DataRow row = dt.Rows[i];

                                //        if (strCurrency != row["CurrCode"].ToString())
                                //        {
                                //            mOP = 0;
                                //            mULI = 0;
                                //        }

                                //        //Edit By: Tomy (30 Apr 2011)
                                //        //mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                //        //mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                //        if (row["OutPrincipal"] != DBNull.Value)
                                //        {
                                //            mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                //            mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                //        }
                                //        //End Edit

                                //        strSQL3 = "Le_SPpruUpdOSGroup " + GroupNo + ",'" + row["CurrCode"].ToString() + "'," + mOP + "," + mULI + "," + (mOP + mULI);
                                //        cmd.CommandText = strSQL3;
                                //        cmd.ExecuteNonQuery();
                                //    }
                                //}
                            }
                        }
                        Counter++;
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    message = 0;
                    trans.Rollback();
                    // Alter by Anthony - 02072012
                    //WebMsgBox.Show(ex.Message);
                    throw ex;
                    // End of alter by Anthony - 02072012
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateGLTfAlloNew(
            //string strSCode,
            string strBranchCode,
            long NoProcess,
            DateTime Period,
            string strVoucher,
            string strBaseCcy,
            ref int message
        )
        {
            if (strBaseCcy == "")
            {
                strBaseCcy = "IDR";
            }
            string strSCode = "";
            //string strBaseCcy;
            int GLSeqNo = 0;
            string strVchNo = "";
            int X = 0;
            int Done;
            bool BedaKurs = false;
            string strOriCcy = "";
            int ErrorCode = 0;
            double TotAmt = 0;
            string strLeaseNo = "";
            string strJKey = "";
            string strCurrCode = "";
            DateTime LastValueDate = DateTime.Today;


            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                SqlTransaction trans = null;
                trans = conn.BeginTransaction();

                clsModule mdl = new clsModule();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    string strSQL;
                    strSQL = "Le_SPpruCreateGLTfAlloPrs '" + Period + "','" + strVoucher + "'";
                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                    da.SelectCommand.Transaction = trans;
                    DataTable dtAllo = new DataTable();
                    da.Fill(dtAllo);

                    string VchNo = "";
                    string strKondisi;

                    Done = 0;
                    NoProcess = NoProcess + dtAllo.Rows.Count;
                    if (dtAllo.Rows.Count == 0)
                    {
                        return;
                    }
                    string strSCode1;

                    // Get Max TransNo From LeTrans
                    string strTransNo = "";
                    object GetMaxTransNo = IDS.Tool.LeaseTools.GetINSTANCE().GetMaxFieldValue(
                            "TransNo",
                            "LeTrans",
                            "SubEntityNo='" + strVoucher + "'", trans
                        );
                    strTransNo = GetMaxTransNo.ToString();
                    //

                    for (int i = 0; i < dtAllo.Rows.Count; i++)
                    {
                        DataRow row = dtAllo.Rows[i];
                        string strxLeaseNo = row["LeaseNo"].ToString();

                        strSCode = "";
                        strSCode1 = mdl.GetJCodeLeasing(5, row["LeaseNo"].ToString()) + Convert.ToChar(64 + Convert.ToInt16(row["AlloType"]));

                        if (!(Convert.ToInt16(row["AlloType"]) == 3 && strSCode1.Substring(2, 1) == "6"))
                        {
                            if (!mdl.GetContractType(strxLeaseNo))
                                return;
                        }

                        if (strSCode != strSCode1)
                        {
                            strSCode = strSCode1;
                            strKondisi = "SCode='" + strSCode + "'  AND YEAR(EntryDate)='" + DateTime.Today.ToString("yyyy") + "' AND MONTH(EntryDate)='" + DateTime.Today.ToString("MM") + "' AND BranchCode='" + strBranchCode + "'";
                            VchNo = IDS.Tool.LeaseTools.GetINSTANCE().GetMaxFieldValue("Voucher", "LeGLTrans", strKondisi, trans).ToString();
                        }
                        if (!string.IsNullOrEmpty(VchNo) && VchNo != "0")
                        {
                            GLSeqNo = Convert.ToInt16(VchNo.Substring(4, 3));
                        }
                        else
                        {
                            GLSeqNo = 0;
                        }

                        if ((IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("CurrCode", "Lease", "LeaseNo='" + row["LeaseNo"].ToString() + "'").ToString() != row["CurrCode"].ToString()) && (row["InsNo"].ToString() == "0") && (row["CurrCode"].ToString() == strBaseCcy))
                            BedaKurs = true;
                        else if ((IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("CurrCode", "Lease", "LeaseNo='" + row["LeaseNo"].ToString() + "'").ToString() != row["CurrCode"].ToString()) && (row["InsNo"].ToString() == "0") && (row["CurrCode"].ToString() == "baseccy"))
                            BedaKurs = true;
                        else
                            BedaKurs = false;

                        strOriCcy = IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("CurrCode", "Lease", "LeaseNo='" + row["LeaseNo"].ToString() + "'").ToString();

                        Done++;
                        ErrorCode = 0;

                        GLSeqNo++;
                        X = 0;
                        TotAmt = 0;
                        strVchNo = DateTime.Today.ToString("yyMM") + GLSeqNo.ToString("000");
                        //}
                        strLeaseNo = row["LeaseNo"].ToString();

                        strSQL = "SELECT * FROM Lease WHERE LeaseNo='" + strLeaseNo + "'";
                        da = new SqlDataAdapter(strSQL, conn);
                        da.SelectCommand.Transaction = trans;
                        DataTable dtLease = new DataTable();
                        da.Fill(dtLease);

                        strJKey = strSCode;

                        strCurrCode = row["CurrCode"].ToString();

                        int GroupType = 0;
                        string strTable = "Lease L INNER JOIN Lessee Ls ON L.LesseeNo=Ls.LesseeNo AND L.LesseeBranch=Ls.BranchCode LEFT JOIN tblGroup G ON Ls.GrpNo=G.GrpNo";
                        GroupType = Convert.ToInt16(IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("ISNULL(G.Category,0)", strTable, "LeaseNo='" + strLeaseNo + "'"));

                        if (GroupType != 1)
                        {
                            GroupType = 0;
                        }

                        strSQL = "select * from LeJournalTD where JCode='" + strJKey + "' and TCurrCode='" + strCurrCode + "' and GroupType=" + GroupType + " ORDER BY Counter ";
                        //add checking counter
                        da = new SqlDataAdapter(strSQL, conn);
                        da.SelectCommand.Transaction = trans;
                        DataTable dtJournalTD = new DataTable();
                        da.Fill(dtJournalTD);

                        if (dtJournalTD.Rows.Count == 0)
                        {
                            return;
                        }

                        CreateJournal(trans, dtJournalTD, row, cmd, strTransNo, strLeaseNo, strVoucher, strBaseCcy, strSCode, strVchNo, strBranchCode, strCurrCode, ErrorCode);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    message = 0;
                    trans.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public void CreateJournal(SqlTransaction trans, DataTable dtJournalTD, DataRow row, SqlCommand cmd, string strTransNo, string strLeaseNo, string strVoucher, string strBaseCcy, string strSCode, string strVchNo, string strBranchCode, string strCurrCode, int ErrorCode)
        {
            clsModule mdl = new clsModule();
            string sAccCashBasis = "";
            double TotAmt = 0;
            int X = 0;
            string strAccount = "";

            for (int c = 0; c < dtJournalTD.Rows.Count; c++)
            {
                DataRow rowJTD = dtJournalTD.Rows[c];
                int DbCr;
                double vAmt;
                int Action;
                int Reverse = 0;

                string strCCode;

                DbCr = 1;
                vAmt = 0;

                Action = 0; //Insert LeGLTrans

                if (rowJTD["DC"].ToString() == "C")
                    DbCr = -1;

                //Check If Reverse
                if (Convert.ToInt16(IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("Status", "LePaymentInst", "InstNo='" + row["InstNo"].ToString() + "'")) != 2)
                    Reverse = 1;

                if (Convert.ToInt16(rowJTD["RetrieveType"]) == 0)
                {
                    strAccount = rowJTD["Acc"].ToString();
                    strCCode = rowJTD["CurrCode"].ToString();

                    if (Reverse == 1)
                        DbCr = DbCr * -1;

                    if (string.IsNullOrEmpty(rowJTD["RetrieveAmt"].ToString()))
                        vAmt = (Convert.ToDouble(row["PayBaseAmt"]) == null || Convert.ToDouble(row["PayBaseAmt"]) == 0) ? Convert.ToDouble(row["AlloLeAmt"]) : Convert.ToDouble(row["PayBaseAmt"]);
                    else
                        vAmt = Convert.ToDouble(row["PayBaseAmt"]);

                    vAmt = vAmt * DbCr;
                }
                else
                {
                    strAccount = rowJTD["Acc"].ToString();
                    if (Reverse == 1)
                        DbCr = DbCr * -1;

                    if (string.IsNullOrEmpty(rowJTD["RetrieveAmt"].ToString()))
                    {
                        vAmt = (Convert.ToDouble(row["PayBaseAmt"]) == null || Convert.ToDouble(row["PayBaseAmt"]) == 0) ? Convert.ToDouble(row["AlloLeAmt"]) : Convert.ToDouble(row["PayBaseAmt"]);
                    }
                    else
                    {
                        string strSQL;
                        strSQL = "SELECT S.Payment AS Payment, JF.Rental AS RentalJF, (S.Payment-JF.Rental) AS Selisih FROM Schedule S ";
                        strSQL += "INNER JOIN ScheduleJF JF ON S.LeaseNo=JF.LeaseNo AND S.Period=JF.Period ";
                        strSQL += "WHERE S.LeaseNo='" + row["LeaseNo"].ToString() + "' AND S.Period='" + row["Period"].ToString() + "'";

                        SqlDataAdapter da = new SqlDataAdapter(strSQL, trans.Connection);
                        da.SelectCommand.Transaction = trans;
                        DataTable dtSchedule = new DataTable();
                        da.Fill(dtSchedule);
                        if (dtSchedule.Rows.Count > 0)
                        {
                            switch (rowJTD["RetrieveAmt"].ToString())
                            {
                                case "CFR":
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Selisih"]);
                                    break;
                                case "OP":
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["RentalJF"]);
                                    break;
                            }
                        }
                    }
                    vAmt = vAmt * DbCr;
                }
                TotAmt = TotAmt + vAmt;

                sAccCashBasis = rowJTD["AccCashBasis"].ToString();

                // Add - Anthont - 20200708
                string branchRep = "";
                using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
                {
                    db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
                    db.CommandType = System.Data.CommandType.Text;
                    db.AddParameter("@leaseno", SqlDbType.VarChar, strLeaseNo);
                    db.Open();

                    db.ExecuteReader();

                    using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
                                    branchRep = rd["BranchCode"] as string;
                                else
                                    branchRep = rd["RepresentativeOffice"] as string;
                            }
                        }
                    }
                }

                // End add - Anthony - 20200708

                //Insert Or Update To LeGLTrans
                X++;
                cmd.Parameters.Clear();
                cmd.CommandText = "Le_SPUpdateGLTrans";
                cmd.Parameters.AddWithValue("@TransNo", strTransNo);
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Scode", strSCode);
                cmd.Parameters.AddWithValue("@Voucher", strVchNo);
                cmd.Parameters.AddWithValue("@Counter", X);
                cmd.Parameters.AddWithValue("@EntryDate", DateTime.Today);
                cmd.Parameters.AddWithValue("@TransDate", Convert.ToDateTime(row["ValueDate"]).Date.Date);
                cmd.Parameters.AddWithValue("@Reverse", Reverse);
                // Alter - Anthony - 20200708
                //cmd.Parameters.AddWithValue("@Dept", IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("BranchCode", "Lease", "LeaseNo='" + strLeaseNo + "'"));
                cmd.Parameters.AddWithValue("@Dept", branchRep);
                // End alter - Anthony - 20200708
                cmd.Parameters.AddWithValue("@Account", strAccount);
                cmd.Parameters.AddWithValue("@Ccy", strBaseCcy);
                cmd.Parameters.AddWithValue("@Amount", vAmt);
                cmd.Parameters.AddWithValue("@Description", rowJTD["Description"].ToString() + " ( Receive No : " + strVoucher + " )");
                //cmd.Parameters.AddWithValue("@Description", rowJTD["Description"].ToString());
                cmd.Parameters.AddWithValue("@LeaseNo", strLeaseNo);
                cmd.Parameters.AddWithValue("@DocNo", strVoucher);
                cmd.Parameters.AddWithValue("@TransferToGL", false);
                cmd.Parameters.AddWithValue("@ErrCode", ErrorCode);
                cmd.Parameters.AddWithValue("@OriTransDate", Convert.ToDateTime(row["ValueDate"]).Date.Date);
                cmd.Parameters.AddWithValue("@BranchCode", strBranchCode);
                cmd.Parameters.AddWithValue("@AccCashBasis", sAccCashBasis);
                cmd.ExecuteNonQuery();
            }

            string strLastLeaseNo;
            DateTime LastValueDate;
            string strLastInstNo;
            double LastAmount;

            strLastLeaseNo = strLeaseNo;
            LastValueDate = Convert.ToDateTime(row["ValueDate"]);
            strLastInstNo = row["InstNo"].ToString();
            LastAmount = Convert.ToDouble(row["Amount"]);

            //Add By: Tomy
            if (TotAmt != 0)
            {
                int msg2 = 1;
                CreateDbfAllo(trans, strTransNo, strLastInstNo, strBaseCcy, strCurrCode, TotAmt, strSCode, strVchNo, LastValueDate, X, strLastLeaseNo, strBranchCode, sAccCashBasis, ref msg2);
                if (msg2 == 0)
                {
                }
            }
        }


        public void CreateGLTfTransNew(
             string strTransNo,
             string strBranchCode,
             string strSCode,
             long NoProcess,
             DateTime Period,
             string strVoucher,
             string strBaseCcy,
             ref int message
         )
        {
            CreateGLTfTransNew(
                strTransNo,
                strBranchCode,
                strSCode,
                NoProcess,
                Period,
                strVoucher,
                strBaseCcy,
                ref message,
                0
            );
        }

        public void CreateGLTfTransNew(
            string strTransNo,
            string strBranchCode,
            string strSCode,
            long NoProcess,
            DateTime Period,
            string strVoucher,
            string sBaseCcy,
            ref int message,
            int xPeriod
        )
        {
            string VchNo, SCode, JCode, vLeNo, sAccNo;
            string sAccCashBasis;
            string sCcy, sTCurrCode;
            Byte X, I, bCounter;
            long Done;
            int DbCr, ErrCode, GLSeqNo = 0;
            double Portion = 0;
            Byte nPeriod;
            double TotDb, TotCr, vAmt, cExchRateJournal;
            double cExchRateVoucher, cAmount, cExchRate;
            DateTime CSLTransDate;
            string sDesc;
            DateTime StopAccrueDate;
            int Action = 0;
            //string sBaseCcy;
            DataTable dtJTD;
            DataTable dtGLTrans;
            DataTable dtLease;

            //Variable For GLTrans
            string GL_SCode, GL_Voucher, GL_Account, GL_Ccy;
            string GL_Dept = "", GL_Description, GL_LeaseNo, GL_DocNo;
            byte GL_Counter;
            DateTime GL_EntryDate, GL_OriTransDate, GL_TransDate;
            bool GL_Reverse, GL_TransferToGL;
            double GL_Amount;
            int GL_ErrCode;
            string GLBranchCode, GLCashBasis;

            string ContractNo = "";
            string CustNo = "";
            string CustBranch = "";
            int GrpNo = 0;
            int GroupType = 0;


            // Get Contract No 
            ContractNo = GetFieldValue(
                 "EntityNo",
                 "LeTrans",
                 "TransNo='" + strTransNo + "'"
                ).ToString();

            // Get Customer No
            CustNo = GetFieldValue(
                 "LesseeNo",
                 "Lease",
                 "LeaseNo='" + ContractNo + "'"
                ).ToString();

            // Get Customer Branch
            CustBranch = GetFieldValue(
                 "LesseeBranch",
                 "Lease",
                 "LeaseNo='" + ContractNo + "'"
                ).ToString();

            // Get Customer Group No
            GrpNo = Convert.ToInt32(GetFieldValue(
                 "isnull(GrpNo,0)",
                 "Lessee",
                 "LesseeNo='" + CustNo + "' And BranchCode='" + CustBranch + "'"
                ));

            // Get Customer Group Type
            if (GrpNo == 0)
            {
                GroupType = 0;
            }
            else
            {
                GroupType = Convert.ToInt32(GetFieldValue(
                     "isnull(Category,0)",
                     "tblGroup",
                     "GrpNo=" + GrpNo + ""
                    ));

                if (GroupType == 2)
                {
                    GroupType = 0;
                }
            }

            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                SqlTransaction trans = null;
                trans = conn.BeginTransaction();
                clsModule mdl = new clsModule();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string strSQL;
                    strSQL = "Le_SPpruCreateGLTfTransNewPrs '" + Period + "','" + strVoucher + "','" + strTransNo + "'";
                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                    da.SelectCommand.Transaction = trans;
                    DataTable dtTrans = new DataTable();
                    da.Fill(dtTrans);
                    NoProcess = NoProcess + dtTrans.Rows.Count;
                    if (dtTrans.Rows.Count == 0)
                    {
                       // WebMsgBox.Show("No Transaction To Be Created!");
                        return;
                    }

                    ErrCode = 0;
                    Done = 0;
                    for (int i = 0; i < dtTrans.Rows.Count; i++)
                    {
                        DataRow rowTrans = dtTrans.Rows[i];
                        Done = Done + 1;
                        if (mdl.Left(rowTrans["JCode"].ToString(), 2) != "LT")
                            JCode = rowTrans["JCode"].ToString(); //mdl.Left(rowTrans["JCode"].ToString(), 1) + "1" + mdl.Right(rowTrans["JCode"].ToString(), 2);
                        else
                            JCode = rowTrans["JCode"].ToString();

                        sTCurrCode = rowTrans["CurrCode"].ToString();

                        string Vch;
                        string strKondisi;

                        strKondisi = "SCode='" + JCode + "' AND YEAR(EntryDate)='" + DateTime.Today.ToString("yyyy") + "' AND MONTH(EntryDate)='" + DateTime.Today.ToString("MM") + "' AND BranchCode='" + strBranchCode + "'";
                        Vch = GetMaxFieldValue("Voucher", "LeGLTrans", strKondisi).ToString();
                        if (!string.IsNullOrEmpty(Vch) && Vch != "0")
                        {
                            GLSeqNo = Convert.ToInt16(Vch.Substring(4, 3));
                        }
                        else
                        {
                            GLSeqNo = 0;
                        }

                        switch (JCode)
                        {
                            // New Contract 
                            // Leasing Normal

                            // Alter by Anthony (Disburse) - Perubahan kode disesuaikan dengan kontrak OJK
                            //case "1L1":
                            //case "1L2":
                            //case "1L3":
                            //case "1L4":
                            //case "1L5":
                            //case "1L6":

                            case "1LIF1": // Investasi - Finance Lease - Normal
                            case "1LPF1": // Proyek - Finance Lease - Normal
                            case "1LGF1": // Multiguna - Finance Lease - Normal
                            case "1LIS1": // Investasi - Sale & Leaseback - Normal
                            case "1LPS1": // Proyek - Sale & Leaseback - Normal
                            case "1LMS1": // Modal Kerja - Sale & Leaseback - Normal
                            case "1SIF1":
                            // Leasing Receivable Assignment
                            case "1LIF4": // Investasi - Finance Lease - Receivable Assignment
                            case "1LPF4": // Proyek - Finance Lease - Receivable Assignment
                            case "1LGF4": // Multiguna - Finance Lease - Receivable Assignment
                            case "1LIS4": // Investasi - Sale & Leaseback - Receivable Assignment
                            case "1LPS4": // Proyek - Sale & Leaseback - Receivable Assignment
                            case "1LMS4": // Modal Kerja - Sale & Leaseback - Receivable Assignment

                            //case "1C1":
                            //case "1C2":
                            //case "1C3":
                            //case "1C4":
                            //case "1C5":
                            //case "1C6":

                            // CF Normal
                            case "1CIB1": // Investasi - Installment Barang - Normal
                            case "1CPB1": // Proyek - Installment Barang - Normal
                            case "1CGB1": // Multiguna - Installment Barang - Normal
                            case "1CPJ1": // Proyek - Installment Jasa - Normal
                            case "1CGJ1": // Multiguna - Installment Jasa - Normal
                            case "1CMU1": // Modal Kerja - Modal Usaha - Normal
                            case "1CGG1": // Multiguna - Fasilitas Dana - Barang - Normal
                            case "1CGS1": // Multiguna - Fasilitas Dana - Jasa - Normal

                            // CF Receivable Assigment
                            case "1CIB4": // Investasi - Installment Barang - Receivable Assigment
                            case "1CPB4": // Proyek - Installment Barang - Receivable Assigment
                            case "1CGB4": // Multiguna - Installment Barang - Receivable Assigment
                            case "1CPJ4": // Proyek - Installment Jasa - Receivable Assigment
                            case "1CGJ4": // Multiguna - Installment Jasa - Receivable Assigment
                            case "1CMU4": // Modal Kerja - Modal Usaha - Receivable Assigment
                            case "1CGG4": // Multiguna - Fasilitas Dana - Barang - Receivable Assignment
                            case "1CGS4": // Multiguna - Fasilitas Dana - Jasa - Receivable Assignment
                                          // End of alter by Anthony (Disburse)




                            // SLB
                            //case "1S1":
                            //case "1S2":
                            //case "1S3":
                            //case "1S4":
                            //case "1S5":

                            // Alter by Anthony - 20151229 - OJK
                            // Partial Terminate
                            //// L
                            //case "2L1":
                            //case "2L2":
                            //case "2L3":
                            //case "2L4":
                            //case "2L5":
                            //case "2L6":

                            //// CF
                            //case "2C1":
                            //case "2C2":
                            //case "2C3":
                            //case "2C4":
                            //case "2C5":
                            //case "2C6":
                            // L
                            case "2LIF1":
                            case "2LPF1":
                            case "2LGF1":
                            case "2LIS1":
                            case "2LPS1":
                            case "2LMS1":

                            // CF
                            case "2CIB1":
                            case "2CPB1":
                            case "2CGB1":
                            case "2CPJ1":
                            case "2CMJ1":
                            case "2CMU1":
                            case "2CGG1":
                            case "2CGS1":

                            //Receivable Assignment
                            // L
                            case "2LIF4":
                            case "2LPF4":
                            case "2LGF4":
                            case "2LIS4":
                            case "2LPS4":
                            case "2LMS4":

                            // CF
                            case "2CIB4":
                            case "2CPB4":
                            case "2CGB4":
                            case "2CPJ4":
                            case "2CMJ4":
                            case "2CMU4":
                            case "2CGG4":
                            case "2CGS4":
                            // End of alter by Anthony - OJK

                            // SLB
                            //case "2S1":
                            //case "2S2":
                            //case "2S3":
                            //case "2S4":
                            //case "2S5":

                            // Alter by Anthony (Full Terminate) - OJK
                            //// Full Terminate
                            //// L
                            //case "3L1":
                            //case "3L2":
                            //case "3L3":
                            //case "3L4":
                            //case "3L5":
                            //case "3L6":

                            case "3LIF1":
                            case "3LPF1":
                            case "3LGF1":
                            case "3LIS1":
                            case "3LPS1":
                            case "3LMS1":

                            //// CF
                            //case "3C1":
                            //case "3C2":
                            //case "3C3":
                            //case "3C4":
                            //case "3C5":
                            //case "3C6":

                            case "3CIB1":
                            case "3CPB1":
                            case "3CGB1":
                            case "3CPJ1":
                            case "3CGJ1":
                            case "3CMU1":
                            case "3CGG1":
                            case "3CGS1":
                            // End of alter by Anthony (Full Terminate)

                            // SLB
                            //case "3S1":
                            //case "3S2":
                            //case "3S3":
                            //case "3S4":
                            //case "3S5":


                            // Alter by Anthony (Floating Interest) - OJK
                            // Floating Interest
                            // L
                            //case "4L1":
                            //case "4L2":
                            //case "4L3":
                            //case "4L4":
                            //case "4L5":
                            //case "4L6":

                            case "4LIF1":
                            case "4LPF1":
                            case "4LGF1":
                            case "4LIS1":
                            case "4LPS1":
                            case "4LMS1":

                            // CF
                            //case "4C1":
                            //case "4C2":
                            //case "4C3":
                            //case "4C4":
                            //case "4C5":
                            //case "4C6":

                            case "4CIB1":
                            case "4CPB1":
                            case "4CGB1":
                            case "4CPJ1":
                            case "4CGJ1":
                            case "4CMU1":
                            case "4CGG1":
                            case "4CGS1":
                            // End of alter by Anthony (Floating)

                            // SLB
                            //case "4S1":
                            //case "4S2":
                            //case "4S3":
                            //case "4S4":
                            //case "4S5":


                            // Alter by Anthony (Rescheduling) - Perubahan kode disesuaikan dengan kontrak OJK
                            // Rescheduling
                            // L
                            //case "21L1":
                            //case "21L2":
                            //case "21L3":
                            //case "21L4":
                            //case "21L5":
                            //case "21L6":

                            case "21LIF1":
                            case "21LPF1":
                            case "21LGF1":
                            case "21LIS1":
                            case "21LPS1":
                            case "21LMS1":

                            // CF
                            //case "21C1":
                            //case "21C2":
                            //case "21C3":
                            //case "21C4":
                            //case "21C5":
                            //case "21C6":

                            case "21CIB1":
                            case "21CPB1":
                            case "21CGB1":
                            case "21CPJ1":
                            case "21CGJ1":
                            case "21CMU1":
                            case "21CGG1":
                            case "21CGS1":
                            // End of alter by Anthony (rescheduling)


                            // Contract Interest Accrued
                            // L
                            case "24L4":

                            // CF
                            case "24C4":

                                // SLB
                                //case "21S1":
                                //case "21S2":
                                //case "21S3":
                                //case "21S4":
                                //case "21S5":

                                X = 1;
                                bCounter = 1;
                                vLeNo = rowTrans["EntityNo"].ToString();
                                strSQL = "SELECT * FROM LeJournalTD where JCode='" + JCode + "' and TCurrCode='" + sTCurrCode + "' and GroupType=" + GroupType + " ORDER BY Counter ";

                                //strSQL += "and Counter=" + X;
                                da = new SqlDataAdapter(strSQL, conn);
                                da.SelectCommand.Transaction = trans;
                                dtJTD = new DataTable();
                                da.Fill(dtJTD);

                                if (dtJTD.Rows.Count == 0)
                                {
                                    //WebMsgBox.Show("Seek Failure in Journal for " + JCode);
                                }
                                else
                                {
                                    TotDb = 0;
                                    TotCr = 0;

                                    SCode = rowTrans["JCode"].ToString();
                                    GLSeqNo = GLSeqNo + 1;
                                    VchNo = DateTime.Today.ToString("yyMM") + GLSeqNo.ToString("000");

                                    for (int c = 0; c < dtJTD.Rows.Count; c++)
                                    {
                                        DataRow rowJTD = dtJTD.Rows[c];
                                        ErrCode = 0;
                                        DbCr = 1;
                                        vAmt = 0;

                                        if (Convert.ToInt16(rowJTD["RetrieveType"]) == 1)
                                        {
                                            if (!string.IsNullOrEmpty(rowTrans["OuterCode"].ToString()))
                                            {
                                                sAccNo = GetFieldValue("GLNo", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
                                                sCcy = rowJTD["CurrCode"].ToString();
                                                sDesc = GetFieldValue("BankName", "tblBank", "BankCode='" + rowTrans["OuterCode"].ToString() + "'").ToString();
                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
                                            }
                                            else
                                            {
                                                sAccCashBasis = rowJTD["AccCashBasis"].ToString();
                                                sAccNo = rowJTD["Acc"].ToString();
                                                sCcy = rowJTD["CurrCode"].ToString();
                                                sDesc = rowJTD["Description"].ToString();
                                            }

                                        }
                                        else
                                        {
                                            sAccCashBasis = rowJTD["AccCashBasis"].ToString();
                                            sAccNo = rowJTD["Acc"].ToString();
                                            sCcy = rowJTD["CurrCode"].ToString();
                                            sDesc = string.IsNullOrEmpty(rowJTD["Description"].ToString()) ? "0" : rowJTD["Description"].ToString();
                                        }

                                        switch (strSCode)
                                        {
                                            //Reverse Disburst
                                            case "L006":
                                                if (rowJTD["DC"].ToString() == "D")
                                                    DbCr = -1;
                                                break;

                                            //Disburst
                                            case "L001":
                                                if (rowJTD["DC"].ToString() == "C")
                                                    DbCr = -1;
                                                break;

                                            //Reverse Partial 
                                            case "L007":
                                                if (rowJTD["DC"].ToString() == "D")
                                                    DbCr = -1;
                                                break;

                                            //Partial
                                            case "L002":
                                                if (rowJTD["DC"].ToString() == "C")
                                                    DbCr = -1;
                                                break;

                                            //Reverse Full
                                            case "L008":
                                                if (rowJTD["DC"].ToString() == "D")
                                                    DbCr = -1;
                                                break;

                                            case "L003":
                                                if (rowJTD["DC"].ToString() == "C")
                                                    DbCr = -1;
                                                break;

                                            //Add By: Tomy
                                            //Floating Interest
                                            case "L004":
                                                if (rowJTD["DC"].ToString() == "C")
                                                    DbCr = -1;
                                                break;

                                            //Rescheduling
                                            case "L021":
                                                if (rowJTD["DC"].ToString() == "C")
                                                    DbCr = -1;
                                                break;

                                            //Contract Interest Accrued
                                            case "L024":
                                                if (rowJTD["DC"].ToString() == "C")
                                                    DbCr = -1;
                                                break;
                                        }

                                        GLCashBasis = sAccCashBasis;
                                        GLBranchCode = strBranchCode;

                                        GL_SCode = SCode;
                                        GL_Voucher = VchNo;
                                        GL_Counter = bCounter;
                                        GL_EntryDate = DateTime.Today;
                                        GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
                                        GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
                                        GL_Reverse = false;
                                        GL_Account = sAccNo;
                                        GL_Ccy = sCcy;
                                        // Alter - Anthony - 20200709
                                        //GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();

                                        using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
                                        {
                                            db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
                                            db.CommandType = System.Data.CommandType.Text;
                                            db.AddParameter("@leaseno", SqlDbType.VarChar, vLeNo);
                                            db.Open();

                                            db.ExecuteReader();

                                            using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
                                            {
                                                if (rd.HasRows)
                                                {
                                                    while (rd.Read())
                                                    {
                                                        if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
                                                            GL_Dept = rd["BranchCode"] as string;
                                                        else
                                                            GL_Dept = rd["RepresentativeOffice"] as string;
                                                    }
                                                }
                                            }

                                            db.Close();
                                        }


                                        // End alter - Anthony - 20200709

                                        if (strSCode == "L021" || strSCode == "L024")
                                        {
                                            //GetvAmtErrCode For Approve (status=0)
                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 2)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
                                        }
                                        else
                                        {
                                            //GetvAmtErrCode For Approve (status=0)
                                            GetvAmtErrCode(Convert.ToInt16(GL_SCode.Substring(0, 1)), rowJTD["RetrieveAmt"].ToString(), vLeNo, rowTrans, TotDb, TotCr, Portion, ref vAmt, ref ErrCode, xPeriod, GroupType);
                                        }

                                        cExchRateJournal = sCcy != sBaseCcy ? GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;
                                        cExchRateVoucher = sTCurrCode != sBaseCcy ? GetExchangeRate(sTCurrCode, Convert.ToDateTime(rowTrans["ValueDate"])) : 1;

                                        if (sCcy != sTCurrCode)
                                            vAmt = (vAmt * cExchRateVoucher / cExchRateJournal);
                                        vAmt = vAmt * DbCr;
                                        GL_Amount = vAmt;
                                        GL_Description = sDesc;
                                        GL_LeaseNo = vLeNo;
                                        //GL_DocNo = rowTrans["TransCode"].ToString();
                                        GL_DocNo = strSCode;
                                        GL_TransferToGL = false;
                                        GL_ErrCode = ErrCode;

                                        // Alter - Anthony - 20220521
                                        //if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
                                        switch (JCode)
                                        {
                                            case "1LIF4":
                                            case "1LPF4":
                                            case "1LGF4":
                                            case "1LIS4":
                                            case "1LPS4":
                                            case "1LMS4":
                                            case "1CIB4":
                                            case "1CPB4":
                                            case "1CGB4":
                                            case "1CPJ4":
                                            case "1CGJ4":
                                            case "1CMU4":
                                            case "1CGG4":
                                            case "1CGS4":

                                            case "1LIF6":
                                            case "1LPF6":
                                            case "1LGF6":
                                            case "1LIS6":
                                            case "1LPS6":
                                            case "1LMS6":
                                            case "1CIB6":
                                            case "1CPB6":
                                            case "1CGB6":
                                            case "1CPJ6":
                                            case "1CGJ6":
                                            case "1CMU6":
                                            case "1CGG6":
                                            case "1CGS6":
                                                if (rowJTD["RetrieveAmt"].ToString() == "HOP")
                                                {
                                                    // Get HOPNO
                                                    string HOPNO, HOPBranchName;

                                                    HOPNO = "";
                                                    HOPBranchName = "";

                                                    // Get HOPNO
                                                    HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
                                                        "HopNo",
                                                        "tblBranch",
                                                        "BranchCode='" + GLBranchCode + "'").ToString();

                                                    // Get HOPBranchName
                                                    HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
                                                        "Name",
                                                        "ACFGLMH",
                                                        "ACC='" + HOPNO + "'").ToString();

                                                    GL_Account = HOPNO;
                                                    GL_Description = HOPBranchName;
                                                }
                                                break;
                                        }


                                        int msg1 = 1;
                                        Le_SPUpdateGLTrans(
                                            strTransNo,
                                            GL_SCode,
                                            GL_Voucher,
                                            GL_Counter,
                                            GL_EntryDate,
                                            GL_TransDate,
                                            GL_Reverse,
                                            GL_Dept,
                                            GL_Account,
                                            GL_Ccy,
                                            GL_Amount,
                                            GL_Description,
                                            GL_LeaseNo,
                                            GL_DocNo,
                                            GL_TransferToGL,
                                            GL_ErrCode,
                                            GL_OriTransDate,
                                            GLBranchCode,
                                            GLCashBasis,
                                            ref msg1);

                                        if (msg1 == 0)
                                        {
                                            //WebMsgBox.Show("Update LeGLTrans Failed!");
                                        }

                                        if (sCcy != sBaseCcy)
                                        {
                                            //CreateEqvJournal
                                            cAmount = vAmt;
                                            cExchRate = GetExchangeRate(sCcy, Convert.ToDateTime(rowTrans["ValueDate"]));
                                            bCounter++;

                                            GLCashBasis = sAccCashBasis;
                                            GLBranchCode = strBranchCode;

                                            GL_SCode = SCode;
                                            GL_Voucher = VchNo;
                                            GL_Counter = bCounter;
                                            GL_EntryDate = DateTime.Today;
                                            GL_TransDate = Convert.ToDateTime(rowTrans["ValueDate"]);
                                            GL_Reverse = false;
                                            GL_Dept = string.IsNullOrEmpty(GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString()) ? vLeNo : GetFieldValue("BranchCode", "Lease", "LeaseNo='" + vLeNo + "'").ToString();
                                            GL_Account = sAccNo;
                                            GL_Ccy = sBaseCcy;
                                            GL_Amount = cAmount * cExchRate;
                                            GL_Description = rowJTD["Description"].ToString();
                                            GL_LeaseNo = vLeNo;
                                            //GL_DocNo = rowTrans["TransCode"].ToString();
                                            GL_DocNo = strSCode;
                                            GL_TransferToGL = false;
                                            GL_ErrCode = ErrCode;
                                            GL_OriTransDate = Convert.ToDateTime(rowTrans["ValueDate"]);

                                            if ((JCode == "1L4" || JCode == "1C4" || JCode == "1L6" || JCode == "1C6") && (rowJTD["RetrieveAmt"].ToString() == "HOP"))
                                            {
                                                // Get HOPNO
                                                string HOPNO, HOPBranchName;

                                                HOPNO = "";
                                                HOPBranchName = "";

                                                // Get HOPNO
                                                HOPNO = LeaseTools.GetINSTANCE().GetFieldValue(
                                                    "HopNo",
                                                    "tblBranch",
                                                    "BranchCode='" + GLBranchCode + "'").ToString();

                                                // Get HOPBranchName
                                                HOPBranchName = LeaseTools.GetINSTANCE().GetFieldValue(
                                                    "Name",
                                                    "ACFGLMH",
                                                    "ACC='" + HOPNO + "'").ToString();

                                                GL_Account = HOPNO;
                                                GL_Description = HOPBranchName;
                                            }
                                            int msg2 = 1;
                                            Le_SPUpdateGLTrans(
                                                strTransNo,
                                                GL_SCode,
                                                GL_Voucher,
                                                GL_Counter,
                                                GL_EntryDate,
                                                GL_TransDate,
                                                GL_Reverse,
                                                GL_Dept,
                                                GL_Account,
                                                GL_Ccy,
                                                GL_Amount,
                                                GL_Description,
                                                GL_LeaseNo,
                                                GL_DocNo,
                                                GL_TransferToGL,
                                                GL_ErrCode,
                                                GL_OriTransDate,
                                                GLBranchCode,
                                                GLCashBasis,
                                                ref msg2);
                                            if (msg2 == 0)
                                            {
                                                //WebMsgBox.Show("Update LeGLTrans Failed!");
                                            }
                                        }

                                        if (vAmt < 0)
                                            TotCr = TotCr + vAmt;
                                        else
                                            TotDb = TotDb + vAmt;

                                        bCounter++;
                                        X++;
                                    }
                                }
                                break;
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    message = 0;
                    trans.Rollback();
                    //WebMsgBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    public void GetvAmtErrCode(
      int Status,
      string RetrieveAmt,
      string vLeNo,
      DataRow rowTrans,
      double TotDb,
      double TotCr,
      double Portion,
      ref double vAmt,
      ref int ErrCode,
      int xPer,
      int CustGroupType
  )
        {
            string strSQL;
            byte nPeriod;

            double LReceivable;
            double UnLeIncome;
            double AdminFee;
            double Instosupplier;
            double Instosales;
            double Instocust;
            double InsurAmt;
            double InsAmtDisc;
            double provision;
            double Residual;
            double Rental;
            double Notary;
            double provisionFee = 0;

            // Add by Anthony - 20160304
            double BiSurvei = 0;
            // End of add by Anthony - 20160304

            double TD, TC;
            object advarr;
            string ContractType;

            clsModule mdl = new clsModule();
            ContractType = mdl.GetJCodeLeasing(1, vLeNo);

            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                try
                {
                    SqlDataAdapter da;
                    DataTable dtSchedule;
                    DataTable dtScheduleIns;
                    DataTable dtLease;



                    #region Disbursement
                    // Disbursement
                    if (Status == 1)
                    {
                        // Add by Yusuf(21 Mar 2011)
                        advarr = LeaseTools.GetINSTANCE().GetFieldValue(
                                "AdvArr",
                                "Lease",
                                "LeaseNo='" + vLeNo + "'"
                            );

                        // Jika Contract Type Join Financing
                        // Alter - Anthony - 20220520
                        //if (ContractType == "1L4" || ContractType == "1C4")

                        if (ContractType == "1CIB4" // CF Investasi Installment Financing Barang
                            || ContractType == "1CGB4" // CF Multiguna Barang 
                            || ContractType == "1CPB4" // CF Project Barang
                            || ContractType == "1CPJ4" // CF Project Jasa
                            || ContractType == "1CGJ4" // CF Multiguna Jasa
                            || ContractType == "1CMU4" // CF Modal Kerja Modal Usaha
                            || ContractType == "1CGG4" // CF Multiguna Fasilitas Dana Barang
                            || ContractType == "1CGS4" // CF Multiguna Fasilitas Dana Jasa
                            || ContractType == "1LIF4" // Leasing Investasi Finance Lease
                            || ContractType == "1LPF4" // Leasing Project Finance Lease
                            || ContractType == "1LGF4" // Leasing Multiguna Finance Lease
                            || ContractType == "1LIS4" // Leasing Investasi Sale & Leaseback
                            || ContractType == "1LPS4" // Leasing Project Sales & Leaseback 
                            || ContractType == "1LMS4" // Leasing Modal Kerja Sale & Leaseback
                            )
                        // End alter - Anthony - 20220520
                        {
                            if (Convert.ToInt16(advarr) == 1)
                            {
                                switch (RetrieveAmt)
                                {
                                    case "CFC": // Porsi Perusahaan Pembiayaan
                                                // Alter - Anthony - 20220520
                                                //strSQL = "select S.Rental-SJF.Rental As Rental from Schedule as S ";
                                                //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo AND S.Period=SJF.Period ";
                                                //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=1";
                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=1";
                                        // End alter - Anthony - 20220520

                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);

                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
                                        break;
                                    // Add - Anthony - 20220520
                                    case "CFCJF": // Rental Porsi Funder
                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=1";

                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);

                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
                                        break;
                                    case "CFCFL": // Rental Porsi Full
                                        strSQL = "SELECT ISNULL(Rental, 0) As Rental FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=1";

                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);

                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
                                        break;
                                }
                            }

                            switch (RetrieveAmt)
                            {
                                case "CFD": // Porsi Perusahan Pembiayaan
                                            // Alter - Anthony - 20220520
                                            //strSQL = "select (S.LeReceivable-SJF.LeReceivable) as LeReceivable from Schedule as S ";
                                            //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo And S.Period=SJF.Period ";
                                            //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=0 And SJF.Period=0";
                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=0";
                                    // End alter - Anthony - 20220520

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    break;
                                // Add - Anthony - 20220520
                                case "CFDJF": // Porsi Funder / JF
                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=0";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    break;
                                case "CFDFL": // Porsi Full
                                    strSQL = "SELECT ISNULL(LeReceivable, 0) AS LeReceivable FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=0";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    break;
                                // End add - Anthony - 20220520

                                case "UCF": // Porsi Perusahan Pembiayaan
                                            // Alter - Anthony - 20220520
                                            //strSQL = "select (S.UnLeIncome-SJF.UnLeIncome) as UnLeIncome from Schedule as S ";
                                            //strSQL += "Inner Join ScheduleJF as SJF on S.LeaseNo=SJF.LeaseNo And S.Period=SJF.Period ";
                                            //strSQL += "where S.LeaseNo='" + vLeNo + "' and S.Period=0 And SJF.Period=0";
                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM SchedulePP WHERE LeaseNo='" + vLeNo + "' AND Period=0";
                                    // End alter - Anthony - 20220520

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
                                    break;
                                // Add - Anthony - 20220520
                                case "UCFJF": // Porsi Funder
                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM ScheduleJF WHERE LeaseNo='" + vLeNo + "' AND Period=0";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
                                    break;
                                case "UCFFL":
                                    strSQL = "SELECT ISNULL(UnLeIncome, 0) AS UnLeIncome FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=0";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
                                    break;
                                // End add - Anthony - 20220520


                                case "AFC":
                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
                                    break;

                                // For Lasing & CF
                                case "LFT":
                                    strSQL = "select ISNULL(Notary, 0) AS Notary from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Notary"]);
                                    break;

                                case "IPT":
                                    strSQL = "select (InsurAmt + InsurIntRate)-InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsurAmt"]);
                                    break;

                                case "IPS":
                                    strSQL = "select ISNULL(Instosupplier, 0) AS Instosupplier from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosupplier"]);
                                    break;

                                case "ISM":
                                    strSQL = "select ISNULL(Instosalesman, 0) AS Instosalesman from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosalesman"]);
                                    break;

                                case "IPC":
                                    strSQL = "select ISNULL(Instocust, 0) AS Instocust from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
                                    break;

                                case "IPD":
                                    strSQL = "select ISNULL(Instocust, 0) AS Instocust from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
                                    break;
                                // Add - Anthony - 20220520
                                case "IPDC": // Insurance Discount
                                    strSQL = "SELECT ISNULL(InsDisc, 0) - (ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0)) AS InsDisc from Lease where LeaseNo='" + vLeNo + "'";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsDisc"]);
                                    break;
                                // End add - Anthon - 20220520
                                case "P":
                                    object SisaIns;
                                    SisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
                                            "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
                                            "Lease",
                                            "LeaseNo='" + vLeNo + "'"
                                        );

                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaIns);
                                    break;
                                case "PAF": // Provision Fee + Admin Fee
                                    strSQL = "SELECT (ISNULL(ProvisionFee, 0) + ISNULL(AdminFee, 0)) AS ProvisionFee FROM Lease WHERE LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);

                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ProvisionFee"]);
                                    break;
                                // Add - Anthony - 20220520
                                case "B": // Biaya Blokir
                                    strSQL = "SELECT ISNULL(BIBlokir, 0) AS BIBlokir FROM Lease WHERE LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BIBlokir"]);
                                    break;
                                case "S":
                                    strSQL = "SELECT ISNULL(BISurvei, 0) AS BISurvei FROM Lease WHERE LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["BISurvei"]);
                                    break;
                                // End add - Anthony - 20220520

                                case "HOP":
                                case "BPH": // Branch Payable to Holding


                                    LReceivable = 0;
                                    UnLeIncome = 0;
                                    AdminFee = 0;
                                    Instosupplier = 0;
                                    Instosales = 0;
                                    Instocust = 0;
                                    InsurAmt = 0;
                                    InsAmtDisc = 0;
                                    provision = 0;
                                    Notary = 0;
                                    Rental = 0;
                                    TD = 0;
                                    TC = 0;

                                    object BCFSisaIns;
                                    BCFSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
                                            "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
                                            "Lease",
                                            "LeaseNo='" + vLeNo + "'"
                                        );

                                    // Biaya Asuransi
                                    double IPT = 0;
                                    strSQL = "SELECT ISNULL(ISNULL(InsurAmt, 0) + ISNULL(InsurIntRate, 0), 0) AS IPT FROM Lease WHERE LeaseNo = '" + vLeNo + "'";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);

                                    if (dtSchedule.Rows.Count == 0)
                                    {
                                        ErrCode = 2;
                                    }
                                    else
                                    {
                                        IPT = Convert.ToDouble(dtSchedule.Rows[0]["IPT"]);
                                    }

                                    advarr = LeaseTools.GetINSTANCE().GetFieldValue(
                                        "AdvArr",
                                        "Lease",
                                        "LeaseNo='" + vLeNo + "'"
                                        );

                                    if (Convert.ToInt16(advarr) == 1)
                                    {
                                        strSQL = "SELECT ISNULL(Rental, 0) AS Rental FROM Schedule WHERE LeaseNo='" + vLeNo + "' AND Period=1";

                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);

                                        if (dtSchedule.Rows.Count == 0)
                                        {
                                            ErrCode = 2;
                                        }
                                        else
                                        {
                                            Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
                                        }
                                    }

                                    double surveyFee = 0;

                                    strSQL = "SELECT s.LeReceivable as LeReceivable, s.UnLeIncome as UnLeIncome, l.Notary, ISNULL(l.BISurvei, 0) AS BiSurvei, ";
                                    strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, ";
                                    strSQL += "l.Instosupplier, l.Instosalesman, ";
                                    strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.ProvisionFee, l.Residual, l.InsDisc As InsDisc ";
                                    strSQL += "FROM Lease AS l ";
                                    strSQL += "INNER JOIN Schedule s ON s.LeaseNo = l.LeaseNo ";
                                    strSQL += "WHERE l.LeaseNo = '" + vLeNo + "' AND s.Period=0";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);

                                    if (dtLease.Rows.Count == 0)
                                    {
                                        ErrCode = 1;
                                    }
                                    else
                                    {
                                        LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
                                        UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
                                        AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
                                        Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
                                        Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
                                        Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
                                        InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
                                        provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BCFSisaIns);
                                        provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
                                        Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
                                        Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
                                        InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
                                        surveyFee = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);
                                    }

                                    TD = LReceivable;
                                    TC = UnLeIncome + AdminFee + provisionFee + surveyFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + Rental;
                                    vAmt = TD - TC;


                                    break;
                            }
                        }
                        else // Normal
                        {
                            //Add by Anthony - 20151201
                            if (Convert.ToInt16(advarr) == 1)
                            {
                                switch (RetrieveAmt)
                                {
                                    case "CFC":
                                        strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);

                                        //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=1";
                                        //da = new SqlDataAdapter(strSQL, conn);
                                        //dtScheduleIns = new DataTable();
                                        //da.Fill(dtScheduleIns);
                                        //if (dtScheduleIns.Rows.Count == 0)
                                        //    ErrCode = 0;
                                        //else
                                        //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["Rental"]);
                                        break;
                                    case "LRC":
                                        if (Convert.ToInt16(advarr) == 1)
                                        {
                                            strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
                                            da = new SqlDataAdapter(strSQL, conn);
                                            dtSchedule = new DataTable();
                                            da.Fill(dtSchedule);
                                            if (dtSchedule.Rows.Count == 0)
                                                ErrCode = 2;
                                            else
                                                vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
                                        }
                                        break;
                                }
                            }
                            // End of add by Anthony - 20151201

                            #region Hasil Pindahan untuk dipakai leasing dan CF - OJK - Anthony 20151202
                            switch (RetrieveAmt)
                            {
                                // Leasing 
                                case "LRD":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);

                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
                                    //da = new SqlDataAdapter(strSQL, conn);
                                    //dtScheduleIns = new DataTable();
                                    //da.Fill(dtScheduleIns);
                                    //if (dtScheduleIns.Rows.Count == 0)
                                    //    ErrCode = 0;
                                    //else
                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
                                    break;

                                case "ULI":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);


                                    break;

                                case "DOL":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Residual"]);
                                    break;

                                case "AFL":
                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
                                    break;

                                case "RV":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
                                    break;

                                // CF 
                                case "CFD":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);

                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
                                    //da = new SqlDataAdapter(strSQL, conn);
                                    //dtScheduleIns = new DataTable();
                                    //da.Fill(dtScheduleIns);
                                    //if (dtScheduleIns.Rows.Count == 0)
                                    //    ErrCode = 0;
                                    //else
                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
                                    break;

                                case "UCF":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);

                                    //strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
                                    //da = new SqlDataAdapter(strSQL, conn);
                                    //dtScheduleIns = new DataTable();
                                    //da.Fill(dtScheduleIns);
                                    //if (dtScheduleIns.Rows.Count == 0)
                                    //    ErrCode = 0;
                                    //else
                                    //    vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
                                    break;

                                case "AFC":
                                    strSQL = "select ISNULL(AdminFee, 0) AS AdminFee from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AdminFee"]);
                                    break;

                                // Add by Anthony - 20160321
                                // Provision (Pengganti Admin Fee semenjak OJK)
                                case "APF":
                                    strSQL = "SELECT ISNULL(ProvisionFee, 0) AS ProvisionFee FROM Lease WHERE LeaseNo = '" + vLeNo + "';";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    DataTable dtProvision = new DataTable();
                                    da.Fill(dtProvision);

                                    if (dtProvision.Rows.Count > 0)
                                        vAmt = Convert.ToDouble(dtProvision.Rows[0]["ProvisionFee"]);
                                    else
                                        vAmt = 0;
                                    break;
                                // End of add by Anthony - 20160321

                                // Add by Anthony - 20160225 - Biaya Survei
                                case "SFC":
                                    using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
                                    {
                                        db.CommandText = "SELECT ISNULL(BISurvei, 0) FROM Lease WHERE LeaseNo = @leaseNo";
                                        db.AddParameter("@leaseNo", SqlDbType.VarChar, vLeNo);
                                        db.CommandType = CommandType.Text;
                                        db.Open();

                                        object biayaSurvei = db.ExecuteScalar();

                                        if (biayaSurvei == null)
                                        {
                                            ErrCode = 2;
                                        }
                                        else
                                        {
                                            vAmt = Convert.ToDouble(biayaSurvei);
                                        }
                                    }
                                    break;
                                // End of add by Anthony - 20160225

                                // For Lasing & CF

                                case "LFT":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Notary"]);
                                    break;

                                case "IPT":
                                    //strSQL = "select ((InsurAmt - InsDisc) + InsurIntRate) - InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
                                    // Modify by Yusuf (21 JUN 2011)
                                    //double NProvision=0;
                                    //object SisaHslIns;
                                    //SisaHslIns = LeaseTools.GetINSTANCE().GetFieldValue(
                                    //        "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
                                    //        "Lease",
                                    //        "LeaseNo='" + vLeNo + "'"
                                    //    );

                                    //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    //da = new SqlDataAdapter(strSQL, conn);
                                    //dtSchedule = new DataTable();
                                    //da.Fill(dtSchedule);
                                    //if (dtSchedule.Rows.Count == 0)
                                    //    ErrCode = 2;
                                    //else
                                    //    NProvision = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaHslIns);

                                    strSQL = "select (InsurAmt + InsurIntRate)-InsDisc As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["InsurAmt"]);
                                    break;

                                case "IPS":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosupplier"]);
                                    break;

                                case "ISM":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instosalesman"]);
                                    break;

                                case "IPC":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
                                    break;

                                case "IPD":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Instocust"]);
                                    break;

                                case "P":
                                    // Alter by Anthony - 20160427
                                    #region OLD - Sebelum Perubahan OJK
                                    //object SisaIns;
                                    //SisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
                                    //        "(InsDisc -(Instosupplier + Instosalesman + Instocust)) As TotalSisaIns",
                                    //        "Lease",
                                    //        "LeaseNo='" + vLeNo + "'"
                                    //    );

                                    //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    //da = new SqlDataAdapter(strSQL, conn);
                                    //dtSchedule = new DataTable();
                                    //da.Fill(dtSchedule);
                                    //if (dtSchedule.Rows.Count == 0)
                                    //    ErrCode = 2;
                                    //else
                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]) + Convert.ToDouble(SisaIns);
                                    // End of alter by Anthony - 20160427
                                    #endregion

                                    strSQL = "SELECT ISNULL(Provision, 0) AS Provision FROM Lease WHERE LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Provision"]);
                                    // End of alter by Anthony - 20160427
                                    break;

                                case "CTL":
                                    LReceivable = 0;
                                    UnLeIncome = 0;
                                    AdminFee = 0;
                                    Instosupplier = 0;
                                    Instosales = 0;
                                    Instocust = 0;
                                    InsurAmt = 0;
                                    provision = 0;

                                    InsAmtDisc = 0;
                                    Residual = 0;
                                    Rental = 0;
                                    Notary = 0;

                                    TD = 0;
                                    TC = 0;

                                    // Add by Anthony - 20160321
                                    provisionFee = 0;
                                    // End of add by Anthony - 20160321

                                    object BLSisaIns;
                                    BLSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
                                            "(ISNULL(InsDisc, 0) -(ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0))) As TotalSisaIns",
                                            "Lease",
                                            "LeaseNo='" + vLeNo + "'"
                                        );

                                    advarr = LeaseTools.GetINSTANCE().GetFieldValue(
                                        "AdvArr",
                                        "Lease",
                                        "LeaseNo='" + vLeNo + "'"
                                        );

                                    if (Convert.ToInt16(advarr) == 1)
                                    {
                                        strSQL = "";
                                        strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                        {
                                            ErrCode = 2;
                                        }
                                        else
                                        {
                                            Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
                                        }
                                    }

                                    strSQL = "";
                                    strSQL = "select s.LeReceivable, s.UnLeIncome, l.Notary, ";
                                    strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, l.Instosupplier, l.Instosalesman, ";
                                    strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.Residual, l.InsDisc As InsDisc ";
                                    // Add by Anthony - 20160304
                                    strSQL += ", BISurvei, ProvisionFee ";
                                    // End of add by Anthony - 20160304
                                    strSQL += "from Lease as l ";
                                    strSQL += "Inner Join Schedule as s on l.LeaseNo=s.LeaseNo  ";
                                    strSQL += "where l.LeaseNo='" + vLeNo + "' And s.Period=0";

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                    {
                                        ErrCode = 1;
                                    }
                                    else
                                    {
                                        LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
                                        UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
                                        AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
                                        Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
                                        Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
                                        Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
                                        InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
                                        provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BLSisaIns);
                                        Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
                                        Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
                                        InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
                                        BiSurvei = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);

                                        // Add by Anthony - 20160321
                                        provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
                                        // End of add by Anthony - 20160321
                                    }

                                    TD = LReceivable + Residual;

                                    // Alter by Anthony - 20160321
                                    //TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Residual + Notary + BiSurvei + Rental;
                                    TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Residual + Notary + BiSurvei + Rental + provisionFee - Convert.ToDouble(BLSisaIns);
                                    // End of alter by Anthony - 20160321
                                    //TC = UnLeIncome + AdminFee + InsurAmt + Notary + Rental;
                                    vAmt = TD - TC;
                                    break;

                                case "CTC":
                                    // Alter - Anthony - 20160321
                                    //if (ContractType != "1C2") // Pakar
                                    if (ContractType != "1CIB2"
                                        || ContractType != "1CPB2"
                                        || ContractType != "1CGB2"
                                        || ContractType != "1CPJ2"
                                        || ContractType != "1CGJ2"
                                        || ContractType != "1CMU2"
                                        || ContractType != "1CGG2"
                                        || ContractType != "1CGS2") // Pakar
                                                                    // End alter - Anthony - 20160321
                                    {
                                        LReceivable = 0;
                                        UnLeIncome = 0;
                                        AdminFee = 0;
                                        Instosupplier = 0;
                                        Instosales = 0;
                                        Instocust = 0;
                                        InsurAmt = 0;
                                        InsAmtDisc = 0;
                                        provision = 0;
                                        Notary = 0;
                                        Rental = 0;
                                        TD = 0;
                                        TC = 0;

                                        // Add by Anthony - 20160321
                                        provisionFee = 0;
                                        // End of add by Anthony - 20160321

                                        object BCFSisaIns;
                                        BCFSisaIns = LeaseTools.GetINSTANCE().GetFieldValue(
                                                "(ISNULL(InsDisc, 0) -(ISNULL(Instosupplier, 0) + ISNULL(Instosalesman, 0) + ISNULL(Instocust, 0))) As TotalSisaIns",
                                                "Lease",
                                                "LeaseNo='" + vLeNo + "'"
                                            );

                                        advarr = LeaseTools.GetINSTANCE().GetFieldValue(
                                            "AdvArr",
                                            "Lease",
                                            "LeaseNo='" + vLeNo + "'"
                                            );

                                        if (Convert.ToInt16(advarr) == 1)
                                        {
                                            strSQL = "";
                                            strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=1";
                                            da = new SqlDataAdapter(strSQL, conn);
                                            dtSchedule = new DataTable();
                                            da.Fill(dtSchedule);
                                            if (dtSchedule.Rows.Count == 0)
                                            {
                                                ErrCode = 2;
                                            }
                                            else
                                            {
                                                Rental = Convert.ToDouble(dtSchedule.Rows[0]["Rental"]);
                                            }
                                        }

                                        strSQL = "";
                                        strSQL = "select s.LeReceivable, s.UnLeIncome, l.Notary, ";
                                        strSQL += "ISNULL(l.AdminFee, 0) AS AdminFee, l.Instosupplier, l.Instosalesman, ";
                                        strSQL += "l.Instocust, (l.InsurAmt + l.InsurIntRate)-l.Instocust As InsurAmt, l.Provision, l.Residual, l.InsDisc As InsDisc ";
                                        strSQL += ", BISurvei, ProvisionFee ";
                                        strSQL += "from Lease as l ";
                                        strSQL += "Inner Join Schedule as s on l.LeaseNo=s.LeaseNo  ";
                                        strSQL += "where l.LeaseNo='" + vLeNo + "' And s.Period=0";

                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtLease = new DataTable();
                                        da.Fill(dtLease);
                                        if (dtLease.Rows.Count == 0)
                                        {
                                            ErrCode = 1;
                                        }
                                        else
                                        {
                                            LReceivable = Convert.ToDouble(dtLease.Rows[0]["LeReceivable"]);
                                            UnLeIncome = Convert.ToDouble(dtLease.Rows[0]["UnLeIncome"]);
                                            AdminFee = Convert.ToDouble(dtLease.Rows[0]["AdminFee"]);
                                            Instosupplier = Convert.ToDouble(dtLease.Rows[0]["Instosupplier"]);
                                            Instosales = Convert.ToDouble(dtLease.Rows[0]["Instosalesman"]);
                                            Instocust = Convert.ToDouble(dtLease.Rows[0]["Instocust"]);
                                            InsurAmt = (Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]) - Convert.ToDouble(dtLease.Rows[0]["InsDisc"]));
                                            provision = Convert.ToDouble(dtLease.Rows[0]["Provision"]) + Convert.ToDouble(BCFSisaIns);
                                            Residual = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
                                            Notary = Convert.ToDouble(dtLease.Rows[0]["Notary"]);
                                            InsAmtDisc = Convert.ToDouble(dtLease.Rows[0]["InsDisc"]);
                                            BiSurvei = Convert.ToDouble(dtLease.Rows[0]["BISurvei"]);

                                            // Add by Anthony - 20160321
                                            provisionFee = Convert.ToDouble(dtLease.Rows[0]["ProvisionFee"]);
                                            // End of add by Anthony - 20160321
                                        }

                                        TD = LReceivable;
                                        // Alter by Anthony - 20160321
                                        //TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + BiSurvei + Rental;
                                        TC = UnLeIncome + AdminFee + Instosupplier + Instosales + Instocust + InsurAmt + provision + Notary + BiSurvei + Rental + provisionFee - Convert.ToDouble(BCFSisaIns);
                                        // End of add by Anthony - 20160321
                                        //TC = UnLeIncome + AdminFee + InsurAmt + Notary + Rental;
                                        vAmt = TD - TC;
                                    }
                                    break;
                                // End

                                case "LRI":
                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtScheduleIns = new DataTable();
                                    da.Fill(dtScheduleIns);
                                    if (dtScheduleIns.Rows.Count == 0)
                                        ErrCode = 0;
                                    else
                                        vAmt = vAmt + Convert.ToDouble(dtScheduleIns.Rows[0]["LeReceivable"]);
                                    break;

                                case "LRL":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    break;

                                case "PRL":
                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtScheduleIns = new DataTable();
                                    da.Fill(dtScheduleIns);
                                    if (dtScheduleIns.Rows.Count == 0)
                                        ErrCode = 0;
                                    else
                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["OutPrinc"]);

                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = vAmt + Convert.ToDouble(dtSchedule.Rows[0]["OutPrinc"]);
                                    break;

                                case "UEL":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
                                    break;

                                case "UEI":
                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=0";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtScheduleIns = new DataTable();
                                    da.Fill(dtScheduleIns);
                                    if (dtScheduleIns.Rows.Count == 0)
                                        ErrCode = 0;
                                    else
                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
                                    break;

                                case "AP":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]) + Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]);
                                    break;

                                case "APL":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]);
                                    break;

                                case "API":
                                    strSQL = "select ((InsurAmt - InsDisc) + InsurIntRate) As InsurAmt from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["InsurAmt"]);
                                    break;

                                case "OPL":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["LeAmount"]);
                                    break;

                                case "SD":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Security"]);
                                    break;

                                case "BL":
                                    vAmt = (TotCr + TotDb) * (-1);
                                    break;

                                case "UNI":
                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
                                    break;

                                case "UT":
                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome", "Lease", "LeaseNo='" + vLeNo + "'"));
                                    if (!string.IsNullOrEmpty(LeaseTools.GetINSTANCE().GetMaxFieldValue("Period", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Payment<>0").ToString()))
                                        vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("Accrual2", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Period=" + LeaseTools.GetINSTANCE().GetMaxFieldValue("Period", "dbo.Schedule", "LeaseNo='" + vLeNo + "' and Payment<>0").ToString()));
                                    if (Convert.ToInt16(LeaseTools.GetINSTANCE().GetFieldValue("LeStatus", "Lease", "LeaseNo='" + vLeNo + "'")) == 7)
                                    {
                                        //StopAccrueDate = Convert.ToDateTime(GetFieldValue("ValueDate", "Trans", "EntityNo='" + vLeNo + "' and TransCode='LL13'"));
                                        //StopAccrueDate = Convert.ToDateTime(GetFieldValue("DueDate", "Schedule", "LeaseNo='" + vLeNo + "' and datediff(month,DueDate,'" + StopAccrueDate + "')=1"));
                                        vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetSumFieldValue("Accrual1", "Schedule", "LeaseNo='10-LS-0003183-001' and datediff(month,DueDate,'6/30/2004')>=0 and datediff(month,DueDate,'6/30/2004')<=2")) + Convert.ToDouble(LeaseTools.GetINSTANCE().GetSumFieldValue("Accrual2", "Schedule", "LeaseNo='10-LS-0003183-001' and datediff(month,DueDate,'6/30/2004')>=0 and datediff(month,DueDate,'6/30/2004')<=2"));
                                    }
                                    break;

                                case "UTL":
                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
                                    break;

                                case "UTI":
                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
                                    strSQL = "select * from ScheduleInsur where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtScheduleIns = new DataTable();
                                    da.Fill(dtScheduleIns);
                                    if (dtScheduleIns.Rows.Count == 0)
                                        ErrCode = 0;
                                    else
                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["UnLeIncome"]);
                                    break;

                                case "RT":
                                    vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutLR", "Lease", "LeaseNo='" + vLeNo + "'"));
                                    break;

                                //case "ADT":
                                //    'Accumulated depreciation-equipment for lease
                                //    'Operating Lease

                                case "ARL":
                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
                                    break;

                                case "AUL":
                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
                                    break;

                                case "CFI":
                                    if (string.IsNullOrEmpty(rowTrans["SubEntityNo"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["SubEntityNo"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
                                    break;

                                case "LRT":
                                case "UIT":
                                case "LIT":
                                case "OIT":
                                case "PT":
                                    ErrCode = 6;
                                    break;

                                case "OP":
                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutPrinc"]);
                                    break;

                                case "UI":
                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]);
                                    break;

                                case "OLR":
                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    break;

                                case "RLI":
                                    if (string.IsNullOrEmpty(rowTrans["InnerCode"].ToString()))
                                        nPeriod = 0;
                                    else
                                        nPeriod = Convert.ToByte(rowTrans["InnerCode"]);
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + nPeriod;
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                    {
                                        int x = 0;
                                        for (byte I = Convert.ToByte(rowTrans["InnerCode"]); I < Convert.ToByte(rowTrans["OuterCode"]); I++)
                                        {
                                            vAmt = vAmt + Convert.ToDouble(dtSchedule.Rows[x]);
                                            x++;
                                        }
                                    }
                                    break;

                                case "GL":
                                    if (Convert.ToInt16(LeaseTools.GetINSTANCE().GetFieldValue("NovasiStatus", "dbo.Lease", "LeaseNo='" + vLeNo + "'")) == 0)
                                        vAmt = Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("GainLoss", "dbo.tTermination", "LeaseNo='" + vLeNo + "' and year(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("yyyy") + "' and month(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("mm") + "' and day(ProcessDate)='" + Convert.ToDateTime(rowTrans["ValueDate"]).ToString("dd") + "'"));
                                    break;

                                case "GLN":
                                    vAmt = Convert.ToDouble(rowTrans["Amount"]);
                                    vAmt = vAmt + Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutIncome_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
                                    vAmt = vAmt - Convert.ToDouble(LeaseTools.GetINSTANCE().GetFieldValue("OutLR_Int", "Lease", "LeaseNo='" + vLeNo + "'"));
                                    break;

                                case "LRS":
                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0) as sumLR from Schedule where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count > 0)
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["sumLR"]);

                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0)) as sumLR from ScheduleInsur where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtScheduleIns = new DataTable();
                                    da.Fill(dtScheduleIns);
                                    if (dtScheduleIns.Rows.Count > 0)
                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["sumLR"]);
                                    break;

                                case "UES":
                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0) as sumLR from Schedule where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count > 0)
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["sumLR"]);

                                    strSQL = "select ISNULL(sum(Rental)-sum(Payment),0)) as sumLR from ScheduleInsur where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtScheduleIns = new DataTable();
                                    da.Fill(dtScheduleIns);
                                    if (dtScheduleIns.Rows.Count > 0)
                                        vAmt = Convert.ToDouble(dtScheduleIns.Rows[0]["sumLR"]);
                                    vAmt = vAmt - Convert.ToDouble(rowTrans["Amount"]);
                                    break;

                                case "UER":
                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter2"].ToString()) ? "0" : rowTrans["DescAfter2"].ToString()) -
                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore2"].ToString()) ? "0" : rowTrans["DescBefore2"].ToString());
                                    break;

                                case "LRR":
                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter3"].ToString()) ? "0" : rowTrans["DescAfter3"].ToString()) -
                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore3"].ToString()) ? "0" : rowTrans["DescBefore3"].ToString());
                                    break;

                                case "OPR":
                                    vAmt = Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescAfter1"].ToString()) ? "0" : rowTrans["DescAfter1"].ToString()) -
                                         Convert.ToDouble(string.IsNullOrEmpty(rowTrans["DescBefore1"].ToString()) ? "0" : rowTrans["DescBefore1"].ToString());
                                    break;

                                default:
                                    //vAmt = vAmt - Convert.ToDouble(rowTrans["Amount"]);
                                    break;
                            }
                            #endregion




                            //Add By : Tomy (20 Agust 2011)
                            //For Receivable Assignment
                            if (ContractType == "1L6" || ContractType == "1C6")
                            {
                                switch (RetrieveAmt)
                                {
                                    case "HOP":
                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutPrincipal"]);
                                        break;

                                    case "DOL":
                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(0);
                                        break;

                                    case "ULI":
                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutIncome"]);
                                        break;

                                    case "LR":
                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["OutLR"]);
                                        break;

                                    case "RV":
                                        strSQL = "select * from LeRecvAssignmentDetail where LeaseNo='" + vLeNo + "'";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(0);
                                        break;
                                }
                            }
                            //End Add
                            //}
                        }
                    }
                    #endregion

                    #region Partial Termination
                    // Partial Termination
                    else if (Status == 2)
                    {
                        object period, BungaDenda, hslp;
                        period = xPer;
                        hslp = Convert.ToInt32(xPer - 1);

                        BungaDenda = LeaseTools.GetINSTANCE().GetFieldValue(
                                "CurrInt",
                                "LetTermination",
                                "LeaseNo='" + vLeNo + "' AND Type=0 AND Period='" + Convert.ToInt32(period) + "'"
                            );

                        switch (RetrieveAmt)
                        {
                            // Leasing
                            case "ULI":
                                //Commented By Marsolim 6 December 2011    
                                //strSQL = "select abs(Adjustment) As Adjustment From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
                                //da = new SqlDataAdapter(strSQL, conn);
                                //dtSchedule = new DataTable();
                                //da.Fill(dtSchedule);
                                //if (dtSchedule.Rows.Count == 0)
                                //    ErrCode = 2;
                                //else
                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
                                //End Comment
                                //Added By Marsolim 6 Dec 2011
                                vAmt = Math.Abs(Convert.ToDouble(rowTrans["DescAfter3"]));
                                //End Add
                                break;

                            case "LRL":
                                #region Commented
                                //if (Convert.ToDouble(BungaDenda) == 0)
                                //{
                                //    //strSQL  = "SELECT (SELECT DescBefore4 From LeTrans ";
                                //    //strSQL += "WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "') ";
                                //    ////strSQL += " - (LeReceivable) + (Rental) ";
                                //    //strSQL += " - (LeReceivable + (Rental-Payment) ";
                                //    //strSQL += "As Hsl ";
                                //    //strSQL += "From Schedule ";
                                //    //strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

                                //    strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

                                //    da = new SqlDataAdapter(strSQL, conn);
                                //    dtSchedule = new DataTable();
                                //    da.Fill(dtSchedule);
                                //    if (dtSchedule.Rows.Count == 0)
                                //        ErrCode = 2;
                                //    else
                                //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                //}
                                //else
                                //{
                                //    //strSQL = "SELECT (LeReceivable - (SELECT DescBefore4 From LeTrans WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "')) * -1 As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
                                //    strSQL  = "SELECT (SELECT DescBefore4 From LeTrans ";
                                //    strSQL += "WHERE EntityNo='" + vLeNo + "' AND SubEntityNo='" + Convert.ToInt32(period) + "') - (LeReceivable) ";
                                //    strSQL += "As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

                                //    da = new SqlDataAdapter(strSQL, conn);
                                //    dtSchedule = new DataTable();
                                //    da.Fill(dtSchedule);
                                //    if (dtSchedule.Rows.Count == 0)
                                //        ErrCode = 2;
                                //    else
                                //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                //}
                                #endregion

                                //Altered By Yusuf 24 Okt 2011
                                //strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

                                //Altered By Marsolim 6 December 2011
                                strSQL = "SELECT (" + Convert.ToDouble(rowTrans["DescBefore4"]) + " - LeReceivable) As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
                                //End Alter

                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);

                                break;

                            // CF
                            case "LIC":
                                //Commented By Marsolim 6 Dec 2011
                                //strSQL = "select abs(Adjustment) As Adjustment From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
                                //da = new SqlDataAdapter(strSQL, conn);
                                //dtSchedule = new DataTable();
                                //da.Fill(dtSchedule);
                                //if (dtSchedule.Rows.Count == 0)
                                //    ErrCode = 2;
                                //else
                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]);
                                //End Comment

                                //Added By Marsolim 6 Dec 2011
                                vAmt = Math.Abs(Convert.ToDouble(rowTrans["DescAfter3"]));
                                //End Add
                                break;

                            case "CFR":


                                //Altered By Yusuf 24 Okt 2011
                                //strSQL = "SELECT ((SELECT LeReceivable From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period ='" + (Convert.ToInt32(period) - 1) + "') - LeReceivable) - Payment As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";

                                //Altered By Marsolim 6 December 2011
                                strSQL = "SELECT (" + Convert.ToDouble(rowTrans["DescBefore4"]) + " - LeReceivable) As Hsl From Schedule WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
                                //End Alter

                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                break;

                            // For All
                            case "DOL":
                                //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                strSQL = "Select (AssetOutPrinc / TotalPrincipal) * ";
                                strSQL += "(Select Security From Lease Where LeaseNo='" + vLeNo + "') As S ";
                                strSQL += "From LetTermination ";
                                strSQL += "Where LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["S"]);
                                break;

                            case "RV":
                                //strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                strSQL = "Select (AssetOutPrinc / TotalPrincipal) * ";
                                strSQL += "(Select Residual From Lease Where LeaseNo='" + vLeNo + "') As D ";
                                strSQL += "From LetTermination ";
                                strSQL += "Where LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "'";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtLease = new DataTable();
                                da.Fill(dtLease);
                                if (dtLease.Rows.Count == 0)
                                    ErrCode = 1;
                                else
                                    vAmt = Convert.ToDouble(dtLease.Rows[0]["D"]);
                                break;

                            case "OVI":
                                strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=0 and Period='" + Convert.ToInt32(period) + "'";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);
                                break;

                            case "GLS":
                                strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=0 and Period='" + Convert.ToInt32(period) + "'";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
                                break;

                            case "CTL":
                                strSQL = "SELECT TValue As Hsl From LetTermination ";
                                strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=0 ";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                break;

                            case "CTC":
                                strSQL = "SELECT TValue As Hsl From LetTermination ";
                                strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=0 ";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                break;
                        }
                    }
                    #endregion

                    #region Early Termination
                    // Early Terminate
                    else if (Status == 3)
                    {
                        object period, hslp, BungaDenda;
                        int TotPeriod;

                        period = xPer;
                        hslp = Convert.ToInt32(xPer - 1);

                        BungaDenda = LeaseTools.GetINSTANCE().GetFieldValue(
                                "CurrInt",
                                "LetTermination",
                                "LeaseNo='" + vLeNo + "' AND Type=1 AND Period='" + Convert.ToInt32(period) + "'"
                            );

                        TotPeriod = Convert.ToInt32(LeaseTools.GetINSTANCE().GetFieldValue(
                                "COUNT(*)-3",
                                "Schedule",
                                "LeaseNo='" + vLeNo + "'"
                            ));

                        if (TotPeriod == Convert.ToInt32(period))
                        {
                            double vCurrIntAmt = 0;
                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                            da = new SqlDataAdapter(strSQL, conn);
                            dtSchedule = new DataTable();
                            da.Fill(dtSchedule);
                            if (dtSchedule.Rows.Count == 0)
                                ErrCode = 2;
                            else
                                vCurrIntAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);

                            double vCFLeaseIncomeAmt = 0;
                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                            da = new SqlDataAdapter(strSQL, conn);
                            dtSchedule = new DataTable();
                            da.Fill(dtSchedule);
                            if (dtSchedule.Rows.Count == 0)
                                ErrCode = 2;
                            else
                                vCFLeaseIncomeAmt = Convert.ToDouble(dtSchedule.Rows[0]["CFLeaseIncome"]);

                            switch (RetrieveAmt)
                            {
                                // Leasing
                                case "ULI":
                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
                                    //Altered By Marsolim 24 Okt 2011
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt;
                                    vAmt = 0;
                                    break;

                                case "LRL":
                                    //if (Convert.ToDouble(BungaDenda) == 0)
                                    //{
                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
                                    //    da = new SqlDataAdapter(strSQL, conn);
                                    //    dtSchedule = new DataTable();
                                    //    da.Fill(dtSchedule);
                                    //    if (dtSchedule.Rows.Count == 0)
                                    //        ErrCode = 2;
                                    //    else
                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    //}
                                    //else
                                    //{
                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                    //    da = new SqlDataAdapter(strSQL, conn);
                                    //    dtSchedule = new DataTable();
                                    //    da.Fill(dtSchedule);
                                    //    if (dtSchedule.Rows.Count == 0)
                                    //        ErrCode = 2;
                                    //    else
                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    //}
                                    if (Convert.ToDouble(BungaDenda) == 0)
                                    {
                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                    }
                                    else
                                    {
                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]) - (Convert.ToDouble(BungaDenda));
                                    }
                                    break;

                                case "LIL":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        //vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCurrIntAmt + vCFLeaseIncomeAmt;
                                        //Altered By Marsolim 24 Okt 2011
                                        vAmt = vCurrIntAmt + vCFLeaseIncomeAmt;
                                    //vAmt = 0;
                                    break;

                                //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                //da = new SqlDataAdapter(strSQL, conn);
                                //dtSchedule = new DataTable();
                                //da.Fill(dtSchedule);
                                //if (dtSchedule.Rows.Count == 0)
                                //    ErrCode = 2;
                                //else
                                //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeIncome"]);

                                // CF
                                case "LIC":
                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
                                    //Altered By Marsolim 24 Okt 2011
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt;
                                    //vAmt = 0;
                                    break;

                                case "CFR":
                                    //if (Convert.ToDouble(BungaDenda) == 0)
                                    //{
                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
                                    //    da = new SqlDataAdapter(strSQL, conn);
                                    //    dtSchedule = new DataTable();
                                    //    da.Fill(dtSchedule);
                                    //    if (dtSchedule.Rows.Count == 0)
                                    //        ErrCode = 2;
                                    //    else
                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    //}
                                    //else
                                    //{
                                    //    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                    //    da = new SqlDataAdapter(strSQL, conn);
                                    //    dtSchedule = new DataTable();
                                    //    da.Fill(dtSchedule);
                                    //    if (dtSchedule.Rows.Count == 0)
                                    //        ErrCode = 2;
                                    //    else
                                    //        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeReceivable"]);
                                    //}
                                    if (Convert.ToDouble(BungaDenda) == 0)
                                    {
                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                    }
                                    else
                                    {
                                        strSQL = "SELECT TValue As Hsl From LetTermination ";
                                        strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                        da = new SqlDataAdapter(strSQL, conn);
                                        dtSchedule = new DataTable();
                                        da.Fill(dtSchedule);
                                        if (dtSchedule.Rows.Count == 0)
                                            ErrCode = 2;
                                        else
                                            vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]) - (Convert.ToDouble(BungaDenda));
                                    }

                                    break;

                                case "CFI":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        //vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCurrIntAmt + vCFLeaseIncomeAmt;
                                        //Altered by Marsolim 24 Okt 2011
                                        vAmt = vCurrIntAmt + vCFLeaseIncomeAmt;
                                    break;
                                ////strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                ////da = new SqlDataAdapter(strSQL, conn);
                                ////dtSchedule = new DataTable();
                                ////da.Fill(dtSchedule);
                                ////if (dtSchedule.Rows.Count == 0)
                                ////    ErrCode = 2;
                                ////else
                                ////    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeIncome"]);
                                //vAmt = 0;
                                //break;

                                // For All
                                case "DOL":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Security"]);
                                    break;

                                case "RV":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
                                    break;

                                case "OVI":
                                    vAmt = 0;
                                    //strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                                    //da = new SqlDataAdapter(strSQL, conn);
                                    //dtSchedule = new DataTable();
                                    //da.Fill(dtSchedule);
                                    //if (dtSchedule.Rows.Count == 0)
                                    //    ErrCode = 2;
                                    //else
                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);
                                    break;

                                case "GLS":
                                    //strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                                    //da = new SqlDataAdapter(strSQL, conn);
                                    //dtSchedule = new DataTable();
                                    //da.Fill(dtSchedule);
                                    //if (dtSchedule.Rows.Count == 0)
                                    //    ErrCode = 2;
                                    //else
                                    //    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
                                    vAmt = 0;
                                    break;

                                case "ADF":
                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AllowancedblfullAmt"]);
                                    break;

                                case "CTL":
                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                    break;

                                case "CTC":
                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                    break;
                            }
                        }
                        else
                        {
                            double vCurrIntAmt1 = 0;
                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                            da = new SqlDataAdapter(strSQL, conn);
                            dtSchedule = new DataTable();
                            da.Fill(dtSchedule);
                            if (dtSchedule.Rows.Count == 0)
                                ErrCode = 2;
                            else
                                vCurrIntAmt1 = Convert.ToDouble(dtSchedule.Rows[0]["CurrInt"]);


                            double vCFLeaseIncomeAmt1 = 0;
                            strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                            da = new SqlDataAdapter(strSQL, conn);
                            dtSchedule = new DataTable();
                            da.Fill(dtSchedule);
                            if (dtSchedule.Rows.Count == 0)
                                ErrCode = 2;
                            else
                                vCFLeaseIncomeAmt1 = Convert.ToDouble(dtSchedule.Rows[0]["CFLeaseIncome"]);

                            switch (RetrieveAmt)
                            {
                                // Leasing
                                case "ULI":
                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
                                    //Altered By Marsolim 24 Okt 2011
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt1;
                                    break;

                                case "LRL":

                                    strSQL = "Select (LeReceivable + Rental - Payment) as LeRec From Schedule Where LeaseNo='" + vLeNo +
                                        "' And Period='" + (period) + "'";
                                    //End Alter

                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeRec"]);

                                    break;

                                case "LIL":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else

                                        vAmt = vCurrIntAmt1 + vCFLeaseIncomeAmt1;
                                    break;

                                // CF
                                case "LIC":
                                    //strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(hslp) + "";
                                    //Altered By Marsolim 24 Okt 2011
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period=" + Convert.ToInt32(period) + "";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["UnLeIncome"]) + vCFLeaseIncomeAmt1;
                                    break;

                                case "CFR":
                                    strSQL = "select LeReceivable-(Select Payment From Schedule Where LeaseNo='" + vLeNo +
                                    "' And Period='" + (period) + "') As LeRec from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(hslp) + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["LeRec"]);

                                    break;

                                case "CFI":
                                    strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else

                                        vAmt = vCurrIntAmt1 + vCFLeaseIncomeAmt1;
                                    break;

                                // For All
                                case "DOL":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Security"]);
                                    break;

                                case "RV":
                                    strSQL = "select * from Lease where LeaseNo='" + vLeNo + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtLease = new DataTable();
                                    da.Fill(dtLease);
                                    if (dtLease.Rows.Count == 0)
                                        ErrCode = 1;
                                    else
                                        vAmt = Convert.ToDouble(dtLease.Rows[0]["Residual"]);
                                    break;

                                case "OVI":
                                    vAmt = 0;

                                    break;

                                case "GLS":
                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["GainLoss"]);
                                    break;

                                case "ADF":
                                    strSQL = "select * from LetTermination where LeaseNo='" + vLeNo + "' and Type=1 and Period='" + Convert.ToInt32(period) + "'";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["AllowancedblfullAmt"]);
                                    break;

                                case "CTL":
                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                    break;

                                case "CTC":
                                    strSQL = "SELECT TValue As Hsl From LetTermination ";
                                    strSQL += "WHERE LeaseNo='" + vLeNo + "' AND Period='" + Convert.ToInt32(period) + "' AND Type=1 ";
                                    da = new SqlDataAdapter(strSQL, conn);
                                    dtSchedule = new DataTable();
                                    da.Fill(dtSchedule);
                                    if (dtSchedule.Rows.Count == 0)
                                        ErrCode = 2;
                                    else
                                        vAmt = Convert.ToDouble(dtSchedule.Rows[0]["Hsl"]);
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region Floating
                    // Floating
                    else if (Status == 4)
                    {
                        // Leasing
                        object period;
                        period = xPer;

                        switch (RetrieveAmt)
                        {
                            case "LRL":
                            case "ULI":
                            case "CRC":
                            case "UCF":
                                strSQL = "select * from Schedule where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = (dtSchedule.Rows[0]["Adjustment"] == DBNull.Value ? 0D : Convert.ToDouble(dtSchedule.Rows[0]["Adjustment"]));
                                break;
                        }
                    }
                    #endregion

                    #region Rescheduling
                    // Rescheduling
                    else if (Status == 21)
                    {
                        // Leasing
                        object period;
                        period = xPer;

                        switch (RetrieveAmt)
                        {
                            case "LRL":
                            case "ULI":
                            case "CRC":
                            case "UCF":
                                strSQL = "select RescAdjust from LeRescheduling where LeaseNo='" + vLeNo + "' and Period='" + Convert.ToInt32(period) + "'";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);
                                if (dtSchedule.Rows.Count == 0)
                                    ErrCode = 2;
                                else
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["RescAdjust"]);


                                break;

                            // Add by Anthony - 20160321
                            case "APF":
                                strSQL = "SELECT ISNULL(ProvisionFee, 0) AS ProvisionFee FROM Lease WHERE LeaseNo = @leaseno;";
                                da = new SqlDataAdapter(strSQL, conn);
                                dtSchedule = new DataTable();
                                da.Fill(dtSchedule);

                                if (dtSchedule.Rows.Count > 0)
                                {
                                    vAmt = 0;
                                }
                                else
                                {
                                    vAmt = Convert.ToDouble(dtSchedule.Rows[0]["ProvisionFee"]);

                                }
                                break;
                                // End of add by Anthony - 20160321

                        }
                    }
                    #endregion

                    #region Contract Interest Accrued For JF
                    // Contract Interest Accrued
                    else if (Status == 24)
                    {

                        if (rowTrans["Amount"] == null)
                            ErrCode = 2;
                        else
                            vAmt = Convert.ToDouble(rowTrans["Amount"]);
                    }
                    #endregion

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    conn.Close();
                }
            }
        }


        public void CreateDbfAllo(
            SqlTransaction trans,
            string strTransNo,
            string strLastInstNo,
            string strBaseCcy,
            string strCurrCode,
            double TotAmt,
            string strSCode,
            string strVchNo,
            DateTime LastValueDate,
            int X,
            string strLastLeaseNo,
            string strBranchCode,
            string strAccCashBasis,
            ref int message
        )
        {
            string strDbNo = "";
            string strCcy = "";
            string strDesc = "";
            double vAmt = 0;
            double ExchRateJournal = 0;
            double ExchRateVoucher = 0;
            bool Reverse;
            int ErrorCode = 0;

            //Variable For GLTrans
            double GL_Amount;

            SqlConnection conn;
            conn = trans.Connection;

            clsModule mdl = new clsModule();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = trans;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                string strSQL;
                SqlDataAdapter da;

                //CreateDbfAllo
                strSQL = "select * from LePaymentInst where InstNo='" + strLastInstNo + "'";
                da = new SqlDataAdapter(strSQL, conn);
                da.SelectCommand.Transaction = trans;
                DataTable dtPI = new DataTable();
                da.Fill(dtPI);
                if (dtPI.Rows.Count == 0)
                    ErrorCode = 6;
                else
                {
                    strSQL = "select * from tblBank where BankCode='" + dtPI.Rows[0]["RecvBank"].ToString() + "'";
                    da = new SqlDataAdapter(strSQL, conn);
                    da.SelectCommand.Transaction = trans;
                    DataTable dtBank = new DataTable();
                    da.Fill(dtBank);

                    if (dtBank.Rows.Count == 0 || string.IsNullOrEmpty(dtBank.Rows[0]["GelAcc"].ToString()))
                    {
                        ErrorCode = 6;
                        strDbNo = ErrorCode.ToString();
                        strCcy = "IDR";
                        strDesc = "UnKnown Bank";
                    }
                    else
                    {
                        DataRow rowBank = dtBank.Rows[0];
                        strCcy = string.IsNullOrEmpty(dtPI.Rows[0]["CurrCode"].ToString()) ? strBaseCcy : dtPI.Rows[0]["CurrCode"].ToString();
                        //strDbNo = rowBank["GLNo"].ToString();
                        strDbNo = rowBank["GelAcc"].ToString();
                        strDesc = rowBank["BankName"].ToString();
                    }
                }

                if (strCcy != strBaseCcy)
                    ExchRateJournal = IDS.Tool.LeaseTools.GetINSTANCE().GetExchangeRate(strCcy, LastValueDate);
                if (strCurrCode != strBaseCcy)
                    ExchRateVoucher = IDS.Tool.LeaseTools.GetINSTANCE().GetExchangeRate(strCurrCode, LastValueDate);
                if (strCcy != strCurrCode)
                    vAmt = (TotAmt * ExchRateVoucher / ExchRateJournal) * -1;
                else
                    vAmt = TotAmt * -1;
                X++;

                Reverse = false;
                if (Convert.ToInt16(IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("Status", "LePaymentInst", "InstNo='" + strLastInstNo + "'")) != 2)
                    Reverse = true;

                if (Reverse == true && vAmt > 0)
                    GL_Amount = (vAmt * -1);
                else
                    GL_Amount = vAmt;

                int msg1 = 1;

                // Alter by Anthony - 20151229
                ////Add By : Tomy (01 Okt 2011)
                //string str;
                //str = "SELECT ISNULL(CashAccLE,'') AS LE, ISNULL(CashAccCF,'') AS CF, ISNULL(CashAccJF,'') AS JF, ";
                //str += "ISNULL(CashAccFT,'') AS FT, ISNULL(CashAccPK,'') AS PK FROM tblBANK WHERE BankCode='" + dtPI.Rows[0]["RecvBank"].ToString() + "'";

                //Add By : Tomy (01 Okt 2011)
                string str;
                str = "SELECT ISNULL(CashAccJF, '') AS JF, ISNULL(CashAccFT,'') AS FT, ISNULL(CashAccInvFL,'') AS InvFL, ISNULL(CashAccInvSL,'') AS InvSL, ISNULL(CashAccInvIB,'') AS InvIB, ISNULL(CashAccPykFL, '') AS PykFL, ISNULL(CashAccPykSL, '') AS PykSL, ISNULL(CashAccPykIB, '') AS PykIB, ISNULL(CashAccPykIJ, '') AS PykIJ, ISNULL(CashAccMoKSL, '') MoKSL, ISNULL(CashAccMoKMU, '') MoKMU, ISNULL(CashAccMtgFL, '') MtgFL, ISNULL(CashAccMtgIB, '') AS MtgIB, ISNULL(CashAccMtgIJ, '') AS MtgIJ, ISNULL(CashAccMtgDG, '') MtgDG, ISNULL(CashAccMtgDS, '') AS MtgDS, ISNULL(CashAccRepossessed, '') AS Repo ";
                str += "FROM tblBANK WHERE BankCode='" + dtPI.Rows[0]["RecvBank"].ToString() + "'";
                // End of alter by Anthony - 20151229

                SqlDataAdapter adapter = new SqlDataAdapter(str, trans.Connection);
                adapter.SelectCommand.Transaction = trans;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    DataRow dtrow = dt.Rows[0];
                    // Alter by Anthony - 20151229 - OJK
                    //string CashAccType = IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("CASE ContractType WHEN 2 THEN 'PK' WHEN 4 THEN 'JF' ELSE (CASE LeaseMethod WHEN 1 THEN 'LE' ELSE 'CF' END) END AS CashAccType", "Lease", "LeaseNo='" + strLastLeaseNo + "'").ToString();

                    StringBuilder sb = new StringBuilder();
                    // Add - Anthony - 20200715
                    sb.Append("CASE ISNULL(LeStatus, 0) ");
                    sb.Append(" WHEN 9 THEN 'Repo' ");
                    sb.Append(" ELSE ");
                    // End add - Anthony - 20200715
                    sb.Append("CASE ContractType ");
                    sb.Append("WHEN 4 THEN 'JF' ");
                    sb.Append("ELSE ( ");
                    sb.Append("CASE FinanceMethod ");
                    sb.Append("WHEN 1 THEN ");
                    sb.Append("CASE IsProject ");
                    sb.Append("WHEN 0 THEN ");
                    sb.Append("CASE LeaseType ");
                    sb.Append("WHEN 1 THEN 'InvFL' ");
                    sb.Append("WHEN 2 THEN 'InvSL' ");
                    sb.Append("WHEN 3 THEN 'InvIB' ");
                    sb.Append("ELSE 'InvFL' ");
                    sb.Append("END ");
                    sb.Append("ELSE ");
                    sb.Append("CASE LeaseType ");
                    sb.Append("WHEN 1 THEN 'PykFL' ");
                    sb.Append("WHEN 2 THEN 'PykSL' ");
                    sb.Append("ELSE ");
                    sb.Append("CASE ISNULL(GoodsService, 0) ");
                    sb.Append("WHEN 1 THEN 'PykIJ' ");
                    sb.Append("ELSE 'PykIB' ");
                    sb.Append("END ");
                    sb.Append("END ");
                    sb.Append("END ");
                    sb.Append("WHEN 2 THEN ");
                    sb.Append("CASE LeaseType ");
                    sb.Append("WHEN 2 THEN 'MoKSL' ");
                    sb.Append("ELSE 'MoKMU' ");
                    sb.Append("END ");
                    sb.Append("ELSE ");
                    sb.Append("CASE LeaseType ");
                    sb.Append("WHEN 1 THEN 'MtgFL' ");
                    sb.Append("WHEN 3 THEN ");
                    sb.Append("CASE ISNULL(GoodsService, 0) ");
                    sb.Append("WHEN 1 THEN 'MtgIJ' ");
                    sb.Append("ELSE 'MtgIB' ");
                    sb.Append("END ");
                    sb.Append("WHEN 5 THEN ");
                    sb.Append("CASE ISNULL(GoodsService, 0) ");
                    sb.Append("WHEN 1 THEN 'MtgDG' ");
                    sb.Append("ELSE 'MtgDS' ");
                    sb.Append("END ");
                    sb.Append("END ");
                    sb.Append("END) ");
                    sb.Append("END END AS CashAccType");

                    string CashAccType = IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue(sb.ToString(), "Lease", "LeaseNo='" + strLastLeaseNo + "'").ToString();
                    // End of alter by Anthony - 20151229 - OJK
                    strAccCashBasis = dtrow[CashAccType].ToString();
                }
                //End Add

                // Add - Anthont - 20200708
                string branchRep = "";
                using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
                {
                    db.CommandText = "SELECT BranchCode, RepresentativeOffice FROM Lease WHERE LeaseNo = @leaseno";
                    db.CommandType = System.Data.CommandType.Text;
                    db.AddParameter("@leaseno", SqlDbType.VarChar, strLastLeaseNo);
                    db.Open();

                    db.ExecuteReader();

                    using (SqlDataReader rd = db.DbDataReader as SqlDataReader)
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if (rd["RepresentativeOffice"] == DBNull.Value || string.IsNullOrEmpty(rd["RepresentativeOffice"] as string))
                                    branchRep = rd["BranchCode"] as string;
                                else
                                    branchRep = rd["RepresentativeOffice"] as string;
                            }
                        }
                    }

                    db.Close();
                }
                // End add - Anthony - 20200708

                //Insert Or Update To LeGLTrans
                cmd.Parameters.Clear();
                cmd.CommandText = "Le_SPUpdateGLTrans";
                cmd.Parameters.AddWithValue("@TransNo", strTransNo);
                cmd.Parameters.AddWithValue("@Action", 0);
                cmd.Parameters.AddWithValue("@Scode", strSCode);
                cmd.Parameters.AddWithValue("@Voucher", strVchNo);
                cmd.Parameters.AddWithValue("@Counter", X);
                cmd.Parameters.AddWithValue("@EntryDate", DateTime.Today);
                cmd.Parameters.AddWithValue("@TransDate", LastValueDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Reverse", Reverse);
                // alter - Anthony - 20200708
                //cmd.Parameters.AddWithValue("@Dept", IDS.Tool.LeaseTools.GetINSTANCE().GetFieldValue("BranchCode", "Lease", "LeaseNo='" + strLastLeaseNo + "'").ToString());
                cmd.Parameters.AddWithValue("@Dept", branchRep);
                // End alter - Anthony - 20200708
                cmd.Parameters.AddWithValue("@Account", strDbNo);
                cmd.Parameters.AddWithValue("@Ccy", strCcy);
                cmd.Parameters.AddWithValue("@Amount", GL_Amount);
                cmd.Parameters.AddWithValue("@Description", strDesc);
                cmd.Parameters.AddWithValue("@LeaseNo", strLastLeaseNo);
                cmd.Parameters.AddWithValue("@DocNo", strLastInstNo);
                cmd.Parameters.AddWithValue("@TransferToGL", false);
                cmd.Parameters.AddWithValue("@ErrCode", ErrorCode);
                cmd.Parameters.AddWithValue("@OriTransDate", LastValueDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@BranchCode", strBranchCode);
                cmd.Parameters.AddWithValue("@AccCashBasis", strAccCashBasis);
                cmd.ExecuteNonQuery();

                if (msg1 == 0)
                {
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                message = 0;
            }
        }

        public void ProcessPayment(//dari lease trans ke lease table
            string strInstNo,
            string strLogUser,
            ref int message
            )
        {
            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                SqlTransaction trans = null;
                trans = conn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Le_SPProcessPayment";
                    cmd.Parameters.AddWithValue("@InstNo", strInstNo);
                    cmd.Parameters.AddWithValue("@UserId", strLogUser);

                    // Add - Anthony - 20190409
                    cmd.CommandTimeout = 0;
                    // End add - Anthony

                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    message = 0;
                    trans.Rollback();
                    // Alter by Anthony - 02072012
                    //WebMsgBox.Show(ex.Message);
                    throw ex;
                    // End of alter by Anthony - 02072012
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        // Add by Anthony - 20180227
        public void UpdateOPULILR(
            SqlTransaction sqlTrans,
            int ByWhat,
            string strLessee,
            string strLease,
            string strCurrency,
            string strBranchCode,
            ref int message
        )
        {
            //using (SqlConnection conn = new SqlConnection(AppTools.GetSQLConnectionString()))
            //{
            //    conn.Open();
            SqlTransaction trans = sqlTrans;
            //trans = conn.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = trans;
                cmd.Connection = trans.Connection;
                cmd.CommandTimeout = 0;
                string strSQL;
                int Counter;
                string GroupNo = "";
                string GuarantorNo = "";
                Counter = 1;

                string strLesseeBranch;
                strLesseeBranch = GetFieldValue("LesseeBranch", "Lease", "LeaseNo='" + strLease + "'").ToString();

                while (Counter <= 3)
                {
                    if (Counter == 1)
                    {
                        switch (ByWhat)
                        {
                            case 0:
                                strSQL = "Le_SPpruScheduleforOPULILRAll";
                                break;
                            case 1:
                                strSQL = "Le_SPpruScheduleforOPULILRByLease '" + strLease + "'";
                                break;
                            case 2:
                                strSQL = "Le_SPpruScheduleforOPULILRByLessee '" + strLessee + "','" + strBranchCode + "'";
                                break;
                        }
                    }
                    else
                    {
                        if (Counter == 2)
                        {
                            strSQL = "Le_SPpruScheduleInsurforOPULILRAll";
                        }
                        else
                        {
                            //Update Another
                            string strSQL1;
                            string strSQL2;
                            string strSQL3;
                            switch (ByWhat)
                            {
                                case 0:
                                    strSQL2 = "Le_SPpruLeaseLesseeforOPULILRAll";
                                    strSQL1 = "Le_SPpruUpdLGforOPULILRAll";
                                    break;
                                default:
                                    //Edit By: Tomy
                                    //strSQL2 = "Le_SPpruLeaseLesseeforOPULILRByLessee '" + strLessee + "'";
                                    //strSQL1 = "Le_SPpruUpdLGforOPULILRByLG '" + strLessee + "'";
                                    strSQL2 = "Le_SPpruLeaseLesseeforOPULILRByLessee '" + strLessee + "' AND BranchCode='" + strLesseeBranch + "'";
                                    strSQL1 = "Le_SPpruUpdLGforOPULILRByLG '" + strLessee + "' AND BranchCode='" + strLesseeBranch + "'";
                                    //End Edit
                                    break;
                            }
                            strSQL = strSQL1;
                            strSQL = strSQL2;

                            SqlDataAdapter da = new SqlDataAdapter(strSQL, trans.Connection);
                            DataTable dt = new DataTable();
                            da.SelectCommand.Transaction = trans;
                            da.Fill(dt);

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {

                                SqlDataAdapter da1 = new SqlDataAdapter("select * from Lessee where LesseeNo='" + strLessee + "' AND BranchCode='" + strLesseeBranch + "'", trans.Connection);
                                DataTable dtLessee = new DataTable();
                                da1.SelectCommand.Transaction = trans;
                                da1.Fill(dtLessee);

                                if (dtLessee.Rows.Count > 0)
                                {
                                    double mOP;
                                    double mULI;
                                    mOP = 0;
                                    mULI = 0;

                                    DataRow row = dt.Rows[i];
                                    if (strCurrency != row["CurrCode"].ToString())
                                    {
                                        mOP = 0;
                                        mULI = 0;
                                    }

                                    //Edit By: Tomy (30 Apr 2011)
                                    //mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                    //mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                    if (row["OutPrincipal"] != DBNull.Value)
                                    {
                                        mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                        mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                    }
                                    //End Edit

                                    strSQL3 = "Le_SPpruUpdOSLessee '" + strLessee + "','" + row["CurrCode"].ToString() + "'," + mOP + "," + mULI + "," + (mOP + mULI) + ",'" + strBranchCode + "','" + strLesseeBranch + "'";
                                    cmd.CommandText = strSQL3;
                                    cmd.ExecuteNonQuery();
                                    if (ByWhat != 0)
                                    {
                                        //Get Group No
                                        GroupNo = dtLessee.Rows[0]["GrpNo"].ToString();

                                        //Get Guarantor No
                                        SqlDataAdapter da2 = new SqlDataAdapter("select * from Lease where LeaseNo='" + strLease + "'", trans.Connection);
                                        DataTable dtLease = new DataTable();
                                        da2.SelectCommand.Transaction = trans;
                                        da2.Fill(dtLease);
                                        GuarantorNo = dtLease.Rows[0]["Guarantor1"].ToString();
                                    }
                                }
                            }

                            //Update Guarantor
                            switch (ByWhat)
                            {
                                case 0:
                                    strSQL = "Le_SPpruLeaseGuarantorforOPULILRAll";
                                    break;
                                default:
                                    strSQL = "Le_SPpruLeaseGuarantorforOPULILRByGuarantor '" + GuarantorNo + "'";
                                    break;
                            }
                            da = new SqlDataAdapter(strSQL, trans.Connection);
                            dt = new DataTable();
                            da.SelectCommand.Transaction = trans;
                            da.Fill(dt);

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                GuarantorNo = dt.Rows[i]["Guarantor1"].ToString();
                                SqlDataAdapter da1 = new SqlDataAdapter("select * from Lessee where LesseeNo='" + GuarantorNo + "' AND BranchCode='" + strLesseeBranch + "'", trans.Connection);
                                DataTable dtGuarantor = new DataTable();
                                da1.SelectCommand.Transaction = trans;
                                da1.Fill(dtGuarantor);

                                if (dtGuarantor.Rows.Count > 0)
                                {
                                    double mOP;
                                    double mULI;
                                    mOP = 0;
                                    mULI = 0;

                                    DataRow row = dt.Rows[i];

                                    if (strCurrency != row["CurrCode"].ToString())
                                    {
                                        mOP = 0;
                                        mULI = 0;
                                    }

                                    //Edit By: Tomy (30 Apr 2011)
                                    //mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                    //mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                    if (row["OutPrincipal"] != DBNull.Value)
                                    {
                                        mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                        mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                    }
                                    //End Edit
                                    strSQL3 = "Le_SPpruUpdOSLessee '" + GuarantorNo + "','" + row["CurrCode"].ToString() + "'," + mOP + "," + mULI + "," + (mOP + mULI) + ",'" + strBranchCode + "','" + strLesseeBranch + "'";
                                    cmd.CommandText = strSQL3;
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            //Update Group
                            switch (ByWhat)
                            {
                                case 0:
                                    strSQL1 = "Le_SPpruLesseeforOPULILRAll";
                                    strSQL2 = "Le_SPpruUpdGroupforOPULILRAll";
                                    break;
                                default:
                                    strSQL1 = "Le_SPpruLesseeforOPULILRByGroup " + GroupNo;
                                    strSQL2 = "Le_SPpruUpdGroupforOPULILRByGroup " + GroupNo;
                                    break;
                            }
                            strSQL = strSQL2;
                            strSQL = strSQL1;
                            da = new SqlDataAdapter(strSQL, trans.Connection);
                            dt = new DataTable();
                            da.SelectCommand.Transaction = trans;
                            da.Fill(dt);

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                GroupNo = dt.Rows[i]["GrpNo"].ToString();
                                SqlDataAdapter da1 = new SqlDataAdapter("select * from tblGroup where GrpNo='" + GroupNo + "'", trans.Connection);
                                DataTable dtGroup = new DataTable();
                                da1.SelectCommand.Transaction = trans;
                                da1.Fill(dtGroup);

                                if (dtGroup.Rows.Count > 0)
                                {
                                    double mOP;
                                    double mULI;
                                    mOP = 0;
                                    mULI = 0;

                                    DataRow row = dt.Rows[i];

                                    if (strCurrency != row["CurrCode"].ToString())
                                    {
                                        mOP = 0;
                                        mULI = 0;
                                    }

                                    //Edit By: Tomy (30 Apr 2011)
                                    //mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                    //mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                    if (row["OutPrincipal"] != DBNull.Value)
                                    {
                                        mOP = mOP + Convert.ToDouble(row["OutPrincipal"] == null ? 0 : row["OutPrincipal"]);
                                        mULI = mULI + Convert.ToDouble(row["OutIncome"] == null ? 0 : row["OutIncome"]);
                                    }
                                    //End Edit

                                    strSQL3 = "Le_SPpruUpdOSGroup " + GroupNo + ",'" + row["CurrCode"].ToString() + "'," + mOP + "," + mULI + "," + (mOP + mULI);
                                    cmd.CommandText = strSQL3;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    Counter++;
                }
                //trans.Commit();
            }
            catch (Exception ex)
            {
                message = 0;
                trans.Rollback();
                // Alter by Anthony - 02072012
                //WebMsgBox.Show(ex.Message);
                throw ex;
                // End of alter by Anthony - 02072012
            }
            finally
            {
                //conn.Close();
            }
            //}
        }
        public void Le_SPUpdateGLTrans(
            string strTransNo,
            string Scode,
            string Voucher,
            byte Counter,
            DateTime EntryDate,
            DateTime TransDate,
            bool Reverse,
            string Dept,
            string Account,
            string Ccy,
            double Amount,
            string Description,
            string LeaseNo,
            string DocNo,
            bool TransferToGL,
            int ErrCode,
            DateTime OriTransDate,
            string strBranchCode,
            string strAccCashBasis,
            ref int message
        )
        {
            int Action;

            using (SqlConnection conn = new SqlConnection(IDS.DataAccess.SqlServer.GetSQLConnectionString()))
            {
                conn.Open();
                SqlTransaction trans = null;
                trans = conn.BeginTransaction();
                clsModule mdl = new clsModule();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    string strSQL;
                    strSQL = "SELECT * FROM LeGLTRANS WHERE SCODE='" + Scode + "' and Voucher='" + Voucher + "' and BranchCode='" + strBranchCode + "' and Counter=" + Counter.ToString();
                    if (!mdl.checkExistingData(strSQL))
                        Action = 0; //Insert LeGLTrans
                    else
                        Action = 1; //Update LeGLTrans
                    cmd.Parameters.Clear();
                    cmd.CommandText = "Le_SPUpdateGLTrans";
                    cmd.Parameters.AddWithValue("@TransNo", strTransNo);
                    cmd.Parameters.AddWithValue("@Action", Action);
                    cmd.Parameters.AddWithValue("@Scode", Scode);
                    cmd.Parameters.AddWithValue("@Voucher", Voucher);
                    cmd.Parameters.AddWithValue("@Counter", Counter);
                    cmd.Parameters.AddWithValue("@EntryDate", EntryDate);
                    cmd.Parameters.AddWithValue("@TransDate", TransDate);
                    cmd.Parameters.AddWithValue("@Reverse", Reverse);
                    cmd.Parameters.AddWithValue("@Dept", Dept);
                    cmd.Parameters.AddWithValue("@Account", Account);
                    cmd.Parameters.AddWithValue("@Ccy", Ccy);
                    cmd.Parameters.AddWithValue("@Amount", Amount);
                    cmd.Parameters.AddWithValue("@Description", Description);
                    cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
                    cmd.Parameters.AddWithValue("@DocNo", DocNo);
                    cmd.Parameters.AddWithValue("@TransferToGL", TransferToGL);
                    cmd.Parameters.AddWithValue("@ErrCode", ErrCode);
                    cmd.Parameters.AddWithValue("@OriTransDate", OriTransDate);
                    cmd.Parameters.AddWithValue("@BranchCode", strBranchCode);
                    cmd.Parameters.AddWithValue("@AccCashBasis", strAccCashBasis);
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    message = 0;
                    trans.Rollback();
                    //WebMsgBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        //Update Monthly Outstanding Per Contract
        public void UpdateMonthlyOutstanding(
        string LeaseNo,
        string xPeriod,
        string OperatorID,
        DateTime LastUpd,
        ref int MsgHsl
        )
        {
            LeaseTools.GetINSTANCE().UpdateMonthlyOutstandingPerContract(
                   LeaseNo,
                   xPeriod,
                   OperatorID,
                   LastUpd,
                   ref MsgHsl
                   );
        }
        public void UpdateMonthlyOutstandingPerContract(
        string LeaseNo,
        string xPeriod,
        string OperatorID,
        DateTime LastUpd,
        ref int Msg
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

                    cmd.CommandText = "Le_SPUpdateOutstanding";
                    cmd.Parameters.AddWithValue("@LeaseNo", LeaseNo);
                    cmd.Parameters.AddWithValue("@xPeriod", xPeriod);
                    cmd.Parameters.AddWithValue("@OperatorID", OperatorID);
                    cmd.Parameters.AddWithValue("@LastUpd", LastUpd);

                    trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null)
                        trans.Rollback();
                    Msg = 0;
                    throw ex;
                }
                finally
                {
                    Msg = 1;
                    conn.Close();
                }
            }
        }
    }
}
