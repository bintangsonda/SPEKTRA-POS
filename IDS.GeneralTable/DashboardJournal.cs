using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public class DashboardJournal
    {
        public int TotalJournal { get; set; }
        public int TotalPayment { get; set; }
        public int TotalTermination { get; set; }

        public DashboardJournal()
        {

        }

        public static DashboardJournal GetData()
        {
            IDS.GeneralTable.DashboardJournal dash = null;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "LoadDashboardJournal";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        dash = new DashboardJournal();
                        //Tinggal diisi sesuai dengan kebutuhan
                        dash.TotalJournal = Convert.ToInt32(dr["TotalJournalNotPosted"]);

                        if (!dr.IsClosed)
                        {
                            dr.Close();
                        }
                    }

                    db.Close();
                }

                return dash;
            }
        }
    }
}