using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class HistoryController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public HistoryController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        //Edited By Renaldi 13 March 2025
        //public JsonResult GetData(string period, string status)
        //{
        //    JsonResult result = new JsonResult();
        //    if(String.IsNullOrEmpty(period))
        //    {
        //        DateTime a = DateTime.Now;
        //        period = a.ToString("yyyyMM");
        //    }
        //    else
        //    {
        //        DateTime datePeriod = Convert.ToDateTime(period);
        //        period = datePeriod.ToString("yyyyMM");
        //    }
        //    try
        //    {

        //        List<IDS.Maintenance.History> his = IDS.Maintenance.History.GetHistories(period);

        //        if(status != "ALL")
        //        {
        //            his = his.Where(x => x.Status == status).ToList();
        //        }
        //        result = Json(Newtonsoft.Json.JsonConvert.SerializeObject(his));
        //        result.MaxJsonLength = int.MaxValue;
        //    }
        //    catch (Exception e)
        //    {

        //    }

        //    return result;
        //}

        public JsonResult GetData(string period, string status)
        {
            //if (sUser == null)
            //    throw new Exception("You do not have access to access this page");

            JsonResult result = new JsonResult(new {});

            if (String.IsNullOrEmpty(period))
            {
                DateTime a = DateTime.Now;
                period = a.ToString("yyyyMM");
            }
            else
            {
                DateTime datePeriod = Convert.ToDateTime(period);
                period = datePeriod.ToString("yyyyMM");
            }

            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();

                var orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{orderColumnIndex}][name]"].FirstOrDefault();

                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //int pageSize = (length != null ? Convert.ToInt32(length) : 0);
                int pageSize = 0;

                switch (length)
                {
                    case null:
                        break;
                    case "-1":
                        pageSize = 0;
                        break;
                    default:
                        pageSize = Convert.ToInt32(length);
                        break;
                }

                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0; // Total keseluruhan data
                int totalRecordsShowing = 0; // Total data setelah filter / search

                List<IDS.Maintenance.History> his = IDS.Maintenance.History.GetHistories(period);

                if (status != "ALL")
                {
                    his = his.Where(x => x.Status == status).ToList();
                }

                totalRecords = his.Count();

                // Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    his = his.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }

                // Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    string searchValueLower = searchValue.ToLower();

                    his = his.Where(x => x.Id.ToString().ToLower().Contains(searchValueLower) ||
                        x.ComputerName.ToLower().Contains(searchValueLower) ||
                        x.DateUpdated.ToString(Tool.GlobalVariable.DEFAULT_DATE_FORMAT).ToLower().Contains(searchValueLower) ||
                        x.IpAddress.ToLower().Contains(searchValueLower) ||
                        x.UserName.ToLower().Contains(searchValueLower) ||
                        x.Status.ToString().ToLower().Contains(searchValueLower) ||
                        x.TableName.ToString().ToLower().Contains(searchValueLower) ||
                        x.KeyField1.ToString().ToLower().Contains(searchValueLower) ||
                        x.KeyField2.ToString().ToLower().Contains(searchValueLower) ||
                        x.KeyField3.ToString().ToLower().Contains(searchValueLower) ||
                        x.KeyField4.ToString().ToLower().Contains(searchValueLower) ||
                        x.KeyField5.ToString().ToLower().Contains(searchValueLower) ||
                        x.KeyField6.ToLower().Contains(searchValueLower)
                        ).ToList();
                }

                totalRecordsShowing = his.Count();

                // Paging
                if (pageSize > 0)
                    his = his.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data                
                result = this.Json(new { draw = draw, recordsFiltered = totalRecordsShowing, recordsTotal = totalRecords, data = his });
                //result.MaxJsonLength = int.MaxValue;
            }
            catch (Exception ex)
            {
            }

            return result;
        }
        //End Edited

        // GET: Maintenance/History
        public ActionResult Index()
        {
            ViewData["Status"] = new SelectList(IDS.Maintenance.History.GetStatus(), "Value", "Text");
            ViewBag.UserMenu = MainMenu;
            ViewBag.UserLogin = sUser;
            return View();
        }
        public ActionResult ViewUpdate(int id)
        {
            var his = IDS.Maintenance.History.GetHistory(id);
            var a = new IDS.Maintenance.CustomViewModel();
            string[] oldVal = his.OldValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] newVal = his.UpdateValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            a.oldValue = oldVal;
            a.newValue = newVal;
            a.Id = his.Id;
            a.TableName = his.TableName;
            a.DateUpdated = his.DateUpdated;
            a.UserName = his.UserName;
            a.Status = his.Status;

            return PartialView("ViewUpdate",a);
        }

        public ActionResult GetLastLogin()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            //if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            //{
            //    //return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            //    return RedirectToAction("error403", "error", new { area = "" });
            //}
            try
            {
                string Userid = sUser.ToString();
                string result = "";
                result = IDS.Maintenance.History.GetLastLogin(Userid);
                DateTime dateTime = Convert.ToDateTime(result);

                string formattedDate = dateTime.ToString("dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                if (result !="")
                {
                    return Json(formattedDate);

                }
                else
                    throw new Exception("Error");

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }
    }
}