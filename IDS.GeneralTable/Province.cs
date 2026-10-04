using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;

namespace IDS.GeneralTable
{
    public sealed class Province
    {
        public string _code { get; set; }
        public string _name { get; set; }

        public string _createdBy { get; set; }
        public DateTime _createdDate { get; set; }
        public string _updatedBy { get; set; }
        public DateTime _updatedDate { get; set; }

        #region Properties
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        public string UpdatedBy
        {
            get { return _updatedBy; }
            set { _updatedBy = value; }
        }

        public DateTime UpdatedDate
        {
            get { return _updatedDate; }
            set { _updatedDate = value; }
        }
        #endregion
        

        public Province()
        {
        }

        public Province(string code)
            : this()
        {
            _code = code;
        }

        public Province(string code, string name)
            : this(code)
        {
            _name = name;
        }

        public int CreateUpdateDelete(IDS.Tool.PageActivity action)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "GTUpdProvince";
                db.AddParameter("@Code", SqlDbType.VarChar, _code);
                db.AddParameter("@Name", SqlDbType.VarChar, _name);
                db.AddParameter("@Operator", SqlDbType.VarChar, _updatedBy);
                db.AddParameter("@Type", SqlDbType.TinyInt, (int)action);
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.Open();

                db.BeginTransaction();
                result = db.ExecuteNonQuery();
                db.CommitTransaction();
            }

            return result;
        }

        public static List<Province> GetItemsList()
        {
            List<Province> items = new List<Province>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ProvinceCode, ProvinceName, OperatorID, LastUpdate FROM GTProvince";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader reader = db.DbDataReader as SqlDataReader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Province item = new Province(Convert.ToString(reader["ProvinceCode"]), Convert.ToString(reader["ProvinceName"]));
                            item.UpdatedBy = Convert.ToString(reader["OperatorID"]);
                            item.UpdatedDate = Convert.ToDateTime(reader["LastUpdate"]);

                            items.Add(item);
                        }
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }

                db.Close();
            }

            return items;
        }

        public static DataTable GetItemsDataTable(bool sort, string orderColumnName, string sortDirection)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                sb.Append("SELECT ProvinceCode, ProvinceName, OperatorID, LastUpdate FROM GTProvince");

                db.CommandText = sb.ToString();
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                dt = db.GetDataTable();

                db.Close();
            }

            if (dt != null && sort && !string.IsNullOrEmpty(orderColumnName))
            {
                IDS.Tool.DataTableExtension.SortDataTable(dt, orderColumnName, sortDirection);
            }

            return dt;
        }
        public static Province GetProvince(string ProvinceCode)
        {
            Province Province = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer())
            {
                db.CommandText = "Select * from GtProvince Where ProvinceCode = @Province";
                db.AddParameter("@Province", SqlDbType.VarChar,ProvinceCode);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader dr = db.DbDataReader as SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        Province = new Province();
                        Province.Code = dr["ProvinceCode"] as string;
                        Province.Name = dr["ProvinceName"] as string;
                        //country.Pr = dr["GnsProvince"] as string;
                        // country.EntryUser = dr["EntryUser"] as string;
                        // country.EntryDate = Convert.ToDateTime(dr["EntryDate"]);                                              
                        Province.CreatedBy = dr["OperatorID"] as string;
                        Province.CreatedDate = Convert.ToDateTime(dr["LastUpdate"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return Province;
        }

        public static DataTable GetItemsForDataSource()
        {
            DataTable dt = new DataTable();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ProvinceCode, ProvinceName FROM GTProvince ORDER BY ProvinceName ASC;";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                dt = db.GetDataTable();

                db.Close();
            }

            return dt;
        }

        public static Province GetItemByCode(string code)
        {
            Province item = null;

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT * FROM GTProvince WHERE ProvinceCode = @code";
                db.AddParameter("@code", SqlDbType.VarChar, code);
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader reader = db.DbDataReader as SqlDataReader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            item = new Province(Convert.ToString(reader["ProvinceCode"]), Convert.ToString(reader["ProvinceName"]));
                            item.UpdatedBy = Convert.ToString(reader["OperatorID"]);
                            item.UpdatedDate = Convert.ToDateTime(reader["LastUpdate"]);
                        }
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }

                db.Close();
            }

            return item;
        }
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetProvinces()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> items = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ProvinceCode, ProvinceName from GTProvince";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader reader = db.DbDataReader as SqlDataReader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem country = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            country.Value = reader["ProvinceCode"] as string;
                            country.Text = reader["ProvinceName"] as string;
                            items.Add(country);
                        }
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }

                db.Close();
            }

            return items;
        }

       
        public static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetProvinces(string Code)
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> items = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

            using (IDS.DataAccess.SqlServer db = new IDS.DataAccess.SqlServer())
            {
                db.CommandText = "SELECT ProvinceCode,ProvinceName from GTProvince WHERE ProvinceCode ='" + Code + "'";
               
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (SqlDataReader reader = db.DbDataReader as SqlDataReader)
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Microsoft.AspNetCore.Mvc.Rendering.SelectListItem country = new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem();
                            country.Value = reader["ProvinceCode"] as string;
                            country.Text = reader["ProvinceName"] as string;
                            items.Add(country);
                        }
                    }

                    if (!reader.IsClosed)
                        reader.Close();
                }

                db.Close();
            }

            return items;
        }
    }
}
