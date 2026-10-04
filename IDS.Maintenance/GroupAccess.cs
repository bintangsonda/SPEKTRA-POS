using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace IDS.Maintenance
{
    public class GroupAccess
    {
        public string GroupCode { get; set; }
        public int Access { get; set; }
        public string Url { get; set; }
        public string ControllerName { get; set; }
        public string MenuCode { get; set; }

        public GroupAccess()
        {
        }

        public int GetUserGropAccessToForm(string userGroupCode, string controllerName)
        {
            if (string.IsNullOrEmpty(userGroupCode))
                return 0;

            Dictionary<string, List<GroupAccess>> dictGroupAccess = IDS.Tool.InMemoryCache.GetInstance().GetOrSet<GroupAccess>("UserGroup.Access", userGroupCode, () => IDS.Maintenance.GroupAccess.GetGroupAccess(userGroupCode), 720);

            if (dictGroupAccess != null)
            {
                if (dictGroupAccess.ContainsKey(userGroupCode))
                {
                    System.Collections.Generic.List<GroupAccess> accesses = null;
                    if (!dictGroupAccess.TryGetValue(userGroupCode, out accesses))
                    {
                        accesses = dictGroupAccess[userGroupCode];

                        if (accesses != null)
                        {
                            int access = 0;
                            access = Convert.ToInt32(accesses.Where(x => x.ControllerName != null && x.ControllerName == controllerName).Select(x => x.Access).FirstOrDefault());

                            return access;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Retrieve Group Access by User Code
        /// </summary>
        /// <param name="userGroupCode">User Group Code</param>
        /// <returns>List of Group Access</returns>
        public static List<IDS.Maintenance.GroupAccess> GetGroupAccess(string userGroupCode)
        {
            List<GroupAccess> items = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MNTSelGroupAccess";
                db.AddParameter("@UserGroupCode", System.Data.SqlDbType.VarChar, userGroupCode);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        items = new List<GroupAccess>();

                        while (dr.Read())
                        {
                            GroupAccess item = new GroupAccess();
                            item.GroupCode = dr["GroupCode"] as string;
                            item.Url = dr["frmName"] as string;
                            item.ControllerName = dr["Controller"] as string;
                            item.Access = Convert.ToInt32(dr["Akses"]);
                            item.MenuCode = IDS.Tool.GeneralHelper.NullToString(dr["MenuCode"]);

                            items.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                    {
                        dr.Close();
                    }
                }

                db.Close();
            }

            return items;
        }

        public static System.Data.DataTable GetGroupAccess_(string projectName, string GroupCode)
        {
            System.Data.DataTable dt_ = new System.Data.DataTable();
            dt_.Clear();
            dt_.Columns.Add("GroupCode");
            dt_.Columns.Add("MenuName");
            dt_.Columns.Add("FrmName");
            dt_.Columns.Add("ProjectName");
            dt_.Columns.Add("Akses");
            dt_.Columns.Add("GroupName");

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                System.Text.StringBuilder b = new StringBuilder();
                b.AppendLine(" select A.GroupCode, W.MenuName, A.frmName,A.ProjectName,");
                b.AppendLine("(CASE WHEN Akses=0 THEN 'Not Set' ");
                b.AppendLine(" WHEN Akses =1 THEN 'Read'");
                b.AppendLine(" WHEN Akses=2 THEN 'Read & Write' ");
                b.AppendLine(" WHEN Akses=3 THEN 'Read & Delete'");
                b.AppendLine(" WHEN Akses=4 THEN 'Read, Write, Delete' END) AS Akses ");
                b.AppendLine(" , G.GroupName from MntGroupAccess A ");
                b.AppendLine("                        inner join MntGroupUser G ON A.GroupCode=G.GroupCode ");
                b.AppendLine("                          inner join MntWebMenu W ON A.frmName=W.MenuURL ");
                b.AppendLine("                         where ProjectName LIKE ISNULL(@ProjectName,'%') ");
                b.AppendLine("                       AND A.GroupCode=@GroupCode;");
                db.CommandText = b.ToString();
                if (string.IsNullOrEmpty(projectName))
                {
                    db.AddParameter("@ProjectName", System.Data.SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@ProjectName", System.Data.SqlDbType.VarChar, projectName);
                }
                db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, GroupCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dt_.Rows.Add(new object[] { dr["GroupCode"].ToString(), dr["MenuName"].ToString(), dr["frmName"].ToString(), dr["ProjectName"].ToString(), dr["Akses"].ToString(), dr["GroupName"].ToString() });
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return dt_;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetProjectMain()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "select ProjectName from MntMainProject order by ProjectName";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem acfcust = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            acfcust.Value = IDS.Tool.GeneralHelper.NullToString(dr["ProjectName"]);
                            acfcust.Text = IDS.Tool.GeneralHelper.NullToString(dr["ProjectName"]);
                            list.Add(acfcust);
                        }
                    }
                }

                db.Close();
            }
            return list;
        }

        public static int MultiSaveToMntGroupAccess(MultiSaveToMntGroupAccess m)
        {
            int result = 0;
            var GroupCode = m.Groupcode.ToString();
            var AksesNya = m.Akses;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                try
                {
                    foreach (var x in m.Data)
                    {
                        db.CommandText = "update MntGroupAccess set Akses=@Akses where frmName IN (@URL) AND GroupCode=@GroupCode";
                        db.CommandType = System.Data.CommandType.Text;
                        db.AddParameter("@Akses", System.Data.SqlDbType.Int, AksesNya);
                        db.AddParameter("@URL", System.Data.SqlDbType.VarChar, x.hfURL);
                        db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, GroupCode);
                        db.Open();
                        db.BeginTransaction();
                        result = db.ExecuteNonQuery();
                        db.CommitTransaction();
                    }

                }
                catch (Microsoft.Data.SqlClient.SqlException sex)
                {
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
                db.Close();
            }
            return result;
        }//UserExist

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetUserGroupAccessList()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> permis = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Not Set", Value = "0" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Read Only", Value = "1" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Create", Value = "2" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Edit", Value = "3" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Delete", Value = "5" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Create Edit", Value = "6" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Create Delete", Value = "7" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Create Edit Delete", Value = "8" });
            permis.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Create Edit Delete Approve", Value = "9" });

            return permis;
        }
    }

    public class MenuUrl
    {
        public string hfURL { get; set; }
    }

    public class MultiSaveToMntGroupAccess
    {
        public int Akses { get; set; }
        public string Groupcode { get; set; }
        public List<MenuUrl> Data { get; set; }
    }
}
