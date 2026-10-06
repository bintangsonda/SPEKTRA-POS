using IDS.DataAccess;
using IDS.Tool;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SPOS.Inventory.Stock
{
    public class StockBalance
    {
        public General.Catalog.Product prod { get; set; }
        public Warehouse wh { get; set; }
        public string Date { get; set; }
        public string Branch { get; set; }
        public string BeginBalance { get; set; }
        public string QtyIn { get; set; }
        public string QtyOut { get; set; }
        public string QtyAdj { get; set; }
        public string EndBalance { get; set; }

        public StockBalance() { }

        public static List<StockBalance> GetStockBalances( DateTime DateFrom,DateTime DateTo, string wh, string prodcode,string Branch)
        {
            List<StockBalance> list = new List<StockBalance>();

            using (SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "InvSelStockBalance";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@DateFrom", System.Data.SqlDbType.DateTime, DateFrom);
                db.AddParameter("@DateTo", System.Data.SqlDbType.DateTime, DateTo);
                db.AddParameter("@Wh", System.Data.SqlDbType.VarChar, wh);
                if (prodcode == "All")
                {
                    db.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, DBNull.Value);
                }
                else
                {
                    db.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, prodcode);
                }
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
                            StockBalance stockBalance = new StockBalance();
                            stockBalance.prod = new General.Catalog.Product();
                            stockBalance.prod.ProdName = IDS.Tool.GeneralHelper.NullToString(dr["ProdName"]);
                            stockBalance.prod.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["prodCode"]);
                            stockBalance.Branch = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                            stockBalance.wh = new Warehouse();
                            stockBalance.wh.Name = IDS.Tool.GeneralHelper.NullToString(dr["WHCode"]);
                            stockBalance.BeginBalance = IDS.Tool.GeneralHelper.NullToDecimal(dr["BeginBalance"],0).ToString();
                            stockBalance.QtyIn = IDS.Tool.GeneralHelper.NullToDecimal(dr["QtyIn"], 0).ToString();
                            stockBalance.QtyOut = IDS.Tool.GeneralHelper.NullToDecimal(dr["QtyOut"], 0).ToString();
                            stockBalance.QtyAdj = IDS.Tool.GeneralHelper.NullToDecimal(dr["QtyAdj"], 0).ToString();
                            stockBalance.EndBalance = IDS.Tool.GeneralHelper.NullToDecimal(dr["EndBalance"], 0).ToString();
                            DateTime DateStock = IDS.Tool.GeneralHelper.NullToDateTime(dr["Date"],DateTime.Now);
                            stockBalance.Date = DateStock.ToString("dd/MMM/yyyy");
                            list.Add(stockBalance);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static int AddQtyOut(string ProdCode,decimal Qty,string BranchCode)
        {
            int hasil = 0;
            using (SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "AddQtyStock";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, ProdCode);
                db.AddParameter("@Qty", System.Data.SqlDbType.Decimal, Qty);
                db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, BranchCode);
                db.AddParameter("@Type", System.Data.SqlDbType.Int, 1);//qty out jadi 1
                db.Open();
                db.BeginTransaction();
                hasil=db.ExecuteNonQuery();
                db.CommitTransaction();
                db.Close();
            }

            return hasil;
        }
    }
}
