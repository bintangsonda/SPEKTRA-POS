using IDS.DataAccess;
using IDS.Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCode
{
    public sealed class clsMenuRequest
    {
        public int RequestID { get; set; }
        public DateTime RequestDate { get; set; }
        public string BranchCode { get; set; }
        public string RequestUserId { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public string KeyField1 { get; set; }
        public string KeyField2 { get; set; }
        public string KeyField3 { get; set; }
        public string ActionName { get; set; }
        public string KeyField4 { get; set; }
        public int Action { get; set; }
        public string UserComment { get; set; }
        public int ApprovalStatus { get; set; }
        public string ApprovalBranch { get; set; }
        public string ApprovalGroup { get; set; }
        public string ApprovalUser { get; set; }
        public DateTime ApprovalDate { get; set; }
        public int RequestStatus { get; set; }
        public string Changes { get; set; }
        public string RequestStatusName { get; set; }

        public clsMenuRequest()
        {
        }
        public static List<clsMenuRequest> GetClsMenuRequests(string branch, string group,int approveStatus, int reqStatus, string menuurl) //Modif by Jeremi 31 Juli 2024 - Tambah menu url
        {
            List<clsMenuRequest> list = new List<clsMenuRequest>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SelRequestMenuApproval";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@ApprovalBranch", System.Data.SqlDbType.VarChar, branch);
                db.AddParameter("@UserGroupCode", System.Data.SqlDbType.VarChar, group);
                //if(reqStatus == -1)
                //{
                //    db.AddParameter("@RequestStatus", System.Data.SqlDbType.Int,  DBNull.Value );

                //}
                //else
                //{
                //    db.AddParameter("@RequestStatus", System.Data.SqlDbType.Int, reqStatus );

                //}
                //Modif by Jeremi 31 Juli 2024
                if (reqStatus == 0 || reqStatus == 1)
                {
                    db.AddParameter("@RequestStatus", System.Data.SqlDbType.Int, reqStatus);
                }
                else
                {
                    db.AddParameter("@RequestStatus", System.Data.SqlDbType.Int, DBNull.Value);
                }
                //db.AddParameter("@RequestStatus", System.Data.SqlDbType.Int, DBNull.Value);
                if (approveStatus == -1)
                {
                    db.AddParameter("@ApprovalStatus", System.Data.SqlDbType.Int, DBNull.Value);

                }
                else
                {
                    db.AddParameter("@ApprovalStatus", System.Data.SqlDbType.Int, approveStatus);

                }
                //Add by Jeremi 31 Juli 2024
                db.AddParameter("@MenuUrl", System.Data.SqlDbType.VarChar, menuurl);
                //End Jeremi
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            clsMenuRequest item = new clsMenuRequest();
                            item.RequestID = IDS.Tool.GeneralHelper.NullToInt(dr["RequestID"],0);
                            item.RequestDate =IDS.Tool.GeneralHelper.NullToDateTime(dr["RequestDate"], DateTime.Now);
                            item.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            item.RequestUserId = IDS.Tool.GeneralHelper.NullToString(dr["RequestUserId"]);
                            item.MenuName = IDS.Tool.GeneralHelper.NullToString(dr["MenuName"]);
                            item.MenuUrl = IDS.Tool.GeneralHelper.NullToString(dr["MenuUrl"]);
                            
                            item.KeyField1 = IDS.Tool.GeneralHelper.NullToString(dr["KeyField1"]);
                            item.KeyField2 = IDS.Tool.GeneralHelper.NullToString(dr["KeyField2"]);
                            item.KeyField3 = IDS.Tool.GeneralHelper.NullToString(dr["KeyField3"]);
                            item.KeyField4 = IDS.Tool.GeneralHelper.NullToString(dr["KeyField4"]);
                            item.ActionName = IDS.Tool.GeneralHelper.NullToString(dr["ActionName"]);
                            item.UserComment = IDS.Tool.GeneralHelper.NullToString(dr["UserComment"]);
                            item.ApprovalBranch = IDS.Tool.GeneralHelper.NullToString(dr["ApprovalBranch"]);
                            item.ApprovalGroup = IDS.Tool.GeneralHelper.NullToString(dr["ApprovalGroup"]);
                            item.ApprovalUser = IDS.Tool.GeneralHelper.NullToString(dr["ApprovalUser"]);
                            item.ApprovalDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["ApprovalDate"], DateTime.Now);
                            item.ApprovalStatus = IDS.Tool.GeneralHelper.NullToInt(dr["ApprovalStatus"], 0);
                            item.Changes = IDS.Tool.GeneralHelper.NullToString(dr["Changes"]);

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
        public int CreateRequest(string userGroup)
        {
            int result = 0;
            try
            {
                bool valid = true;
                StringBuilder sb = new StringBuilder();
                string error = "";

                if (valid == false)
                {
                    throw new Exception(error);
                }
                this.ApprovalStatus = 0;
                this.RequestStatus = 0;
                this.RequestDate = DateTime.Now;
                using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
                {
                    try
                    {
                        db.CommandText = "CreateUserMenuRequest";
                        db.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                        db.AddParameter("@RequestUserId", System.Data.SqlDbType.VarChar, RequestUserId);
                        db.AddParameter("@MenuUrl", System.Data.SqlDbType.VarChar, MenuUrl);
                        db.AddParameter("@KeyField1", System.Data.SqlDbType.VarChar, KeyField1);
                        db.AddParameter("@KeyField2", System.Data.SqlDbType.VarChar, KeyField2);
                        db.AddParameter("@KeyField3", System.Data.SqlDbType.VarChar, KeyField3);
                        db.AddParameter("@KeyField4", System.Data.SqlDbType.VarChar, KeyField4);
                        db.AddParameter("@Action", System.Data.SqlDbType.Int, Action);
                        db.AddParameter("@UserComment", System.Data.SqlDbType.VarChar, UserComment);
                        db.AddParameter("@ApprovalBranch", System.Data.SqlDbType.VarChar, ApprovalBranch);
                        db.AddParameter("@ApprovalGroup", System.Data.SqlDbType.VarChar, userGroup);
                        db.CommandType = System.Data.CommandType.StoredProcedure;
                        db.Open();
                        db.BeginTransaction();
                        result = db.ExecuteNonQuery();

                        //db.CommandText = "update mntwebmenu set menurequest = menurequest+1 where menuurl=@MenuUrl";
                        //db.AddParameter("@MenuUrl", System.Data.SqlDbType.VarChar, MenuUrl);
                        //db.CommandType = System.Data.CommandType.Text;
                        //result = db.ExecuteNonQuery();
                        db.CommitTransaction();

                    }
                    catch
                    {
                        if (db.DbCommand.Transaction != null)
                            db.RollbackTransaction();

                        throw;
                    }
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public static int GetApprovedRequestAndRequestStatusOpen(string userId, string branchCode, string menuUrl, object keyField1, object keyField2, object keyField3, object keyField4)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT TOP 1 * FROM MntMenuRequestApproval WHERE KeyField1=@keyField1 AND RequestUserId=@requestUser AND BranchCode=@branchCode AND MenuUrl=@menuUrl AND RequestStatus=0 AND ApprovalStatus=1";
                db.AddParameter("@keyField1", SqlDbType.VarChar, keyField1 == null ? DBNull.Value : keyField1);
                //db.AddParameter("@keyField2", SqlDbType.VarChar, keyField2 == null ? DBNull.Value : keyField2);
                //db.AddParameter("@keyField3", SqlDbType.VarChar, keyField3 == null ? DBNull.Value : keyField3);
                //db.AddParameter("@keyField4", SqlDbType.VarChar, keyField4 == null ? DBNull.Value : keyField4);

                db.AddParameter("@requestUser", SqlDbType.VarChar, userId);
                db.AddParameter("@branchCode", SqlDbType.VarChar, branchCode);
                db.AddParameter("@menuUrl", SqlDbType.VarChar, menuUrl);
                db.CommandType = CommandType.Text;
                db.Open();

                result = Convert.ToInt32(db.ExecuteScalar());

                db.Close();
            }

            return result;
        }

        public static bool CloseApprovedRequest(int requestID, string dataChanges)
        {
            bool result = false;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {

                db.CommandText = "UPDATE MntMenuRequestApproval SET RequestStatus=1, Changes=@dataChanges WHERE RequestID = @requestId AND RequestStatus=0 AND ApprovalStatus=1";
                db.AddParameter("@dataChanges", SqlDbType.VarChar, dataChanges);
                db.AddParameter("@requestID", SqlDbType.Int, requestID);
                db.CommandType = CommandType.Text;
                db.Open();

                result = Convert.ToBoolean(db.ExecuteScalar());

                db.Close();
            }

            return result;
        }
        public static int UpdateMntWebMenu(string MenuUrl)
        {
            int result = 0;
            using (SqlServer db = new SqlServer())
            {
                try
                {
                    db.CommandText = "UPDATE MNTWebMenu SET MenuRequest=MenuRequest-1 WHERE MenuUrl = @MenuUrl";
                    db.CommandType = CommandType.Text;
                    db.AddParameter("@MenuUrl", SqlDbType.VarChar, MenuUrl);
                    db.Open();
                    db.BeginTransaction();
                    result = db.ExecuteNonQuery();
                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    result = 0;
                }
                finally
                {
                    db.Close();
                }
            }
            return result;
        }
        public static int UpdateStatusApproval(int RequestId, int status, int req, string operatorID)
        {
            int result = 0;
            using (SqlServer db = new SqlServer())
            {
                try
                {
                    db.CommandText = "UPDATE MntMenuRequestApproval SET ApprovalStatus = @status,ApprovalUser = @operatorid, ApprovalDate = getdate(), RequestStatus = @req  WHERE RequestID = @RequestID";
                    db.CommandType = CommandType.Text;
                    db.AddParameter("@RequestID", SqlDbType.Int, RequestId);
                    db.AddParameter("@operatorid", SqlDbType.VarChar, operatorID);
                    db.AddParameter("@status", SqlDbType.TinyInt, status);
                    db.AddParameter("@req", SqlDbType.TinyInt, req);
                    db.Open();
                    db.BeginTransaction();
                    db.ExecuteNonQuery();
                    db.CommitTransaction();

                    result = 1;
                }
                catch (Exception ex)
                {
                    result = 0;
                }
                finally
                {
                    db.Close();
                }
            }
            return result;
        }
        //Add Jeremi 18 Mei 2024
        public static int GetApprovalStatus(string keyfield1, string Branch, string MenuUrl)
        {
            int requestMenu = -1;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ApprovalStatus FROM MntMenuRequestApproval WHERE requestID = (SELECT MAX(requestID) FROM MntMenuRequestApproval WHERE KeyField1 = @KeyField1 AND menuurl = @MenuUrl AND BranchCode = @branch)";
                db.AddParameter("@KeyField1", System.Data.SqlDbType.VarChar, keyfield1);
                db.AddParameter("@MenuUrl", System.Data.SqlDbType.VarChar, MenuUrl);
                db.AddParameter("@branch", System.Data.SqlDbType.VarChar, Branch);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        requestMenu = IDS.Tool.GeneralHelper.NullToInt(dr["ApprovalStatus"], -1);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return requestMenu;
        }
        public string GetParentGroup(string UserGroup)
        {
            string parent = "";
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT GroupParent FROM MntGroupUser WHERE GroupCode = @UserGroup";
                db.AddParameter("@UserGroup", System.Data.SqlDbType.VarChar, UserGroup);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        parent = IDS.Tool.GeneralHelper.NullToString(dr["GroupParent"], "");
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return parent;
        }

        //Add Jeremi 27 Mei 2024
        public int DelApproval(string KeyField1)
        {
            int del = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    db.Open();
                    db.BeginTransaction();

                    db.CommandText = "DELETE FROM MntMenuRequestApproval WHERE KeyField1 = @KeyField1;";
                    db.CommandType = CommandType.Text;
                    db.AddParameter("@KeyField1", SqlDbType.VarChar, KeyField1);
                    del = db.ExecuteNonQuery();

                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    if (db.Transaction != null)
                    {
                        db.Transaction.Rollback();
                    }
                }
                finally
                {
                    db.Close();
                }

            }

            return del;
        }

        public static string GetMenuURL(int reqID)
        {
            string parent = "";
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT MenuURL FROM MntGroupUser WHERE requestid = @RequestID";
                db.AddParameter("@RequestID", System.Data.SqlDbType.VarChar, reqID);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        parent = IDS.Tool.GeneralHelper.NullToString(dr["MenuURL"], "");
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return parent;
        }
        //Add by Jeremi 2 Agustus 2024
        public static string CheckUserApproval(int reqID)
        {
            string user = "";
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ApprovalUser FROM MntMenuRequestApproval WHERE requestid = @RequestID";
                db.AddParameter("@RequestID", System.Data.SqlDbType.VarChar, reqID);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        user = IDS.Tool.GeneralHelper.NullToString(dr["ApprovalUser"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return user;
        }
    }
}
