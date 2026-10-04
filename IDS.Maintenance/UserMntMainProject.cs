using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Maintenance
{
    public class UserMntMainProject
    {

        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        [System.ComponentModel.DataAnnotations.Required]
        public string ProjectName { get; set; }



        [System.ComponentModel.DataAnnotations.MaxLength(20)]
        [System.ComponentModel.DataAnnotations.Required]
        public string LogUser { get; set; }


        public DateTime LastUpdate { get; set; }
        public UserMntMainProject()
        {

        }

        public static System.Data.DataTable MntMainProjectDataTable()
        {
            System.Data.DataTable dt_ = new System.Data.DataTable();
            dt_.Clear();
            dt_.Columns.Add("ProjectName");
            dt_.Columns.Add("OPERATORID");
            dt_.Columns.Add("LastUpdate");
            dt_.Columns.Add("EntryUser");
            dt_.Columns.Add("EntryDate");
            dt_.Rows.Clear();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("select projectname, OPERATORID, lastupdate,ENTRYUSER,ENTRYDATE from MntMainProject order by projectname");
                db.CommandText = sb.ToString();
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dt_.Rows.Add(new object[] { dr["projectname"].ToString(), dr["OPERATORID"].ToString(), dr["lastupdate"].ToString(), dr["ENTRYUSER"].ToString(), dr["ENTRYDATE"].ToString() });
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return dt_;
        }

        public static bool InsDelMNTMainProject(string ProjName, string LogUser, int tip, string entriuser)
        {
            int result = 0;
            bool success = false;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                try
                {
                    db.CommandText = "MaintenanceProject";
                    db.CommandType = System.Data.CommandType.StoredProcedure;
                    db.AddParameter("@init", System.Data.SqlDbType.TinyInt, tip);
                    db.AddParameter("@ProjName", System.Data.SqlDbType.VarChar, ProjName);
                    db.AddParameter("@LogUser", System.Data.SqlDbType.VarChar, LogUser);
                    db.AddParameter("@ENTRYUSER", System.Data.SqlDbType.VarChar, entriuser);
                    db.Open();
                    db.BeginTransaction();
                    result = db.ExecuteNonQuery();
                    db.CommitTransaction();
                    success = true;
                }
                catch (Microsoft.Data.SqlClient.SqlException sex)
                {
                    success = false;
                    if (db.Transaction != null)
                        db.RollbackTransaction();
                }
                db.Close();
            }
            return success;
        }//UserExist

        public static string NamaDataBase()
        {
            string return_ = "";
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                return_ = db.GetServerName();
            }
            return return_;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GEtAllDatabase()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> list = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                string databaseName = db.GetDatabaseName();
                list.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = databaseName, Value = databaseName });
            }
            return list;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetMntMainProjectForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> mntMain = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MaintenanceProject";

                db.AddParameter("@init", System.Data.SqlDbType.TinyInt, 1);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        mntMain = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            SelectListItem mnt = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            mnt.Value = dr["ProjectName"] as string;
                            mnt.Text = dr["ProjectName"] as string;

                            mntMain.Add(mnt);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return mntMain;
        }

    }
}
