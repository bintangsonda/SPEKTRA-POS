using IDS.DataAccess;
using IDS.Tool;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Catalog
{
    public class Categories
    {
        [Display(Name = "CategoriesCode")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Categories Code is required")]
        [MaxLength(5), StringLength(5)]
        public string CategoriesCode { get; set; }

        [Display(Name = "CategoriesName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Categories Name is required")]
        [MaxLength(50), StringLength(50)]
        public string CategoriesName { get; set; }
        public int ItemStock { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }

        public Categories() { }

        public static List<Categories> GetData()
        {
            List<Categories> list = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "Select * from Categories";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<Categories>();

                        while (dr.Read())
                        {
                            Categories sourceCode = new Categories();
                            sourceCode.CategoriesCode = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]);
                            sourceCode.CategoriesName = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesName"]);
                            sourceCode.ItemStock = IDS.Tool.GeneralHelper.NullToInt16(dr["ItemStock"],0);
                            sourceCode.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            sourceCode.LastUpdate = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.Now);

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
                    cmd.CommandText = "InsCategories";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@CategoriesCode", System.Data.SqlDbType.VarChar, CategoriesCode);
                    cmd.AddParameter("@CategoriesName", System.Data.SqlDbType.VarChar, CategoriesName);

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

        public static Categories GetDataByCategoryCode(string CategoryCode)
        {
            Categories categories = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select * from categories where CategoriesCode=@CategoryCode";
                db.AddParameter("@CategoryCode", System.Data.SqlDbType.VarChar, CategoryCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        categories = new Categories();
                        categories.CategoriesCode = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]);
                        categories.CategoriesName = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesName"]);
                        categories.ItemStock = IDS.Tool.GeneralHelper.NullToInt16(dr["ItemStock"],0);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return categories;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCboCategories()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select CategoriesCode,CategoriesName from categories";
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
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["CategoriesName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]);
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
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetCboCategoriesTransaction()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select CategoriesCode,CategoriesName from categories";
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
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["CategoriesName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]);
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
