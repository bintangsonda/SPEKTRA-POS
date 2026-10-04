using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IDS.Web.UI.Controllers
{
    public class MainController : MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;

        private string sUser;
        private string sUserGroup;

        public MainController(IHttpContextAccessor contextAccessor, IMemoryCache memoryCache) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;

            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
        }

        // GET: Main
        public ActionResult Index()
            {
            if (string.IsNullOrEmpty(sUser))
                return RedirectToAction("index", "login");


            if (sUserGroup != null)
            {
                return RedirectToAction("Index", "Mainboard", new { area = "GeneralTable" });
            }

            //ViewBag.UserMenu = MainMenu;

            //return View();
            return RedirectToAction("Index", "Mainboard", new { area = "GeneralTable" });
        }


        public ActionResult Signout()
        {
            if (string.IsNullOrEmpty(sUser))
            {
                return RedirectToAction("index", "Login", new { area = "" });
            }

            IDS.Tool.Log log = new Tool.Log();
            log.SaveSysLog1("", "", "LoginAuthorization", sUser, sUser, "Logout");

            //By Renaldi For Login
            _memoryCache.Remove("UserSession_" + sUser);
            //End

            HttpContext.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_AM_GROUP, "");
            HttpContext.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE, "");
            HttpContext.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS, "");
            HttpContext.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE, "");
            HttpContext.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_ID, "");
            HttpContext.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_MENU, "");

            HttpContext.Session.Clear();
            return RedirectToAction("index", "Login", new { area = "" });
        }
    }
}
