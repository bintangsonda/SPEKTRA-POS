using IDS.Tool;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Maintenance
{
    public class MasterArea
    {
        [Required, Display(Name = "Area Code")]
        public string AreaCode { get; set; }

        [Required, Display(Name = "Area Name")]
        public string AreaName { get; set; }

        public List<string> BranchesList { get; set; }

        public DateTime LastUpdate { get; set; }
        public string OperatorID { get; set; }

        public static List<MasterArea> GetData()
        {
            List<MasterArea> list = new List<MasterArea>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblMstArea";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            MasterArea item = new MasterArea();
                            item.AreaCode = Tool.GeneralHelper.NullToString(dr["AreaCode"]);
                            item.AreaName = Tool.GeneralHelper.NullToString(dr["AreaName"]);
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

        public static MasterArea GetDataForEdit(string areaCode)
        {
            MasterArea item = new MasterArea();
            item.BranchesList = new List<string>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select tblMstArea.AreaCode, tblMstArea.AreaName, tblMstArea.OperatorID, tblMstArea.LastUpdate, tblMstAreaDtl.BranchCode from tblMstArea LEFT JOIN tblMstAreaDtl on tblMstArea.AreaCode=tblMstAreaDtl.AreaCode where tblMstArea.AreaCode=@AreaCode";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@AreaCode", SqlDbType.VarChar, areaCode);
                db.Open();
                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            item.AreaCode = Tool.GeneralHelper.NullToString(dr["AreaCode"]);
                            item.AreaName = Tool.GeneralHelper.NullToString(dr["AreaName"]);
                            string branch = Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            item.BranchesList.Add(branch);
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
                    if (FormAction == 1)
                    {
                        cmd.CommandText = "tblMstAreaInsUpDel";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                        cmd.AddParameter("@AreaName", System.Data.SqlDbType.VarChar, AreaName);
                        cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                        cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 1);

                        result = cmd.ExecuteNonQuery();
                        if (result <= 0)
                        {
                            if (cmd.Transaction != null)
                                cmd.RollbackTransaction();
                            message = "Something is wrong during inserting data";
                            return result;
                        }

                        foreach (var branchCode in BranchesList)
                        {
                            cmd.CommandText = "tblMstAreaDtlInsUpDel";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                            cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, branchCode);
                            cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                            cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 1);

                            result = cmd.ExecuteNonQuery();
                            if (result <= 0)
                            {
                                if (cmd.Transaction != null)
                                    cmd.RollbackTransaction();
                                message = "Something is wrong during inserting data";
                                return result;
                            }
                        }
                    }
                    else if (FormAction == 2)
                    {
                        cmd.CommandText = "tblMstAreaInsUpDel";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                        cmd.AddParameter("@AreaName", System.Data.SqlDbType.VarChar, AreaName);
                        cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                        cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 2);

                        result = cmd.ExecuteNonQuery();
                        if (result <= 0)
                        {
                            if (cmd.Transaction != null)
                                cmd.RollbackTransaction();
                            message = "Something is wrong during updating data";
                            return result;
                        }

                        cmd.CommandText = "tblMstAreaDtlInsUpDel";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                        cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                        cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, DBNull.Value);
                        cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 3);

                        result = cmd.ExecuteNonQuery();
                        if (result <= 0)
                        {
                            if (cmd.Transaction != null)
                                cmd.RollbackTransaction();
                            message = "Something is wrong during updating data";
                            return result;
                        }

                        foreach (var branchCode in BranchesList)
                        {
                            cmd.CommandText = "tblMstAreaDtlInsUpDel";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                            cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, branchCode);
                            cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
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

        public int Delete(ref string message)
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
                    
                    cmd.CommandText = "tblMstAreaInsUpDel";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@AreaCode", System.Data.SqlDbType.VarChar, AreaCode);
                    cmd.AddParameter("@AreaName", System.Data.SqlDbType.VarChar, DBNull.Value);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, DBNull.Value);
                    cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 3);

                    result = cmd.ExecuteNonQuery();
                    if (result <= 0)
                    {
                        if (cmd.Transaction != null)
                            cmd.RollbackTransaction();
                        message = "Something is wrong during deleting data";
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

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetMasterAreaForDataSource()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM tblMstArea";
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
                            branch.Text = dr["AreaCode"].ToString() + " - " + dr["AreaName"].ToString();
                            branch.Value = dr["AreaCode"].ToString();
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

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetMasterAreaByGroupCodeForDataSource(string groupCode)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select tblMstAreaGroup.GroupCode, tblMstAreaGroup.AreaCode, tblMstArea.AreaName from tblMstAreaGroup left join tblMstArea on tblMstAreaGroup.AreaCode = tblMstArea.AreaCode WHERE GroupCode=@GroupCode";
                db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, Tool.GeneralHelper.StringToDBNull(groupCode));
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
                            branch.Text = dr["AreaCode"].ToString() + " - " + dr["AreaName"].ToString();
                            branch.Value = dr["AreaCode"].ToString();
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
