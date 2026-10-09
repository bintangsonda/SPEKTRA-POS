using General.Catalog;
using IDS.DataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOS.Inventory.Stock
{
    public class StockTransactionH
    {
        public string TransNo { get; set; }
        public string TransCode { get; set; }
        public DateTime TransDate { get; set; }
        public string BranchCode { get; set; }
        public string WarehouseCode { get; set; }  
        public int? Process { get; set; }
        public DateTime? ProcessDate { get;set; }
        public string voucher { get; set; }
        public string Remark { get; set; }
        public string OperatorID { get; set; }
        public List<StockTransactionD> StockTransactionDetails { get; set; }
        public StockTransactionH() { }

        public static List<StockTransactionH> GetStockTransaction(DateTime DateFrom,DateTime DateTo, string Branch)
        {
            List<StockTransactionH> list = new List<StockTransactionH>();

            using (SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "InvSelStockTransaction";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@DateFrom", System.Data.SqlDbType.DateTime, DateFrom);
                db.AddParameter("@DateTo", System.Data.SqlDbType.DateTime, DateTo);
                if (Branch == "All")
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, Branch);
                }

                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        
                        while (dr.Read())
                        {
                            StockTransactionH stockTransaction = new StockTransactionH();
                            stockTransaction.TransNo = IDS.Tool.GeneralHelper.NullToString(dr["TransNo"]); ;
                            stockTransaction.TransCode = IDS.Tool.GeneralHelper.NullToString(dr["TransCode"]);
                            stockTransaction.TransDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["TransDate"], DateTime.Now);
                            stockTransaction.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                            stockTransaction.WarehouseCode = IDS.Tool.GeneralHelper.NullToString(dr["WHCode"]);
                            stockTransaction.Process = IDS.Tool.GeneralHelper.NullToInt(dr["Process"], 0);
                            stockTransaction.ProcessDate = IDS.Tool.GeneralHelper.NullToDateTime(dr["ProcessDate"], DateTime.Now);
                            stockTransaction.voucher = IDS.Tool.GeneralHelper.NullToString(dr["Voucher"]);
                            stockTransaction.Remark = IDS.Tool.GeneralHelper.NullToString(dr["Remark"]);
                            // Tambahkan Header ke list
                            list.Add(stockTransaction);
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
                    cmd.CommandText = "InsUpDelStockTrans";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.ClearParameter();
                    cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, ExecCode);
                    cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, TransNo);
                    cmd.AddParameter("@TransCode", System.Data.SqlDbType.VarChar, TransCode);
                    cmd.AddParameter("@TransDate", System.Data.SqlDbType.DateTime, TransDate);
                    cmd.AddParameter("@WHCode", System.Data.SqlDbType.VarChar, "RIB");
                    cmd.AddParameter("@Remark", System.Data.SqlDbType.VarChar, Remark);
                    cmd.AddParameter("@Branch", System.Data.SqlDbType.VarChar, BranchCode);
                    cmd.AddParameter("@OperatorID", System.Data.SqlDbType.VarChar, OperatorID);

                    cmd.BeginTransaction();
                    object obj = cmd.ExecuteScalar();

                    if (obj != null && obj != DBNull.Value)
                    {
                        TransNo = obj.ToString();
                    }
                    if (StockTransactionDetails != null && StockTransactionDetails.Count > 0)
                    {
                        cmd.CommandText = "InsUpDelStockTransD";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ClearParameter();
                        cmd.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@SeqNo", System.Data.SqlDbType.TinyInt, 0);
                        cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, TransNo);
                        cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, "");
                        cmd.AddParameter("@Qty", System.Data.SqlDbType.Money, 0);
                        cmd.AddParameter("@Remark", System.Data.SqlDbType.VarChar, "");

                        result = cmd.ExecuteNonQuery();

                        int count = 1;
                        foreach (var data in StockTransactionDetails)
                        {
                            cmd.CommandText = "InsUpDelStockTransD";
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.ClearParameter();
                            cmd.AddParameter("@Type", System.Data.SqlDbType.Int, 1);
                            cmd.AddParameter("@SeqNo", System.Data.SqlDbType.Int, 0);
                            cmd.AddParameter("@TransNo", System.Data.SqlDbType.VarChar, TransNo);
                            cmd.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, data.ProdCode);
                            cmd.AddParameter("@Qty", System.Data.SqlDbType.Decimal, data.Qty);
                            cmd.AddParameter("@Remark", System.Data.SqlDbType.VarChar, data.Remark);
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

    }
}
