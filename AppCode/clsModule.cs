using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace AppCode
{
    public class clsModule
    {
        #region Variable Declaration
        int x;
       
        #endregion

        #region Null String To Trim()
        public string SetFromNullString(SqlDataReader drTemp, string fieldName, bool isNum)
        {
            if (!DBNull.Value.Equals(drTemp[fieldName]))
            {
                if (isNum == false)
                {
                    return (string)drTemp[fieldName];
                }
                else
                {
                    if (Convert.ToDouble(drTemp[fieldName]) == 0)
                    {
                        return "0.00";
                    }
                    else
                    {
                        return Convert.ToString(drTemp[fieldName]);
                    }
                }
            }
            else
            {
                if (isNum == false)
                {
                    return String.Empty;
                }
                else
                {
                    return "0.00";
                }
            }
        }
        #endregion

        #region Set Null Double to Zero
        public double SetNullDouble(string Text)
        {
            double doVar = 0;

            if (Text == null || Text.Trim() == "" || Text.Trim() == "0.00")
            {
                doVar = 0;
            }
            else
            {
                doVar = Convert.ToDouble(Text);
            }
            return doVar;
        }
        #endregion

        #region Set Null Int to Zero
        public int SetNullInt(string Text)
        {
            int iVar = 0;
            if (Text == null || Text.Trim() == "" || Text.Trim() == "0.00")
            {
                iVar = 0;
            }
            else
            {
                iVar = Convert.ToInt32(Convert.ToDouble(Text));
            }
            return iVar;
        }
        #endregion

        #region Set Null Tinyint to Zero
        public byte SetNullTinyInt(string Text)
        {
            byte byVar = 0;
            if (Text == null || Text.Trim() == "" || Text.Trim() == "0.00" || Text == string.Empty)
            {
                byVar = 0;
            }
            else
            {
                byVar = Convert.ToByte(Text);
            }
            return byVar;
        }
        #endregion

        #region Set Null DB SQLParameter
        public SqlParameter setNullParam(string TextFrom)
        {
            SqlParameter ParamTemp = new SqlParameter();
            if (string.IsNullOrEmpty(TextFrom))
            {
                ParamTemp.Value = DBNull.Value;
            }
            else
            {
                ParamTemp.Value = TextFrom;
            }

            return ParamTemp;
        }
        #endregion

        #region Set Null Bigint to Zero
        public long SetNullBigInt(string Text)
        {
            long lnVar = 0;
            if (Text == null || Text.Trim() == "" || Text.Trim() == "0.00")
            {
                lnVar = 0;
            }
            else
            {
                lnVar = Convert.ToInt64(Convert.ToDouble(Text));
            }
            return lnVar;
        }
        #endregion

        #region Set Null float to Zero
        public float SetNullReal(string Text)
        {
            float flVar = 0;
            if (Text == null || Text.Trim() == "" || Text.Trim() == "0.00")
            {
                flVar = 0;
            }
            else
            {
                flVar = Convert.ToSingle(Text);
            }
            return flVar;
        }
        #endregion

        #region Set Null Decimal To Zero
        public decimal SetNullDecimal(string Text)
        {
            decimal decVar = 0;
            if (Text == null || Text.Trim() == "" || Text.Trim() == "0.00")
            {
                decVar = 0;
            }
            else
            {
                decVar = Convert.ToDecimal(Convert.ToDouble(Text));
            }
            return decVar;
        }
        #endregion

        #region Set Numeric Format
        public string SetNumericFormat(string Text, int ConType)
        {
            if (Text.Trim() == "" || Text == null)
            {
                Text = "0";
            }

            string strCText = "";
            switch (ConType)
            {
                case 1:
                    strCText = Text;
                    break;
                case 2:
                    strCText = Convert.ToString(Convert.ToInt32(Text));
                    break;
                case 3:
                    strCText = Convert.ToString(Convert.ToInt64(Text));
                    break;
                case 4:
                    strCText = string.Format("{0:N}", Convert.ToDecimal(Text));
                    break;
                case 5:
                    strCText = string.Format("{0:N}", Convert.ToDouble(Text));
                    break;
                case 6:
                    strCText = string.Format("{0:N}", Convert.ToSingle(Text));
                    break;

            }
            return strCText;
        }
        #endregion

        //Add by Jeremi 14 Juni 2025
        //End Jeremi
        #region Parsing 'NULL to String Statement
        public string setNullParse(string FromString)
        {
            if (FromString == null)
            {
                FromString = "";
            }
            FromString = FromString.Replace("'", "");
            if (FromString.Trim() == "")
            {
                FromString = "NULL";
            }
            else
            {
                FromString = "'" + FromString + "'";
            }

            return FromString;
        }
        #endregion

        #region Replace ' To Trim()
        public string setStringChars(string FromString)
        {
            FromString = FromString.Replace("'", "");
            if (FromString.Trim() == "")
            {
                FromString = string.Empty;
            }

            return FromString;
        }
        #endregion

        #region Replace " " String To NULL Statement in SQL
        public string strSetVarcharStatement(string FromString)
        {
            if (FromString != null)
            {
                FromString = FromString.Replace("'", "");
            }
            else
            {
                FromString = "";
            }


            if (FromString.Trim() == "")
            {
                FromString = "NULL";
            }
            else
            {
                FromString = "'" + FromString + "'";
            }
            return FromString;
        }
        #endregion

        #region Replace " " String To NULL Statement In Option No Apostroph
        public string strSetVarcharStatement(string FromString, bool ReturnApostroph, bool ReturnNull)
        {
            if (FromString != null)
            {
                FromString = FromString.Replace("'", "");
            }
            else
            {
                FromString = "";
            }


            if (FromString.Trim() == "")
            {
                if (ReturnNull)
                {
                    FromString = "NULL";
                }
                else
                {
                    FromString = "";
                }
            }
            else
            {
                if (ReturnApostroph == true)
                {
                    FromString = "'" + FromString + "'";
                }
            }
            return FromString;
        }
        #endregion

        #region Replace " " String To NULL Statement In Option No Apostroph
        public string strSetVarcharStatement(string FromString, bool ReturnApostroph)
        {
            if (FromString != null)
            {
                FromString = FromString.Replace("'", "");
            }
            else
            {
                FromString = "";
            }


            if (FromString.Trim() == "")
            {
                FromString = "NULL";
            }
            else
            {
                if (ReturnApostroph == true)
                {
                    FromString = "'" + FromString + "'";
                }
            }
            return FromString;
        }
        #endregion

      

        #region Set Null Value From Object
        public object NVL(object Value, object ValueIfNull)
        {
            if (Convert.IsDBNull(Value) == true)
            {
                return ValueIfNull;
            }

            return Value;
        }
        #endregion

        #region Initiate NULL Value From SQLParameter
        public SqlParameter NullParamStr(object sender)
        {
            SqlParameter param = new SqlParameter();
            if (sender == null)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = sender.ToString().Replace("'", "");
            }
            return param;
        }
        #endregion

        #region Initiate 0 Value From SQLParameter
        public SqlParameter NullParamVal(object sender)
        {
            SqlParameter param = new SqlParameter();
            if (sender == null)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = sender.ToString().Replace("'", "");
            }
            return param;
        }
        #endregion

        #region string From DBNULL
        public string DBNullToString(object value)
        {
            if (value is DBNull)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }
        #endregion

        #region Int From DBNULL
        public int DBNullToInt(object value, int ifDBNullValue)
        {
            if (value is DBNull)
                return ifDBNullValue;

            return Convert.ToInt32(value);
        }

        public double DBNullToDouble(object value, double ifDBNullValue)
        {
            if (value is DBNull)
                return ifDBNullValue;

            return Convert.ToDouble(value);
        }

        public decimal DBNullToDecimal(object value, decimal ifDBNullValue)
        {
            if (value is DBNull)
                return ifDBNullValue;

            return Convert.ToDecimal(value);
        }
        #endregion

        #region DateTime ==> Varchar(6) Period
        public string SetPeriod(DateTime Date)
        {
            int mn = 0;
            int yr = 0;
            string period;
            mn = Convert.ToInt16(Date.Month);
            yr = Convert.ToInt16(Date.Year);
            if (mn < 10)
            {
                period = "0" + Convert.ToString(mn);
            }
            else
            {
                period = Convert.ToString(mn);
            }
            period = Convert.ToString(yr) + period;
            return period;
        }

        public string SetPeriod(DateTime Date, bool ReturnYear)
        {
            int mn = 0;

            string period;

            if (ReturnYear)
            {
                return SetPeriod(Date);
            }
            else
            {
                mn = Convert.ToInt16(Date.Month);

                if (mn < 10)
                {
                    period = "0" + Convert.ToString(mn);
                }
                else
                {
                    period = Convert.ToString(mn);
                }
                return period;
            }
        }

        public string SetPeriod(int DateMonth)
        {
            int mn = DateMonth;
            string period;

            if (mn < 10)
            {
                period = "0" + Convert.ToString(mn);
            }
            else
            {
                period = Convert.ToString(mn);
            }
            return period;
        }

        public string SetPeriod(int DateMonth, int DateYear)
        {
            int mn = DateMonth;
            int yr = DateYear;
            string period;

            if (mn < 10)
            {
                period = "0" + Convert.ToString(mn);
            }
            else
            {
                period = Convert.ToString(mn);
            }
            period = Convert.ToString(yr) + period;
            return period;
        }
        #endregion

        #region Translate Words To Month W/ Overloads Function
        public byte WordsToMonth(string Words)
        {
            byte bt = 0;
            switch (Words.ToUpper().Trim())
            {
                case "JANUARY":
                    bt = 1;
                    break;
                case "FEBRUARY":
                    bt = 2;
                    break;
                case "MARCH":
                    bt = 3;
                    break;
                case "APRIL":
                    bt = 4;
                    break;
                case "MAY":
                    bt = 5;
                    break;
                case "JUNE":
                    bt = 6;
                    break;
                case "JULY":
                    bt = 7;
                    break;
                case "AUGUST":
                    bt = 8;
                    break;
                case "SEPTEMBER":
                    bt = 9;
                    break;
                case "OCTOBER":
                    bt = 10;
                    break;
                case "NOVEMBER":
                    bt = 11;
                    break;
                case "DECEMBER":
                    bt = 12;
                    break;
            }
            return bt;
        }

        //TRANSLATE WORDS TO PERIOD
        public byte WordsToMonth(string Words, bool Indonesian)
        {
            if (Indonesian == false)
            {
                return WordsToMonth(Words);
            }
            else
            {
                byte bt = 0;
                switch (Words.ToUpper().Trim())
                {
                    case "JANUARI":
                        bt = 1;
                        break;
                    case "FEBRUARI":
                        bt = 2;
                        break;
                    case "MARET":
                        bt = 3;
                        break;
                    case "APRIL":
                        bt = 4;
                        break;
                    case "MEI":
                        bt = 5;
                        break;
                    case "JUNI":
                        bt = 6;
                        break;
                    case "JULI":
                        bt = 7;
                        break;
                    case "AGUSTUS":
                        bt = 8;
                        break;
                    case "SEPTEMBER":
                        bt = 9;
                        break;
                    case "OKTOBER":
                        bt = 10;
                        break;
                    case "NOVEMBER":
                        bt = 11;
                        break;
                    case "DESEMBER":
                        bt = 12;
                        break;
                }
                return bt;
            }
        }
        #endregion

        #region Translate Month To Words W/ Overloads Function
        public string MonthToWords(string DateToConvert)
        {

            if (DateToConvert.Trim() == "")
            {
                return "";
            }

            string strRes = "";
            int iMonth = 0;

            if (DateToConvert.Length == 6)
            {
                iMonth = Convert.ToInt16(DateToConvert.Substring(4, 2));
            }
            else
            {
                iMonth = Convert.ToInt16(DateToConvert);
            }

            switch (iMonth)
            {
                case 1:
                    strRes = "January";
                    break;
                case 2:
                    strRes = "February";
                    break;
                case 3:
                    strRes = "March";
                    break;
                case 4:
                    strRes = "April";
                    break;
                case 5:
                    strRes = "May";
                    break;
                case 6:
                    strRes = "June";
                    break;
                case 7:
                    strRes = "July";
                    break;
                case 8:
                    strRes = "August";
                    break;
                case 9:
                    strRes = "September";
                    break;
                case 10:
                    strRes = "October";
                    break;
                case 11:
                    strRes = "November";
                    break;
                case 12:
                    strRes = "December";
                    break;
            }
            return strRes;
        }

        public string MonthToWords(DateTime DateToConvert)
        {
            string strRes = "";
            int iMonth = 0;
            iMonth = DateToConvert.Month;
            switch (iMonth)
            {
                case 1:
                    strRes = "January";
                    break;
                case 2:
                    strRes = "February";
                    break;
                case 3:
                    strRes = "March";
                    break;
                case 4:
                    strRes = "April";
                    break;
                case 5:
                    strRes = "May";
                    break;
                case 6:
                    strRes = "June";
                    break;
                case 7:
                    strRes = "July";
                    break;
                case 8:
                    strRes = "August";
                    break;
                case 9:
                    strRes = "September";
                    break;
                case 10:
                    strRes = "October";
                    break;
                case 11:
                    strRes = "November";
                    break;
                case 12:
                    strRes = "December";
                    break;
            }
            return strRes;
        }

        public string MonthToWords(DateTime DateToConvert, bool TranslateToIndonesian)
        {
            if (TranslateToIndonesian == false)
            {
                return MonthToWords(DateToConvert);
            }
            else
            {
                string strRes = "";
                int iMonth = 0;
                iMonth = DateToConvert.Month;
                switch (iMonth)
                {
                    case 1:
                        strRes = "Januari";
                        break;
                    case 2:
                        strRes = "Februari";
                        break;
                    case 3:
                        strRes = "Maret";
                        break;
                    case 4:
                        strRes = "April";
                        break;
                    case 5:
                        strRes = "Mei";
                        break;
                    case 6:
                        strRes = "Juni";
                        break;
                    case 7:
                        strRes = "Juli";
                        break;
                    case 8:
                        strRes = "Agustus";
                        break;
                    case 9:
                        strRes = "September";
                        break;
                    case 10:
                        strRes = "Oktober";
                        break;
                    case 11:
                        strRes = "November";
                        break;
                    case 12:
                        strRes = "Desember";
                        break;
                }
                return strRes;
            }
        }


        public string MonthToWords(string DateToConvert, bool TranslateToIndonesian)
        {

            if (DateToConvert.Trim() == "")
            {
                return "";
            }

            if (TranslateToIndonesian == false)
            {
                return MonthToWords(DateToConvert);
            }
            else
            {
                string strRes = "";
                int iMonth = 0;
                iMonth = Convert.ToInt16(DateToConvert.Substring(4, 2));
                switch (iMonth)
                {
                    case 1:
                        strRes = "Januari";
                        break;
                    case 2:
                        strRes = "Februari";
                        break;
                    case 3:
                        strRes = "Maret";
                        break;
                    case 4:
                        strRes = "April";
                        break;
                    case 5:
                        strRes = "Mei";
                        break;
                    case 6:
                        strRes = "Juni";
                        break;
                    case 7:
                        strRes = "Juli";
                        break;
                    case 8:
                        strRes = "Agustus";
                        break;
                    case 9:
                        strRes = "September";
                        break;
                    case 10:
                        strRes = "Oktober";
                        break;
                    case 11:
                        strRes = "November";
                        break;
                    case 12:
                        strRes = "Desember";
                        break;
                }
                return strRes;
            }
        }

      

        #endregion

        #region VB Function Left, Right, Mid
        #region Left
        public string Left(string Text, int length)
        {
            if (string.IsNullOrEmpty(Text)) return "";
            string result = Text.Length > length ? Text.Substring(0, length) : Text;
            return result;
        }
        #endregion

        #region Right
        public string Right(string Text, int length)
        {
            string result = Text.Substring(Text.Length - length, length);
            return result;
        }
        #endregion

        #region Mid +2 Overloads
        public string Mid(string Text, int startIndex, int length)
        {
            string result = Text.Substring(startIndex, length);
            return result;
        }
        public string Mid(string Text, int startIndex)
        {
            string result = Text.Substring(startIndex);
            return result;
        }
        #endregion
        #endregion

        public bool checkExistingData(string sqlStatement)
        {

            DataSet dsNew = showTable(sqlStatement);
            x = dsNew.Tables[0].Rows.Count;
            if (x == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        string stringConn;
        SqlConnection sConn;
        public DataSet showTable(string strSql)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(strSql, Open());
                // add by Anthony - 20160120
                da.SelectCommand.CommandTimeout = 120;
                // End of add by Anthony - 20160120
                ds.CaseSensitive = false;
                da.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Close();
            }
        }
        public SqlConnection Open()
        {
            stringConn = IDS.DataAccess.SqlServer.GetSQLConnectionString();
            
            sConn = new SqlConnection(stringConn);
            sConn.Open();
            return sConn;
        }
        public SqlConnection Close()
        {
            //string stringConn = buildConnection();
            //SqlConnection sConn = new SqlConnection(stringConn);
            sConn.Close();
            sConn.Dispose();
            GC.WaitForPendingFinalizers();
            return sConn;
        }


        #region Set 2 Digit To String
        public string SetDigit(int byVal)
        {
            if (byVal < 10)
            {
                return "0" + Convert.ToString(byVal);
            }
            else
            {
                return Convert.ToString(byVal);
            }
        }
        #endregion

        #region Check Number
        public Boolean IsNaN(string Values)
        {
            char[] Template;
            bool IsNotANumber = false;
            Template = Values.ToCharArray();

            for (int ctr = 0; ctr < Template.Length; ctr++)
            {
                if (Template[ctr] < '0' && Template[ctr] > '9')
                {
                    IsNotANumber = true;
                    break;
                }
            }
            return IsNotANumber;
        }
        #endregion

        #region PayMethod
        public string PayMth(int PayCode)
        {
            string str;
            str = "";
            switch (PayCode)
            {
                case 0:
                    str = "Giro Bilyet";
                    break;
                case 1:
                    str = "Cheque";
                    break;
                case 2:
                    str = "Cash";
                    break;
                case 3:
                    str = "Inkaso";
                    break;
                case 4:
                    str = "Standing Order";
                    break;
                case 5:
                    str = "Cummulative";
                    break;
                default:
                    str = "Bank Transfer";
                    break;
            }
            return str;
        }
        #endregion

        #region Guardian
        public string Guardian(int Gtype)
        {
            string str;
            str = "";
            switch (Gtype)
            {
                case 0:
                    str = "Personal";
                    break;
                case 1:
                    str = "Corporate";
                    break;
                case 2:
                    str = "Payment Guarantee";
                    break;
                default:
                    str = "Hipotik Land and Building";
                    break;
            }
            return str;
        }
        #endregion

        #region Hari
        public string Hari(string NHari)
        {
            string str;
            str = "";
            switch (NHari)
            {
                case "Sunday":
                    str = "Minggu";
                    break;
                case "Monday":
                    str = "Senin";
                    break;
                case "Tuesday":
                    str = "Selasa";
                    break;
                case "Wednesday":
                    str = "Rabu";
                    break;
                case "Thursday":
                    str = "Kamis";
                    break;
                case "Friday":
                    str = "Jumat";
                    break;
                default:
                    str = "Sabtu";
                    break;
            }
            return str;
        }
        #endregion

        #region CurrType
        public int CurrType(string Ctype)
        {
            int code;
            switch (Ctype)
            {
                case "IDR":
                    code = 1;
                    break;
                case "USD":
                    code = 2;
                    break;
                default:
                    code = 3;
                    break;
            }
            return code;
        }
        #endregion



        #region RoundNominal
        public double RoundNominal(double nominal, string curr)
        {
            SqlCommand sql = new SqlCommand();
            SqlConnection sqlconn = new SqlConnection();
            sqlconn.ConnectionString = IDS.DataAccess.SqlServer.GetSQLConnectionString();
            sqlconn.Open();
            sql.Connection = sqlconn;
            sql.CommandText = "Le_SPGetCurrency";
            sql.CommandType = CommandType.StoredProcedure;
            sql.Parameters.AddWithValue("@CurrCode", curr);
            sql.Parameters.Add("@DecimalPlace", SqlDbType.Float);
            sql.Parameters.Add("@Rounding", SqlDbType.Float);
            sql.Parameters.Add("@NUD", SqlDbType.Float);
            sql.Parameters["@DecimalPlace"].Direction = ParameterDirection.Output;
            sql.Parameters["@Rounding"].Direction = ParameterDirection.Output;
            sql.Parameters["@NUD"].Direction = ParameterDirection.Output;
            sql.ExecuteNonQuery();
            double DecPlace = Convert.ToDouble(sql.Parameters["@DecimalPlace"].Value);
            double Rounding = Convert.ToDouble(sql.Parameters["@Rounding"].Value);
            double NUD = Convert.ToDouble(sql.Parameters["@NUD"].Value);
            double RoundResult;
            if (Rounding != 1)
            {
                double a = 0;
                if (nominal > Rounding)
                    a = Convert.ToInt64(Right(Convert.ToInt64(nominal).ToString(), Rounding.ToString().Length - 1));
                RoundResult = Convert.ToInt64(nominal) - Convert.ToInt64(a);
                if ((a * 2) >= Rounding)
                    RoundResult = RoundResult + Rounding;
                sqlconn.Close();
                if (a != 0)
                {
                    nominal = (double)RoundResult;
                }
            }
            return (double)Math.Round(Convert.ToDecimal(nominal), (int)DecPlace);
        }
        #endregion

        #region GetJCodeLeasing
        public string GetJCodeLeasing(int Code, string LeaseNo)
        {
            SqlCommand sql = new SqlCommand();
            SqlConnection sqlconn = new SqlConnection();
            sqlconn.ConnectionString = IDS.DataAccess.SqlServer.GetSQLConnectionString();
            sqlconn.Open();
            sql.Connection = sqlconn;
            sql.CommandText = "Le_SPGetJCode";
            sql.CommandType = CommandType.StoredProcedure;
            sql.Parameters.AddWithValue("@Code", Code);
            sql.Parameters.AddWithValue("@LeaseNo", LeaseNo);
            sql.Parameters.Add("@JCode", SqlDbType.VarChar, 8);
            sql.Parameters["@JCode"].Direction = ParameterDirection.Output;
            sql.ExecuteNonQuery();
            string JCode = sql.Parameters["@JCode"].Value.ToString();
            return JCode;
        }
        //Add by Jeremi 14 Juni 2025
        
        //End Jeremi
        // Add - Anthony - 20200512
        public string GetJCodeLeasing(IDS.DataAccess.SqlServer db, int Code, string LeaseNo)
        {
            string JCode = "";
            if (db != null)
            {
                db.Open();
                db.CommandText = "Le_SPGetJCode";
                db.CommandType = CommandType.StoredProcedure;
                db.AddParameter("@Code", SqlDbType.VarChar, Code);
                db.AddParameter("@LeaseNo", SqlDbType.VarChar, LeaseNo);
                db.AddParameter("@JCode", SqlDbType.VarChar, 8, ParameterDirection.Output);
                db.ExecuteNonQueryWithParamOutput();

                object result = null;

                if (db.DbCommand.Parameters.Count > 0 && db.DbCommand.Parameters["@JCode"] != null)
                    result = db.DbCommand.Parameters["@JCode"] as SqlParameter;

                if (result != null && result != DBNull.Value)
                    JCode = (db.DbCommand.Parameters["@JCode"] as SqlParameter).Value.ToString();
            }

            db.DbCommand.Parameters.Clear();
            return JCode;
        }
        // End add - Anthony - 20200512

        public DateTime GetLastDayOnMonth(int m, int y)
        {
            SqlCommand sql = new SqlCommand();
            SqlConnection sqlconn = new SqlConnection();
            sqlconn.ConnectionString = IDS.DataAccess.SqlServer.GetSQLConnectionString();
            sqlconn.Open();
            sql.Connection = sqlconn;
            sql.CommandText = "Select dbo.LastDayOnMonth(" + m + "," + y + ")";
            sql.CommandType = CommandType.Text;
            object DT = sql.ExecuteScalar();
            return Convert.ToDateTime(DT);
        }
        #endregion


      
    }
}
