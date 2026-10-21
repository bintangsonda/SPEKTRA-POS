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
        public int RecipeCode { get; set; }
        public string RecipeName { get; set; }
        public string BranchCode { get; set; }
        public string ProdCode { get; set; }
        public string ProdName { get; set; }
        public string OperatorID { get; set; }
        public string SalesType { get; set; }
        public DateTime LastUpdated { get; set; }

        public List<ProductRecipeD> RecipeDetail { get; set; }


        public static List<ProductRecipeH> GetData(string BranchCode)
        {
            List<ProductRecipeH> list = null;

            using (SqlServer db = new SqlServer())
            {

                db.CommandText = @"
                        SELECT ProductRecipeH.*, product.prodname
                        FROM ProductRecipeH
                        INNER JOIN product
                            ON product.prodcode = ProductRecipeH.prodcode and productrecipeh.BranchCode=Product.Branch
                        WHERE (@Branch IS NULL OR ProductRecipeH.BranchCode = @Branch)
                    ";
                if (BranchCode == "All" || BranchCode==null)
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, BranchCode);
                }
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
                            Datatables.RecipeCode = IDS.Tool.GeneralHelper.NullToInt(dr["RecipeCode"],0);
                            Datatables.RecipeName = IDS.Tool.GeneralHelper.NullToString(dr["RecipeName"]);
                            Datatables.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            Datatables.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            Datatables.ProdName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            Datatables.SalesType = IDS.Tool.GeneralHelper.NullToString(dr["SalesType"]);
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
                    var paramRecipeCode = new Microsoft.Data.SqlClient.SqlParameter(
                        "@RecipeCode",
                        System.Data.SqlDbType.Int
                    );
                    paramRecipeCode.Direction = System.Data.ParameterDirection.InputOutput;
                    paramRecipeCode.Value = RecipeCode;
                    cmd.DbCommand.Parameters.Add(paramRecipeCode);
                    //cmd.AddParameter("@RecipeCode", System.Data.SqlDbType.Int, RecipeCode);
                    cmd.AddParameter("@BranchCode", System.Data.SqlDbType.VarChar, BranchCode);
                    cmd.AddParameter("@RecipeName", System.Data.SqlDbType.VarChar, RecipeName);
                    cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, ProdCode);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.AddParameter("@SalesType", System.Data.SqlDbType.VarChar, SalesType);
                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    if (ExecCode == 1)
                    {
                        RecipeCode = Convert.ToInt32(paramRecipeCode.Value); ;
                    }
                   
                    if (RecipeDetail != null && RecipeDetail.Count > 0)
                    {
                        cmd.CommandText = "InsProductRecipeDetail";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@RecipeCode", System.Data.SqlDbType.Int, RecipeCode);
                        cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, "");
                        cmd.AddParameter("@Qty", System.Data.SqlDbType.Decimal, 0);
                        cmd.AddParameter("@SeqNo", System.Data.SqlDbType.Money, 0);
                        result = cmd.ExecuteNonQuery();

                        int count = 1;
                        foreach (var data in RecipeDetail)
                        {
                            cmd.CommandText = "InsProductRecipeDetail";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                            cmd.AddParameter("@RecipeCode", System.Data.SqlDbType.Int, RecipeCode);
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
                db.CommandText = "select ProductRecipeH.*,productrecipeD.ProdCode as DProdCode,productrecipeD.Qty from ProductRecipeH inner join productrecipeD on productRecipeD.RecipeCode=ProductRecipeH.RecipeCode where ProductRecipeH.RecipeCode=@RecipeCode";
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
                            ProductRecipeD details = new ProductRecipeD();
                            Data1.RecipeCode = IDS.Tool.GeneralHelper.NullToInt(dr["RecipeCode"],0);
                            Data1.RecipeName = IDS.Tool.GeneralHelper.NullToString(dr["RecipeName"]);
                            Data1.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
                            Data1.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            Data1.SalesType = IDS.Tool.GeneralHelper.NullToString(dr["SalesType"]);
                            details.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["DProdCode"]);                           
                            details.Qty = IDS.Tool.GeneralHelper.NullToDecimal(dr["Qty"],0);
                            Data1.RecipeDetail.Add(details);

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
                db.CommandText = "select * from productrecipeH where recipecode=@RecipeCode";
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
                            RecipeCode = IDS.Tool.GeneralHelper.NullToInt(dr["RecipeCode"],0),
                            SalesType = IDS.Tool.GeneralHelper.NullToString(dr["SalesType"]),
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
