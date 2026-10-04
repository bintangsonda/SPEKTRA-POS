using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic.Core;
using Transaction.Employee;

namespace SPOS.Web.UI.Areas.Inventory.Controllers
{
    [Area("Employee")]
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

        public JsonResult GetData(string period, string location, string werehouse, string prodCode)
        {
            if (string.IsNullOrEmpty(period))
                period = DateTime.Now.ToString("yyyyMM");
            else
            {
                DateTime datePeriod = Convert.ToDateTime(period);
                period = datePeriod.ToString("yyyyMM");
            }
            if (sUser == null)
                throw new Exception("You do not have access to access this page");

            List<SPOS.Inventory.Stock.StockBalance> stockBalances = new List<SPOS.Inventory.Stock.StockBalance>();
            JsonResult result = new JsonResult(new { });
            if (string.IsNullOrEmpty(location))
            {
                location = "ALL";
            }
            if (string.IsNullOrEmpty(werehouse))
            {
                werehouse = "ALL";
            }
            if (string.IsNullOrEmpty(prodCode))
            {
                prodCode = "ALL";
            }
            stockBalances = SPOS.Inventory.Stock.StockBalance.GetStockBalances(1, period, location, werehouse, prodCode);

            result = Json(Newtonsoft.Json.JsonConvert.SerializeObject(stockBalances));



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

            ViewData["SelectListYear"] = new SelectList(SPOS.Inventory.Stock.StockBalance.GetYearForDataSource(), "Value", "Text", DateTime.Now.Year.ToString());
            ViewData["SelectListMonth"] = new SelectList(SPOS.Inventory.Stock.StockBalance.GetMonthForDataSource(), "Value", "Text", DateTime.Now.Month.ToString().PadLeft(2, '0'));
            ViewData["SelectListWh"] = new SelectList(SPOS.Inventory.Stock.Warehouse.GetWareHouseForDataSource(), "Value", "Text");
            ViewData["SelectListLocation"] = new SelectList(IDS.GeneralTable.Location.GetLocationDatasource(), "Value", "Text");
            ViewData["SelectListProdCode"] = new SelectList(General.Catalog.Product.GetProductForDataSource(), "Value", "Text");

            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser.ToString();

            return View();
        }
    }
}
