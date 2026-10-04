using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Maintenance
{
    public class History
    {
        public int Id { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public DateTime DateUpdated { get; set; }
        public string TableName { get; set; }
        public string KeyField1 { get; set; }
        public string KeyField2 { get; set; }
        public string KeyField3 { get; set; }
        public string KeyField4 { get; set; }
        public string KeyField5 { get; set; }
        public string KeyField6 { get; set; }

        public string UpdateValue { get; set; }
        public string OldValue { get; set; }
        public string IpAddress { get; set; }
        public string Status { get; set; }


        public History()
        {

        }
        public static List<History> GetHistories(string period)
        {
            List<History> histories = new List<History>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {

                string year = period.Substring(0, 4);
                string month = period.Substring(4, 2);
                db.CommandText = "MntSelHistory";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 1);
                db.AddParameter("@month", System.Data.SqlDbType.VarChar, month);
                db.AddParameter("@year", System.Data.SqlDbType.VarChar, year);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                       
                        while(dr.Read())
                        {
                            History history = new History();
                            history.Id = Tool.GeneralHelper.NullToInt(dr["Id"], 0);
                            history.ComputerName = Tool.GeneralHelper.NullToString(dr["ComputerName"]);
                            history.Status = Tool.GeneralHelper.NullToString(dr["Status"]);
                            history.UserName = Tool.GeneralHelper.NullToString(dr["UserName"]);
                            history.KeyField1 = Tool.GeneralHelper.NullToString(dr["KeyField1"]);
                            history.KeyField2 = Tool.GeneralHelper.NullToString(dr["KeyField2"]);
                            history.KeyField3 = Tool.GeneralHelper.NullToString(dr["KeyField3"]);
                            history.KeyField4 = Tool.GeneralHelper.NullToString(dr["KeyField4"]);
                            history.KeyField5 = Tool.GeneralHelper.NullToString(dr["KeyField5"]);
                            history.KeyField6 = Tool.GeneralHelper.NullToString(dr["KeyField6"]);
                            history.IpAddress = Tool.GeneralHelper.NullToString(dr["IpAddress"]);
                            history.DateUpdated = Convert.ToDateTime(dr["DateUpdated"]);
                            history.TableName = Tool.GeneralHelper.NullToString(dr["TableName"]);
                            history.OldValue = Tool.GeneralHelper.NullToString(dr["OldValue"]);
                            history.UpdateValue = Tool.GeneralHelper.NullToString(dr["UpdateValue"]);
                            histories.Add(history);
                        }

                       
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return histories;
        }

        public static History GetHistory(int id)
        {
            History his = null;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelHistory";
                db.CommandType = System.Data.CommandType.StoredProcedure;

                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 2);
                db.AddParameter("@Id",System.Data.SqlDbType.Int, id);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while(dr.Read())
                        {
                            his = new History();
                            his.Id = Tool.GeneralHelper.NullToInt(dr["Id"], 0);
                            his.ComputerName = Tool.GeneralHelper.NullToString(dr["ComputerName"]);
                            his.UserName = Tool.GeneralHelper.NullToString(dr["UserName"]);
                            his.KeyField1 = Tool.GeneralHelper.NullToString(dr["KeyField1"]);
                            his.KeyField2 = Tool.GeneralHelper.NullToString(dr["KeyField2"]);
                            his.KeyField3 = Tool.GeneralHelper.NullToString(dr["KeyField3"]);
                            his.KeyField4 = Tool.GeneralHelper.NullToString(dr["KeyField4"]);
                            his.KeyField5 = Tool.GeneralHelper.NullToString(dr["KeyField5"]);
                            his.KeyField6 = Tool.GeneralHelper.NullToString(dr["KeyField6"]);
                            his.DateUpdated = Tool.GeneralHelper.NullToDateTime(dr["DateUpdated"], DateTime.Now);
                            his.TableName = Tool.GeneralHelper.NullToString(dr["TableName"]);
                            his.OldValue = Tool.GeneralHelper.NullToString(dr["OldValue"]);
                            his.Status = Tool.GeneralHelper.NullToString(dr["Status"]);
                            his.IpAddress = Tool.GeneralHelper.NullToString(dr["IpAddress"]);
                            
                            his.UpdateValue = Tool.GeneralHelper.NullToString(dr["UpdateValue"]);
                        }
                        
                            
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return his;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetStatus()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> stats = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            stats.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "ALL", Value = "ALL" });
            stats.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "INSERT", Value = "INSERT" });
            stats.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "UPDATE", Value = "UPDATE" });
            stats.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "DELETE", Value = "DELETE" });
            stats.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "PROCESS", Value = "PROCESS" });

            return stats;
        }

        

        public static string GetLastLogin(string id)
        {
            string his = null;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                
                db.CommandText = "select top(1) DateUpdated from(select top(2) ID,DateUpdated from tbltranslog where Status = 'LOGIN' and username = '" + id + "' order by id desc) as sub order by DateUpdated asc ";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                         
                            his = Tool.GeneralHelper.NullToString(dr["DateUpdated"]);
                        }


                    }
                    //Add by Jeremi 17 September 2024
                    else
                    {
                        his = Convert.ToString(DateTime.Now);
                    }
                    //End Jeremi
                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return his;
        }
    }
}
