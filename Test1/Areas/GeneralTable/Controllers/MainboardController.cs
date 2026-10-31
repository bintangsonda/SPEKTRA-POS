using IDS.GeneralTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Web.UI.Controllers
{
    [Area("GeneralTable")]
    public class MainboardController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        public MainboardController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        // GET: GeneralTable/DashboardStatistic
        public ActionResult Index(DateTime? dateFrom,
            DateTime? dateTo,
            string branchCode = "")
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            var branch = sBranchCode;

            var branchList = IDS.GeneralTable.Branch.GetBranchForDatasource();

            if (sUserGroup != "SuAd")
            {
                branchList = branchList
                    .Where(x => x.Value == branch)
                    .ToList();
            }

            ViewData["SelectListBranchCode"] = new SelectList(
                branchList,
                "Value",
                "Text",
                branch
            );
            ViewBag.UserMenu = MainMenu;

            DateTime from = dateFrom?.Date
                ?? DateTime.Today.AddDays(-6);

            DateTime to = dateTo?.Date
                ?? DateTime.Today;

            // Validasi rentang tanggal.
            if (from > to)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tanggal awal tidak boleh melebihi tanggal akhir.");

                from = to;
            }
            if (branchCode == "All")
            {
                branchCode = "";
            }
            // Ambil data dashboard dari class.
            SalesDashboard dashboard =
                SalesDashboard.GetDashboard(
                    from,
                    to,
                    branchCode ?? "");

            // Kirim data ke View.
            return View(dashboard);
        }
    }
}