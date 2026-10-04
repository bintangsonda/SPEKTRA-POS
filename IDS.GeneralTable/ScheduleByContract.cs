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
    public class ScheduleByContract
    {

        public string LeaseNo { get; set; }
        public string LesseeName { get; set; }
        public decimal OverDue { get; set; }

        public ScheduleByContract()
        {

        }

        public static (List<string> headers, List<dynamic> SBC) GetScheduleByContract(string period)
        {
            List<string> headers = new List<string>();
            List<dynamic> SBC = new List<dynamic>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "RptSchedulePembiayaan";
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
    }
}
