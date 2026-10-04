using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS.DataAccess;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace SPOS.Inventory.Stock
{
    public class Warehouse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Telp { get; set; }
        public string Fax { get; set; }
        public string ContactPerson { get; set; }
        public bool IsDefault { get; set; }
        public string EntryUser { get; set; }
        public DateTime EntryDate { get; set; }
        public string OperatorID { get; set; }
        public DateTime LastUpdate { get; set; }
        public string CodeDeliverPO { get; set; }
        public string CodeDeliverPOName { get; set; }
        public Warehouse() { }

        public static List<SelectListItem> GetWareHouseForDataSource()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "select * from Inwarehouse";
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 3);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SelectListItem wh = new SelectListItem();
                            wh.Value = IDS.Tool.GeneralHelper.NullToString(dr["WHCode"]);
                            wh.Text = wh.Value + " - " + IDS.Tool.GeneralHelper.NullToString(dr["WHName"]);

                            list.Add(wh);
                        }
                    }
                }

                db.Close();
            }

            return list;
        }
    }
}
