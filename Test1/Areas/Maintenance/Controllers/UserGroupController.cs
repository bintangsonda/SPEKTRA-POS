using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Data;
using IDS.Maintenance;
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class UserGroupController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public UserGroupController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
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
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();

                var orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{orderColumnIndex}][name]"].FirstOrDefault();

                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0; // Total keseluruhan data
                int totalRecordsShowing = 0; // Total data setelah filter / search

                List<IDS.Maintenance.UserGroup> userGroups = IDS.Maintenance.UserGroup.GetUserGroup();

                totalRecords = userGroups.Count;

                if (length == "-1")
                {
                    pageSize = totalRecords;
                }

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    userGroups = userGroups.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                // Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string searchValueLower = searchValue.ToLower();

                    userGroups = userGroups.Where(x => x.GroupCode.ToLower().Contains(searchValueLower) ||
                                             x.GroupName.ToLower().Contains(searchValueLower) ||
                                              x.GroupParent.ToLower().Contains(searchValueLower) ||
                                             x.OperatorID.ToLower().Contains(searchValueLower) ||
                                             x.LastUpdate.ToString(Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT).ToLower().Contains(searchValueLower)).ToList();
                }

                totalRecordsShowing = userGroups.Count();

                // Paging
                userGroups = userGroups.Skip(skip).Take(pageSize).ToList();

                // Returning Json Data
                result = this.Json(new { draw = draw, recordsFiltered = totalRecordsShowing, recordsTotal = totalRecords, data = userGroups });
            }
            catch
            {

            }

            return result;
        }

        public JsonResult GetPrivileges(string groupCode, string projectName)
        {
            if (sUser == null)
                throw new Exception("You do not have access to access this page");

            JsonResult result = new JsonResult(new { });

            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();

                var orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{orderColumnIndex}][name]"].FirstOrDefault();

                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0; // Total keseluruhan data
                int totalRecordsShowing = 0; // Total data setelah filter / search

                List<IDS.Maintenance.UserGroupAccessView> userGroups = IDS.Maintenance.UserGroupAccessView.GetUserGroupAccessView(groupCode, projectName);

                totalRecords = userGroups.Count;

                if (length == "-1")
                {
                    pageSize = totalRecords;
                }

                // Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    userGroups = userGroups.OrderBy(sortColumn + " " + sortColumnDir).ToList();
                //}

                // Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string searchValueLower = searchValue.ToLower();

                    userGroups = userGroups.Where(x => x.UserGroupCode.ToLower().Contains(searchValueLower) ||
                                             x.UserGroupName.ToLower().Contains(searchValueLower) ||
                                             x.MenuUrl.ToLower().Contains(searchValueLower) ||
                                             x.MenuName.ToLower().Contains(searchValueLower)).ToList();
                }

                totalRecordsShowing = userGroups.Count();

                // Paging
                userGroups = userGroups.Skip(skip).Take(pageSize).ToList();

                // Returning Json Data
                result = this.Json(new { draw = draw, recordsFiltered = totalRecordsShowing, recordsTotal = totalRecords, data = userGroups });
            }
            catch
            {

            }

            return result;
        }

        // GET: Maintenance/UserGroup
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
            ViewData["SelectListGroup"] =new SelectList(IDS.Maintenance.UserGroup.GetUserGroupForDatasource(), "Value", "Text");
            ViewBag.UserLogin = sUser;
            return View();
        }

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
            ViewData["SelectListGroup"] =
                new SelectList(IDS.Maintenance.UserGroup.GetUserGroupForDatasource(), "Value", "Text");
            ViewData["UserGroupList"] = new SelectList(IDS.Maintenance.UserGroup.GetUserGroupForDatasource() , "Value", "Text");
            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ViewData["FormAction"] = 1;
            return PartialView("Create", new IDS.Maintenance.UserGroup());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? FormAction, IDS.Maintenance.UserGroup userGroup)
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

            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }

                try
                {
                    userGroup.OperatorID = userGroup.EntryUser = currentUser;

                    userGroup.InsUpDelUserGroup((int)IDS.Tool.PageActivity.Insert);

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

        public ActionResult Edit(string groupCode)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            ViewData["SelectListGroup"] =
                new SelectList(IDS.Maintenance.UserGroup.GetUserGroupForDatasource(), "Value", "Text");

            ViewData["UserGroupList"] = new SelectList(IDS.Maintenance.UserGroup.GetUserGroupForDatasource(), "Value", "Text");
            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            IDS.Maintenance.UserGroup userGroup = IDS.Maintenance.UserGroup.GetUserGroup(groupCode);



            ViewData["FormAction"] = 2;

            if (userGroup != null)
            {
                return Json(userGroup);

            }
            else
            {
                return null;

            }
        }

        // POST: Maintenance/UserGroup/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(IDS.Maintenance.UserGroup userGroup)
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

            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }

                try
                {
                    userGroup.OperatorID = currentUser;

                    userGroup.InsUpDelUserGroup((int)IDS.Tool.PageActivity.Edit);

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

        public ActionResult Delete(string groupCodeList)
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

            if (string.IsNullOrWhiteSpace(groupCodeList))
                return Json("Failed");

            try
            {
                string[] userGroupCode = groupCodeList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string op = sUser.ToString();
                if (userGroupCode.Length > 0)
                {
                    IDS.Maintenance.UserGroup userGroup = new IDS.Maintenance.UserGroup();
                    userGroup.InsUpDelUserGroup((int)IDS.Tool.PageActivity.Delete, userGroupCode, op);
                }

                return Json("success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public ActionResult Privileges()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());
            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser;
            if (AccessLevel.ReadAccess == -1 || (AccessLevel.CreateAccess == 0 || AccessLevel.EditAccess == 0 || AccessLevel.DeleteAccess == 0))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
                //return RedirectToAction("error403", "error", new { area = "" });
            }

            try
            {
                if (string.IsNullOrEmpty(Request.Query["k"]))
                {
                    return BadRequest("Invalid parameter");
                }

                ViewData["Page.Insert"] = AccessLevel.CreateAccess;
                ViewData["Page.Edit"] = AccessLevel.EditAccess;
                ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

                ViewData["FormAction"] = 1;

                ViewData["ProjectNameList"] = new SelectList(IDS.Maintenance.GroupAccess.GetProjectMain(), "Text", "Text", "");
                ViewData["AccessList"] = new SelectList(IDS.Maintenance.GroupAccess.GetUserGroupAccessList(), "Value", "Text", "");

                string userGroupCode = IDS.Tool.UrlEncryption.DecryptParam(Request.Query["k"]);

                UserGroup userGroup = IDS.Maintenance.UserGroup.GetUserGroup(userGroupCode);

                ViewData["UserGroupCode"] = "";
                ViewData["UserGroupName"] = "";

                if (userGroup != null && !string.IsNullOrEmpty(userGroup.GroupCode))
                {
                    ViewData["UserGroupCode"] = userGroup.GroupCode;
                    ViewData["UserGroupName"] = userGroup.GroupName;

                    List<IDS.Maintenance.UserGroupAccessView> data = IDS.Maintenance.UserGroupAccessView.GetUserGroupAccessView(userGroupCode, null);

                    return View("Privileges", data);
                }
                else
                {
                    return View("Privileges", new List<IDS.Maintenance.UserGroupAccessView>());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult SavePrivileges(List<UserGroupAccessView> data)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || (AccessLevel.EditAccess == 0))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (data == null || data.Count() == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Invalid access data", MediaTypeNames.Text.Plain);
            }

            try
            {
                IDS.Maintenance.UserGroupAccessView.UpdateGroupAccess(Tool.PageActivity.Edit, data);

                return Json(new { message = "Access has been updated" });
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<JsonResult> LoadMntGroupUser()
        {
            JsonResult bj = null;

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var ProjectName = o.SelectToken("ProjectName").ToString();
                var Groupcode = o.SelectToken("Groupcode").ToString();

                List<UserGroupAccessView> data = IDS.Maintenance.UserGroupAccessView.GetUserGroupAccessView(Groupcode, ProjectName);

                return Json(data);
            }

            return Json(new List<UserGroupAccessView>());
        }

        [HttpPost]
        public JsonResult GetProjectMain()
        {
            // Alter - Anthony - 20221101
            //return Json(IDS.Maintenance.GroupAccess.GetProjectMain());
            var mainMenu = UserMenu.GetParentMenu().Select(x => new { Text = x.MenuName, Value = x.MenuNumber });
            return Json(mainMenu);
            // End alter - Anthony - 20221101
        }

        [HttpPost]
        public async Task<JsonResult> MultiSaveToMntGroupAccess()
        {
            JsonResult bj = null;

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var MultiSaveToMntGroupAccess_ = Newtonsoft.Json.JsonConvert.DeserializeObject<IDS.Maintenance.MultiSaveToMntGroupAccess>(json);
                if (IDS.Maintenance.GroupAccess.MultiSaveToMntGroupAccess(MultiSaveToMntGroupAccess_) > 0)
                {
                    bj = Json(new { status = "success" });
                }
                else
                {
                    bj = Json(new { status = "error" });
                }
            }
            return bj;
        }

        public JsonResult GetDataForEditGroupArea(string groupCode)
        {
            if (sUser == null)
            {
                return Json(new { success = false });
            }
            List<string> list = IDS.Maintenance.UserGroup.GetDataForEditGroupArea(groupCode);

            return Json(new { list = list, success = true });
        }

        //[HttpPost]
        //public ActionResult CreateGroupArea(string groupCode, List<string> areaList)
        //{
        //    if (sUser == null)
        //        return RedirectToAction("index", "Main", new { area = "" });

        //    IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

        //    string currentUser = sUser;

        //    if (string.IsNullOrWhiteSpace(currentUser))
        //    {
        //        return Json(new { msg = "Session timeout. Please relogin", success = false });
        //    }

        //    if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
        //    {
        //        return Json(new { msg = "You have no access to Edit Data", success = false });
        //    }

        //    string message = "";
        //    try
        //    {
        //        int result = IDS.Maintenance.UserGroup.InsUpDelGroupArea(groupCode, areaList, currentUser, ref message);

        //        if (result > 0)
        //        {
        //            return Json(new { msg = "Group Area has been edited.", success = true });
        //        }
        //        else
        //            return Json(new { msg = message, success = false });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { msg = ex.Message, success = false });
        //    }
        //}
    }
}
