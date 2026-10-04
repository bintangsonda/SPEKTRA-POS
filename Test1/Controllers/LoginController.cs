using IDS.Maintenance;
using IDS.Tool;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Reflection;

namespace IDS.Web.UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _env;

        public LoginController(IHttpContextAccessor _context, IMemoryCache memoryCache, IWebHostEnvironment env)
        {
            _contextAccessor = _context;
            _memoryCache = memoryCache;
            _env = env;
        }

        [HttpPost]
        public IActionResult index(Models.UserLogin login)
        {
            int count1 = 0;

            if (User != null)
            {
                ViewBag.ValidationResult = "";
                bool isValid = false;

                if (string.IsNullOrWhiteSpace(login.UserID) || string.IsNullOrWhiteSpace(login.Password))
                {
                    ViewBag.ValidationResult = "Incorrect user ID or Password";
                    ViewBag.LoginFailed = true;
                    isValid = false;
                }
                else
                {
                    IDS.Tool.clsCryptho crypt = new IDS.Tool.clsCryptho();
                    string encryptPassword = crypt.Encrypt(login.Password, "ids");
                    IDS.Maintenance.User user = IDS.Maintenance.User.UserLogin(login.UserID, encryptPassword);

                    if (user != null)
                    {
                        //Fungsi buat cek dah ada yang login atau belum
                        //--MATIIN DULU DI FISCUS NEW SOALNYA BELUM ADA FUNGSI LOGOUT
                        //string key = "UserSession_" + user.UserID;
                        //if (_memoryCache.TryGetValue(key, out string sessionStatus))
                        //{
                        //    ViewBag.ValidationResult = "This user is already logged in.";
                        //    isValid = false;

                        //    return View(login);
                        //}
                        //End

                        if (user.Status == IDS.Maintenance.UserStatus.InActive)
                        {
                            ViewBag.ValidationResult = "Your account is not active or has been block. Please contact your administrator.";
                            isValid = false;

                            return View(login);
                        }
                        else if (user.Akumulasi > 10)
                        {
                            ViewBag.ValidationResult = "Your account has been block cause of failed login attempt. Please contact your administrator.";
                            isValid = false;

                            return View(login);
                        }
                        else if (!string.IsNullOrWhiteSpace(user.ExpiredCode))
                        {
                            ViewBag.ValidationResult = "Your account has been expired. Please contact your administrator.";
                            isValid = false;

                            return View(login);
                        }
                        else
                        {
                            _contextAccessor?.HttpContext?.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_ID, user.UserID);
                            _contextAccessor?.HttpContext?.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE, user.UserGroup.GroupCode);
                            _contextAccessor?.HttpContext?.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE, user.Branch.BranchCode);
                            _contextAccessor?.HttpContext?.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS, user.Branch.HOStatus.ToString());
                            _contextAccessor?.HttpContext?.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_EMAIL, user.EmailAddress);

                            //By Renaldi For Login
                            string sessionId = Guid.NewGuid().ToString();
                            _memoryCache.Set("UserSession_" + user.UserID, sessionId, TimeSpan.FromMinutes(1));
                            //End

                            IDS.Tool.Log log = new Tool.Log();
                            log.SaveSysLog1("", "", "LoginAuthorization", user.UserID, user.UserID, "LOGIN");

                            // TODO: Disesuaikan dengan user.BranchCode.
                            //Session[Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS] = ;

                            // Retrieve User Group Menu
                            List<IDS.Maintenance.UserMenu> userMenus = IDS.Maintenance.UserMenu.GetParentMenu();
                            List<IDS.Maintenance.UserMenu> userMenusClone = new List<IDS.Maintenance.UserMenu>(userMenus);
                            Dictionary<string, List<IDS.Maintenance.GroupAccess>> cache = IDS.Tool.InMemoryCache.GetInstance().GetOrSet(IDS.Tool.GlobalVariable.CACHE_USER_GROUP_ACCESS, user.UserGroup.GroupCode, () => IDS.Maintenance.GroupAccess.GetGroupAccess(user.UserGroup.GroupCode), IDS.Tool.GlobalVariable.CACHE_DURATION_USER_GROUP_ACCESS);
                            if (cache == null)
                            {
                                // TODO: Redirect ke UnAuthorize
                            }

                            //if (userMenus != null)
                            //{
                            //    foreach (IDS.Maintenance.UserMenu menu in userMenusClone)
                            //    {
                            //        userMenus = IDS.Maintenance.UserMenu.GetChildMenu(userMenus, menu.MenuCode.Substring(0, 2), user.UserGroup.GroupCode);
                            //    }
                            //}
                            if (userMenus != null)
                            {
                                int countchild;
                                foreach (Maintenance.UserMenu menu in userMenusClone)
                                {

                                    userMenus = Maintenance.UserMenu.GetChildMenu(userMenus, menu.MenuCode.Substring(0, 2), user.UserGroup.GroupCode);
                                    count1 = Maintenance.UserMenu.CheckChildMenu(menu.MenuCode.Substring(0, 2), user.UserGroup.GroupCode, 3);
                                    if (count1 == 0)
                                    {
                                        userMenus = userMenus.Where(x => x.MenuProject != menu.MenuProject).ToList();
                                    }
                                }
                                foreach (Maintenance.UserMenu a in userMenus.Where(x => x.MenuLevel == 1 && String.IsNullOrEmpty(x.MenuURL) || x.MenuLevel == 2 && String.IsNullOrEmpty(x.MenuURL) || x.MenuLevel == 3 && String.IsNullOrEmpty(x.MenuURL)))
                                {
                                    countchild = 0;
                                    if (a.MenuLevel == 1)
                                    {
                                        countchild = Maintenance.UserMenu.CheckChildMenu(a.MenuCode.Substring(0, 4), user.UserGroup.GroupCode, 1);
                                        if (countchild == 0)
                                        {
                                            userMenus = userMenus.Where(x => x.MenuCode != a.MenuCode).ToList();
                                        }
                                    }
                                    else if (a.MenuLevel == 2)
                                    {
                                        countchild = Maintenance.UserMenu.CheckChildMenu(a.MenuCode.Substring(0, 6), user.UserGroup.GroupCode, 2);
                                        if (countchild == 0)
                                        {
                                            userMenus = userMenus.Where(x => x.MenuCode != a.MenuCode).ToList();
                                        }
                                    }
                                    else
                                    {
                                        countchild = Maintenance.UserMenu.CheckChildMenu(a.MenuCode.Substring(0, 8), user.UserGroup.GroupCode, 3);
                                        if (countchild == 0)
                                        {
                                            userMenus = userMenus.Where(x => x.MenuCode != a.MenuCode).ToList();
                                        }
                                    }
                                }
                            }

                            string uMenu = JsonConvert.SerializeObject(userMenus);
                            _contextAccessor?.HttpContext?.Session.SetString(IDS.Tool.GlobalVariable.SESSION_USER_MENU, uMenu);


                            return RedirectToAction("index", "Main");
                        }
                    }
                    else
                    {
                        isValid = false;
                        ViewBag.ValidationResult = "Incorrect user ID or Password";
                        ViewBag.LoginFailed = true;

                        return View(login);
                    }
                }
            }

            return View();
        }

        public IActionResult Index()
        {
            try
            {
                //var rootPath = _env.ContentRootPath;
                //if (IDSLicensing.License.ValidateLicense("FISCUS", rootPath + "\\"))
                //{
                //    return View();
                //}
                //else
                //{
                //    return RedirectToAction("ErrorLicense", "Error");
                //}
                return View();
            }
            catch
            {
                return RedirectToAction("ErrorLicense", "Error");
            }
        }

        //By Renaldi For Login
        [HttpPost]
        public ActionResult UpdateLoginMemoryCache()
        {
            if (!string.IsNullOrEmpty(_contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID)))
            {
                string userId = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
                string sessionId = Guid.NewGuid().ToString();

                if (!string.IsNullOrEmpty(userId))
                {
                    _memoryCache.Set("UserSession_" + userId, sessionId, TimeSpan.FromMinutes(1));
                }
            }

            return new EmptyResult(); // No response body
        }
        //End 
    }
}
