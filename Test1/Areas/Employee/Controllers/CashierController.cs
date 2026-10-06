using General.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace SPOS.Web.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class CashierController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public CashierController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _env = env;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
            _env = env;
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
            ViewData["OtherMethod"] = new SelectList("", "Value", "Text");
            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["FormAction"] = 1;
            ViewData["BranchList"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(), "Value", "Text", sBranchCode.ToString());
            //ViewData["Product"] = new SelectList(Product.GetData(sBranchCode, ""), "Value", "Text");
            ViewData["CategoriesList"] = new SelectList(General.Catalog.Categories.GetCboCategoriesTransaction(), "Value", "Text");
            string BranchName = Transaction.Employee.CashierH.GetBranchName(sBranchCode);
            Transaction.Employee.CashierH Datas= new Transaction.Employee.CashierH();
            Datas.TransDate=DateTime.Now;
            Datas.BranchCode = sBranchCode;
            Datas.BranchName = BranchName;
            Datas.OperatorID = sUser;
            Datas.TransCode = Transaction.Employee.CashierH.GetNewTransCode(Datas.TransDate, Datas.BranchCode);
            ViewBag.UserMenu = MainMenu;

            return View(Datas);
        }

        public ActionResult GetDataProduct(string BranchCode,string Categories, string SearchMenu)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1)
            {
                return RedirectToAction("Index", "Main", new { Area = "" });
            }
            var products = General.Catalog.Product.GetData(BranchCode, Categories, SearchMenu) ?? new List<General.Catalog.Product>();

            var data = products
                .Where(p=>p.Status==true)
                 .OrderBy(p => p.ProdName)
                .Select(p => new
                {
                    p.ProdCode,
                    p.ProdName,
                    p.Harga,
                    p.Image
                })
                .ToList();


            return Json(data);

        }

        public ActionResult GetProductDetail(string productCode)
        {
            var headers = General.Catalog.VariantHeader
                .GetVariantProduct(productCode)
                ?? new List<General.Catalog.VariantHeader>();

            var data = headers.Select(x => new
            {
                x.VarianCode,
                x.VarianName,
                x.VarianMode,
                x.VarianType,
                x.Minimal,
                x.Maximal,
                Variants = x.VariantDetails.Select(v => new
                {
                    v.Name,
                    v.Value
                }).ToList()
            }).ToList();

            return Json(data);
        }

        public ActionResult SaveBillPayment(Transaction.Employee.CashierH dataTransaksi)    
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }



            ModelState.Clear();
            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;
                string currentBranchCode = sBranchCode as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }
                try
                {

                    dataTransaksi.OperatorID = currentUser;
                    dataTransaksi.BranchCode = currentBranchCode;
                    //CEK IF EXISTS MAKA AKAN UPDATE, KALAU BELUM MAKA CREATE
                    int type =Transaction.Employee.CashierH.CekExistTransCode(dataTransaksi.TransCode);
                    int result = dataTransaksi.InsUpDel(type);
                    int resultStockBalance = 0;
                    if (result > 0)
                    {
                        
                        if (dataTransaksi.Status == 0)
                        {
                            return Json(new { msg = "Save Bill Successfully", success = 1, sno = dataTransaksi.TransCode });
                        }
                        else
                        {
                            //update stock
                            for (int i = 0; i < dataTransaksi.Details.Count; i++)
                            {
                                resultStockBalance = SPOS.Inventory.Stock.StockBalance.AddQtyOut(dataTransaksi.Details[i].ProductCode, dataTransaksi.Details[i].Qty, dataTransaksi.BranchCode);
                                if (resultStockBalance < 1)
                                {
                                    return Json(new { msg = "Process Bill Successfully,But Update Stock Failed!", success = 1, sno = dataTransaksi.TransCode });
                                }
                            }
                            return Json(new { msg = "Process Bill Successfully", success = 1, sno = dataTransaksi.TransCode });
                        }
                    }
                    else
                    {
                        return Json(new { msg = "Failed Save Bill. Please tell your administrator", success = 0 });
                    }
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

        public JsonResult GetNewTransNo()
        {
            return Json(Transaction.Employee.CashierH.GetNewTransNo());
        }

        public JsonResult GetBillList()
        {
            string BranchCode = sBranchCode.ToString();
            var data = Transaction.Employee.CashierH.GetBillList(BranchCode);

            return Json(data);
        }

        public JsonResult Edit(string TransCode)
        {          
            Transaction.Employee.CashierH Datas = new Transaction.Employee.CashierH();
            Datas = Transaction.Employee.CashierH.GetDataBill(TransCode);
            return Json(Datas);
        }

        public JsonResult GetPayType(string PayMethod)
        {
            return Json(Transaction.Employee.CashierH.GetPayType(PayMethod));
        }

    }
}
