using AppCode;
using IDS.DataAccess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace General.Catalog
{
    public class Product
    {
        public string ProdCode { get; set; }
        public string Branch { get; set; }
        public string CategoriesCode { get; set; }
        public string ProdName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public decimal Harga { get; set; }
        public string Image { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<VariantHeader> VarianProduct { get; set; }


        public Product()
        {
            // Default constructor
        }
        public static List<Product> GetData(string Branch, string Category, string SearchMenu)
        {
            List<Product> list = null;

            using (SqlServer db = new SqlServer())
            {
                if (string.IsNullOrEmpty(SearchMenu))
                {
                    db.CommandText = @"
                                        SELECT *
                                        FROM Product
                                        WHERE Branch LIKE '%' + ISNULL(@Branchs,'') + '%'
                                           AND CategoriesCode LIKE '%' + ISNULL(@Category,'') + '%'
                                          
                                    ";
                }
                else
                {
                    db.CommandText = @"
                                        SELECT *
                                        FROM Product
                                        WHERE Branch LIKE '%' + ISNULL(@Branchs,'') + '%'
                                           AND CategoriesCode LIKE '%' + ISNULL(@Category,'') + '%'
                                          AND prodname LIKE '%' + ISNULL(@SearchMenu,'') + '%'
                                    ";
                    db.AddParameter("@SearchMenu", SqlDbType.VarChar, SearchMenu);

                }
                if (string.IsNullOrEmpty(Category) ||Category=="All")
                {
                    db.AddParameter("@Category", SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@Category", SqlDbType.VarChar, Category);
                }
                if (string.IsNullOrEmpty(Branch))
                {
                    db.AddParameter("@Branchs", SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@Branchs", SqlDbType.VarChar, Branch);
                }


                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<Product>();

                        while (dr.Read())
                        {
                            Product sourceCode = new Product();
                            sourceCode.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            sourceCode.Branch = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                            sourceCode.CategoriesCode = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]);
                            sourceCode.ProdName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            sourceCode.Description = IDS.Tool.GeneralHelper.NullToString(dr["Description"]);
                            sourceCode.Status = IDS.Tool.GeneralHelper.NullToBool(dr["Status"],false);
                            sourceCode.Harga = IDS.Tool.GeneralHelper.NullToDecimal(dr["Harga"],0);
                            sourceCode.Image = IDS.Tool.GeneralHelper.NullToString(dr["Image"]);
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
                    cmd.CommandText = "InsProduct";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@Type", SqlDbType.TinyInt, ExecCode);

                    // PARAMETER LIST
                    cmd.AddParameter("@ProdCode", SqlDbType.VarChar, ProdCode);
                    cmd.AddParameter("@Branch", SqlDbType.VarChar, Branch);
                    cmd.AddParameter("@CategoriesCode", SqlDbType.VarChar, CategoriesCode);
                    cmd.AddParameter("@ProdName", SqlDbType.VarChar, ProdName);
                    cmd.AddParameter("@Description", SqlDbType.VarChar, Description);
                    cmd.AddParameter("@Status", SqlDbType.Bit, Status);
                    cmd.AddParameter("@Harga", SqlDbType.Money, Harga);
                    cmd.AddParameter("@Image", SqlDbType.VarChar, Image); // Base64 atau file path
                    cmd.AddParameter("@OperatorID", SqlDbType.VarChar, OperatorID);
                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();


                    if (result > 0 && VarianProduct!=null)
                    {
                        cmd.ClearParameter();
                        cmd.CommandText = "InsProductVariant";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@Type", SqlDbType.TinyInt, 3);
                        cmd.AddParameter("@ProdCode", SqlDbType.VarChar, ProdCode);
                        cmd.AddParameter("@Branch", SqlDbType.VarChar, Branch);
                        cmd.AddParameter("@VariantCode", SqlDbType.VarChar, "empty");
                        cmd.BeginTransaction();
                        result = cmd.ExecuteNonQuery();
                        cmd.CommitTransaction();
                        if (VarianProduct.Count > 0)
                        {
                            for (int i = 0; i < VarianProduct.Count; i++)
                            {
                                cmd.ClearParameter();
                                cmd.CommandText = "InsProductVariant";
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.AddParameter("@Type", SqlDbType.TinyInt, 1);
                                cmd.AddParameter("@ProdCode", SqlDbType.VarChar, ProdCode);
                                cmd.AddParameter("@Branch", SqlDbType.VarChar, Branch);
                                cmd.AddParameter("@VariantCode", SqlDbType.VarChar, VarianProduct[i].VarianCode);
                                cmd.BeginTransaction();
                                result = cmd.ExecuteNonQuery();
                                cmd.CommitTransaction();

                            }
                        }
                        
                    }


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

        

        public int DuplicateProduct(string ProdCode, string Outlet, string OutletTo,string OperatorID)
        {
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer())
            {
                try
                {
                    cmd.Open();
                    cmd.CommandText = "DuplicateProduct";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // PARAMETER LIST
                    cmd.AddParameter("@ProdCode", SqlDbType.VarChar, ProdCode);
                    cmd.AddParameter("@Branch", SqlDbType.VarChar, Outlet);
                    cmd.AddParameter("@BranchTo", SqlDbType.VarChar, OutletTo);
                    cmd.AddParameter("@OperatorID", SqlDbType.VarChar, OperatorID);
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


        public static Product GetDataEditProduct(string ProdCode,string Branch)
        {
            Product Products = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select * from Product P left join productvariant V on V.prodcode=P.ProdCode and V.branch=P.Branch where P.Prodcode=@ProdCode and P.Branch=@Branch";
                db.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, ProdCode);
                db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, Branch);

                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    Products = new Product();
                    Products.VarianProduct = new List<VariantHeader>();
                    if(dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            VariantHeader details = new VariantHeader();
                            Products.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            Products.Branch = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                            Products.CategoriesCode = IDS.Tool.GeneralHelper.NullToString(dr["CategoriesCode"]);
                            Products.ProdName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            Products.Description = IDS.Tool.GeneralHelper.NullToString(dr["Description"]);
                            Products.Status = IDS.Tool.GeneralHelper.NullToBool(dr["Status"], false);
                            Products.Harga = IDS.Tool.GeneralHelper.NullToDecimal(dr["Harga"], 0);
                            Products.Image = IDS.Tool.GeneralHelper.NullToString(dr["Image"]);
                            Products.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            details.VarianCode = IDS.Tool.GeneralHelper.NullToString(dr["VariantCode"]);
                            Products.VarianProduct.Add(details);
                        }
                           
                        
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return Products;
        }

        public static List<SelectListItem> GetDataVarianProduct()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select VarianCode,VarianName from VariantH where VarianStatus=1";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem spacc = new SelectListItem();
                            spacc.Value = IDS.Tool.GeneralHelper.NullToString(dr["VarianCode"]);
                            spacc.Text =IDS.Tool.GeneralHelper.NullToString(dr["VarianName"]);

                            list.Add(spacc);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }

        public static List<SelectListItem> GetProductForDataSource()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select * from product";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem product2 = new SelectListItem();
                        product2.Value = "All";
                        product2.Text = "All";
                        list.Add(product2);
                        while (dr.Read())
                        {
                            SelectListItem product = new SelectListItem();
                            //Modif by Jeremi 15 Oktober 2025
                            product.Value = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            product.Text = product.Value + " - " + IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]).Replace(@"""", "&#8221;").Replace("@", "&#64;").TrimEnd();
                            //End Jeremi

                            list.Add(product);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }

        public static List<SelectListItem> GetBranchCode()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select * from tblBranch";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem product2 = new SelectListItem();
                        product2.Value = "All";
                        product2.Text = "All";
                        list.Add(product2);
                        while (dr.Read())
                        {
                            SelectListItem product = new SelectListItem();
                            product.Value = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            product.Text = product.Value + " - " + IDS.Tool.GeneralHelper.NullToString(dr["BranchName"]);
                            list.Add(product);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }

    }
}
