using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;

namespace IDS.Maintenance
{
    public class UserGroupAccessView
    {
        public string UserGroupCode { get; set; }
        public string UserGroupName { get; set; }
        public string MenuUrl { get; set; }
        public string MenuName { get; set; }
        public string MenuCode { get; set; }
        public string ProjectName { get; set; }
        public int Access { get; set; }
        public string AccessName { get; set; }

        public UserGroupAccessView()
        {
            Access = 0;
        }

        public static List<UserGroupAccessView> GetUserGroupAccessView(string groupCode, string projectCode)
        {
            List<UserGroupAccessView> result = new List<UserGroupAccessView>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "SelUserGroupAccessView";
                db.AddParameter("@ProjectCode", System.Data.SqlDbType.VarChar, GeneralHelper.StringToDBNull(projectCode));
                db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, GeneralHelper.StringToDBNull(groupCode));
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            UserGroupAccessView item = new UserGroupAccessView();
                            item.UserGroupCode = GeneralHelper.NullToString(dr["GroupCode"], "");
                            item.UserGroupName = GeneralHelper.NullToString(dr["GroupName"], "");
                            item.MenuName = GeneralHelper.NullToString(dr["MenuName"]);
                            item.MenuUrl = GeneralHelper.NullToString(dr["frmName"]);
                            item.ProjectName = GeneralHelper.NullToString(dr["ProjectName"]);
                            item.Access = GeneralHelper.NullToInt(dr["Akses"], 0);
                            item.AccessName = GeneralHelper.NullToString(dr["AksesName"]);
                            item.MenuCode = GeneralHelper.NullToString(dr["MenuCode"]);

                            result.Add(item);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return result;
        }

        public static void UpdateGroupAccess(IDS.Tool.PageActivity action, List<UserGroupAccessView> data)
        {
            try
            {
                if (data.Count > 0)
                {
                    using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
                    {
                        db.CommandText = "MNTUpdateGroupAccess";
                        db.CommandType = System.Data.CommandType.StoredProcedure;
                        db.Open();

                        db.BeginTransaction();

                        for (int i = 0; i < data.Count; i++)
                        {
                            db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, data[i].UserGroupCode);
                            db.AddParameter("@frmName", System.Data.SqlDbType.VarChar, data[i].MenuUrl);
                            db.AddParameter("@ProjectName", System.Data.SqlDbType.VarChar, data[i].ProjectName);
                            db.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, data[i].MenuCode == null ? DBNull.Value : (object)data[i].MenuCode);
                            db.AddParameter("@Akses", System.Data.SqlDbType.TinyInt, data[i].Access);
                            db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, (int)action);

                            db.ExecuteNonQuery();
                        }

                        db.CommitTransaction();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}