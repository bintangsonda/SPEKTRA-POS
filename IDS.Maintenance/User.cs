using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Maintenance
{
    public enum UserStatus : int
    {
        Active = 1,
        InActive = 0
    }

    public class User
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "User ID is required")]
        [MaxLength(20), StringLength(20)]
        public string UserID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "User ID is required")]
        [MaxLength(20), StringLength(20)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword
        {
            get;
            set;
        }
        [Required(ErrorMessage = "email is required")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string EmailAddress { get; set; }
        public UserGroup UserGroup { get; set; }

        public string ExpiredCode { get; set; }
        [Required]
        public string SecurityCode { get; set; }
        [Required]
        public string SecurityAnswer { get; set; }
        public int Akumulasi { get; set; }
        public UserStatus Status { get; set; }
        public IDS.GeneralTable.Branch Branch { get; set; }

        public string EntryUser { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryDateString { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }

        //public string AreaCode { get; set; }
        //public string DepartmentCode { get; set; }

        public User()
        {

        }

        public static User UserLogin(string userID, string password)
        {
            Maintenance.User user = null;

            Tool.clsCryptho crypt = new Tool.clsCryptho();

            string passwd = crypt.Encrypt(password, "ids");

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MNTUserLogin";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@User", System.Data.SqlDbType.VarChar, userID);
                db.AddParameter("@Password", System.Data.SqlDbType.VarChar, password);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        user = new User();
                        user.UserID = dr["UserId"] as string;
                        user.Password = dr["Password"] as string;
                        user.Branch = GeneralTable.Branch.GetBranch(dr["BranchCode"] as string);
                        user.UserName = dr["UserName"] as string;
                        user.UserGroup = Maintenance.UserGroup.GetUserGroup(dr["GroupCode"] as string);
                        user.Status = (UserStatus)Convert.ToInt32(dr["Status"]);
                        user.EmailAddress = dr["EmailAddress"] as string;
                        //user.SecurityCode = dr["SecurityCode"] as string;
                        //user.SecurityAnswer = dr["SecurityAnswer"] as string;
                        //user.Akumulasi = dr["Akumulasi"] == DBNull.Value ? 0 : Convert.ToInt16(dr["Akumulasi"]);
                        user.ExpiredCode = dr["expiredCode"] as string;
                        //user.EntryUser = dr["EntryUser"] as string;
                        //user.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                    }

                    if (!dr.IsClosed)
                    {
                        dr.Close();
                    }
                }

                db.Close();
            }

            return user;
        }

        public static IList<User> GetUser()
        {
            List<User> users = new List<User>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelUser";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@UserID", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            User user = new User();
                            user.UserID = dr["UserId"] as string;
                            user.Password = dr["Password"] as string;
                            user.Branch = new GeneralTable.Branch();
                            user.Branch.BranchCode = Tool.GeneralHelper.NullToString(dr["BranchCode"]);

                            user.UserName = dr["UserName"] as string;

                            user.UserGroup = new Maintenance.UserGroup();
                            user.UserGroup.GroupCode = Tool.GeneralHelper.NullToString(dr["GroupCode"]);

                            user.Status = (UserStatus)Convert.ToInt32(dr["Status"]);
                            user.EmailAddress = dr["EmailAddress"] as string;
                            user.SecurityCode = dr["SecurityCode"] as string;
                            user.SecurityAnswer = dr["SecurityAnswer"] as string;
                            user.Akumulasi = dr["Akumulasi"] == DBNull.Value ? 0 : Convert.ToInt16(dr["Akumulasi"]);
                            user.ExpiredCode = dr["expiredCode"] as string;
                            user.EntryUser = dr["EntryUser"] as string;
                            user.EntryDate = Tool.GeneralHelper.NullToDateTime(dr["EntryDate"],DateTime.MinValue);
                            user.EntryDateString = user.EntryDate == DateTime.MinValue ? "" : user.EntryDate.ToString("dd-MMM-yyyy");
                            user.OperatorID = Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            user.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            users.Add(user);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
            }

            return users;
        }

        public static IList<User> GetUserForGrid()
        {
            List<User> users = new List<User>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelUser";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@UserID", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            User user = new User();
                            user.UserID = dr["UserId"] as string;
                            user.Password = dr["Password"] as string;

                            user.Branch = new GeneralTable.Branch();
                            user.Branch.BranchCode = dr["branchcode"] as string;

                            user.UserName = dr["UserName"] as string;
                            user.UserGroup = new UserGroup();
                            user.UserGroup.GroupCode = Tool.GeneralHelper.NullToString(dr["GroupCode"]);

                            user.Status = (UserStatus)Convert.ToInt32(dr["Status"]);
                            user.EmailAddress = dr["EmailAddress"] as string;
                            //user.SecurityCode = dr["SecurityCode"] as string;
                            //user.SecurityAnswer = dr["SecurityAnswer"] as string;
                            //user.Akumulasi = dr["Akumulasi"] == DBNull.Value ? 0 : Convert.ToInt16(dr["Akumulasi"]);
                            user.ExpiredCode = dr["expiredCode"] as string;
                            //user.EntryUser = dr["EntryUser"] as string;
                            //user.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                            user.OperatorID = Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            user.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);

                            users.Add(user);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
            }

            return users;
        }

        public static User GetUser(string userID)
        {
            User user = new User();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelUser";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@UserID", System.Data.SqlDbType.VarChar, userID);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 2);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        user.UserID = dr["UserId"] as string;
                        user.Password = dr["Password"] as string;

                        user.Branch = new IDS.GeneralTable.Branch();
                        user.Branch.BranchCode = Tool.GeneralHelper.NullToString(dr["BranchCode"]);

                        user.UserName = dr["UserName"] as string;

                        user.UserGroup = new Maintenance.UserGroup();
                        user.UserGroup.GroupCode = Tool.GeneralHelper.NullToString(dr["GroupCode"]);

                        user.Status = (UserStatus)Convert.ToInt32(dr["Status"]);
                        user.EmailAddress = dr["EmailAddress"] as string;
                        user.SecurityCode = dr["SecurityCode"] as string;
                        user.SecurityAnswer = dr["SecurityAnswer"] as string;
                        user.Akumulasi = dr["Akumulasi"] == DBNull.Value ? 0 : Convert.ToInt16(dr["Akumulasi"]);
                        user.ExpiredCode = dr["expiredCode"] as string;

                        //user.AreaCode = Tool.GeneralHelper.NullToString(dr["AreaCode"]);
                        //user.DepartmentCode = Tool.GeneralHelper.NullToString(dr["DepartmentCode"]);
                        //user.EntryUser = dr["EntryUser"] as string;
                        //user.EntryDate = Convert.ToDateTime(dr["EntryDate"]);
                        //user.OperatorID = Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                        //user.LastUpdate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
            }

            return user;
        }

        public static void UserPassChange(string userid, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                    throw new Exception("Password can not be blank");

                if (string.IsNullOrWhiteSpace(userid))
                    throw new Exception("Invalid User ID");

                UpdatePassword(newPassword, userid);
            }
            catch
            {
                throw;
            }

        }

        public static bool UserPassChange(string userid, string oldpass, string newpass, string newpaaconfirms)
        {
            if (PassMatch(userid, oldpass))
            {
                UpdatePassword(newpass, userid);
                return true;
            }
            return false;

        }

        private static bool PassMatch(string userid, string pass)
        {
            bool exist_ = false;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "select userid,password from MntUser where userid=@UserId and Password=@Password";
                db.AddParameter("@UserId", System.Data.SqlDbType.VarChar, userid);
                db.AddParameter("@Password", System.Data.SqlDbType.VarChar, pass);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        exist_ = true;
                    }
                    else
                    {
                        exist_ = false;
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return exist_;
        }//UserExist

        private static void UpdatePassword(string PASS, string UserId)
        {
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "update MntUser set password=@Password where userid=@UserId";
                db.AddParameter("@Password", System.Data.SqlDbType.VarChar, PASS);
                db.AddParameter("@UserId", System.Data.SqlDbType.VarChar, UserId);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.BeginTransaction();
                int result = db.ExecuteNonQuery();
                db.CommitTransaction();
                db.Close();
            }
        }

        public static System.Data.DataTable GetWebMenuMaster()
        {
            System.Data.DataTable dt_ = new System.Data.DataTable();
            dt_.Clear();
            dt_.Columns.Add("MenuCode");
            dt_.Columns.Add("GroupCode");
            dt_.Columns.Add("FrmName");
            dt_.Columns.Add("ProjectName");
            dt_.Columns.Add("Akses");
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "select MenuCode,GroupCode,frmName,ProjectName,Akses from mntGroupAccess";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dt_.Rows.Add(new object[] { dr["MenuCode"].ToString(), dr["GroupCode"].ToString(), dr["frmName"].ToString(), dr["ProjectName"].ToString(), dr["Akses"].ToString() });
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return dt_;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetMenuListForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "select ProjectName from MntMainProject union select NULL";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem acfcust = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            acfcust.Value = IDS.Tool.GeneralHelper.NullToString(dr["ProjectName"]);
                            acfcust.Text = IDS.Tool.GeneralHelper.NullToString(dr["ProjectName"]);
                            list.Add(acfcust);
                        }


                    }
                }
                db.Close();
            }
            return list;
        }//GetMenuListForDataSource

        public static void GetUserFromBranch(string Branch, System.Data.DataTable dt)
        {

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("SELECT userid as username, username as fullname,");
                sb.AppendLine("emailaddress, groupcode, ENTRYDATE, expiredcode,");
                sb.AppendLine("akumulasi, status, branchcode");
                sb.AppendLine("FROM MntUser");
                sb.AppendLine("WHERE branchcode LIKE ISNULL (@Branch,'%') ORDER BY userid;");

                db.CommandText = sb.ToString();
                db.CommandType = System.Data.CommandType.Text;
                if (string.IsNullOrEmpty(Branch))
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, Branch);
                }
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dt.Rows.Add(new object[] { dr["username"].ToString(), dr["fullname"].ToString(), dr["emailaddress"].ToString(), dr["groupcode"].ToString(), dr["ENTRYDATE"].ToString(), dr["expiredcode"].ToString(), dr["akumulasi"].ToString(), dr["status"].ToString() });
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetSecurityCode()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> groups = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "SELECT SecurityCode, SecurityDesc FROM MntSec ORDER BY SecurityDesc";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            item.Value = Tool.GeneralHelper.NullToString(dr["SecurityCode"]);
                            item.Text = Tool.GeneralHelper.NullToString(dr["SecurityDesc"]);
                            groups.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
            }
            return groups;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetExpCode()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> RP = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "7 Day", Value = "7D" });
            RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "14 Day", Value = "14D" });
            RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "1 Month", Value = "1M" });
            return RP;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Getgroupcode()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> groups = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "SELECT groupcode, groupname FROM mntgroupuser ORDER BY groupcode";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem item = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            item.Value = Tool.GeneralHelper.NullToString(dr["groupcode"]);
                            item.Text = Tool.GeneralHelper.NullToString(dr["groupname"]);
                            groups.Add(item);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
            }

            return groups;
        }

        public static bool UserExist(string userID)
        {
            bool exist_ = false;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "SELECT userid FROM MntUser WHERE userid = @UserId";
                db.AddParameter("@UserId", System.Data.SqlDbType.VarChar, userID);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        exist_ = true;
                    }
                    else
                    {
                        exist_ = false;
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return exist_;
        }//UserExist

        public static bool SaveUser(string userID, string UserName, string Password, string EmailAddress, string GroupCode, System.DateTime createdDate, string expiredCode, string SecurityCode, string SecurityAnswer, int Akumulasi, bool Status, string BranchCode, string OperatorID)
        {
            int result = 0;
            bool success = false;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                try
                {
                    db.CommandText = "MntSaveUser";
                    db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                    db.AddParameter("@UserId", System.Data.SqlDbType.VarChar, userID);
                    db.AddParameter("@UserName", System.Data.SqlDbType.VarChar, UserName);
                    db.AddParameter("@Password", System.Data.SqlDbType.VarChar, Password);
                    db.AddParameter("@Email", System.Data.SqlDbType.VarChar, EmailAddress);
                    db.AddParameter("@Group", System.Data.SqlDbType.VarChar, GroupCode);
                    db.AddParameter("@Date", System.Data.SqlDbType.DateTime, createdDate);
                    db.AddParameter("@exp", System.Data.SqlDbType.VarChar, expiredCode);
                    db.AddParameter("@SCode", System.Data.SqlDbType.VarChar, SecurityCode);
                    db.AddParameter("@SAnsw", System.Data.SqlDbType.VarChar, SecurityAnswer);
                    db.AddParameter("@Akumulasi", System.Data.SqlDbType.TinyInt, Akumulasi);
                    db.AddParameter("@Status", System.Data.SqlDbType.Bit, Status);
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, BranchCode);
                    db.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    db.AddParameter("@ENTRYUSER", System.Data.SqlDbType.VarChar, OperatorID);

                    db.CommandType = System.Data.CommandType.StoredProcedure;
                    db.Open();
                    db.BeginTransaction();
                    result = db.ExecuteNonQuery();
                    db.CommitTransaction();
                    success = true;
                }
                catch (SqlException sex)
                {
                    success = false;
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
                db.Close();
            }
            return success;
        }//UserExist

        public static bool UpdateUser(string userID, string UserName, string Password, string EmailAddress, string GroupCode, System.DateTime createdDate, string expiredCode, string SecurityCode, string SecurityAnswer, string Akumulasi, bool Status, string BranchCode, string AMGroupCode, string ENTRYUSER)
        {
            int result = 0;
            bool success = false;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                try
                {
                    db.CommandText = "MntSaveUser";
                    db.CommandType = System.Data.CommandType.StoredProcedure;
                    db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                    db.AddParameter("@userid", System.Data.SqlDbType.VarChar, userID);
                    db.AddParameter("@username", System.Data.SqlDbType.VarChar, UserName);
                    db.AddParameter("@password", System.Data.SqlDbType.VarChar, Password);
                    db.AddParameter("@email", System.Data.SqlDbType.VarChar, EmailAddress);
                    db.AddParameter("@group", System.Data.SqlDbType.VarChar, GroupCode);
                    db.AddParameter("@date", System.Data.SqlDbType.DateTime, createdDate);
                    db.AddParameter("@exp", System.Data.SqlDbType.VarChar, expiredCode);
                    db.AddParameter("@scode", System.Data.SqlDbType.VarChar, SecurityCode);
                    db.AddParameter("@sansw", System.Data.SqlDbType.VarChar, SecurityAnswer);
                    db.AddParameter("@akumulasi", System.Data.SqlDbType.TinyInt, int.Parse(Akumulasi));
                    db.AddParameter("@status", System.Data.SqlDbType.Bit, Status);
                    db.AddParameter("@branch", System.Data.SqlDbType.VarChar, BranchCode);
                    db.AddParameter("@LastUpdate", System.Data.SqlDbType.DateTime, System.DateTime.Now);
                    db.AddParameter("@AMGroupCode", System.Data.SqlDbType.VarChar, AMGroupCode);
                    db.AddParameter("@ENTRYUSER", System.Data.SqlDbType.VarChar, ENTRYUSER);
                    db.Open();
                    db.BeginTransaction();
                    result = db.ExecuteNonQuery();
                    db.CommitTransaction();
                    success = true;
                }
                catch (SqlException sex)
                {
                    success = false;
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
                db.Close();
            }
            return success;
        }//UserExist

        public static bool DeleteUserId(string userID)
        {
            int result = 0;
            bool success = false;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                try
                {
                    db.CommandText = "DELETE FROM MntUser WHERE UserId=@UserId";
                    db.CommandType = System.Data.CommandType.Text;
                    db.AddParameter("@userid", System.Data.SqlDbType.VarChar, userID);
                    db.Open();
                    db.BeginTransaction();
                    result = db.ExecuteNonQuery();
                    db.CommitTransaction();
                    success = true;
                }
                catch (SqlException sex)
                {
                    success = false;
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
                db.Close();
            }
            return success;
        }//UserExist

        public static System.Data.DataTable GetUserFromId(string userid)
        {
            System.Data.DataTable dt_ = new System.Data.DataTable();
            dt_.Clear();
            dt_.Columns.Add("userid");
            dt_.Columns.Add("username");
            dt_.Columns.Add("email");
            dt_.Columns.Add("group");
            dt_.Columns.Add("datecreated");
            dt_.Columns.Add("expired");
            dt_.Columns.Add("accum");
            dt_.Columns.Add("active");
            dt_.Columns.Add("scode");
            dt_.Columns.Add("branch");
            dt_.Columns.Add("sansw");
            IDS.Maintenance.User.GetUser(userid, dt_);
            return dt_;
        }

        private static void GetUser(string userid, System.Data.DataTable dt)
        {

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("SELECT userid as username, username as fullname,");
                sb.AppendLine("emailaddress, groupcode, ENTRYDATE, expiredcode,");
                sb.AppendLine("akumulasi, status, branchcode, SecurityCode, SecurityAnswer");
                sb.AppendLine("FROM MntUser");
                sb.AppendLine("WHERE userid =@userid ORDER BY userid;");

                db.CommandText = sb.ToString();
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@userid", System.Data.SqlDbType.VarChar, userid);
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        dt.Rows.Add(new object[] { dr["username"].ToString(), dr["fullname"].ToString(), dr["emailaddress"].ToString(), dr["groupcode"].ToString(), dr["ENTRYDATE"].ToString(), dr["expiredcode"].ToString(), dr["akumulasi"].ToString(), dr["status"].ToString(), dr["SecurityCode"].ToString(), dr["branchcode"].ToString(), dr["SecurityAnswer"].ToString() });
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
        }


        public int InsUpDelUser(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    cmd.CommandText = "MntSaveUser";
                    cmd.AddParameter("@type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@userid", System.Data.SqlDbType.VarChar, UserID);
                    cmd.AddParameter("@username", System.Data.SqlDbType.VarChar, UserName);
                    cmd.AddParameter("@password", System.Data.SqlDbType.VarChar, Password);
                    cmd.AddParameter("@email", System.Data.SqlDbType.VarChar, EmailAddress);
                    cmd.AddParameter("@group", System.Data.SqlDbType.VarChar, UserGroup.GroupCode);
                    cmd.AddParameter("@exp", System.Data.SqlDbType.VarChar, ExpiredCode);
                    cmd.AddParameter("@scode", System.Data.SqlDbType.VarChar, SecurityCode);
                    cmd.AddParameter("@sansw", System.Data.SqlDbType.VarChar, SecurityAnswer);
                    cmd.AddParameter("@akumulasi", System.Data.SqlDbType.VarChar, Akumulasi);
                    cmd.AddParameter("@branch", System.Data.SqlDbType.VarChar, Branch.BranchCode);

                    cmd.AddParameter("@status", System.Data.SqlDbType.Bit, Status);

                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@ENTRYUSER", System.Data.SqlDbType.VarChar, OperatorID);
                    //cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                    //cmd.AddParameter("@DepartmentCode", System.Data.SqlDbType.VarChar, DepartmentCode);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("User ID is already exists. Please Click Button Add New.");
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
        public int InsUpDelUser(int ExecCode, string[] data)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    cmd.CommandText = "MntSaveUser";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "MntSaveUser";
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 2);
                        cmd.AddParameter("@userid", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
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
                            throw new Exception("User Id is already exists. Please choose other User Id.");
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

        //Add By Renaldi 22 April 2025
        public static bool IsUserHasSubordinate(string UserGroup)
        {
            bool result = false;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select count(*) from MntUser where GroupCode in (select GroupCode from MntGroupUser where GroupParent=@UserGroup)";
                db.AddParameter("@UserGroup", System.Data.SqlDbType.VarChar, UserGroup);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                object resultExecute = db.ExecuteScalar();
                if (resultExecute != null && resultExecute != DBNull.Value)
                {
                    int count = Tool.GeneralHelper.NullToInt(resultExecute, 0);
                    if (count > 0)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }
        //End Add

        public static string GetUserGroupCode(string userId)
        {
            string result = "";

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select GroupCode from MntUser where UserId=@UserId";
                db.AddParameter("@UserId", System.Data.SqlDbType.VarChar, userId);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                object resultExecute = db.ExecuteScalar();
                if (resultExecute != null && resultExecute != DBNull.Value)
                {
                    result = resultExecute.ToString();
                }
            }

            return result;
        }
    }
}