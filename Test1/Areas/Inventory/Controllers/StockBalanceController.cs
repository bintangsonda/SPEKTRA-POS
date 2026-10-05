using General.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SPOS.Inventory.Stock;
using System.Linq.Dynamic.Core;
using Transaction.Employee;

namespace SPOS.Web.UI.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class StockBalanceController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public StockBalanceController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _env = env;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
            _env = env;
        }

        public JsonResult GetData(string DateFromSelected, string DateToSelected, string werehouse, string prodCode, string Branch)
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


                DateTime DateFrom = DateTime.Now;
                DateTime DateTo = Convert.ToDateTime(DateToSelected).Date
                                    .AddHours(23)
                                    .AddMinutes(59);

                if (string.IsNullOrEmpty(DateFromSelected))
                {
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                else
                {
                    DateFrom = Convert.ToDateTime(DateFromSelected);
                }

                if (sUser == null)
                    throw new Exception("You do not have access to access this page");

                List<SPOS.Inventory.Stock.StockBalance> datalist = new List<SPOS.Inventory.Stock.StockBalance>();
                if (string.IsNullOrEmpty(werehouse))
                {
                    werehouse = "ALL";
                }
                datalist = StockBalance.GetStockBalances(DateFrom, DateTo, werehouse, prodCode, Branch);

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

                        datalist = datalist.Where(x => x.prod.ProdCode.ToLower().Contains(searchValueLower) ||
                                                 x.prod.ProdName.ToLower().Contains(searchValueLower) ||
                                                 x.wh.Name.ToString().Contains(searchValueLower) ||
                                                 x.Branch.ToString().Contains(searchValueLower) ||
                                                 x.BeginBalance.ToString().Contains(searchValueLower) ||
                                                 x.QtyIn.ToString().Contains(searchValueLower) ||
                                                 x.QtyOut.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.QtyAdj.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.EndBalance.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.Date.ToString().ToLower().Contains(searchValueLower) 
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
                RedirectToAction("Index", "Main", new { Area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ViewData["SelectListWh"] = new SelectList(SPOS.Inventory.Stock.Warehouse.GetWareHouseForDataSource(), "Value", "Text");
            ViewData["SelectListProdCode"] = new SelectList(General.Catalog.Product.GetProductForDataSource(), "Value", "Text","All");
            ViewData["SelectListBranchCode"] = new SelectList(General.Catalog.Product.GetBranchCode(), "Value", "Text","All");
            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser.ToString();

            return View();
        }
    }
}
