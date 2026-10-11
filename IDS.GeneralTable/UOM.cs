using IDS.DataAccess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.GeneralTable
{
    public class UOM
    {
        public string UOMCode { get; set; }
        public string UOMName { get; set; }
        public string OperatorID { get; set; }
        public DateTime CreatedDate { get; set; }
        public UOM()
        {

        }

        public static List<UOM> GetData()
        {
            List<UOM> list = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "Select * from UOM";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<UOM>();

                        while (dr.Read())
                        {
                            UOM sourceCode = new UOM();
                            sourceCode.UOMCode = IDS.Tool.GeneralHelper.NullToString(dr["UOMCode"]);
                            sourceCode.UOMName = IDS.Tool.GeneralHelper.NullToString(dr["UOMName"]);
                            sourceCode.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["CreatedBy"]);
                            sourceCode.CreatedDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["CreatedDate"], DateTime.Now);

                            list.Add(sourceCode);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }

        public int InsUpDel(int ExecCode)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    cmd.CommandText = "InsUOM";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@UOMCode", System.Data.SqlDbType.VarChar, UOMCode);
                    cmd.AddParameter("@UOMName", System.Data.SqlDbType.VarChar, UOMName);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);

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
                            throw new Exception("Category code is already exists. Please choose other loan code.");
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

        public static UOM GetDataByTypeCode(string UOMCode)
        {
            UOM UOM = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select * from UOM where UOMCode=@UOMCode";
                db.AddParameter("@UOMCode", System.Data.SqlDbType.VarChar, UOMCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        UOM = new UOM();
                        UOM.UOMCode = IDS.Tool.GeneralHelper.NullToString(dr["UOMCode"]);
                        UOM.UOMName = IDS.Tool.GeneralHelper.NullToString(dr["UOMName"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return UOM;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCboUOM()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select UOMCode,UOMName from UOM";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();

                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["UOMCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["UOMName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["UOMCode"]);
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
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCboUOMTransaction()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select UOMCode,UOMName from UOM";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                        branches.Add(new SelectListItem
                        {
                            Text = "--Select Menu Category--",
                            Value = "All"
                        });
                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["UOMCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["UOMName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["UOMCode"]);
                            branches.Add(branch);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }


            return branches;
        }
    }
}
