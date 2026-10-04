using General.Catalog;
using IDS.GeneralTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using System.Web;

namespace SPOS.Web.UI.Areas.GeneralTable.Controllers
{
    [Area("GeneralTable")]
    public class BranchController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public BranchController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _env = env;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
            _env = env;
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

                List<Branch> datalist = Branch.GetBranch();
                if (datalist != null)
                {
                    totalRecords = datalist.Count;


                    // Sorting    
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        datalist = datalist.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                    }

                    // Search    
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        string searchValueLower = searchValue.ToLower();

                        datalist = datalist.Where(x => x.BranchCode.ToLower().Contains(searchValueLower) ||
                                                 x.BranchName.ToLower().Contains(searchValueLower) ||
                                                 x.Address1.ToString().Contains(searchValueLower) ||
                                                 x.Phone1.ToString().Contains(searchValueLower) ||
                                                 x.BranchManager.ToString().Contains(searchValueLower) ||
                                                 x.NPWP.ToString().Contains(searchValueLower) ||
                                                 x.OperatorID.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.LastUpdate.ToString(IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT).ToLower().Contains(searchValueLower)
                                                 ).ToList();
                    }

                    totalRecordsShowing = datalist.Count();

                    // Paging
                    if (pageSize > 0)
                        datalist = datalist.Skip(skip).Take(pageSize).ToList();

                    // Returning Json Data
                    result = this.Json(new { draw = draw, recordsFiltered = totalRecordsShowing, recordsTotal = totalRecords, data = datalist });
                }
            }
            catch
            {
                throw;
            }

            return result;
        }


        public ActionResult Index()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1)
            {
                return RedirectToAction("Index", "Main", new { Area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["FormAction"] = 1;
            ViewData["BranchList"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(), "Value", "Text", sBranchCode.ToString());

            ViewBag.UserMenu = MainMenu;

            return View();
        }

        public ActionResult Create(int? FormAction, IDS.GeneralTable.Branch dataModal)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["MessageFromExcel"] = "";

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

                    dataModal.OperatorID = currentUser;
                    int result = dataModal.InsUpDel((int)FormAction);

                    if (result > 0)
                    {
                        if ((int)FormAction == 1)
                        {
                            if (string.IsNullOrWhiteSpace(dataModal.BranchCode))
                                return Json(new { msg = "Failed, data is empty!", success = 0 });
                            else
                                return Json(new { msg = "New Outlet has been save. Outlet with Code: " + dataModal.BranchCode, success = 1, sno = dataModal.BranchCode });
                        }
                        else
                        {
                            return Json(new { msg = "Edit Outlet has been save.", success = 1 });
                        }
                    }
                    else
                        throw new Exception("Error");
                }
                catch (Exception ex)
                {
                    return Json(new { msg = ex.Message, success = 0 });
                }
            }
            else
            {
                return Json(new { msg = "Failed!, Not Valid Mode!", success = 0 });
            }
        }

        public ActionResult Delete(Branch DataModal)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ModelState.Clear();
            if (DataModal != null)
            {
                if (ModelState.IsValid)
                {
                    string branch = sBranchCode.ToString();
                    string currentUser = sUser as string;

                    if (string.IsNullOrWhiteSpace(currentUser))
                    {
                        return Json("SessionTimeOut");
                    }

                    try
                    {
                        DataModal.OperatorID = currentUser;
                        DataModal.BranchName = "Empty";

                        int result = DataModal.InsUpDel(3);
                        if (result < 1)
                        {
                            return Json(new { msg = "Failed to Delete Outlet No : " + DataModal.BranchCode, success = 0 });
                        }
                        return Json(new { msg = "success Delete Data", success = 1 });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { msg = ex.Message, success = 0 });
                    }
                }
                else
                {
                    return Json(new { msg = "Failed!, Not Valid Mode!", success = 0 });
                }
            }
            else
            {
                return Json(new { msg = "Failed!, Not Valid Mode!" });
            }
        }

        public ActionResult Edit(string BranchCode)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup ?? "", this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ViewData["FormAction"] = 2;
            Branch data = Branch.GetBranch(BranchCode);
            if (data != null)
            {
                return Json(data);
            }
            return null;
        }
    }
}
