using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IDS.Maintenance
{
    public class UserMenu
    {
        [Display(Name = "Menu Number")]
        public int MenuNumber { get; set; }

        [Display(Name = "Menu Code")]
        public string MenuCode { get; set; }

        [Display(Name = "Menu Parent Code")]
        public string MenuParentCode { get; set; }

        [Display(Name = "Menu Project")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Menu project is required")]
        public string MenuProject { get; set; }

        [Display(Name = "Menu Level")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Menu level is required")]
        public int MenuLevel { get; set; }

        [Display(Name = "Menu Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Menu name is required")]
        public string MenuName { get; set; }

        [Display(Name = "Menu URL")]
        public string MenuURL { get; set; }

        [Display(Name = "Menu ToolTip")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Menu tooltip is required")]
        public string MenuToolTip { get; set; }

        [Display(Name = "Menu Controller")]
        public string Controller { get; set; }

        [Display(Name = "Menu Area")]
        public string Area { get; set; }
        public string Icon { get; set; }

        [Display(Name = "Created By")]
        public string EntryUser { get; set; }

        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "dd/MMM/YYYY HH:mm:ss", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Operator ID")]
        public string OperatorID { get; set; }

        [Display(Name = "Last Update")]
        [DisplayFormat(DataFormatString = "dd/MMM/YYYY HH:mm:ss", ConvertEmptyStringToNull = true, NullDisplayText = "")]
        public DateTime LastUpdate { get; set; }

        public UserMenu()
        {
        }

        public static UserMenu GetMenu(string menuCode)
        {
            UserMenu user = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSaveMenu";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, menuCode);
                db.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 6);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();

                        user = new UserMenu();
                        user.MenuCode = dr["MenuCode"] as string;
                        user.MenuName = dr["MenuName"] as string;
                        user.MenuProject = dr["MenuProject"] as string;
                        user.MenuLevel = Tool.GeneralHelper.NullToInt(dr["MenuLevel"], 0);
                        user.MenuURL = Tool.GeneralHelper.NullToString(dr["MenuUrl"], "");
                        user.MenuToolTip = Tool.GeneralHelper.NullToString(dr["MenuToolTip"], "");
                        user.Controller = Tool.GeneralHelper.NullToString(dr["Controller"], "");
                        user.Area = Tool.GeneralHelper.NullToString(dr["Area"], "");
                        user.OperatorID = Tool.GeneralHelper.NullToString(dr["OperatorID"], "");
                        user.LastUpdate = Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.Now);
                        user.MenuNumber = Tool.GeneralHelper.NullToInt(dr["MenuNumber"], 0);
                        user.Icon = Tool.GeneralHelper.NullToString(dr["MenuIcon"]);
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return user;
        }
        public static List<UserMenu> GetParentMenu()
        {
            List<UserMenu> parentMenu = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "SELECT * FROM mntWebMenu where MenuLevel = 0 ORDER BY MenuCode";
                db.CommandType = System.Data.CommandType.Text;
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        parentMenu = new List<UserMenu>();

                        while (dr.Read())
                        {
                            UserMenu menu = new UserMenu();
                            menu.MenuNumber = Convert.ToInt32(dr["MenuNumber"]);
                            menu.MenuCode = dr["MenuCode"].ToString();
                            menu.MenuProject = dr["MenuProject"].ToString();
                            menu.MenuLevel = Convert.ToInt32(dr["MenuLevel"]);
                            menu.MenuName = (dr["MenuName"] == DBNull.Value) ? "" : dr["MenuName"].ToString();
                            menu.MenuURL = dr["MenuUrl"] == DBNull.Value ? "" : dr["MenuUrl"].ToString();
                            menu.MenuToolTip = dr["MenuToolTip"] == DBNull.Value ? "" : dr["MenuToolTip"].ToString();
                            menu.Area = dr["Area"] as string;
                            menu.Icon = Tool.GeneralHelper.NullToString(dr["MenuIcon"]);
                            menu.Controller = dr["Controller"] as string;

                            parentMenu.Add(menu);
                        }

                        if (!dr.IsClosed)
                            dr.Close();
                    }

                    db.Close();
                }
            }

            return parentMenu;
        }

        public static List<UserMenu> GetChildMenu(string menuCode, string groupCode)
        {
            List<UserMenu> menuList = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelMenu";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, menuCode);
                db.AddParameter("@Grp", System.Data.SqlDbType.VarChar, groupCode);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        menuList = new List<UserMenu>();

                        while (dr.Read())
                        {
                            UserMenu menu = new UserMenu();
                            menu.MenuNumber = Convert.ToInt32(dr["MenuNumber"]);
                            menu.MenuCode = dr["MenuCode"].ToString();
                            menu.MenuProject = dr["MenuProject"].ToString();
                            menu.MenuLevel = Convert.ToInt32(dr["MenuLevel"]);
                            menu.MenuName = (dr["MenuName"] == DBNull.Value) ? "" : dr["MenuName"].ToString();
                            menu.MenuURL = dr["MenuUrl"] == DBNull.Value ? "" : dr["MenuUrl"].ToString();
                            menu.MenuToolTip = dr["MenuToolTip"] == DBNull.Value ? "" : dr["MenuToolTip"].ToString();
                            menu.Area = dr["Area"] == DBNull.Value ? "" : dr["Area"].ToString();
                            menu.Icon = Tool.GeneralHelper.NullToString(dr["MenuIcon"]);
                            menu.Controller = dr["Controller"] == DBNull.Value ? "" : dr["Controller"].ToString();

                            menuList.Add(menu);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return menuList;
        }

        public static int CheckChildMenu(string menuCode, string groupCode, int type)
        {
            int total = 0;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "SelMenuChild";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, menuCode);
                db.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, groupCode);
                db.AddParameter("@type", System.Data.SqlDbType.VarChar, type);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            total = Tool.GeneralHelper.NullToInt(dr["TOTAL"], 0);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }
            return total;
        }

        public static List<UserMenu> GetChildMenu(List<UserMenu> menuList, string menuCode, string groupCode)
        {
            if (menuList == null)
                menuList = new List<UserMenu>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelMenu";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Code", System.Data.SqlDbType.VarChar, menuCode);
                db.AddParameter("@Grp", System.Data.SqlDbType.VarChar, groupCode);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            UserMenu menu = new UserMenu();
                            menu.MenuNumber = Convert.ToInt32(dr["MenuNumber"]);
                            menu.MenuCode = dr["MenuCode"].ToString();
                            menu.MenuProject = dr["MenuProject"].ToString();
                            menu.MenuLevel = Convert.ToInt32(dr["MenuLevel"]);
                            menu.MenuName = (dr["MenuName"] == DBNull.Value) ? "" : dr["MenuName"].ToString();
                            menu.MenuURL = dr["MenuUrl"] == DBNull.Value ? "" : dr["MenuUrl"].ToString();
                            menu.MenuToolTip = dr["MenuToolTip"] == DBNull.Value ? "" : dr["MenuToolTip"].ToString();
                            menu.Area = dr["Area"] == DBNull.Value ? "" : dr["Area"].ToString();
                            menu.Icon = Tool.GeneralHelper.NullToString(dr["MenuIcon"]);
                            menu.Controller = dr["Controller"] == DBNull.Value ? "" : dr["Controller"].ToString();

                            menuList.Add(menu);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return menuList;
        }



        public int InsUpDel(int ExecCode)
        {
            int result = 0;

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    if (ExecCode == 1 && MenuCode != null)
                    {
                        throw new Exception("Menu code is already exists. Please choose other Menu code.");
                    }
                    if (ExecCode == 1)
                    {
                        if (MenuLevel == 0)
                        {
                            cmd.CommandText = "SELECT DISTINCT ISNULL(MAX(LEFT(MenuCode, 2)), '00') + 1 FROM mntWebMenu";
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Open();
                            var query = Convert.ToString(cmd.ExecuteScalar());
                            MenuCode = '0' + query;

                            if (MenuCode == "00")
                                MenuCode = "01";

                            MenuCode = MenuCode.PadRight(16, '0');
                        }
                        else
                        {
                            MenuCode = MenuParentCode.Substring(0, MenuLevel * 2);

                            //cmd.CommandText = "SELECT MAX(MenuCode) FROM mntWebMenu WHERE MenuCode LIKE @menuCode + '%' AND MenuLevel = @level";
                            cmd.CommandText = "SELECT CAST(LEFT(MAX(MenuCode),(@level+1) * 2) + 1 AS INT) FROM mntWebMenu WHERE MenuCode LIKE  @menuCode + '%' AND MenuLevel = @level";
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.AddParameter("@menuCode", System.Data.SqlDbType.VarChar, MenuCode);
                            cmd.AddParameter("@level", System.Data.SqlDbType.TinyInt, MenuLevel);
                            cmd.Open();
                            var query = Convert.ToString(cmd.ExecuteScalar());
                            if (query == "")
                            {
                                MenuCode = MenuCode + "01";
                            }
                            else
                            {
                                MenuCode = '0' + query;
                            }


                            if (MenuCode == "00")
                                MenuCode = "01";

                            MenuCode = MenuCode.PadRight(16, '0');
                        }
                    }

                    cmd.CommandText = "MntSaveMenu";
                    cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, ExecCode);


                    if (ExecCode == 1)
                    {
                        cmd.AddParameter("@MenuNumber", System.Data.SqlDbType.BigInt, DBNull.Value);
                    }
                    else
                        cmd.AddParameter("@MenuNumber", System.Data.SqlDbType.BigInt, MenuNumber);

                    cmd.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, MenuCode);
                    cmd.AddParameter("@MenuName", System.Data.SqlDbType.VarChar, MenuName);
                    cmd.AddParameter("@MenuProject", System.Data.SqlDbType.VarChar, MenuProject);
                    cmd.AddParameter("@MenuLevel", System.Data.SqlDbType.TinyInt, MenuLevel);
                    cmd.AddParameter("@MenuUrl", System.Data.SqlDbType.VarChar, MenuURL);
                    cmd.AddParameter("@ControllerName", System.Data.SqlDbType.VarChar, Controller);
                    cmd.AddParameter("@AreaName", System.Data.SqlDbType.VarChar, Area);
                    cmd.AddParameter("@MenuIcon", System.Data.SqlDbType.VarChar, Icon);
                    cmd.AddParameter("@MenuToolTip", System.Data.SqlDbType.VarChar, MenuToolTip);
                    cmd.AddParameter("@LogUser", System.Data.SqlDbType.VarChar, OperatorID);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();

                    if (ExecCode == 1)
                    {
                        if (MenuLevel != 0)
                        {
                            List<UserGroup> usrGroups = IDS.Maintenance.UserGroup.GetUserGroup().ToList();
                            if (usrGroups != null)
                            {
                                cmd.CommandText = "MntSaveMenu";
                                cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 5);
                                cmd.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, MenuCode);
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Open();

                                cmd.BeginTransaction();
                                result = cmd.ExecuteNonQuery();
                                cmd.CommitTransaction();
                                foreach (var a in usrGroups)
                                {
                                    cmd.CommandText = "MntSaveMenu";
                                    cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 4);
                                    cmd.AddParameter("@GroupCode", System.Data.SqlDbType.VarChar, a.GroupCode);
                                    cmd.AddParameter("@MenuProject", System.Data.SqlDbType.VarChar, MenuProject);
                                    cmd.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, MenuCode);
                                    cmd.AddParameter("@MenuIcon", System.Data.SqlDbType.VarChar, Icon);
                                    cmd.AddParameter("@MenuUrl", System.Data.SqlDbType.VarChar, MenuURL);
                                    cmd.AddParameter("@Akses", System.Data.SqlDbType.TinyInt, 0);
                                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                    cmd.Open();

                                    cmd.BeginTransaction();
                                    result = cmd.ExecuteNonQuery();
                                    cmd.CommitTransaction();
                                }
                            }
                        }

                    }
                    else
                    {
                        cmd.CommandText = "MntSaveMenu";
                        cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, 7);
                        cmd.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, MenuCode);
                        cmd.AddParameter("@MenuProject", System.Data.SqlDbType.VarChar, MenuProject);
                        cmd.AddParameter("@MenuUrl", System.Data.SqlDbType.VarChar, MenuURL);
                        cmd.AddParameter("@MenuIcon", System.Data.SqlDbType.VarChar, Icon);
                        cmd.BeginTransaction();
                        result = cmd.ExecuteNonQuery();
                        cmd.CommitTransaction();


                    }






                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Menu code is already exists. Please choose other Menu code.");
                        default:
                            throw;
                    }
                }
                catch
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
                }
                finally
                {
                    cmd.Close();
                }
            }

            return result;
        }

        public int InsUpDelMenu(int ExecCode, string[] data)
        {
            int result = 0;

            if (data == null)
                throw new Exception("No data found");

            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {
                    cmd.CommandText = "MntSaveMenu";
                    cmd.Open();
                    cmd.BeginTransaction();

                    for (int i = 0; i < data.Length; i++)
                    {
                        cmd.CommandText = "MntSaveMenu";
                        cmd.AddParameter("@Init", System.Data.SqlDbType.TinyInt, ExecCode);
                        cmd.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, data[i]);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.CommitTransaction();
                }
                catch (SqlException sex)
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    switch (sex.Number)
                    {
                        case 2627:
                            throw new Exception("Menu Code is already exists. Please choose other Menu Code.");
                        case 547:
                            throw new Exception("One or more data can not be delete while data used for reference.");
                        default:
                            throw;
                    }
                }
                catch
                {
                    if (cmd.Transaction != null)
                        cmd.RollbackTransaction();

                    throw;
                }
                finally
                {
                    cmd.Close();
                }
            }

            return result;
        }
        public static List<UserMenu> GetAllUserMenu()
        {
            List<UserMenu> menuList = new List<UserMenu>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                //db.CommandText = "MntSelMenuNoAccess";
                db.CommandText = "MntSelGenMenu";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 1);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        menuList = new List<UserMenu>();

                        while (dr.Read())
                        {
                            UserMenu menu = new UserMenu();
                            menu.MenuNumber = Convert.ToInt32(dr["MenuNumber"]);
                            menu.MenuCode = dr["MenuCode"].ToString();
                            menu.MenuProject = dr["MenuProject"].ToString();
                            menu.MenuLevel = Convert.ToInt32(dr["MenuLevel"]);
                            menu.MenuName = (dr["MenuName"] == DBNull.Value) ? "" : dr["MenuName"].ToString();
                            menu.MenuURL = dr["MenuUrl"] == DBNull.Value ? "" : dr["MenuUrl"].ToString();
                            menu.MenuToolTip = dr["MenuToolTip"] == DBNull.Value ? "" : dr["MenuToolTip"].ToString();
                            menu.Controller = dr["Controller"] == DBNull.Value ? "" : dr["Controller"].ToString();
                            menu.Area = dr["Area"] == DBNull.Value ? "" : dr["Area"].ToString();

                            menu.Icon = Tool.GeneralHelper.NullToString(dr["MenuIcon"]);
                            menu.OperatorID = dr["OperatorID"] == DBNull.Value ? "" : dr["OperatorID"].ToString();
                            menu.LastUpdate = Tool.GeneralHelper.NullToDateTime(dr["LastUpdate"], DateTime.Now);
                            menu.EntryUser = Tool.GeneralHelper.NullToString(dr["ENTRYUSER"], "");
                            menu.EntryDate = Tool.GeneralHelper.NullToDateTime(dr["ENTRYDATE"], DateTime.Now);

                            menuList.Add(menu);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return menuList;
        }

        public static UserMenu GetUserMenuByMenuNumber(int menuNumber)
        {
            IDS.Maintenance.UserMenu menu = null;

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelGenMenu";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@MenuProject", System.Data.SqlDbType.VarChar, DBNull.Value);
                db.AddParameter("@MenuLevel", System.Data.SqlDbType.TinyInt, DBNull.Value);
                db.AddParameter("@MenuNumber", System.Data.SqlDbType.Int, menuNumber);
                db.AddParameter("@Type", System.Data.SqlDbType.TinyInt, 4);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        menu = new UserMenu();
                        menu.MenuNumber = Convert.ToInt32(dr["MenuNumber"]);
                        menu.MenuCode = dr["MenuCode"].ToString();

                        menu.Icon = Tool.GeneralHelper.NullToString(dr["MenuIcon"]);
                        menu.MenuProject = dr["MenuProject"].ToString();
                        menu.MenuLevel = Convert.ToInt32(dr["MenuLevel"]);
                        menu.MenuName = (dr["MenuName"] == DBNull.Value) ? "" : dr["MenuName"].ToString();
                        menu.MenuURL = dr["MenuUrl"] == DBNull.Value ? "" : dr["MenuUrl"].ToString();
                        menu.MenuToolTip = dr["MenuToolTip"] == DBNull.Value ? "" : dr["MenuToolTip"].ToString();
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return menu;
        }

        public static List<UserMenu> GetUserMenuByProjectAndLevel(string menuProjectName, int menuLevel)
        {
            List<UserMenu> list = new List<UserMenu>();

            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "MntSelGenMenu";
                db.CommandType = System.Data.CommandType.StoredProcedure;
                db.AddParameter("@MenuProject", System.Data.SqlDbType.VarChar, menuProjectName);
                db.AddParameter("@MenuLevel", System.Data.SqlDbType.TinyInt, menuLevel);
                db.AddParameter("@type", System.Data.SqlDbType.TinyInt, 3);
                db.Open();

                db.ExecuteReader();

                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            UserMenu menu = new UserMenu();
                            menu.MenuNumber = Convert.ToInt32(dr["MenuNumber"]);
                            menu.MenuCode = dr["MenuCode"].ToString();
                            menu.MenuProject = dr["MenuProject"].ToString();

                            menu.Icon = Tool.GeneralHelper.NullToString(dr["MenuIcon"]);
                            menu.MenuLevel = Convert.ToInt32(dr["MenuLevel"]);
                            menu.MenuName = (dr["MenuName"] == DBNull.Value) ? "" : dr["MenuName"].ToString();
                            menu.MenuURL = dr["MenuUrl"] == DBNull.Value ? "" : dr["MenuUrl"].ToString();
                            menu.MenuToolTip = dr["MenuToolTip"] == DBNull.Value ? "" : dr["MenuToolTip"].ToString();
                            menu.Area = dr["Area"] as string;
                            menu.Controller = dr["Controller"] as string;

                            list.Add(menu);
                        }
                    }

                    if (!dr.IsClosed)
                        dr.Close();
                }

                db.Close();
            }

            return list;
        }

        public static int GetMenuNumberFromMenuName(string MenuName)
        {
            int return_ = 0;
            using (DataAccess.SqlServer db = new DataAccess.SqlServer(true))
            {
                db.CommandText = "SELECT TOP 1 MenuNumber FROM MntWebMenu where MenuName=@MenuName";
                db.CommandType = System.Data.CommandType.Text;
                db.AddParameter("@MenuName", System.Data.SqlDbType.VarChar, MenuName);
                db.Open();
                db.ExecuteReader();
                using (Microsoft.Data.SqlClient.SqlDataReader dr = db.DbDataReader as Microsoft.Data.SqlClient.SqlDataReader)
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        return_ = Tool.GeneralHelper.NullToInt(dr["MenuNumber"], 0);
                    }
                    if (!dr.IsClosed)
                        dr.Close();
                }
                db.Close();
            }
            return return_;
        }


        public static bool DeleteMenuFromMenuNumber(string MenuNumber, string MenuCode, string MenuName)
        {
            bool return_ = false;
            int result = 0;
            using (IDS.DataAccess.SqlServer cmd = new IDS.DataAccess.SqlServer(true))
            {
                try
                {

                    cmd.CommandText = "DELETE FROM MntWebMenu  WHERE MenuNumber = @MenuNumber and MenuCode=@MenuCode and MenuName=@MenuName";
                    cmd.AddParameter("@MenuNumber", System.Data.SqlDbType.VarChar, MenuNumber);
                    cmd.AddParameter("@MenuCode", System.Data.SqlDbType.VarChar, MenuCode);
                    cmd.AddParameter("@MenuName", System.Data.SqlDbType.VarChar, MenuName);
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Open();

                    cmd.BeginTransaction();
                    result = cmd.ExecuteNonQuery();
                    cmd.CommitTransaction();
                    return_ = true;
                }
                catch (SqlException sex)
                {
                    return false;
                }
                cmd.Close();
            }
            return return_;
        }

    }
}
