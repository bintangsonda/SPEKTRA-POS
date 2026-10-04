using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using iTextSharp.text.pdf;
//using System.Web.Http.Results;
using System.Net;
//using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace IDS.Tool
{
    public class Log
    {
        #region Variable Declaration

        string[] strArrOld;
        string[] strArrNew;
        string[] strArrKeys;
        string[] strArrField;
        // Alter by Anthony - 20150630
        //string strFieldList = "";
        //string strCommand = "";
        StringBuilder strFieldList;
        StringBuilder strCommand;
        // End of alter by Anthony - 20150630

        clsConnection ConLog = new clsConnection();
        #endregion

        #region Save The Saved Value Log to Database
        /// <Author Comment>
        /// Date Created            :   2009-10-08
        /// Created by              :   [S]
        /// Date Modified           :   -
        /// Modified by             :   -
        /// Description             :   To Save Insert And Delete Value
        /// </Author Comment>
        public void SaveSysLog(string strOldValue, string strNewValue, string strTable, string strKeyFieldList, string strUser)
        {
            try
            {
                SqlCommand comReadLog = new SqlCommand("EXEC GTRetrieveObj " + strTable, ConLog.Open());

                // Add - Anthony
                SqlCommand comSave = new SqlCommand();
                comSave.Connection = ConLog.Open();
                // End add - Anthony

                using (SqlDataReader drReadLog = comReadLog.ExecuteReader())
                {
                    if (drReadLog.HasRows)
                    {
                        // Add by Anthony - 20150630
                        strFieldList = new StringBuilder();
                        // End of add by Anthony - 20150630

                        while (drReadLog.Read())
                        {
                            // Alter by Anthony - 20150630
                            //strFieldList = strFieldList + Convert.ToString(drReadLog[0]) + ",";
                            strFieldList.Append(Convert.ToString(drReadLog[0]) + ",");
                            // End of alter by Anthony - 20150630
                        }

                        // Alter by Anthony - 20150630
                        //if (strFieldList.Trim().Length != 0)
                        //{
                        //    strFieldList = mdl.Left(strFieldList, strFieldList.Trim().Length - 1);
                        //}
                        if (strFieldList.ToString().Trim().Length != 0)
                        {
                            if (strFieldList.ToString().Trim().EndsWith(","))
                            {
                                strFieldList.Remove(strFieldList.ToString().Trim().Length - 1, strFieldList.ToString().Length - (strFieldList.ToString().Trim().Length - 1));
                            }
                        }
                        // End of alter by Anthony - 20150630

                        strArrField = strFieldList.ToString().Split(new Char[] { ',' });
                        strArrNew = strNewValue.Split(new Char[] { ',' });
                        strArrOld = strOldValue.Split(new Char[] { ',' });
                        strArrKeys = strKeyFieldList.Split(new Char[] { ',' });
                        drReadLog.Close();

                        // Add by Anthony - 20150630
                        strCommand = new StringBuilder();
                        // End of add by Anthony - 20150630

                        // Alter by Anthony - 20150630
                        #region Alter by Anthony - 20150630
                        //for (int xx = 0; xx < strArrField.Length; xx++)
                        //{
                        //    int xy = 0;
                        //    strCommand = "EXEC GTSetTransLog '" + strUser +
                        //                 "','" + DateTime.Now + "','" + Convert.ToString(DateTime.Now) +
                        //                 "'," + strTable +
                        //                 "," + strArrField[xx];
                        //    for (xy = 0; xy < strArrKeys.Length; xy++)
                        //    {
                        //        strCommand = strCommand + "," + strArrKeys[xy];
                        //        if (xy >= 5)
                        //        {
                        //            break;
                        //        }
                        //    }
                        //    if (xy != 6)
                        //    {
                        //        do
                        //        {
                        //            strCommand = strCommand + ",NULL";
                        //            xy++;
                        //        } while (xy < 6);
                        //    }

                        //    if (strArrNew.Length > 0)
                        //    {
                        //        strCommand = strCommand + ",'" + strArrNew[xx] + "'";
                        //    }
                        //    else
                        //    {
                        //        strCommand = strCommand + ",NULL";
                        //    }

                        //    if (strArrOld.Length == strArrField.Length)
                        //    {
                        //        strCommand = strCommand + ",'" + strArrOld[xx] + "'";
                        //    }
                        //    else
                        //    {
                        //        strCommand = strCommand + ",NULL";
                        //    }

                        //    SqlCommand comSave = new SqlCommand(strCommand, ConLog.Open());
                        //    comSave.ExecuteNonQuery();

                        //}
                        #endregion

                        for (int xx = 0; xx < strArrField.Length; xx++)
                        {
                            int xy = 0;
                            strCommand.Append("EXEC GTSetTransLog '" + strUser +
                                         "','" + DateTime.Now + "','" + Convert.ToString(DateTime.Now) +
                                         "'," + strTable +
                                         "," + strArrField[xx]);
                            for (xy = 0; xy < strArrKeys.Length; xy++)
                            {
                                strCommand.Append(",").Append(strArrKeys[xy]);
                                if (xy >= 5)
                                {
                                    break;
                                }
                            }
                            if (xy != 6)
                            {
                                do
                                {
                                    strCommand.Append(",NULL");
                                    xy++;
                                } while (xy < 6);
                            }

                            if (strArrNew.Length > 0)
                            {
                                strCommand.Append(",'").Append(strArrNew[xx].Replace("'", "\'\'")).Append("'");
                            }
                            else
                            {
                                strCommand.Append(",NULL");
                            }

                            if (strArrOld.Length == strArrField.Length)
                            {
                                strCommand.Append(",'").Append(strArrOld[xx].Replace("'", "\'\'")).Append("'");
                            }
                            else
                            {
                                strCommand.Append(",NULL");
                            }

                            // Alter - Anthony - dipindahkan ke atas menjadi satu, karena makan resource
                            //SqlCommand comSave = new SqlCommand(strCommand.ToString(), ConLog.Open());
                            comSave.CommandText = strCommand.ToString();
                            // End alter - Anthony

                            comSave.ExecuteNonQuery();

                            strCommand.Remove(0, strCommand.Length);
                        }
                        // End of alter by Anthony - 20150630
                    }
                }
            }
            catch (Exception exLog)
            {
                //WebMsgBox.Show(exLog.ToString());
                throw;
            }
            finally
            {
                ConLog.Close();
            }
        }
        #endregion

        #region Save Deleted Value Log To Database
        /// <Author Comment>
        /// Date Created            :   2009-10-09
        /// Created by              :   [S]
        /// Date Modified           :   -
        /// Modified by             :   -
        /// Description             :   To Save Delete Value
        /// </Author Comment>
        public void DelSysLog(string strKeyFieldList, string strTableName, string strUser)
        {
            string strCommand = "";
            int xx = 0;
            try
            {
                strArrKeys = strKeyFieldList.Split(new Char[] { ',' });
                strCommand = " EXEC GTSetTransLog '" + strUser + "','" + DateTime.Now +
                             "','" + DateTime.Now + "','" + strTableName +
                             "','" + strUser + "'";
                for (xx = 0; xx < strArrKeys.Length; xx++)
                {
                    strCommand = strCommand + ",'" + strArrKeys[xx] + "'";
                    if (xx >= 5)
                    {
                        break;
                    }
                }

                if (xx != 5)
                {
                    do
                    {
                        strCommand = strCommand + ",NULL";
                        xx++;
                    } while (xx < 6);
                }
                strCommand = strCommand + ",'[Delete]',NULL";
                SqlCommand ComDelLog = new SqlCommand(strCommand, ConLog.Open());
                ComDelLog.ExecuteNonQuery();
            }
            catch (Exception exDel)
            {
                //WebMsgBox.Show(Convert.ToString(exDel));
                throw;
            }
            finally
            {
                ConLog.Close();
            }
        }
        #endregion



        #region Generate Log Array
        public string GenLogArray(string SqlSelectStatement)
        {
            #region Alter - Anthony - 20180827
            // Alter - Anthony - 20180827
            //string strList = "";
            //SqlCommand comLogArr = new SqlCommand(SqlSelectStatement, ConLog.Open());
            //using (SqlDataReader drLog = comLogArr.ExecuteReader())
            //{
            //    if (drLog.HasRows)
            //    {
            //        while (drLog.Read())
            //        {
            //            for (int x = 0; x < drLog.FieldCount; x++)
            //            {
            //                strList = strList + drLog[x] + ",";
            //            }
            //            strList = mdl.Left(strList, strList.Length - 1);
            //        }
            //        drLog.Close();
            //    }

            //    ConLog.Close();
            //}
            //return strList;
            // End Alter - Anthony - 20180827
            #endregion

            StringBuilder sb = new StringBuilder();
            SqlCommand comLogArr = new SqlCommand(SqlSelectStatement, ConLog.Open());
            using (SqlDataReader drLog = comLogArr.ExecuteReader())
            {
                if (drLog.HasRows)
                {
                    while (drLog.Read())
                    {
                        for (int x = 0; x < drLog.FieldCount; x++)
                        {
                            sb.Append(drLog[x] == DBNull.Value ? string.Empty : Convert.ToString(drLog[x])).Append(",");
                        }

                        sb.Remove(sb.Length - 1, 1);
                    }
                    drLog.Close();
                }

                ConLog.Close();
            }

            return sb.ToString();
        }

        public string GenLogArray(string SqlSelectStatement, bool IsJFSqlConnection)
        {
            #region Alter - Anthony - 20180827
            // Alter - Anthony - 20180827
            //string strList = "";
            //SqlCommand comLogArr = new SqlCommand(SqlSelectStatement, ConLog.Open());
            //using (SqlDataReader drLog = comLogArr.ExecuteReader())
            //{
            //    if (drLog.HasRows)
            //    {
            //        while (drLog.Read())
            //        {
            //            for (int x = 0; x < drLog.FieldCount; x++)
            //            {
            //                strList = strList + drLog[x] + ",";
            //            }
            //            strList = mdl.Left(strList, strList.Length - 1);
            //        }
            //        drLog.Close();
            //    }

            //    ConLog.Close();
            //}
            //return strList;
            // End Alter - Anthony - 20180827
            #endregion

            StringBuilder sb = new StringBuilder();
            SqlCommand comLogArr = new SqlCommand(SqlSelectStatement, ConLog.Open(IsJFSqlConnection));
            using (SqlDataReader drLog = comLogArr.ExecuteReader())
            {
                if (drLog.HasRows)
                {
                    while (drLog.Read())
                    {
                        for (int x = 0; x < drLog.FieldCount; x++)
                        {
                            sb.Append(drLog[x] == DBNull.Value ? string.Empty : Convert.ToString(drLog[x])).Append(",");
                        }

                        sb.Remove(sb.Length - 1, 1);
                    }
                    drLog.Close();
                }

                ConLog.Close();
            }

            return sb.ToString();
        }


        // Add - Anthony - 20180828 - Untuk retrieve data Baru (Updated) didalam connection transaction ReadCommited
        public string GenLogArrayWithReadUnCommitedTransaction(string SqlSelectStatement)
        {
            StringBuilder sb = new StringBuilder();
            SqlCommand comLogArr = new SqlCommand(SqlSelectStatement, ConLog.Open());

            SqlTransaction trans;
            trans = ConLog.GetConnection.BeginTransaction(IsolationLevel.ReadUncommitted);
            comLogArr.Transaction = trans;

            using (SqlDataReader drLog = comLogArr.ExecuteReader())
            {
                if (drLog.HasRows)
                {
                    while (drLog.Read())
                    {
                        for (int x = 0; x < drLog.FieldCount; x++)
                        {
                            sb.Append(drLog[x] == DBNull.Value ? string.Empty : Convert.ToString(drLog[x])).Append(",");
                        }

                        sb.Remove(sb.Length - 1, 1);
                    }
                }

                if (!drLog.IsClosed)
                    drLog.Close();

                ConLog.Close();
            }

            return sb.ToString();
        }

        public string GenLogArrayWithReadUnCommitedTransaction(string SqlSelectStatement, bool isJFSQLConnection)
        {
            StringBuilder sb = new StringBuilder();
            SqlCommand comLogArr = new SqlCommand(SqlSelectStatement, ConLog.Open(isJFSQLConnection));

            SqlTransaction trans;
            trans = ConLog.GetConnection.BeginTransaction(IsolationLevel.ReadUncommitted);
            comLogArr.Transaction = trans;

            using (SqlDataReader drLog = comLogArr.ExecuteReader())
            {
                if (drLog.HasRows)
                {
                    while (drLog.Read())
                    {
                        for (int x = 0; x < drLog.FieldCount; x++)
                        {
                            sb.Append(drLog[x] == DBNull.Value ? string.Empty : Convert.ToString(drLog[x])).Append(",");
                        }

                        sb.Remove(sb.Length - 1, 1);
                    }
                }

                if (!drLog.IsClosed)
                    drLog.Close();

                ConLog.Close();
            }

            return sb.ToString();
        }
        #endregion

        public string GetIp()
        {
            //string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            var context = HttpContextHelper.Current;

            string ip = context?.Request?.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                //ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                ip = context?.Connection?.RemoteIpAddress?.ToString();
            }
            return ip;
        }

        public string GetComputerName(string clientIP)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(clientIP);
                return hostEntry.HostName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public void SaveSysLog1(string strOldValue, string strNewValue, string strTable, string strKeyFieldList, string strUser, string status)
        {
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    string[] List = strKeyFieldList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    string myIp = GetIp();
                    string hostName = Environment.MachineName;

                    cmd.CommandText = "MntUpdHistory";
                    cmd.AddParameter("@UserName", System.Data.SqlDbType.VarChar, strUser);
                    cmd.AddParameter("@TableName", System.Data.SqlDbType.VarChar, strTable);
                    cmd.AddParameter("@OldValue", System.Data.SqlDbType.VarChar, strOldValue);
                    cmd.AddParameter("@UpdateValue", System.Data.SqlDbType.VarChar, strNewValue);
                    cmd.AddParameter("@Status", System.Data.SqlDbType.VarChar, status);
                    cmd.AddParameter("@IpAddress", System.Data.SqlDbType.VarChar, myIp);
                    cmd.AddParameter("@HostName", System.Data.SqlDbType.VarChar, hostName);
                    for (int i = 0; i < List.Length; i++)
                    {
                        cmd.AddParameter("@KeyField" + (i + 1), System.Data.SqlDbType.VarChar, List[i]);
                    }
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Err");
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
        }

        public void SaveSysLog1(string strOldValue, string strNewValue, string strTable, string strKeyFieldList, string strUser, string status, bool isJF_SQLConnection)
        {
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(isJF_SQLConnection))
            {
                try
                {
                    string[] List = strKeyFieldList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    string myIp = GetIp();
                    string hostName = Environment.MachineName;

                    cmd.CommandText = "MntUpdHistory";
                    cmd.AddParameter("@UserName", System.Data.SqlDbType.VarChar, strUser);
                    cmd.AddParameter("@TableName", System.Data.SqlDbType.VarChar, strTable);
                    cmd.AddParameter("@OldValue", System.Data.SqlDbType.VarChar, strOldValue);
                    cmd.AddParameter("@UpdateValue", System.Data.SqlDbType.VarChar, strNewValue);
                    cmd.AddParameter("@Status", System.Data.SqlDbType.VarChar, status);
                    cmd.AddParameter("@IpAddress", System.Data.SqlDbType.VarChar, myIp);
                    cmd.AddParameter("@HostName", System.Data.SqlDbType.VarChar, hostName);
                    for (int i = 0; i < List.Length; i++)
                    {
                        cmd.AddParameter("@KeyField" + (i + 1), System.Data.SqlDbType.VarChar, List[i]);
                    }
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Err");
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
        }
    }
}

