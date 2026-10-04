using IDS.GeneralTable;
using IDS.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class PPAPExcel
    {

        public string LeaseNo { get; set; }
        public string LesseeName { get; set; }
        public decimal OverDue { get; set; }

        public PPAPExcel()
        {

        }

        public static (List<string> headers, List<dynamic> SBC) GetScheduleByContract(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptPPAPPSAK";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetNewBookingLastYear(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));

            // Create DateTime objects for the start and end of July 2023
            DateTime startDate = new DateTime(year - 1, month, 1);
            DateTime endDate = new DateTime(year - 1, month, DateTime.DaysInMonth(year, month));

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptNewBookingLastYear";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@From", System.Data.SqlDbType.DateTime, startDate);
                db.AddParameter("@To", System.Data.SqlDbType.DateTime, endDate);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetNewBooking(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptNewBooking";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@From", System.Data.SqlDbType.DateTime, startDate);
                db.AddParameter("@To", System.Data.SqlDbType.DateTime, endDate);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetFactoringData(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select CASE WHEN FTTransH.AgreementNo = 'AP-FAS/KMF/2023/12/000016' THEN 'PT Mevtek Premier - Fal 2' ELSE Lessee.LesseeFullName END AS Nama, FTTransH.CreditLimit as Limit, FTTransH.EndAgreeDate as [Exp Date], FTTransH.DiscRate as [Rate Bunga(%)], FTSoaH.SOANo AS [No Daftar Penerimaan], FTOutstanding.OutPrincipal AS [Pokok Pembiayaan] , FTSoaH.SOATotal AS [Nilai Jaminan Invoice], FTSoaH.SOADate as [Start], FTSoaH.SOADateTo as [End], NULL AS KOL, NULL AS Kolektilibilitas from FTTransH left join FTSoaH on FTTransH.AgreementNo = FTSoaH.CustomerCode inner join Lessee on FTTransH.ClientID = Lessee.LesseeNo inner join FTOutstanding on FTTransH.AgreementNo = FTOutstanding.FacilityNo AND FTSoaH.SOANo = FTOutstanding.SOANo where MONTH(FTSoaH.SOADate) <= SUBSTRING(@Period,6,1) AND YEAR(FTSoaH.SOADate) = SUBSTRING(@Period,1,4) AND FTOutstanding.Period = @Period AND FTOutstanding.OutPrincipal > 0 ORDER BY Lessee.LesseeFullName";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.AddParameter("@PeriodDate", System.Data.SqlDbType.DateTime, endDate);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetFactoringDetail(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "select FTTransH.AgreementNo, CASE WHEN FTTransH.AgreementNo = 'AP-FAS/KMF/2023/12/000016' THEN 'PT Mevtek Premier - Fal 2' ELSE Lessee.LesseeFullName END AS Nama, CreditLimit as Restru, 1 as Restru_FI, NULL AS Kol, NULL AS [Min PPAP], NULL AS [Plafon], NULL AS [Outstanding], NULL AS [Sisa Limit], NULL AS Agunan, NULL AS EAD, NULL AS PD, NULL AS LGD, NULL AS ECL from FTTransH left join FTSoaH on FTTransH.AgreementNo = FTSoaH.CustomerCode inner join Lessee on FTTransH.ClientID = Lessee.LesseeNo inner join FTOutstanding on FTTransH.AgreementNo = FTOutstanding.FacilityNo AND FTSoaH.SOANo = FTOutstanding.SOANo where MONTH(FTSoaH.SOADate) <= SUBSTRING(@Period,6,1) AND YEAR(FTSoaH.SOADate) = SUBSTRING(@Period,1,4) AND FTOutstanding.Period = @Period AND FTOutstanding.OutPrincipal > 0 GROUP BY Lessee.LesseeFullName,FTTransH.AgreementNo, CreditLimit";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetFactoringCalculation(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "WITH Numbers AS (SELECT 1 AS N UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6) SELECT NULL AS Saldo, NULL AS PPAP FROM Numbers";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetMultiguna(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetHystoricalMultiguna(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptHystoricalMultiguna";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetDataMultiguna(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptDataMultiguna";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetHystoricalInvestasi(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptHystoricalInvestasi";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetDataLeasing(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptDataLeasing";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetHystoricalModalKerja(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptHystoricalMultiguna";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetDataModalKerja(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptDataModalKerja";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static (List<string> headers, List<dynamic> SBC) GetHystoricalFactoring(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptHystoricalFactoring";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@period", System.Data.SqlDbType.VarChar, period);
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        // Retrieve column names (headers) from the result set metadata
                        var schemaTable = dr.GetSchemaTable();
                        foreach (System.Data.DataRow row in schemaTable.Rows)
                        {
                            headers.Add(row["ColumnName"].ToString());
                        }

                        while (dr.Read())
                        {
                            dynamic pph = new ExpandoObject();
                            var pphDict = pph as IDictionary<string, object>;

                            foreach (var header in headers)
                            {
                                var value = dr[header];
                                pphDict[header] = value;
                            }

                            SBC.Add(pph);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return (headers, SBC);
        }
        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            // Calculate the first day of the next month
            DateTime firstDayOfNextMonth = date.AddMonths(1).AddDays(-date.Day + 1);

            // The last day of the current month is one day before the first day of the next month
            DateTime lastDayOfMonth = firstDayOfNextMonth.AddDays(-1);

            return lastDayOfMonth;
        }
    }
}
