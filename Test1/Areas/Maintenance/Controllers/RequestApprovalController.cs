using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class RequestApprovalController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public RequestApprovalController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        public JsonResult GetData(string branch, int? reqstatus, int? appstatus, string menuurl)
        {
            if (sUser == null)
                throw new Exception("You do not have access to access this page");

            if (menuurl == null)
            {
                menuurl = "";
            }

            JsonResult result = new JsonResult(new {});

            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();

                var orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{orderColumnIndex}][name]"].FirstOrDefault();

                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int pageSize = 0;
                switch (length)
                {
                    case null:
                        break;
                    case "-1":
                        pageSize = 0;
                        break;
                    default:
                        pageSize = Convert.ToInt32(length);
                        break;
                }

                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0; // Total keseluruhan data
                int totalRecordsShowing = 0; // Total data setelah filter / search
                string group = sUserGroup;
                //Add Jeremi 25 Mei 2024
                if (group == "SuAd" || group == "ADM")
                {
                    group = "";
                }
                List<AppCode.clsMenuRequest> req = AppCode.clsMenuRequest.GetClsMenuRequests(branch, group, appstatus == null ? -1 : Convert.ToInt32(appstatus), reqstatus == null ? -1 : Convert.ToInt32(reqstatus), menuurl);

                totalRecords = req.Count;
                
                // Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    req = req.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                // Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string lowerSearchValue = searchValue.ToLower();

                    req = req.Where(x => Tool.GeneralHelper.NullToString(x.RequestID).ToLower().Contains(lowerSearchValue) ||
                                             Tool.GeneralHelper.NullToString(x.RequestDate).ToLower().Contains(lowerSearchValue) ||
                                             Tool.GeneralHelper.NullToString(x.ApprovalGroup).ToLower().Contains(lowerSearchValue) ||

                                             IDS.Tool.GeneralHelper.NullToString(x.ApprovalBranch).Contains(lowerSearchValue) ||
                                             IDS.Tool.GeneralHelper.NullToString(x.BranchCode).Contains(lowerSearchValue) ||
                                             IDS.Tool.GeneralHelper.NullToString(x.MenuUrl).Contains(lowerSearchValue) ||
                                             IDS.Tool.GeneralHelper.NullToString(x.Action).Contains(lowerSearchValue) ||
                                           
                                             IDS.Tool.GeneralHelper.NullToString(x.ApprovalUser).Contains(lowerSearchValue))
                                             .ToList();
                }

                totalRecordsShowing = req.Count();

                // Paging
                if (pageSize > 0)
                    req = req.Skip(skip).Take(pageSize).ToList();

                // Returning Json Data
                result = this.Json(new { draw = draw, recordsFiltered = totalRecordsShowing, recordsTotal = totalRecords, data = req });
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        // GET: Maintenance/RequestApproval
        public ActionResult Index()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1)
            {
                RedirectToAction("Index", "Main", new { Area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            if (Convert.ToBoolean(sHO))
            {
                ViewData["HOStatus"] = 1;
                ViewData["SelectListBranch"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(), "Value", "Text", sBranchCodeCode);
            }
            else
            {
                ViewData["HOStatus"] = 0;
                ViewData["SelectListBranch"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(sBranchCodeCode), "Value", "Text", sBranchCodeCode);
            }
            ViewData["SelectListApproval"] = new SelectList(AppCode.IDSTools.GetApprovalStatus(), "Value", "Text");
            ViewData["SelectListRequest"] = new SelectList(AppCode.IDSTools.GetRequestStatus(), "Value", "Text");
            ViewBag.UserMenu = MainMenu;
            return View();
           
        }
        public ActionResult UpdateStatusApproval(int requestID, int status, int req)
        {
            try
            {
                //string MenuURL = "";
                //MenuURL = AppCode.clsMenuRequest.GetMenuURL(requestID);
                //if(MenuURL == "") {
                //    return Json(new { Message = "Error update data, please contact your administrator" });
                //}
                string operatorID = sUser.ToString();

                //Add by Jeremi 2 Agustus 2024 - Cek apakah Data Approval sudah di Approve atau di Reject oleh user lain
                string checkUser = AppCode.clsMenuRequest.CheckUserApproval(requestID);
                if (!string.IsNullOrEmpty(checkUser))
                {
                    return Json(new { Message = "This Approval Data has been updated by another user! Last Updated by " + checkUser });
                }
                //End Jeremi

                int result = AppCode.clsMenuRequest.UpdateStatusApproval(requestID, status, req, operatorID);
                if(result > 0)
                {
                    //result = AppCode.clsMenuRequest.UpdateMntWebMenu( MenuURL);
                    //if (result > 0)
                    //{
                    //    return Json(new { Message = "Success Update Data" });
                    //}
                    return Json(new { Message = "Success Update Data" });
                }
                return Json(new { Message = "Error update data, please contact your administrator" });
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
            return View();
        }
    }
}