using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic.Core;
using Transaction.Employee;

namespace SPOS.Web.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class PurchaseOrderController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public PurchaseOrderController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _env = env;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
            _env = env;
        }

        public JsonResult GetData(string Branch)
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

                List<PurchaseOrderH> datalist = PurchaseOrderH.GetData(Branch);
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

                        datalist = datalist.Where(x => x.POHNo.ToLower().Contains(searchValueLower) ||
                                                 x.POHDate.ToString(IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT).ToLower().Contains(searchValueLower) ||
                                                 x.BranchCode.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.Supplier.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.TotalAmt.ToString().Contains(searchValueLower) ||
                                                 x.CreatedBy.ToString().ToLower().Contains(searchValueLower) ||
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

        public JsonResult GetProductListForPO(string Branch)
        {
            if (string.IsNullOrEmpty(Branch))
            {
                Branch = sBranchCode;
            }
            var products = General.Catalog.Product.GetProductListForPO(Branch);

            return Json(products);
        }

        public ActionResult Create(int? FormAction, Transaction.Employee.PurchaseOrderH dataModal)
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
            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }
                try
                {
                    dataModal.CreatedBy = currentUser;
                    dataModal.LastUpdateBy = currentUser;

                    if (dataModal.PODetail == null || dataModal.PODetail.Count == 0)
                    {
                        return Json(new { msg = "Purchase Order must have at least 1 item detail!", success = 0 });
                    }

                    int result = dataModal.InsUpDel((int)FormAction);

                    if (result > 0)
                    {
                        if ((int)FormAction == 1)
                        {
                            return Json(new { msg = "New Purchase Order has been save. PO with No: " + dataModal.POHNo, success = 1, sno = dataModal.POHNo });
                        }
                        else
                        {
                            return Json(new { msg = "Edit Purchase Order has been save.", success = 1, sno = dataModal.POHNo });
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

        public ActionResult Delete(PurchaseOrderH DataModal)
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
                    string currentUser = sUser as string;

                    if (string.IsNullOrWhiteSpace(currentUser))
                    {
                        return Json("SessionTimeOut");
                    }

                    try
                    {
                        DataModal.LastUpdateBy = currentUser;
                        DataModal.Supplier = "Empty";
                        DataModal.BranchCode = "";
                        DataModal.Remark = "";

                        int result = DataModal.InsUpDel(3);
                        if (result < 1)
                        {
                            return Json(new { msg = "Failed to Delete Purchase Order No : " + DataModal.POHNo, success = 0 });
                        }
                        return Json(new { msg = "success Delete Data", success = 1, sno = DataModal.POHNo });
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

        public ActionResult Edit(string POHNo)
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
            PurchaseOrderH data = PurchaseOrderH.GetDataByPOHNo(POHNo);
            if (data != null)
            {
                return Json(data);
            }
            return null;
        }

    }
}
