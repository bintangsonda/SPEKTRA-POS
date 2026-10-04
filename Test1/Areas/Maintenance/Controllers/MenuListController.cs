using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class MenuListController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public MenuListController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        public JsonResult GetData()
        {
            if (sUser == null)
                throw new Exception("You do not have access to access this page");

            JsonResult result = new JsonResult(new { });

            try
            {

                List<IDS.Maintenance.UserMenu> usermenu = IDS.Maintenance.UserMenu.GetAllUserMenu();


                result = Json(Newtonsoft.Json.JsonConvert.SerializeObject(usermenu));
            }
            catch (Exception e)
            {

            }

            return result;
        }
        // GET: Maintenance/MenuList
        public ActionResult Index()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1 || AccessLevel.ReadAccess == -1)
            {
                return RedirectToAction("Index", "Main", new { Area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser;

            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                //return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                return RedirectToAction("error403", "error", new { area = "" });
            }
            ViewData["MenuLevel"] = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>() {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "0", Text = "0" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "1", Text = "1" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "2", Text = "2" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "3", Text = "3" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "4", Text = "4" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "5", Text = "5" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "6", Text = "6" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "7", Text = "7" }
            };
            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["ParentMenu"] = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            ViewData["SelectListProject"] = new SelectList(IDS.Maintenance.UserMntMainProject.GetMntMainProjectForDataSource(), "Value", "Text");
            ViewData["FormAction"] = 1;
            return PartialView("Create", new IDS.Maintenance.UserMenu());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? FormAction, IDS.Maintenance.UserMenu user)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                //return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ModelState.Clear();

            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }

                try
                {
                    user.OperatorID = currentUser;
                    if (user.MenuProject != "Reports")
                    {
                        if (!string.IsNullOrWhiteSpace(user.Area) && !string.IsNullOrWhiteSpace(user.Controller))
                        {
                            user.MenuURL = user.Area + '/' + user.Controller;
                        }
                        else if (string.IsNullOrWhiteSpace(user.Area) && !string.IsNullOrWhiteSpace(user.Controller))
                        {
                            user.MenuURL = user.Controller;
                        }
                    }




                    user.InsUpDel((int)IDS.Tool.PageActivity.Insert);

                    return Json(user.MenuCode);
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }
            else
            {
                return Json("Not Valid Mode");
            }
        }

        public ActionResult Edit(string menuCode)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            IDS.Maintenance.UserMenu user = IDS.Maintenance.UserMenu.GetMenu(menuCode);
            ViewData["MenuLevel"] = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>() {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "0", Text = "0" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "1", Text = "1" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "2", Text = "2" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "3", Text = "3" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "4", Text = "4" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "5", Text = "5" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "6", Text = "6" },
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = "7", Text = "7" }
            };
            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["ParentMenu"] = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            ViewData["SelectListProject"] = new SelectList(IDS.Maintenance.UserMntMainProject.GetMntMainProjectForDataSource(), "Value", "Text");

            ViewData["FormAction"] = 2;

            if (user != null)
            {
                return PartialView("Create", user);
            }
            else
            {
                return PartialView("Create", new IDS.Maintenance.UserMenu());
            }
        }

        // POST: GeneralTable/Bank/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(IDS.Maintenance.UserMenu user)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ModelState.Clear();

            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }

                try
                {
                    user.OperatorID = currentUser;
                    if (user.MenuProject != "Reports")
                    {
                        user.MenuURL = user.Area + '/' + user.Controller;
                    }

                    user.InsUpDel((int)IDS.Tool.PageActivity.Edit);

                    return Json("Success");
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }
            else
            {
                return Json("Not Valid Mode");
            }
        }

        public ActionResult Delete(string menuCodeList)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.DeleteAccess == 0)
            {
                return Json("NoAccess");
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            if (string.IsNullOrWhiteSpace(menuCodeList))
                return Json("Failed");

            try
            {
                string[] menusCode = menuCodeList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (menusCode.Length > 0)
                {
                    IDS.Maintenance.UserMenu user = new IDS.Maintenance.UserMenu();
                    user.InsUpDelMenu((int)IDS.Tool.PageActivity.Delete, menusCode);
                }

                return Json("Menu data has been delete successfull");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public JsonResult RefreshMenuList()
        {
            System.Data.DataTable dt = IDS.Maintenance.User.GetWebMenuMaster();
            var studentNamesWithPercentage = dt.AsEnumerable().Select(x => new
            {
                MenuCode = x.Field<string>("MenuCode"),
                GroupCode = x.Field<string>("GroupCode"),
                FrmName = x.Field<string>("FrmName"),
                ProjectName = x.Field<string>("ProjectName"),
                Akses = x.Field<string>("Akses")
            }).ToList();
            return Json(studentNamesWithPercentage);
        }

        public JsonResult GetMenuListForDataSource()
        {
            return Json(IDS.Maintenance.User.GetMenuListForDataSource());
        }

        public JsonResult GetMenuParent(string MenuProject, int level)
        {
            List<IDS.Maintenance.UserMenu> menuName = IDS.Maintenance.UserMenu.GetUserMenuByProjectAndLevel(MenuProject, level == 0 ? 255 : level - 1);
            return Json(menuName);
        }

        // List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetMenuListForDataSource()
    }
}