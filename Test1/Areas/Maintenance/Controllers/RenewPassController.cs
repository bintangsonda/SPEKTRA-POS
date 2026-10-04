using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class RenewPassController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public RenewPassController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        // GET: Maintenance/RenewPass
        public ActionResult Index()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser;
            ViewBag.Title = "Renew Password";
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ChangePass()
        {
            var result = new { status = "error", msg = "ErrDesc" };

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var oldpass = o.SelectToken("oldpass").ToString();
                var newpass = o.SelectToken("newpass").ToString();
                var confirmpass = o.SelectToken("confirmpass").ToString();
                if (IDS.Maintenance.User.UserPassChange(Convert.ToString(sUser), new Tool.clsCryptho().Encrypt(oldpass, "ids"), new Tool.clsCryptho().Encrypt(newpass, "ids"), new Tool.clsCryptho().Encrypt(confirmpass, "ids")))
                {
                    result = new { status = "success", msg = "Sucess Change Password" };
                }
                else
                {
                    result = new { status = "error", msg = "Wrong Password" };
                }
            }
            return Json(result);
        }
    }
}