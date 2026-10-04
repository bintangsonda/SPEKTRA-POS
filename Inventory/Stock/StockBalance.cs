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
        public string Period { get; set; }
        public string PartNo { get; set; }
        public string BeginBalance { get; set; }
        public string QtyIn { get; set; }
        public string QtyOut { get; set; }
        public string QtyReturnIn { get; set; }
        public string QtyReturnOut { get; set; }
        public string QtyAdj { get; set; }
        public string QtyOrderIn { get; set; }
        public string QtyOrderOut { get; set; }
        public string BegBalValue { get; set; }
        public string AvgValue { get; set; }

        public StockBalance() { }

        public static List<StockBalance> GetStockBalances(int type, string period, string location, string wh, string prodcode)
        {
            List<StockBalance> list = new List<StockBalance>();

            using (SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "InvSelStockBalance";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, type);
                db.AddParameter("@Period", System.Data.SqlDbType.VarChar, period);
                db.AddParameter("@Loc", System.Data.SqlDbType.VarChar, location);
                db.AddParameter("@Wh", System.Data.SqlDbType.VarChar, wh);
                db.AddParameter("@ProdCode", System.Data.SqlDbType.VarChar, prodcode);

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
                            stockBalance.prod.Description = IDS.Tool.GeneralHelper.NullToString(dr["Description"]);
                            stockBalance.prod.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["prodCode"]);
                            stockBalance.wh = new Warehouse();
                            if (type == 1)
                            {
                                stockBalance.wh.Name = IDS.Tool.GeneralHelper.NullToString(dr["WHName"]);
                            }
                            else
                            {
                                stockBalance.wh.Name = "";
                            }

                            stockBalance.BeginBalance = IDS.Tool.GeneralHelper.NullToString(dr["BeginBalance"]);
                            stockBalance.QtyIn = IDS.Tool.GeneralHelper.NullToString(dr["QtyIn"]);
                            stockBalance.QtyOut = IDS.Tool.GeneralHelper.NullToString(dr["QtyOut"]);
                            stockBalance.QtyReturnIn = IDS.Tool.GeneralHelper.NullToString(dr["QtyReturnIn"]);
                            stockBalance.QtyReturnOut = IDS.Tool.GeneralHelper.NullToString(dr["QtyReturnOut"]);
                            stockBalance.QtyAdj = IDS.Tool.GeneralHelper.NullToString(dr["QtyAdj"]);
                            stockBalance.QtyOrderIn = IDS.Tool.GeneralHelper.NullToString(dr["QtyOrderIn"]);
                            stockBalance.QtyOrderOut = IDS.Tool.GeneralHelper.NullToString(dr["QtyOrderOut"]);
                            stockBalance.BegBalValue = IDS.Tool.GeneralHelper.NullToString(dr["BegBalValue"]);
                            stockBalance.AvgValue = IDS.Tool.GeneralHelper.NullToString(dr["vQty"]);
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

        public static List<SelectListItem> GetYearForDataSource()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            int MinYear = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select distinct Periode from (select left(Periode,4) as Periode from INStockProduct union select cast(year(getdate()) as varchar(4))) as tbl order by Periode desc";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {

                        while (dr.Read())
                        {


                            MinYear = Convert.ToInt16(dr["Periode"]);

                            if (MinYear == 0)
                            {
                                MinYear = DateTime.Now.Year;
                            }

                            //year.Value = MinYear;
                            //year.Text = MinYear;

                            //list.Add(year);
                        }
                        int iYear;

                        int NowYear = DateTime.Now.Year;
                        int MaxFutureYear = DateTime.Now.Year + 10;
                        for (iYear = MinYear; iYear <= MaxFutureYear; iYear++)
                        {
                            int iYearL = iYear;
                            SelectListItem year = new SelectListItem();
                            year.Value = iYearL.ToString();
                            year.Text = iYearL.ToString();
                            list.Add(year);
                        }

                    }
                }
                db.Close();
            }
            return list;
        }

        public static List<SelectListItem> GetMonthForDataSource()
        {
            List<SelectListItem> Month = new List<SelectListItem>();
            Month.Add(new SelectListItem() { Text = "January", Value = "01" });
            Month.Add(new SelectListItem() { Text = "February", Value = "02" });
            Month.Add(new SelectListItem() { Text = "March", Value = "03" });
            Month.Add(new SelectListItem() { Text = "April", Value = "04" });
            Month.Add(new SelectListItem() { Text = "May", Value = "05" });
            Month.Add(new SelectListItem() { Text = "Juny", Value = "06" });
            Month.Add(new SelectListItem() { Text = "July", Value = "07" });
            Month.Add(new SelectListItem() { Text = "August", Value = "08" });
            Month.Add(new SelectListItem() { Text = "September", Value = "09" });
            Month.Add(new SelectListItem() { Text = "October", Value = "10" });
            Month.Add(new SelectListItem() { Text = "November", Value = "11" });
            Month.Add(new SelectListItem() { Text = "Desember", Value = "12" });

            return Month;
        }

        public static List<SelectListItem> GetLocationFordataSource()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "Select distinct LocationCode, LocationName from tblLocation order by LocationName asc";
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
                            coa.Value = IDS.Tool.GeneralHelper.NullToString(dr["LocationCode"]);
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["LocationCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["LocationName"]);
                            list.Add(coa);
                        }
                    }
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
                db.CommandText = "Select distinct WHCode, WHName,isDefault from INWarehouse order by isDefault desc";
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

        public static List<SelectListItem> GetProductGroupFordataSource()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "Select distinct GroupCode,GroupName from INProductGroup  order by GroupCode asc";
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
                            coa.Value = IDS.Tool.GeneralHelper.NullToString(dr["GroupCode"]);
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["GroupCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["GroupName"]);
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
                //select Distinct tblCity.CityCode, tblSalesH.customercode from tblSalesH LEFT JOIN tblCustomer ON tblSalesH.customercode=tblcustomer.custcode JOIN tblCity ON tblCustomer.city = tblCity.CityCode order by tblSalesH.customercode ASC
                db.CommandText = "Select distinct ProdCode,Description from INProduct  order by ProdCode asc";
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
                            coa.Text = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]) + " - " + IDS.Tool.GeneralHelper.NullToString(dr["Description"]);
                            list.Add(coa);
                        }
                    }
                }
                db.Close();
            }

            return list;
        }

        public static bool CheckDataInstockProduct(string period)
        {
            bool extinctData = false;
            using (SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select * from INStockProduct where Periode='" + period + "'";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        extinctData = true;
                        //while (dr.Read())
                        //{

                        //}
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return extinctData;
        }

        public static List<StockBalance> GetProductWithNegativeStock(string year, string month)
        {
            List<StockBalance> list = new List<StockBalance>();

            using (SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "SELECT INStockProduct.ProdCode, INProduct.Description, dbo.GetValuesForDT(INProduct.ProdCode,(Isnull(BeginBalance,0)+isnull(QtyIn,0)-isnull(QtyOut,0)- isnull(Qtyreturnout,0)+isnull(Qtyadj,0)),INProduct.UoMOut) as vQty FROM INStockProduct inner join INProduct on INStockProduct.ProdCode = INProduct.ProdCode WHERE Isnull(BeginBalance,0)+isnull(QtyIn,0)-isnull(QtyOut,0)- isnull(Qtyreturnout,0)+isnull(qtyreturnIn,0)+isnull(Qtyadj,0) < 0 AND Periode = @Year + @Month";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@Year", System.Data.SqlDbType.VarChar, year);
                db.AddParameter("@Month", System.Data.SqlDbType.VarChar, month);

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
                            stockBalance.prod.Description = IDS.Tool.GeneralHelper.NullToString(dr["Description"]);
                            stockBalance.prod.ProdCode = IDS.Tool.GeneralHelper.NullToString(dr["ProdCode"]);
                            stockBalance.AvgValue = IDS.Tool.GeneralHelper.NullToString(dr["vQty"]);
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
    }
}
