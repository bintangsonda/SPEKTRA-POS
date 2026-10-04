using AppCode;
using DocumentFormat.OpenXml.Spreadsheet;
using General.Catalog;
using IDS.Maintenance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic.Core;
using Transaction.Employee;
namespace SPOS.Web.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class TransactionListController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public TransactionListController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _env = env;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
            _env = env;
        }
        public JsonResult GetData(DateTime TransDateFrom, DateTime TransDateTo, string TransMethod, string CustName)
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
                TransDateTo = TransDateTo.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                List<CashierH> datalist = CashierH.GetData(TransDateFrom,TransDateTo, TransMethod, CustName);
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

                        datalist = datalist.Where(x => x.TransCode.ToLower().Contains(searchValueLower) ||
                                                 x.CustID.ToLower().Contains(searchValueLower) ||
                                                 x.TransTotalAmt.ToString().Contains(searchValueLower) ||
                                                 x.TransPayMtd.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.TransPayMtd.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.TransPayType.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.TransRemark.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.OperatorID.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.TransDate.ToString(IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT).ToLower().Contains(searchValueLower)||
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
            ViewData["GroupCode"] = sUserGroup;
            ViewData["UserID"] = sUser;
            ViewData["MethodList"] = new SelectList(General.Catalog.PaymntMethod.GetMethodList(), "Value", "Text");
            ViewData["CustNameList"] = new SelectList(General.Catalog.Customer.CustList(), "Value", "Text");
            ViewBag.UserMenu = MainMenu;



            return View();
        }

        public ActionResult Edit(string TransCode)
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
            ViewData["GroupCode"] = sUserGroup;
            CashierH DataEditTransaction = new CashierH();
            DataEditTransaction = CashierH.GetDataBill(TransCode);
            ViewBag.UserMenu = MainMenu;



            return View("Create", DataEditTransaction);
        }
    }
}
