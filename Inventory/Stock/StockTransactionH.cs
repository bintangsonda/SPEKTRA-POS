using IDS.DataAccess;
using Microsoft.Data.SqlClient;
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
        public int Process { get; set; }
        public DateTime ProcessDate { get;set; }
        public string voucher { get; set; }
        public string Remark { get; set; } 
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
                            stockTransaction.BranchCode = IDS.Tool.GeneralHelper.NullToString(dr["BranchCode"]);
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

    }
}
