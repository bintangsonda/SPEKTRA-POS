using IDS.DataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Catalog
{
    public class ProductRecipeH
    {
        public string RecipeCode { get; set; }
        public string RecipeName { get; set; }
        public string BranchCode { get; set; }
        public string ProdCode { get; set; }
        public string ProdName { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdated { get; set; }

        List<ProductRecipeD> RecipeDetail { get; set; }


        public static List<ProductRecipeH> GetData()
        {
            List<ProductRecipeH> list = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "Select ProductRecipeH.*, product.prodname from ProductRecipeH inner join product on product.prodcode=ProductRecipeH.prodcode";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list = new List<ProductRecipeH>();

                        while (dr.Read())
                        {
                            ProductRecipeH Datatables = new ProductRecipeH();
                            Datatables.RecipeCode = IDS.Tool.GeneralHelper.NullToString(dr["RecipeCode"]);
                            Datatables.RecipeName = IDS.Tool.GeneralHelper.NullToString(dr["RecipeName"]);
                            Datatables.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            Datatables.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            Datatables.ProdName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            Datatables.OperatorID = IDS.Tool.GeneralHelper.NullToString(dr["OperatorID"]);
                            Datatables.LastUpdated = IDS.Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.Now);

                            list.Add(Datatables);
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
                    cmd.CommandText = "InsProductRecipe";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@RecipeCode", System.Data.SqlDbType.VarChar, RecipeCode);
                    cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                    cmd.AddParameter("@RecipeName", System.Data.SqlDbType.VarChar, RecipeName);
                    cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, ProdCode);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@LastUpdate", System.Data.SqlDbType.DateTime, LastUpdated);

                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();

                    if (RecipeDetail != null && RecipeDetail.Count > 0)
                    {
                        cmd.CommandText = "InsProductRecipeDetail";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@RecipeCode", System.Data.SqlDbType.VarChar,RecipeCode);
                        cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, "");
                        cmd.AddParameter("@Qty", System.Data.SqlDbType.Decimal, 0);
                        cmd.AddParameter("@SeqNo", System.Data.SqlDbType.Money, 0);
                        result = cmd.ExecuteNonQuery();

                        int count = 1;
                        foreach (var data in RecipeDetail)
                        {
                            cmd.CommandText = "InsVariantDetail";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                            cmd.AddParameter("@RecipeCode", System.Data.SqlDbType.VarChar, RecipeCode);
                            cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, data.ProdCode);
                            cmd.AddParameter("@Qty", System.Data.SqlDbType.Decimal, data.Qty);
                            cmd.AddParameter("@SeqNo", System.Data.SqlDbType.Money, data.seqNo);
                            result = cmd.ExecuteNonQuery();
                            count++;
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

        public static ProductRecipeH GetDataByRecipeCode(string RecipeCode)
        {
            ProductRecipeH Data1 = null;

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select ProductRecipeH.*,ProductRecipeD.Name from ProductRecipeH left join ProductRecipeD on ProductRecipeH.RecipeCode=ProductRecipeD.RecipeCode  where ProductRecipeH.RecipeCode=@RecipeCode";
                db.AddParameter("@RecipeCode", System.Data.SqlDbType.VarChar, RecipeCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    Data1 = new ProductRecipeH();
                    Data1.RecipeDetail = new List<ProductRecipeD>();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            
                            Data1.RecipeCode = IDS.Tool.GeneralHelper.NullToString(dr["VarianCode"]);
                            Data1.RecipeName = IDS.Tool.GeneralHelper.NullToString(dr["VarianName"]);
                            Data1.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["VarianStatus"]);
                            Data1.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["VarianType"]);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return Data1;
        }
        public static List<ProductRecipeH> GetRecipeHeadDetail(string RecipeCode)
        {
            List<ProductRecipeH> result = new List<ProductRecipeH>();

            using (SqlServer db = new SqlServer())
            {
                // ===== 1. GET HEADER =====
                db.CommandText = "select * from productrecipe where recipecode=@RecipeCode";
                db.AddParameter("@RecipeCode", System.Data.SqlDbType.VarChar, RecipeCode);
                db.Open();

                db.ExecuteReader();
                using (var dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    while (dr.Read())
                    {
                        ProductRecipeH header = new ProductRecipeH
                        {
                            ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]),
                            ProdName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]),
                            BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]),
                            RecipeName = IDS.Tool.GeneralHelper.NullToString(dr["RecipeName"]),
                            RecipeCode = IDS.Tool.GeneralHelper.NullToString(dr["RecipeCode"]),
                            RecipeDetail = new List<ProductRecipeD>()
                        };

                        result.Add(header);
                    }
                }

                db.Close();

                // ===== 2. GET DETAIL PER HEADER =====
                foreach (var header in result)
                {
                    db.CommandText = "Select productrecipeD.*,product.prodname from productrecipeD inner join product on product.code = productrecipe.prodcode where recipeCode=@RecipeCode";
                    db.ClearParameter();
                    db.AddParameter("@RecipeCode", System.Data.SqlDbType.VarChar, RecipeCode);
                    db.Open();
                    db.ExecuteReader();

                    using (var drDetail = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                    {
                        while (drDetail.Read())
                        {
                            header.RecipeDetail.Add(new ProductRecipeD
                            {
                                ProdCode = IDS.Tool.GeneralHelper.NullToString(drDetail["ProdCode"]),
                                ProdName = IDS.Tool.GeneralHelper.NullToString(drDetail["ProdName"]),
                                seqNo = IDS.Tool.GeneralHelper.NullToInt(drDetail["seqNo"], 0),
                                Qty = IDS.Tool.GeneralHelper.NullToDecimal(drDetail["Qty"], 0)
                            });
                        }
                    }

                    db.Close();
                }
            }

            return result;
        }
    }
}
