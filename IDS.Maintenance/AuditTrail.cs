using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Maintenance
{
    public class AuditTrail
    {
        public int ID { get; set; }
        public string TableName { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
        public string Key { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<AuditDetail> Details { get; set; } = new List<AuditDetail>();

        public AuditTrail()
        {
            
        }
        public static List<AuditTrail> GetData(DateTime? from, DateTime? to, string user, string table)
        {
            List<AuditTrail> list = new List<AuditTrail>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "sp_GetAuditData";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@StartDate", System.Data.SqlDbType.DateTime, from);
                db.AddParameter("@EndDate", System.Data.SqlDbType.DateTime, to);
                db.AddParameter("@TableName", System.Data.SqlDbType.VarChar, Tool.GeneralHelper.StringToDBNull(table));
                db.AddParameter("@UserId", System.Data.SqlDbType.VarChar, Tool.GeneralHelper.StringToDBNull(user));
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            AuditTrail audit = new AuditTrail();
                            audit.ID = Tool.GeneralHelper.NullToInt(dr["ID"], 0);
                            audit.UserId = Tool.GeneralHelper.NullToString(dr["UserName"]);
                            audit.Status = Tool.GeneralHelper.NullToString(dr["Status"]);
                            audit.TableName = Tool.GeneralHelper.NullToString(dr["TableName"]);
                            audit.UpdatedDate = Tool.GeneralHelper.NullToDateTime(dr["DateUpdated"], DateTime.Now);
                            audit.Key = Tool.GeneralHelper.NullToString(dr["Key"]);
                            string json = Tool.GeneralHelper.NullToString(dr["ChangeJSON"]);
                            if (!string.IsNullOrEmpty(json))
                            {
                                if (json.TrimStart().StartsWith("["))
                                {
                                    var details = JsonConvert.DeserializeObject<List<AuditDetail>>(json);
                                    audit.Details = details
                                        .Select(d => new AuditDetail
                                        {
                                            ColumnName = d.ColumnName,
                                            OldValue = d.OldValue,
                                            NewValue = d.NewValue
                                        }).ToList();
                                    audit.Details = details;
                                }
                                else if (json.StartsWith("{"))
                                {
                                    var obj = JObject.Parse(json);
                                    var details = new List<AuditDetail>();

                                    foreach (var prop in obj.Properties())
                                    {
                                        details.Add(new AuditDetail
                                        {
                                            ColumnName = prop.Name,
                                            OldValue = "",
                                            NewValue = prop.Value?.ToString()
                                        });
                                    }

                                    audit.Details = details;
                                }
                            }
                            list.Add(audit);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }
        public static List<AuditTrail> GetAuditTrailsSingle(string Key)
        {
            List<AuditTrail> list = new List<AuditTrail>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "sp_GetAuditLogSingle";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@KeyList", System.Data.SqlDbType.Int, Key);
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            AuditTrail audit = new AuditTrail();
                            audit.UserId = Tool.GeneralHelper.NullToString(dr["UserName"]);
                            audit.Status = Tool.GeneralHelper.NullToString(dr["Status"]);
                            audit.UpdatedDate = Tool.GeneralHelper.NullToDateTime(dr["DateUpdated"], DateTime.Now);
                            audit.Key = Key;
                            string json = Tool.GeneralHelper.NullToString(dr["ChangeJSON"]);
                            if (!string.IsNullOrEmpty(json))
                            {
                                if (json.TrimStart().StartsWith("["))
                                {
                                    var details = JsonConvert.DeserializeObject<List<AuditDetail>>(json);
                                    audit.Details = details
                                        .Select(d => new AuditDetail
                                        {
                                            ColumnName = d.ColumnName,
                                            OldValue = d.OldValue,
                                            NewValue = d.NewValue
                                        }).ToList();
                                    audit.Details = details;
                                }
                                else if (json.StartsWith("{"))
                                {
                                    var obj = JObject.Parse(json);
                                    var details = new List<AuditDetail>();

                                    foreach (var prop in obj.Properties())
                                    {
                                        details.Add(new AuditDetail
                                        {
                                            ColumnName = prop.Name,
                                            OldValue = "", 
                                            NewValue = prop.Value?.ToString()
                                        });
                                    }

                                    audit.Details = details;
                                }
                            }
                            list.Add(audit);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }
        public static List<AuditTrail> GetAuditTrails(string TableName,string Key)
        {
            List<AuditTrail> list = new List<AuditTrail>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "sp_GetAuditLog";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@TableName", System.Data.SqlDbType.VarChar, TableName);
                db.AddParameter("@KeyList", System.Data.SqlDbType.VarChar, Key);
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            AuditTrail audit = new AuditTrail();
                            audit.UserId = Tool.GeneralHelper.NullToString(dr["UserName"]);
                            audit.Status = Tool.GeneralHelper.NullToString(dr["Status"]);
                            audit.UpdatedDate = Tool.GeneralHelper.NullToDateTime(dr["DateUpdated"], DateTime.Now);
                            audit.Key = Key;
                            string json = Tool.GeneralHelper.NullToString(dr["ChangeJSON"]);
                            if (!string.IsNullOrEmpty(json))
                            {
                                if (json.TrimStart().StartsWith("["))
                                {
                                    var details = JsonConvert.DeserializeObject<List<AuditDetail>>(json);
                                    audit.Details = details
                                        .Select(d => new AuditDetail
                                        {
                                            ColumnName = d.ColumnName,
                                            OldValue = d.OldValue,
                                            NewValue = d.NewValue
                                        }).ToList();
                                    audit.Details = details;
                                }
                                else if (json.StartsWith("{"))
                                {
                                    var obj = JObject.Parse(json);
                                    var details = new List<AuditDetail>();

                                    foreach (var prop in obj.Properties())
                                    {
                                        details.Add(new AuditDetail
                                        {
                                            ColumnName = prop.Name,
                                            OldValue = "",
                                            NewValue = prop.Value?.ToString()
                                        });
                                    }

                                    audit.Details = details;
                                }
                            }
                            list.Add(audit);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }

        public static List<AuditTrail> GetAuditTrailsHD(string TableNameH,string TableNameD, string Key)
        {
            List<AuditTrail> list = new List<AuditTrail>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "sp_GetAuditLogHD";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@TableNameHeader", System.Data.SqlDbType.VarChar, TableNameH);
                db.AddParameter("@TableNameDetail", System.Data.SqlDbType.VarChar, TableNameD);
                db.AddParameter("@KeyList", System.Data.SqlDbType.VarChar, Key);
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            AuditTrail audit = new AuditTrail();
                            audit.UserId = Tool.GeneralHelper.NullToString(dr["UserName"]);
                            audit.Status = Tool.GeneralHelper.NullToString(dr["Status"]);
                            audit.UpdatedDate = Tool.GeneralHelper.NullToDateTime(dr["DateUpdated"], DateTime.Now);
                            audit.Key = Key;
                            string json = Tool.GeneralHelper.NullToString(dr["ChangeJSON"]);
                            if (!string.IsNullOrEmpty(json))
                            {
                                if (json.TrimStart().StartsWith("["))
                                {
                                    var details = JsonConvert.DeserializeObject<List<AuditDetail>>(json);
                                    audit.Details = details
                                        .Select(d => new AuditDetail
                                        {
                                            ColumnName = d.ColumnName,
                                            OldValue = d.OldValue,
                                            NewValue = d.NewValue
                                        }).ToList();
                                    audit.Details = details;
                                }
                                else if (json.StartsWith("{"))
                                {
                                    var obj = JObject.Parse(json);
                                    var details = new List<AuditDetail>();

                                    foreach (var prop in obj.Properties())
                                    {
                                        details.Add(new AuditDetail
                                        {
                                            ColumnName = prop.Name,
                                            OldValue = "",
                                            NewValue = prop.Value?.ToString()
                                        });
                                    }

                                    audit.Details = details;
                                }
                            }
                            list.Add(audit);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return list;
        }


        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetTable()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> tables = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "select distinct tablename from tblaudittrail";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        tables = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem currency = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            currency.Value = dr["tablename"] as string;
                            currency.Text = dr["tablename"] as string;

                            tables.Add(currency);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            

            return tables;
        }

        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetUser()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> tables = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "select userid from mntuser";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        tables = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                        while (dr.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem currency = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            currency.Value = dr["userid"] as string;
                            currency.Text = dr["userid"] as string;

                            tables.Add(currency);
                        }
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }


            return tables;
        }
    }

    public class AuditDetail
    {
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
