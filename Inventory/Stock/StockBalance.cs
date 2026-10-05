using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.DataAccess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using IDS.Tool;

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
                            stockBalance.BeginBalance = IDS.Tool.GeneralHelper.NullToString(dr["BeginBalance"]);
                            stockBalance.QtyIn = IDS.Tool.GeneralHelper.NullToString(dr["QtyIn"]);
                            stockBalance.QtyOut = IDS.Tool.GeneralHelper.NullToString(dr["QtyOut"]);
                            stockBalance.QtyAdj = IDS.Tool.GeneralHelper.NullToString(dr["QtyAdj"]);
                            stockBalance.EndBalance = IDS.Tool.GeneralHelper.NullToString(dr["EndBalance"]);
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

        public static List<SelectListItem> GetWareHouseForDataSource()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "Select distinct WHCode, WHName,isDefault from INWarehouse order by WHCode desc";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list.Add(new SelectListItem() { Text = "ALL", Value = "ALL" });
                        while (dr.Read())
                        {
                            SelectListItem coa = new SelectListItem();
                            coa.Value = IDS.Tool.GeneralHelper.NullToString(dr["WHCode"]);
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["WHCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["WHName"]);
                            list.Add(coa);
                        }
                    }
                }
                db.Close();
            }
            return list;
        }
       
        public static List<SelectListItem> GetProductCodeForDataSource()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "Select distinct ProdCode,prodname from Product order by ProdCode asc";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        list.Add(new SelectListItem() { Text = "ALL", Value = "ALL" });
                        while (dr.Read())
                        {
                            SelectListItem coa = new SelectListItem();
                            coa.Value = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["prodname"]);
                            list.Add(coa);
                        }
                    }
                }
                db.Close();
            }

            return list;
        }

    }
}
