using IDS.GeneralTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class RptHistoryController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public RptHistoryController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        // GET: Maintenance/RptHistory
        public ActionResult Index(string period, string status)
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

            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser;

            var a = Syspar.GetInstance();
            ViewBag.Name = a.Name;
            ViewBag.Address1 = a.Address1;
            ViewBag.Address2 = a.Address2;
            ViewBag.Address3 = a.Address3;

            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Clock = DateTime.Now.ToShortTimeString();
            List<IDS.Maintenance.History> his = null;
            if (String.IsNullOrEmpty(period))
            {
                DateTime b = DateTime.Now;
                period = b.ToString("yyyyMM");
            }
            else
            {
                DateTime datePeriod = Convert.ToDateTime(period);
                period = datePeriod.ToString("yyyyMM");
            }
            try
            {

                his = IDS.Maintenance.History.GetHistories(period);

                if (status != "ALL")
                {
                    his = his.Where(x => x.Status.ToLower() == status.ToLower()).ToList();
                }
              
            }
            catch (Exception e)
            {

            }

            return View(his);
        }
    }
}