
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using IDS.Tool;

namespace IDS.GeneralTable
{
    public class SalesDashboard
    {
        // ==========================================
        // FILTER DASHBOARD
        // ==========================================

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string BranchCode { get; set; } = "";

        // ==========================================
        // SUMMARY PENJUALAN
        // ==========================================

        public decimal TotalSales { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalItemsSold { get; set; }
        public decimal AverageTransaction { get; set; }

        // ==========================================
        // DATA GRAFIK
        // ==========================================

        public List<SalesChartData> SalesChart { get; set; }
            = new List<SalesChartData>();

        // ==========================================
        // BARANG TERLARIS
        // ==========================================

        public List<TopSellingProduct> TopProducts { get; set; }
            = new List<TopSellingProduct>();

        // ==========================================
        // TRANSAKSI TERBARU
        // ==========================================

        public List<RecentSalesTransaction> RecentTransactions
        { get; set; } = new List<RecentSalesTransaction>();


        // TRANSAKSI PENDING
        public int TotalPendingTransactions { get; set; }


        // ==========================================
        // CONSTRUCTOR
        // ==========================================

        public SalesDashboard()
        {
            DateFrom = DateTime.Today.AddDays(-6);
            DateTo = DateTime.Today;
        }

        // ==========================================
        // FUNCTION UTAMA DASHBOARD
        // ==========================================

        public static SalesDashboard GetDashboard(
            DateTime dateFrom,
            DateTime dateTo,
            string branchCode = null)
        {
            if (dateFrom.Date > dateTo.Date)
            {
                throw new ArgumentException(
                    "Tanggal awal tidak boleh melebihi tanggal akhir.");
            }

            SalesDashboard dashboard = new SalesDashboard
            {
                DateFrom = dateFrom.Date,
                DateTo = dateTo.Date,
                BranchCode = branchCode ?? ""
            };

            LoadSummary(dashboard);
            LoadSalesChart(dashboard);
            LoadTopProducts(dashboard);
            LoadRecentTransactions(dashboard);
            LoadPendingTransactions(dashboard);


            return dashboard;
        }

        // ==========================================
        // PARAMETER QUERY
        // ==========================================

        private static void AddFilterParameters(
            DataAccess.SqlServer db,
            SalesDashboard dashboard)
        {
            db.AddParameter(
                "@DateFrom",
                SqlDbType.DateTime,
                dashboard.DateFrom.Date);

            db.AddParameter(
                "@DateTo",
                SqlDbType.DateTime,
                dashboard.DateTo.Date.AddDays(1));

            db.AddParameter(
                "@BranchCode",
                SqlDbType.VarChar,
                dashboard.BranchCode);
        }

        // ==========================================
        // SUMMARY PENJUALAN
        // ==========================================

        private static void LoadSummary(SalesDashboard dashboard)
        {
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = @"
                    SELECT
                        ISNULL(SUM(TransTotalAmt), 0) AS TotalSales,
                        COUNT(*) AS TotalTransactions
                    FROM TransInvH
                    WHERE TransDate >= @DateFrom
                      AND TransDate < @DateTo
                      AND Status = 1
                      AND (
                            @BranchCode = ''
                            OR BranchCode = @BranchCode
                          );";

                db.CommandType = CommandType.Text;

                AddFilterParameters(db, dashboard);

                db.Open();
                db.ExecuteReader();

                using (SqlDataReader dr =
                    db.DbDataReader as SqlDataReader)
                {
                    if (dr != null && dr.Read())
                    {
                        dashboard.TotalSales =
                            Tool.GeneralHelper.NullToDecimal(
                                dr["TotalSales"], 0);

                        dashboard.TotalTransactions =
                            Convert.ToInt32(dr["TotalTransactions"]);
                    }
                }

                db.Close();
            }

            dashboard.AverageTransaction =
                dashboard.TotalTransactions > 0
                    ? dashboard.TotalSales /
                      dashboard.TotalTransactions
                    : 0;
        }

        // ==========================================
        // GRAFIK PENJUALAN HARIAN
        // ==========================================

        private static void LoadSalesChart(SalesDashboard dashboard)
        {
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = @"
                    SELECT
                        CAST(TransDate AS DATE) AS SalesDate,
                        SUM(TransTotalAmt) AS TotalSales
                    FROM TransInvH
                    WHERE TransDate >= @DateFrom
                      AND TransDate < @DateTo
                      AND Status = 1
                      AND (
                            @BranchCode = ''
                            OR BranchCode = @BranchCode
                          )
                    GROUP BY CAST(TransDate AS DATE)
                    ORDER BY SalesDate;";

                db.CommandType = CommandType.Text;

                AddFilterParameters(db, dashboard);

                db.Open();
                db.ExecuteReader();

                Dictionary<DateTime, decimal> salesByDate =
                    new Dictionary<DateTime, decimal>();

                using (SqlDataReader dr =
                    db.DbDataReader as SqlDataReader)
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            DateTime date =
                                Convert.ToDateTime(dr["SalesDate"]).Date;

                            decimal sales =
                                Tool.GeneralHelper.NullToDecimal(
                                    dr["TotalSales"], 0);

                            salesByDate[date] = sales;
                        }
                    }
                }

                db.Close();

                for (DateTime date = dashboard.DateFrom.Date;
                     date <= dashboard.DateTo.Date;
                     date = date.AddDays(1))
                {
                    decimal sales = 0;

                    if (salesByDate.ContainsKey(date))
                    {
                        sales = salesByDate[date];
                    }

                    dashboard.SalesChart.Add(new SalesChartData
                    {
                        SalesDate = date,
                        Label = date.ToString("dd MMM"),
                        TotalSales = sales
                    });
                }
            }
        }

        // ==========================================
        // TOP 5 BARANG TERLARIS
        // ==========================================

        private static void LoadTopProducts(SalesDashboard dashboard)
        {
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = @"
                    SELECT TOP 5
                        D.ProdCode,
                        MAX(D.ProdName) AS ProdName,
                        SUM(D.Qty) AS QuantitySold,
                        SUM(D.ProdAmt * D.Qty) AS TotalSales
                    FROM TransInvD D
                    INNER JOIN TransInvH H
                        ON H.TransCode = D.TransCode
                       AND H.BranchCode = D.BranchCode
                    WHERE H.TransDate >= @DateFrom
                      AND H.TransDate < @DateTo
                      AND H.Status = 1
                      AND (
                            @BranchCode = ''
                            OR H.BranchCode = @BranchCode
                          )
                    GROUP BY D.ProdCode
                    ORDER BY SUM(D.Qty) DESC;";

                db.CommandType = CommandType.Text;

                AddFilterParameters(db, dashboard);

                db.Open();
                db.ExecuteReader();

                using (SqlDataReader dr =
                    db.DbDataReader as SqlDataReader)
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            dashboard.TopProducts.Add(
                                new TopSellingProduct
                                {
                                    ProdCode =
                                        Convert.ToString(dr["ProdCode"]) ?? "",

                                    ProdName =
                                        Convert.ToString(dr["ProdName"]) ?? "",

                                    QuantitySold =
                                        Tool.GeneralHelper.NullToDecimal(
                                            dr["QuantitySold"], 0),

                                    TotalSales =
                                        Tool.GeneralHelper.NullToDecimal(
                                            dr["TotalSales"], 0)
                                });
                        }
                    }
                }

                db.Close();
            }
        }

        // ==========================================
        // 10 TRANSAKSI TERBARU
        // ==========================================

        private static void LoadRecentTransactions(
            SalesDashboard dashboard)
        {
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = @"
                    SELECT TOP 10
                        TransCode,
                        TransDate,
                        BranchCode,
                        TransTotalAmt
                    FROM TransInvH
                    WHERE TransDate >= @DateFrom
                      AND TransDate < @DateTo
                      AND Status = 1
                      AND (
                            @BranchCode = ''
                            OR BranchCode = @BranchCode
                          )
                    ORDER BY TransDate DESC;";

                db.CommandType = CommandType.Text;

                AddFilterParameters(db, dashboard);

                db.Open();
                db.ExecuteReader();

                using (SqlDataReader dr =
                    db.DbDataReader as SqlDataReader)
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            dashboard.RecentTransactions.Add(
                                new RecentSalesTransaction
                                {
                                    TransCode =
                                        Convert.ToString(dr["TransCode"]) ?? "",

                                    TransDate =
                                        Convert.ToDateTime(dr["TransDate"]),

                                    BranchCode =
                                        Convert.ToString(dr["BranchCode"]) ?? "",

                                    TotalAmount =
                                        Tool.GeneralHelper.NullToDecimal(
                                            dr["TransTotalAmt"], 0)
                                });
                        }
                    }
                }

                db.Close();
            }
        }


        private static void LoadPendingTransactions(SalesDashboard dashboard)
        {
            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = @"
            SELECT COUNT(*) AS TotalPendingTransactions
            FROM TransInvH
            WHERE TransDate >= @DateFrom
              AND TransDate < @DateTo
              AND Status = 0
              AND (
                    @BranchCode = ''
                    OR BranchCode = @BranchCode
                  );";

                db.CommandType = CommandType.Text;

                AddFilterParameters(db, dashboard);

                db.Open();
                db.ExecuteReader();

                using (SqlDataReader dr =
                    db.DbDataReader as SqlDataReader)
                {
                    if (dr != null && dr.Read())
                    {
                        dashboard.TotalPendingTransactions =
                            Convert.ToInt32(
                                dr["TotalPendingTransactions"]);
                    }
                }

                db.Close();
            }
        }

    }




    // ==============================================
    // MODEL GRAFIK
    // ==============================================

    public class SalesChartData
    {
        public DateTime SalesDate { get; set; }

        public string Label { get; set; } = "";

        public decimal TotalSales { get; set; }
    }

    // ==============================================
    // MODEL BARANG TERLARIS
    // ==============================================

    public class TopSellingProduct
    {
        public string ProdCode { get; set; } = "";

        public string ProdName { get; set; } = "";

        public decimal QuantitySold { get; set; }

        public decimal TotalSales { get; set; }
    }

    // ==============================================
    // MODEL TRANSAKSI TERBARU
    // ==============================================

    public class RecentSalesTransaction
    {
        public string TransCode { get; set; } = "";

        public DateTime TransDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string BranchCode { get; set; } = "";
    }
}
