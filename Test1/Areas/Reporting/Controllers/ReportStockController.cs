using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SPOS.Inventory.Stock;
using System.Globalization;

namespace SPOS.Web.UI.Areas.Reporting.Controllers
{
    [Area("Reporting")]
    public class ReportStockController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public ReportStockController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
            _env = env;
        }

        // Halaman report (view saja)
        public ActionResult Index(DateTime? date, string branch)
        {
            if (sUser == null)
                return RedirectToAction("Index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel =
                IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(
                    Convert.ToString(sUserGroup),
                    this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1)
                return RedirectToAction("Index", "Main", new { area = "" });

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            DateTime reportDate = (date ?? DateTime.Today).Date;
            if (string.IsNullOrEmpty(branch))
                branch = "All";

            ViewData["SelectListBranchCode"] = new SelectList(
                IDS.GeneralTable.Branch.GetBranchForDatasourceWithAll(),
                "Value",
                "Text",
                branch == "All" ? "" : branch);

            ViewBag.ReportDate = reportDate;
            ViewBag.ReportBranch = branch;
            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser.ToString();

            List<StockReportBranch> data = StockTransactionH.GetStockReport(reportDate, branch);
            return View(data);
        }

        // Export ke Excel (ClosedXML) — layout: blok per branch berdampingan
        [HttpGet]
        public IActionResult ExportExcel(DateTime? date, string branch)
        {
            if (sUser == null)
                return RedirectToAction("Index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel =
                IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(
                    Convert.ToString(sUserGroup),
                    this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1)
                return RedirectToAction("Index", "Main", new { area = "" });

            DateTime reportDate = (date ?? DateTime.Today).Date;
            if (string.IsNullOrEmpty(branch))
                branch = "All";

            List<StockReportBranch> data = StockTransactionH.GetStockReport(reportDate, branch);

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Stock Akhir");

                int branchCount = data.Count > 0 ? data.Count : 1;
                int totalCols = branchCount * 3;

                // Baris judul
                ws.Cell(1, 1).Value = BuildReportTitle(reportDate, branch, data);
                ws.Range(1, 1, 1, totalCols).Merge();
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 12;
                ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                XLColor[] headerColors = new XLColor[]
                {
                    XLColor.Khaki,
                    XLColor.LightYellow,
                    XLColor.Wheat,
                    XLColor.Moccasin,
                    XLColor.LightSalmon
                };

                // Header nama branch (merge 3 kolom) + sub header BAHAN/AKHIR/SATUAN
                for (int b = 0; b < data.Count; b++)
                {
                    int col = b * 3 + 1;

                    ws.Range(2, col, 2, col + 2).Merge();
                    var nameCell = ws.Cell(2, col);
                    nameCell.Value = data[b].BranchName;
                    nameCell.Style.Font.Bold = true;
                    nameCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    nameCell.Style.Fill.BackgroundColor = headerColors[b % headerColors.Length];

                    ws.Cell(3, col).Value = "BAHAN";
                    ws.Cell(3, col).Style.Font.Bold = true;
                    ws.Cell(3, col).Style.Fill.BackgroundColor = XLColor.LightGray;

                    ws.Cell(3, col + 1).Value = "AKHIR";
                    ws.Cell(3, col + 1).Style.Font.Bold = true;
                    ws.Cell(3, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(3, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;

                    ws.Cell(3, col + 2).Value = "SATUAN";
                    ws.Cell(3, col + 2).Style.Font.Bold = true;
                    ws.Cell(3, col + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell(3, col + 2).Style.Fill.BackgroundColor = XLColor.LightGray;

                    ws.Column(col).Width = 28;
                    ws.Column(col + 1).Width = 12;
                    ws.Column(col + 2).Width = 10;
                }

                // Baris data
                int maxRows = 0;
                foreach (var b in data)
                {
                    if (b.Items.Count > maxRows)
                        maxRows = b.Items.Count;
                }

                for (int i = 0; i < maxRows; i++)
                {
                    int row = 4 + i;
                    for (int b = 0; b < data.Count; b++)
                    {
                        if (i >= data[b].Items.Count)
                            continue;

                        var item = data[b].Items[i];
                        int col = b * 3 + 1;

                        ws.Cell(row, col).Value = item.ProdName;
                        ws.Cell(row, col + 1).Value = item.EndBalance;
                        ws.Cell(row, col + 1).Style.NumberFormat.Format = "#,##0.##";
                        ws.Cell(row, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell(row, col + 2).Value = item.UOM;
                        ws.Cell(row, col + 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    }
                }

                // Border seluruh area tabel
                int lastRow = 3 + (maxRows > 0 ? maxRows : 1);
                ws.Range(2, 1, lastRow, totalCols).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.Range(2, 1, lastRow, totalCols).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.SheetView.FreezeRows(3);

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    string branchPart = branch == "All" ? "AllOutlet" : branch;
                    string fileName = "StockAkhir_" + branchPart + "_" + reportDate.ToString("yyyyMMdd") + ".xlsx";

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }

        // Judul: "STOCK AKHIR ALL OUTLET SELASA 20 OCT 26"
        private string BuildReportTitle(DateTime reportDate, string branch, List<StockReportBranch> data)
        {
            string branchLabel = "ALL OUTLET";
            if (branch != "All")
            {
                branchLabel = branch;
                if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].BranchName))
                    branchLabel = data[0].BranchName;
            }

            string dayName = reportDate.ToString("dddd", new CultureInfo("id-ID")).ToUpper();
            string dateText = reportDate.ToString("dd MMM yy", CultureInfo.InvariantCulture).ToUpper();

            return "STOCK AKHIR " + branchLabel + " " + dayName + " " + dateText;
        }
    }
}
