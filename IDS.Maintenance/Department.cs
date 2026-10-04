using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.Tool;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Maintenance
{
    public class Department
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }

        public static List<Department> GetData()
        {
            List<Department> list = new List<Department>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblDepartment";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Department item = new Department();
                            item.DepartmentCode = Tool.GeneralHelper.NullToString(dr["DepartmentCode"]);
                            item.DepartmentName = Tool.GeneralHelper.NullToString(dr["DepartmentName"]);
                            item.OperatorID = Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            item.LastUpdate = Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.MinValue);
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

        public static Department GetDataForEdit(string departmentCode)
        {
            Department item = new Department();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select * from tblDepartment where DepartmentCode=@DepartmentCode";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@DepartmentCode", SqlDbType.VarChar, departmentCode);
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            item.DepartmentCode = Tool.GeneralHelper.NullToString(dr["DepartmentCode"]);
                            item.DepartmentName = Tool.GeneralHelper.NullToString(dr["DepartmentName"]);
                            item.OperatorID = Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            item.LastUpdate = Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.MinValue);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return item;
        }

        public int InsUpDel(int FormAction, ref string message)
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

                    cmd.CommandText = "tblDepartmentInsUpDel";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@DepartmentCode", System.Data.SqlDbType.VarChar, DepartmentCode);
                    cmd.AddParameter("@DepartmentName", System.Data.SqlDbType.VarChar, DepartmentName);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@Type", System.Data.SqlDbType.Int, FormAction);

                    result = cmd.ExecuteNonQuery();
                    if (result <= 0)
                    {
                        if (cmd.Transaction != null)
                            cmd.RollbackTransaction();
                        message = "Something is wrong. Please contact your administrator";
                        return result;
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
                            message = "This data is already exists! Please input with another Department Code";
                            throw new Exception("This data is already exists! Please input with another Department Code");
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

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetDepartmentForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblDepartment";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        //SelectListItem a = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                        //a.Text = "All";
                        //a.Value = "All";
                        //branches.Add(a);
                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(); //<string, string>(dr["BranchCode"].ToString(), dr["BranchName"].ToString());
                            branch.Text = dr["DepartmentCode"].ToString() + " - " + dr["DepartmentName"].ToString();
                            branch.Value = dr["DepartmentCode"].ToString();
                            branches.Add(branch);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            branches = branches.OrderBy(x => x.Text).ToList();

            return branches;
        }

    }
}
