using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.Rendering;
using IDS.Maintenance;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class UserController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public UserController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        public JsonResult GetData(string BranchCode)
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
                IList<IDS.Maintenance.User> user=new List<IDS.Maintenance.User>();
                if (BranchCode==null || BranchCode == "")
                {
                    user = IDS.Maintenance.User.GetUser().ToList();
                }
                else
                {
                     user = IDS.Maintenance.User.GetUser().Where(x => x.Branch.BranchCode == BranchCode).ToList();

                }
                int Actives = user.Count(x => x.Status == UserStatus.Active);//ini buat nilai di komponen data harusnya, tpi untuk user ga dipake
                int Inactives = user.Count(x => x.Status == UserStatus.InActive);//ini buat nilai di komponen data harusnya, tpi untuk user ga dipake
                totalRecords = user.Count;

                // Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    user = user.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                // Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string searchValueLower = searchValue.ToLower();

                    user = user.Where(x =>
                                        (x.UserID.ToString() ?? "").ToLower().Contains(searchValueLower) ||
                                        (x.UserName ?? "").ToLower().Contains(searchValueLower) ||
                                        (x.Branch?.BranchCode ?? "").ToLower().Contains(searchValueLower) ||
                                        (x.UserGroup?.GroupCode ?? "").ToLower().Contains(searchValueLower) ||
                                        (x.EmailAddress ?? "").ToLower().Contains(searchValueLower) ||
                                        (x.ExpiredCode ?? "").ToLower().Contains(searchValueLower) ||
                                        (x.Status.ToString() ?? "").ToLower().Contains(searchValueLower)
                                    ).ToList();
                }

                totalRecordsShowing = user.Count();

                //Paging
                //Edited By Renaldi 18 October 2025
                //user = user.Skip(skip).Take(pageSize).ToList();
                if (pageSize == -1)
                {
                    user = user.ToList();
                }
                else
                {
                    user = user.Skip(skip).Take(pageSize).ToList();
                }
                //End Edited



                //Returning Json Data    
                result = this.Json(new { draw = draw, recordsFiltered = totalRecordsShowing, recordsTotal = totalRecords, data = user, totalActiveUser = Actives , totalInactiveUser =Inactives});
                

            }
            catch
            {
            }
            return result;
        }

        // GET: Maintenance/User
        public ActionResult Index()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup ?? "", this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1 || AccessLevel.ReadAccess == -1)
            {
                return RedirectToAction("Index", "Main", new { Area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ViewData["ListUser"] = Newtonsoft.Json.JsonConvert.SerializeObject(GetDataTable());
            ViewData["SelectListGroup"] = new SelectList(IDS.Maintenance.User.Getgroupcode(), "Value", "Text");

            var branchList = IDS.GeneralTable.Branch.GetBranchForDatasource().ToList();
            // Buat list dengan tambahan "All" di paling atas
            var branchListWithAll = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(branchList);
            branchListWithAll.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = "", Text = "All" });
            // Tentukan SelectListBranch tergantung user group
            IEnumerable<SelectListItem> filteredBranch;
            if (sUserGroup == "SuAd")
            {
                filteredBranch = branchList; // Super Admin bisa lihat semua
            }
            else
            {
                filteredBranch = branchList.Where(x => x.Value == sBranchCodeCode);
            }
            // Simpan ke ViewData
            ViewData["SelectListBranch"] = new SelectList(filteredBranch, "Value", "Text", sBranchCodeCode);
            ViewData["listBranch"] = new SelectList(branchListWithAll, "Value", "Text");

            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> a = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>() {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() {Text = "Aktif", Value = "1"},
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() {Text = "Non Aktif", Value= "0"}
            };
            ViewData["SelectListStatus"] = new SelectList(a, "Value", "Text", "");
            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser;



            return View("Index");
        }




        // GET: Maintenance/User/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                //return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                return RedirectToAction("error403", "error", new { area = "" });
            }
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> a = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>() {
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() {Text = "Aktif", Value = "1"},
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() {Text = "Non Aktif", Value= "0"}
            };
            ViewData["SelectListStatus"] = new SelectList(a, "Value", "Text", "");
            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["SelectListGroup"] = new SelectList(IDS.Maintenance.User.Getgroupcode(), "Value", "Text");
            if (sUserGroup == "SuAd")
            {
                ViewData["SelectListBranch"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(), "Value", "Text", Tool.GeneralHelper.NullToString(sBranchCodeCode));

            }
            else
            {
                ViewData["SelectListBranch"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource().Where(x => x.Value == sBranchCodeCode), "Value", "Text", Tool.GeneralHelper.NullToString(sBranchCodeCode));

            }
            // ViewData["SelectListBranch"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(), "Value", "Text");
            ViewData["SelectListSecurity"] = new SelectList(IDS.Maintenance.User.GetSecurityCode(), "Value", "Text");
            ViewData["SelectListExp"] = new SelectList(IDS.Maintenance.User.GetExpCode(), "Value", "Text");

            //ViewData["SelectListArea"] = new SelectList("", "Value", "Text");
            //ViewData["AreaExist"] = false;

            //ViewData["SelectListDepartment"] = new SelectList(IDS.Maintenance.Department.GetDepartmentForDataSource(), "Value", "Text");

            ViewData["FormAction"] = 1;
            return PartialView("Create", new IDS.Maintenance.User());
        }

        // POST: Maintenance/User/Create
        [HttpPost]
        public ActionResult Create(IDS.Maintenance.User user)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                //return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;


            string currentUser = sUser as string;

            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return Json("SessionTimeOut");
            }

            user.Password = new Tool.clsCryptho().Encrypt(user.Password, "ids");

            try
            {
                user.OperatorID = user.EntryUser = currentUser;
                user.SecurityCode = "Hero";
                user.SecurityAnswer = "Hero";

                user.InsUpDelUser(0);

                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }

        // GET: Maintenance/User/Edit/5
        public ActionResult Edit(string userid)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());
            IDS.Maintenance.User user = IDS.Maintenance.User.GetUser(userid);
            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
           

            if (user != null)
            {
                return Json(user);
            }
            return null;
        }

        // POST: Maintenance/User/Edit/5
        [HttpPost]
        public ActionResult Edit(IDS.Maintenance.User user)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            //if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
            //{
            //    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            //}

            //ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            //ViewData["Page.Edit"] = AccessLevel.EditAccess;
            //ViewData["Page.Delete"] = AccessLevel.DeleteAccess;


            string currentUser = sUser as string;

            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return Json("SessionTimeOut");
            }

            if (user.Password != user.ConfirmPassword)
            {
                return Json("Password does not match");
            }

            try
            {
                user.OperatorID = currentUser;
                user.SecurityCode = "Hero";
                user.SecurityAnswer = "Hero";
                user.InsUpDelUser(1);

                return Json("Success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }

        public ActionResult Delete(string userCodeList)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.DeleteAccess == 0)
            {
                return Json("NoAccess");
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            if (string.IsNullOrWhiteSpace(userCodeList))
                return Json("Failed");

            try
            {
                string[] userCode = userCodeList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (userCode.Length > 0)
                {
                    IDS.Maintenance.User user = new IDS.Maintenance.User();
                    user.InsUpDelUser((int)IDS.Tool.PageActivity.Delete, userCode);
                }

                return Json("User data has been delete successfull");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        private System.Data.DataTable GetDataTable()
        {
            System.Data.DataTable dt_ = new System.Data.DataTable();
            dt_.Clear();
            dt_.Columns.Add("userid");
            dt_.Columns.Add("username");
            dt_.Columns.Add("email");
            dt_.Columns.Add("group");
            dt_.Columns.Add("datecreated");
            dt_.Columns.Add("expired");
            dt_.Columns.Add("accum");
            dt_.Columns.Add("active");
            IDS.Maintenance.User.GetUserFromBranch(sBranchCodeCode, dt_);
            return dt_;
        }

        [HttpPost]
        public async Task<string> RefreshUser()
        {
            string return_ = "";

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();

            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var Branch = o.SelectToken("Branch").ToString();

                return_ = Newtonsoft.Json.JsonConvert.SerializeObject(GetDataTable(), Newtonsoft.Json.Formatting.Indented);

            }
            return return_;
        }
        private System.Data.DataTable GetDataTable2(string branch)
        {
            System.Data.DataTable dt_ = new System.Data.DataTable();
            dt_.Clear();
            dt_.Columns.Add("userid");
            dt_.Columns.Add("username");
            dt_.Columns.Add("email");
            dt_.Columns.Add("group");
            dt_.Columns.Add("datecreated");
            dt_.Columns.Add("expired");
            dt_.Columns.Add("accum");
            dt_.Columns.Add("active");
            IDS.Maintenance.User.GetUserFromBranch(branch, dt_);
            return dt_;
        }

        [HttpPost]
        public async Task<string> RefreshUser2()
        {
            string return_ = "";

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();

            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var Branch = o.SelectToken("Branch").ToString();

                return_ = Newtonsoft.Json.JsonConvert.SerializeObject(GetDataTable2(Branch), Newtonsoft.Json.Formatting.Indented);

            }
            return return_;
        }

        public JsonResult GetSecurityCode()
        {
            return Json(IDS.Maintenance.User.GetSecurityCode());
        }

        public JsonResult Getgroupcode()
        {
            return Json(IDS.Maintenance.User.Getgroupcode());
        }

        public JsonResult GetBranch()
        {
            return Json(IDS.GeneralTable.Branch.GetBranchForDatasource());
        }

        public JsonResult GetExp()
        {
            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> RP = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
            RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "7 Day", Value = "7D" });
            RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "14 Day", Value = "14D" });
            RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "1 Month", Value = "1M" });
            return Json(RP);
        }

        [HttpPost]
        public async Task<JsonResult> SaveUser()
        {
            ReplayToClient c = new ReplayToClient();

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            //if (Tool.GeneralHelper.ValidateJSON(json))
            //{
            var o = Newtonsoft.Json.Linq.JObject.Parse(json);
            var userid = o.SelectToken("userid").ToString();
            var username = o.SelectToken("username").ToString();
            var password = o.SelectToken("password").ToString();
            var email = o.SelectToken("email").ToString();
            var group = o.SelectToken("group").ToString();
            var AMGroupCode = o.SelectToken("amgroupcode").ToString();
            var date = o.SelectToken("date").ToString();
            var exp = o.SelectToken("exp").ToString();
            var scode = o.SelectToken("scode").ToString();
            var sansw = o.SelectToken("sansw").ToString();
            var akumulasi = Tool.GeneralHelper.NullToInt(o.SelectToken("akumulasi"), 0);
            var status = o.SelectToken("status").ToString();
            var branch = o.SelectToken("branch").ToString();

            if (IDS.Maintenance.User.UserExist(userid))
            {
                c.msg_ = "Data Already Exist!";
                c.response_ = "error";
            }
            else
            {
                if (IDS.Maintenance.User.SaveUser(userid, username, new Tool.clsCryptho().Encrypt(password, "ids"), email, group, stringToDatetime(date), exp, scode, sansw, akumulasi, Convert.ToBoolean(status), branch, sUser))
                {
                    c.msg_ = "success";
                    c.response_ = "ok";
                }
                else
                {
                    c.msg_ = "Same thing Wrong!";
                    c.response_ = "error";
                }
            }
            //}
            return Json(c);
        }

        private static System.DateTime stringToDatetime(string dateString)
        {
            System.DateTime return_ = DateTime.Now;
            string dateTime = dateString;
            if (IsvalidDatetIme(dateTime))
            {
                return_ = System.Convert.ToDateTime(dateTime);
            }
            return return_;
        }

        private static bool IsvalidDatetIme(string s)
        {
            bool return_ = false;
            try
            {
                System.Convert.ToDateTime(s);
                return_ = true;
            }
            catch (Exception x)
            {
                return_ = false;
            }
            return return_;
        }

        [HttpPost]
        public async Task<string> GetUserByUserId()
        {
            string return_ = "Eesponse From Server!";

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            string json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var userid = o.SelectToken("userid").ToString();
                return_ = Newtonsoft.Json.JsonConvert.SerializeObject(IDS.Maintenance.User.GetUserFromId(userid), Newtonsoft.Json.Formatting.Indented);
            }
            return return_;
        }

        [HttpPost]
        public async Task<JsonResult> UpdateUser()
        {
            ReplayToClient c = new ReplayToClient();

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            string json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var userid = o.SelectToken("userid").ToString();
                var username = o.SelectToken("username").ToString();
                var password = o.SelectToken("password").ToString();
                var email = o.SelectToken("email").ToString();
                var group = o.SelectToken("group").ToString();
                var AMGroupCode = o.SelectToken("amgroupcode").ToString();
                var date = o.SelectToken("date").ToString();
                var exp = o.SelectToken("exp").ToString();
                var scode = o.SelectToken("scode").ToString();
                var sansw = o.SelectToken("sansw").ToString();
                var akumulasi = o.SelectToken("akumulasi").ToString();
                var status = o.SelectToken("status").ToString();
                var branch = o.SelectToken("branch").ToString();

                if (IDS.Maintenance.User.UpdateUser(userid, username, password, email, group, stringToDatetime(date), exp, scode, sansw, akumulasi, Convert.ToBoolean(status), branch, AMGroupCode, userid))
                {
                    c.msg_ = "success";
                    c.response_ = "ok";
                }
                else
                {
                    c.msg_ = "Same thing Wrong!";
                    c.response_ = "error";
                }

            }
            return Json(c);
        }

        [HttpPost]
        public async Task<string> DeleteUserId()
        {
            string return_ = "{'msg_':'data cant be deleted','response_':'error'}";

            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            using var reader = new StreamReader(Request.Body);
            string json = await reader.ReadToEndAsync();

            if (Tool.GeneralHelper.ValidateJSON(json))
            {
                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
                var userid = o.SelectToken("userid").ToString();
                if (IDS.Maintenance.User.DeleteUserId(userid))
                {
                    return_ = Newtonsoft.Json.JsonConvert.SerializeObject(new ReplayToClient() { msg_ = "data has been deleted!", response_ = "ok" });
                }
            }
            return return_;
        }

        public JsonResult GetBranchStatus()
        {
            //System.IO.Stream req = Request.InputStream;
            //req.Seek(0, System.IO.SeekOrigin.Begin);
            //string json = new System.IO.StreamReader(req).ReadToEnd();
            
            string ho_ = sHO;
            string branch_ = sBranchCodeCode;
            return Json(new { ho = ho_, branch = branch_ });
        }

        public JsonResult ChangePassword(IFormCollection data)
        {
            if (sUser == null)
                RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1 || AccessLevel.ReadAccess == -1)
            {
                RedirectToAction("Index", "Main", new { Area = "" });
            }

            int InsertAccess = AccessLevel.CreateAccess;
            int EditAccess = AccessLevel.EditAccess;

            if (InsertAccess <= 0 || EditAccess <= 0)
            {
                return Json("Your do not have access to change user password");
            }


            if (string.IsNullOrWhiteSpace(data["UserID"]) || string.IsNullOrWhiteSpace(data["Password"]) || string.IsNullOrWhiteSpace(data["ConfirmPassword"]))
            {
                return Json("Error: Invalid data. Please contact your administrator");
            }

            if (data["Password"] != data["ConfirmPassword"])
            {
                return Json("Password does not match");
            }

            try
            {
                string userID = Convert.ToString(data["UserId"]);
                string password = new Tool.clsCryptho().Encrypt(data["Password"], "ids");

                IDS.Maintenance.User.UserPassChange(userID, password);

                return Json(string.Format("User {0} password has been changed", userID));
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            
        }

        public JsonResult GetAreaList(string groupCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(groupCode))
                    return this.Json(new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>());

                List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> areaList = IDS.Maintenance.MasterArea.GetMasterAreaByGroupCodeForDataSource(groupCode);

                return Json(new { status = 200, message = "Success", data = areaList });
            }
            catch
            {
                return Json(new { status = 500, message = "Failed to get area data" });
            }
        }
    }
    public class ReplayToClient
    {
        public string response_ { get; set; }
        public string msg_ { get; set; }

    }
}
