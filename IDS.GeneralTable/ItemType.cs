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
    public class ItemType
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string OperatorID { get; set; }
        public DateTime CreatedDate { get; set; }
        public ItemType()
        {

        }
        public static List<ItemType> GetData()
        {
            List<ItemType> list = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "Select * from ItemType";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<ItemType>();

                        while (dr.Read())
                        {
                            ItemType sourceCode = new ItemType();
                            sourceCode.TypeCode = IDS.Tool.GeneralHelper.NullToString(dr["TypeCode"]);
                            sourceCode.TypeName = IDS.Tool.GeneralHelper.NullToString(dr["TypeName"]);
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
                    cmd.CommandText = "InsItemType";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@TypeCode", System.Data.SqlDbType.VarChar, TypeCode);
                    cmd.AddParameter("@TypeName", System.Data.SqlDbType.VarChar, TypeName);
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

        public static ItemType GetDataByTypeCode(string TypeCode)
        {
            ItemType ItemType = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select * from ItemType where ItemTypeCode=@TypeCode";
                db.AddParameter("@TypeCode", System.Data.SqlDbType.VarChar, TypeCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        ItemType = new ItemType();
                        ItemType.TypeCode = IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeCode"]);
                        ItemType.TypeName = IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeName"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return ItemType;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCboItemType()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select ItemTypeCode,ItemTypeName from ItemType";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new SelectListItem
                        {
                            Text = "All",
                            Value = "All"
                        };
                        branches.Add(a);
                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeCode"]);
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
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCboItemTypeTransaction()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select ItemTypeCode,ItemTypeName from ItemType";
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
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["ItemTypeCode"]);
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
