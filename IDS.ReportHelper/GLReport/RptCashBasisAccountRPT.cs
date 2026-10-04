using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptCashBasisAccountRPT
    {
        public static DataTable GetData()
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM ACCGLMH";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }
    }
}
