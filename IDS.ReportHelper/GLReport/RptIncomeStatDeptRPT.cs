using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.ReportHelper.GLReport
{
    public class RptIncomeStatDeptRPT
    {
        public static DataTable GetData(string pPeriod, string pCode, string dept)
        {
            System.Data.DataTable dt = null;

            using (IDS.DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SP_IncomeStatDep";
                db.AddParameter("@pPeriod", System.Data.SqlDbType.VarChar, pPeriod);
                db.AddParameter("@pCode", System.Data.SqlDbType.VarChar, pCode);
                db.AddParameter("@dept", System.Data.SqlDbType.VarChar, dept);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                dt = db.GetDataTable();
            }

            return dt;
        }

        public static List<SelectListItem> GetDepartmentForDataSource(string branchCode)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "SELECT Distinct CODE, NAME FROM ACFDEPT WHERE BranchCode =@branchcode";
                db.AddParameter("@branchcode", System.Data.SqlDbType.VarChar, branchCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string departmentCode = dr["CODE"].ToString();
                            string departmentName = dr["Name"].ToString();
                            list.Add(new SelectListItem() { Text = departmentName, Value = departmentCode });
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
