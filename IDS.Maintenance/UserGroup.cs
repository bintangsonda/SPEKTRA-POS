using IDS.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace IDS.Maintenance
{
    public class UserGroup
    {
        [Display(Name = "User Group Code")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Group Code is required")]
        [MaxLength(20), StringLength(20)]
        public string GroupCode { get; set; }

        [Display(Name = "User Group Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Group Name is required")]
        [MaxLength(50), StringLength(50)]
        public string GroupName { get; set; }
        [Display(Name = "User Group Parent")]
        [MaxLength(50), StringLength(50)]
        public string GroupParent { get; set; }
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

        public UserGroup()
        {
        }

        public UserGroup(string code, string name)
        {
            GroupCode = code;
            GroupName = name;
        }

        /// <summary>
        /// Mengambil user userGroup berdasarkan parameter kode group user
        /// </summary>
        /// <param name="userGroupCode">Kode Group User</param>
        /// <returns></returns>
        public static UserGroup GetUserGroup(string userGroupCode)
        {
            UserGroup group = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelUserGroup";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, userGroupCode);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        group = new UserGroup();
                        group.GroupCode = dr["GroupCode"] as string;
                        group.GroupName = dr["GroupName"] as string;
                        group.EntryUser = dr["EntryUser"] as string;
                        //group.Level = Tool.GeneralHelper.NullToString(dr["Level"]);
                        //group.Parent = Tool.GeneralHelper.NullToString(dr["Parent"]);
                        //group.EntryDate = dConvert.ToDateTime(dr["EntryDate"]);
                        group.EntryDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["EntryDate"], DateTime.Now);
                        group.OperatorID = dr["OperatorID"] as string;
                        //group.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                        group.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.Now);

                        group.GroupParent = dr["GroupParent"] as string;
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
            }

            return group;
        }

        public static List<UserGroup> GetUserGroup()
        {
            List<IDS.Maintenance.UserGroup> list = new List<UserGroup>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelUserGroup";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            UserGroup userGroup = new UserGroup();
                            userGroup.GroupCode = dr["GroupCode"] as string;
                            userGroup.GroupName = dr["GroupName"] as string;
                            userGroup.GroupParent = Tool.GeneralHelper.NullToString(dr["GroupParent"]);
                            userGroup.EntryUser = Tool.GeneralHelper.NullToString(dr["ENTRYUSER"]);
                            userGroup.EntryDate = Tool.GeneralHelper.NullToDateTime(dr["ENTRYDATE"], DateTime.Now);
                            userGroup.OperatorID = Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            //userGroup.Level = Tool.GeneralHelper.NullToString(dr["Level"]);
                            //userGroup.Parent = Tool.GeneralHelper.NullToString(dr["Parent"]);
                            userGroup.LastUpdate = Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.Now);

                            list.Add(userGroup);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetUserGroupForDatasource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> items = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelUserGroup";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            item.Value = Tool.GeneralHelper.NullToString(dr["GroupCode"]);
                            item.Text = Tool.GeneralHelper.NullToString(dr["GroupName"]);

                            items.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return items;
        }

        public int InsUpDelUserGroup(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    cmd.CommandText = "MntUserGroup";
                    cmd.AddParameter("@type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, GroupCode);
                    cmd.AddParameter("@GroupName", System.Data.SqlDbType.VarChar, GroupName);
                    cmd.AddParameter("@GroupParent", System.Data.SqlDbType.VarChar, GroupParent);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@ENTRYUSER", System.Data.SqlDbType.VarChar, OperatorID);
                    //cmd.AddParameter("@Level", System.Data.SqlDbType.VarChar, Level);
                    //cmd.AddParameter("@Parent", System.Data.SqlDbType.VarChar, Parent);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();
                    //Log log = new Log();
                    //string oldData = log.GenLogArray("SELECT * FROM MntGroupUser WHERE GroupCode =  '" + GroupCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES ");
                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    //string newData = log.GenLogArray("SELECT * FROM MntGroupUser WHERE GroupCode =  '" + GroupCode + "' FOR JSON AUTO, INCLUDE_NULL_VALUES ");
                    //var status = "";
                    //if (ExecCode == 1)
                    //{
                    //    status = "INSERT";
                    //}
                    //else
                    //{
                    //    status = "UPDATE";
                    //}
                    //log.SaveSysLog1(oldData, newData, "MntGroupUser", GroupCode, OperatorID, status);
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("User Group code is already exists. Please choose other User Group code.");
                        default:
                            throw;
                    }
                }
                catch (Exception ex)
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

        public int InsUpDelUserGroup(int ExecCode, string[] data, string op)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    cmd.CommandText = "MntUserGroup";
                    cmd.Open();
                    cmd.BeginTransaction();
                    //Log log = new Log();
                    string[] oldData = new string[data.Length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "MntUserGroup";
                        cmd.AddParameter("@type", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        //oldData[i] = log.GenLogArray("SELECT * FROM MntGroupUser WHERE GroupCode =  '" + data[i] + "' FOR JSON AUTO, INCLUDE_NULL_VALUES ");
                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                    //for (int i = 0; i < oldData.Length; i++)
                    //{
                    //    log.SaveSysLog1(oldData[i], "", "MntGroupUser", data[i], op, "DELETE");
                    //}
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("User Group Code is already exists. Please choose other User Group Code.");
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

        public override string ToString()
        {
            return Convert.ToString(GroupName);
        }

        public static List<string> GetDataForEditGroupArea(string groupCode)
        {
            List<string> list = new List<string>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblMstAreaGroup WHERE GroupCode=@GroupCode";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@GroupCode", SqlDbType.VarChar, groupCode);
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string item = Tool.GeneralHelper.NullToString(dr["AreaCode"]);
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

        public static int InsUpDelGroupArea(string groupCode, List<string> areaList, string currentUser, ref string message)
        {
            int result = 0;

            string operatorID = string.Empty;
            string myIp = string.Empty;
            if (HttpContextHelper.Current != null && HttpContextHelper.Current.Session != null && HttpContextHelper.Current.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID) != null)
            {
                operatorID = HttpContextHelper.Current.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID).ToString();
                myIp = HttpContextHelper.GetIp();
            }

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    if (!string.IsNullOrEmpty(operatorID))
                    {
                        cmd.CommandText = "EXEC sp_set_session_context 'AppUser', @User";
                        cmd.AddParameter("@User", System.Data.SqlDbType.VarChar, Tool.GeneralHelper.StringToDBNull(operatorID));
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.BeginTransaction();
                        cmd.ExecuteNonQuery();
                        cmd.CommitTransaction();
                    }
                    if (!string.IsNullOrEmpty(myIp))
                    {
                        cmd.CommandText = "EXEC sp_set_session_context 'IPUser', @IPUser";
                        cmd.AddParameter("@IPUser", System.Data.SqlDbType.VarChar, Tool.GeneralHelper.StringToDBNull(myIp));
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.BeginTransaction();
                        cmd.ExecuteNonQuery();
                        cmd.CommitTransaction();
                        cmd.ClearParameter();
                    }

                    cmd.BeginTransaction();

                    cmd.CommandText = "tblMstAreaGroupInsUpDel";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, groupCode);
                    cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, DBNull.Value);
                    cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 3);
                    result = cmd.ExecuteNonQuery();

                    foreach (var area in areaList)
                    {
                        cmd.CommandText = "tblMstAreaGroupInsUpDel";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, groupCode);
                        cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, area);
                        cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, currentUser);
                        cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 1);

                        result = cmd.ExecuteNonQuery();
                        if (result <= 0)
                        {
                            if (cmd.Transaction != null)
                                cmd.RollbackTransaction();
                            message = "Something is wrong during updating data";
                            return result;
                        }
                    }


                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            message = "This data is already exists! Please input with another Area Code";
                            throw new Exception("This data is already exists! Please input with another Area Code");
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
    }
}