using AppCode;
using DocumentFormat.OpenXml.Spreadsheet;
using IDS.Maintenance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic.Core;
using Transaction.Employee;


namespace SPOS.Web.UI.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ShiftController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public ShiftController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
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

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["FormAction"] = 1;
            ViewData["BranchList"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(), "Value", "Text", sBranchCode.ToString());
            ViewData["GroupCode"] = sUserGroup;
            ViewData["UserID"] = sUser;

            ViewBag.UserMenu = MainMenu;



            return View();
        }

        public ActionResult Edit(DateTime Dates,string Outlet)
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
            List<Shift> data = Shift.GetDataShift(Dates,Outlet);
            if (data != null)
            {
                return Json(data);
            }
            return null;
        }

        public ActionResult ProsesShift(DateTime Dates,int Type,string User, string Outlet, decimal KasAwal, decimal KasAkhir)
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

            try
            {
                //cek apakah sudah ada data start dan belum ada close 
                bool isAllow = Shift.CekShift(Dates, Outlet, Type);//1 untuk cek open shift, 2 untuk cek close shift
                if (isAllow != true)
                {
                    if(Type == 1)
                    {
                        return Json(new { msg = "Shift must be closed first for Outlet: " + Outlet + " and User :" + User, success = 0 });
                    }
                    else
                    {
                        return Json(new { msg = "Shift must be Open first for Outlet: " + Outlet + " and User :" + User, success = 0 });
                    }

                }
                //cek apakah sudah ada data start dan belum ada close
                Shift SaveData = new Shift();
                int result = SaveData.InsUpDel(Dates, Type, User, Outlet, KasAwal, KasAkhir);

                if (result > 0)
                {
                    if (Type == 1)
                    {
                        return Json(new { msg = "Shift Opened for Outlet: " + Outlet+" and User :" +User, success = 1});
                    }
                    else
                    {
                        return Json(new { msg = "Shift Closed for Outlet: " + Outlet + " and User :" + User, success = 1 });
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

    }
}
