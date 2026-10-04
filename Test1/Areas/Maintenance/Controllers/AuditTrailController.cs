using IDS.Maintenance;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic.Core;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class AuditTrailController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public AuditTrailController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }
        public IActionResult LogDetail(string key)
        {
            List<AuditTrail> trail = new List<AuditTrail>();
            trail = IDS.Maintenance.AuditTrail.GetAuditTrailsSingle(key);
            if (trail != null)
            {
                return PartialView("Log", trail);

            }
            else
            {
                return PartialView("Log", new List<AuditTrail>());
            }
        }
        public IActionResult Log(string MenuName,string key)
        {
            List<AuditTrail> trail = new List<AuditTrail>();
            trail = IDS.Maintenance.AuditTrail.GetAuditTrails(MenuName, key);
            if(trail != null)
            {
                return PartialView("Log", trail);

            }
            else
            {
                return PartialView("Log", new List<AuditTrail>());
            }
        }

        public IActionResult LogHD(string MenuHeader,string MenuDetail, string key)
        {
            List<AuditTrail> trail = new List<AuditTrail>();
            trail = IDS.Maintenance.AuditTrail.GetAuditTrailsHD(MenuHeader,MenuDetail, key);
            if (trail != null)
            {
                return PartialView("Log", trail);

            }
            else
            {
                return PartialView("Log", new List<AuditTrail>());
            }
        }

        public JsonResult GetData(DateTime? from, DateTime? to, string table, string user)
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
                if (from == null)
                {
                    from  = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
                }
                if (to == null)
                {
                    to = DateTime.Now.Date.AddHours(23).AddMinutes(59);
                }
                
                List<IDS.Maintenance.AuditTrail> userGroups = IDS.Maintenance.AuditTrail.GetData(from, to, user, table);

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

                    userGroups = userGroups.Where(x => x.ID.ToString().ToLower().Contains(searchValueLower) ||
                                             x.TableName.ToLower().Contains(searchValueLower) ||
                                              x.UserId.ToLower().Contains(searchValueLower) ||
                                             x.Status.ToLower().Contains(searchValueLower) ||
                                             x.Key.ToLower().Contains(searchValueLower) ||
                                             x.UpdatedDate.ToString(Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT).ToLower().Contains(searchValueLower)).ToList();
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

        public IActionResult Index()
        {
            ViewBag.UserMenu = MainMenu;

            ViewData["TableList"] = new SelectList(IDS.Maintenance.AuditTrail.GetTable(), "Value", "Text");
            ViewData["UserList"] = new SelectList(IDS.Maintenance.AuditTrail.GetUser(), "Value", "Text");

            ViewBag.UserLogin = sUser;
            return View();
        }
        [HttpGet]
        public IActionResult Filter(DateTime? from, DateTime? to, string data, string user)
        {
            ViewBag.UserMenu = MainMenu;
            if (from == null)
            {
                from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).Date;
            }
            if (to == null)
            {
                to = DateTime.Now.Date.AddHours(23).AddMinutes(59);
            }
            if(string.IsNullOrEmpty(user))
            {
                user = "All";
            }
            if(string.IsNullOrEmpty(data))
            {
                data = "All";
            }
            List<AuditTrail> trail = new List<AuditTrail>();
            trail = IDS.Maintenance.AuditTrail.GetData(from,to,user,data);
            ViewBag.UserLogin = sUser;
            ViewData["From"] = from?.ToString("dd MMM yyyy");
            ViewData["To"] = to?.ToString("dd MMM yyyy");
            ViewData["Data"] = data;
            ViewData["User"] = user;
            if (trail != null)
            {
                return View(trail);
            }
            else
            {
                return View(new List<AuditTrail>());
            }
        }
    }
}
