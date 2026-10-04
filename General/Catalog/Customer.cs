using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Catalog
{
    public class Customer
    {
        public string CustCode { get; set; }
        public string CustName { get; set; }
        public Customer() { }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> CustList()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> branches = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer(true))
            {
                db.CommandText = "select CustCode,CustName from tblcustomer";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        SelectListItem a = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();

                        while (dr.Read())
                        {
                            SelectListItem branch = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            branch.Text = IDS.Tool.GeneralHelper.NullToString(dr["CustName"]);
                            branch.Value = IDS.Tool.GeneralHelper.NullToString(dr["CustCode"]);
                            branches.Add(branch);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            branches = branches.OrderBy(x => x.Text).ToList();

            return branches;
        }
    }
}
