using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Policy;
using IDS.Maintenance;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.Routing;
using IDS.Tool;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IDS.Web.UI.Controllers
{
    public class MenuController : Controller
    {
        protected string MainMenu = "";
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public MenuController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);

            List<UserMenu> userMenu = new List<UserMenu>();
            if (contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_MENU) != null)
            {
                userMenu = JsonConvert.DeserializeObject<List<UserMenu>>(_contextAccessor.HttpContext.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_MENU));
            }

            MainMenu = ParseMenuToHTML(userMenu);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            string controllerName = context.RouteData.Values["controller"]?.ToString();

            //Lempar ke halaman login
            if (sUser == null && controllerName != "Main" && (context.Result is ViewResult || context.Result is PartialViewResult))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new { controller = "Main", action = "Index", area = "" }));
                return;
            }

            base.OnActionExecuted(context);
        }



        #region Materialize Menu
        public static string ParseMenuToHTML(List<IDS.Maintenance.UserMenu> userMenu)
        {
            var sb = new System.Text.StringBuilder();

            if (userMenu == null || userMenu.Count == 0)
                return string.Empty;

            var menus = new List<IDS.Maintenance.UserMenu>(userMenu);
            var parentMenu = menus.Where(x => x.MenuLevel == 0).ToList();

            sb.Append("<ul class=\"menu-inner py-1\">");

            foreach (var menu in parentMenu)
            {
                // Header kategori utama (optional)
                sb.Append($"<li class=\"menu-header small text-uppercase\"><span class=\"menu-header-text\">{menu.MenuName.ToUpper()}</span></li>");

                // Ambil semua anak dari parent ini
                var children = menus.Where(x => x.MenuCode.StartsWith(menu.MenuCode.Substring(0, 2)) && x.MenuLevel == 1).ToList();
                foreach (var child in children)
                {
                    ProcessChildMenu(sb, menus, child);
                }
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        private static void ProcessChildMenu(System.Text.StringBuilder sb, List<IDS.Maintenance.UserMenu> menus, IDS.Maintenance.UserMenu menu)
        {
            // Cek apakah punya sub-menu
            bool hasSub = menus.Any(x => x.MenuCode.StartsWith(menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2))
                                      && x.MenuLevel > menu.MenuLevel);

            string menuUrl = string.Empty;

            if (!string.IsNullOrEmpty(menu.MenuURL) && menu.MenuURL.StartsWith("~"))
            {
                menuUrl = AppPathBase.PathBase + menu.MenuURL.TrimStart('~');
            }
            else if (!string.IsNullOrEmpty(menu.Area) && !string.IsNullOrEmpty(menu.Controller))
            {
                menuUrl = AppPathBase.PathBase + "/" + menu.Area + "/" + menu.Controller;
            }
            else
            {
                menuUrl = "javascript:void(0);";
            }

            // Struktur Materialize menu-item
            sb.Append("<li class=\"menu-item\">");

            if (hasSub)
            {
                sb.Append($"<a href=\"javascript:void(0);\" class=\"menu-link menu-toggle\">");
            }
            else
            {
                sb.Append($"<a href=\"{menuUrl}\" class=\"menu-link\">");
            }

            // Icon (jika ada)
            if (!string.IsNullOrEmpty(menu.Icon))
            {
                sb.Append($"<i class=\"menu-icon icon-base {menu.Icon}\"></i>");
            }
            else
            {
                //sb.Append("<i class=\"menu-icon ri ri-circle-line\"></i>");
            }

            sb.Append($"<div data-i18n=\"{menu.MenuName}\">{menu.MenuName}</div>");
            sb.Append("</a>");

            // Jika ada anak lagi
            if (hasSub)
            {
                sb.Append("<ul class=\"menu-sub\">");

                var subMenus = menus.Where(x =>
                    x.MenuCode.StartsWith(menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2))
                    && x.MenuLevel == menu.MenuLevel + 1
                ).ToList();

                foreach (var sub in subMenus)
                {
                    ProcessChildMenu(sb, menus, sub);
                }

                sb.Append("</ul>");
            }

            sb.Append("</li>");
        }

        #endregion Materialize Menu




        #region admin LTE Menu 
        //public static string ParseMenuToHTML(List<IDS.Maintenance.UserMenu> userMenu)
        //{

        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //    if (userMenu != null)
        //    {
        //        List<IDS.Maintenance.UserMenu> parentMenu = null;
        //        List<IDS.Maintenance.UserMenu> menus = new List<IDS.Maintenance.UserMenu>(userMenu as List<IDS.Maintenance.UserMenu>);

        //        if (menus != null && menus.Count > 0)
        //        {
        //            parentMenu = new List<Maintenance.UserMenu>(menus.Where(x => x.MenuLevel == 0).ToList());
        //            sb.Append("<ul class=\"nav nav-pills nav-sidebar flex-column nav-child-indent nav-compact\" data-widget=\"treeview\" role=\"menu\" data-accordion=\"true\">");

        //            foreach (Maintenance.UserMenu menu in parentMenu)
        //            {
        //                sb.Append("<li class=\"nav-header\">" + menu.MenuName.ToUpper() + "</li>");


        //                if (menus.Where(x => x.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2) == menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2)).Count() > 0)
        //                {
        //                    ProcessChildMenu(sb, menus, menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2), menu.MenuLevel);
        //                }
        //            }
        //            sb.Append("</ul>");
        //        }

        //        return sb.ToString();
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        //private static System.Text.StringBuilder ProcessChildMenu(System.Text.StringBuilder sb, List<IDS.Maintenance.UserMenu> menus, string parentCode, int level)
        //{
        //    if (menus.Where(x => x.MenuCode.Substring(0, (level + 1) * 2) == parentCode && x.MenuCode.Substring((level + 1) * 2, 2) != "00").Count() > 0)
        //    {
        //        string menuUrl = "";

        //        foreach (Maintenance.UserMenu menu in menus.Where(x => x.MenuCode.Substring(0, (level + 1) * 2) == parentCode && x.MenuCode.Substring((level + 1) * 2, 2) != "00" && x.MenuCode.Substring((level + 2) * 2, 2) == "00").ToList())
        //        {
        //            switch (menu.MenuURL != null && !string.IsNullOrEmpty(menu.MenuURL) && menu.MenuURL.StartsWith("~", StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                default:
        //                    menuUrl = AppPathBase.PathBase + "/" + menu.Area + "/" + menu.Controller;
        //                    break;
        //            };

        //            sb.Append("<li class=\"nav-item\">");

        //            if (menuUrl != AppPathBase.PathBase + "//")
        //            {
        //                sb.Append("<a href=\"" + menuUrl + "\" class=\"nav-link\">");
        //            }
        //            else
        //            {
        //                sb.Append("<a class=\"nav-link\" style=\"cursor: pointer;\">");
        //            }

        //            if (!string.IsNullOrEmpty(menu.Icon))
        //            {
        //                sb.Append("<i class=\"nav-icon " + menu.Icon + "\"></i>");
        //            }
        //            else
        //            {
        //                sb.Append("<i class=\"nav-icon fas fa-angle-right\"></i>");
        //            }
        //            sb.Append("<p>")
        //            .Append(menu.MenuName);

        //            if (menus.Where(x => x.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2) == menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2) && x.MenuCode.Substring((menu.MenuLevel + 1) * 2, 2) != "00").Count() > 0)
        //            {
        //                sb.Append("<i class=\"right fas fa-angle-left\"></i>")
        //                .Append("</p>")
        //                .Append("</a>");

        //                ProcessSubChildMenu(sb, menus, menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2), menu.MenuLevel);
        //            }
        //            else
        //            {
        //                sb.Append("</p>")
        //                .Append("</a>");
        //            }

        //            sb.Append("</li>");
        //        }
        //    }

        //    return sb;
        //}

        //private static System.Text.StringBuilder ProcessSubChildMenu(System.Text.StringBuilder sb, List<IDS.Maintenance.UserMenu> menus, string parentCode, int level)
        //{
        //    if (menus.Where(x => x.MenuCode.Substring(0, (level + 1) * 2) == parentCode && x.MenuCode.Substring((level + 1) * 2, 2) != "00").Count() > 0)
        //    {
        //        sb.Append("<ul class=\"nav nav-treeview\">");

        //        string menuUrl = "";

        //        foreach (Maintenance.UserMenu menu in menus.Where(x => x.MenuCode.Substring(0, (level + 1) * 2) == parentCode && x.MenuCode.Substring((level + 1) * 2, 2) != "00" && x.MenuCode.Substring((level + 2) * 2, 2) == "00").ToList())
        //        {
        //            switch (menu.MenuURL != null && !string.IsNullOrEmpty(menu.MenuURL) && menu.MenuURL.StartsWith("~", StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                case true:

        //                    break;
        //                default:

        //                    menuUrl = AppPathBase.PathBase + "/" + menu.Area + "/" + menu.Controller;
        //                    break;
        //            };

        //            sb.Append("<li class=\"nav-item\">");
        //            if (menuUrl != AppPathBase.PathBase + "//")
        //            {
        //                sb.Append("<a href=\"" + menuUrl + "\" class=\"nav-link\">");
        //            }
        //            else
        //            {
        //                sb.Append("<a class=\"nav-link\" style=\"cursor: pointer;\">");
        //            }

        //            if (!string.IsNullOrEmpty(menu.Icon))
        //            {
        //                sb.Append("<i class=\"nav-icon " + menu.Icon + "\"></i>");
        //            }
        //            else
        //            {
        //                sb.Append("<i class=\"nav-icon fas fa-angle-right\"></i>");
        //            }

        //            sb.Append("<p>")
        //            .Append(menu.MenuName);


        //            if (menus.Where(x => x.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2) == menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2) && x.MenuCode.Substring((menu.MenuLevel + 1) * 2, 2) != "00").Count() > 0)
        //            {
        //                sb.Append("<i class=\"right fas fa-angle-left\"></i>")
        //                .Append("</p>")
        //                .Append("</a>");

        //                ProcessSubChildMenu(sb, menus, menu.MenuCode.Substring(0, (menu.MenuLevel + 1) * 2), menu.MenuLevel);
        //            }
        //            else
        //            {
        //                sb.Append("</p>")
        //                .Append("</a>");
        //            }

        //            sb.Append("</li>");
        //        }

        //        sb.Append("</ul>");
        //    }

        //    return sb;
        //}
        #endregion admin LTE Menu 
    }
}
