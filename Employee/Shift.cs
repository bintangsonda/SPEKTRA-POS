using IDS.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public string Branch { get; set; }
        public string ShiftUser { get; set; }
        public DateTime StartShift { get; set; }
        public string StartShiftString { get; set; }
        public DateTime CloseShift { get; set; }
        public string CloseShiftString { get; set; }
        public decimal KasAwal { get; set; }
        public decimal KasAkhir { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }

        public Shift()
        {

        }
        
        public static List<Shift> GetDataShift(DateTime Dates, string Outlet)
        {
            List<Shift> data = new List<Shift>();

            using (SqlServer db = new SqlServer())
            {
                db.CommandText = "select * from Shift where Branch=@Outlet and startShift=@StartShift";
                db.AddParameter("@Branch", System.Data.SqlDbType.VarChar, Outlet);
                db.AddParameter("@StartShift", System.Data.SqlDbType.DateTime, Dates);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Shift details = new Shift();
                            details.ShiftID = IDS.Tool.GeneralHelper.NullToInt(dr["ShiftID"],0);
                            details.Branch = IDS.Tool.GeneralHelper.NullToString(dr["Branch"]);
                            details.ShiftUser = IDS.Tool.GeneralHelper.NullToString(dr["ShiftUser"]);
                            details.StartShift = IDS.Tool.GeneralHelper.NullToDateTime(dr["StartShift"],DateTime.MinValue);
                            details.CloseShift = IDS.Tool.GeneralHelper.NullToDateTime(dr["StartShift"], DateTime.MinValue);
                            if (details.StartShift == DateTime.MinValue)
                            {
                                details.StartShiftString = "";
                            }
                            else
                            {
                                details.StartShiftString = details.StartShift.ToString("dd/MM/yyyy");
                            }
                            if (details.CloseShift == DateTime.MinValue)
                            {
                                details.CloseShiftString = "";
                            }
                            else
                            {
                                details.CloseShiftString = details.StartShift.ToString("dd/MM/yyyy");
                            }
                            details.KasAwal = IDS.Tool.GeneralHelper.NullToDecimal(dr["ProdCode"],0);
                            details.KasAkhir = IDS.Tool.GeneralHelper.NullToDecimal(dr["ProdCode"], 0);
                            data.Add(details);
                        }


                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return data;
        }

    }
}
