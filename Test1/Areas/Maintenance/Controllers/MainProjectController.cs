using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class MainProjectController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public MainProjectController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        // GET: Maintenance/MainProject
        public ActionResult Index()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            ViewBag.UserLogin = sUser;

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1)
            {
                RedirectToAction("Index", "Main", new { Area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ViewData["MntMainProject"] = Newtonsoft.Json.JsonConvert.SerializeObject(IDS.Maintenance.UserMntMainProject.MntMainProjectDataTable());

            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser;
            return View("Index");
        }

        public async Task<JsonResult> InsMaintenanceProject()
        {
            if (sUser == null)
                RedirectToAction("index", "Main", new { area = "" });

            string return_ = "{'status':'error','msg':'Data Cant be saved'}";

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var projectname = o.SelectToken("projectname").ToString();
                //var loguser = o.SelectToken("loguser").ToString();
                var loguser = Convert.ToString(sUser);
                var type = o.SelectToken("type").ToString();
                if (IDS.Maintenance.UserMntMainProject.InsDelMNTMainProject(projectname, loguser, int.Parse(type), "import"))
                {
                    return_ = "{'status':'ok','msg':'Data Hasbeen saved'}";
                }
            }
            return Json(return_);
        }

        public JsonResult refresh()
        {
            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(IDS.Maintenance.UserMntMainProject.MntMainProjectDataTable()));
        }

    }
}