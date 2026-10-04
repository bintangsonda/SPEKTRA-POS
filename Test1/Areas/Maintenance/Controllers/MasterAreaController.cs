using IDS.Web.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Syncfusion.EJ2.Base;
using System.Linq;

namespace IDS.Web.UI.Areas.Maintenance.Controllers
{
    [Area("Maintenance")]
    public class MasterAreaController : MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCodeCode;
        private string sHO;

        public MasterAreaController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCodeCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
        }

        [HttpPost]
        //public JsonResult GetData([FromBody] DataManagerRequest dm)
        //{
        //    try
        //    {
        //        IEnumerable<IDS.Maintenance.MasterArea> data = IDS.Maintenance.MasterArea.GetData();

        //        var dataOps = new DataOperations();

        //        // Filtering
        //        if (dm.Where != null && dm.Where.Any())
        //        {
        //            data = dataOps.PerformFiltering(data, dm.Where, "and");
        //        }

        //        // Search
        //        if (dm.Search != null && dm.Search.Any())
        //        {
        //            data = dataOps.PerformSearching(data, dm.Search);
        //        }

        //        // Sorting
        //        if (dm.Sorted != null && dm.Sorted.Any())
        //        {
        //            data = dataOps.PerformSorting(data, dm.Sorted);
        //        }

        //        int count = data.Count();

        //        // Paging
        //        if (dm.Skip != 0)
        //            data = data.Skip(dm.Skip);

        //        if (dm.Take != 0)
        //            data = data.Take(dm.Take);

        //        return Json(new { result = data, count = count });
        //    }
        //    catch
        //    {
        //        return Json(new { result = new List<object>(), count = 0 });
        //    }
        //}

        public ActionResult Index()
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

            ViewData["Branch"] = IDS.GeneralTable.Branch.GetBranchForDatasource();

            return View();
        }

        [HttpPost]
        public ActionResult Create(int? FormAction, IDS.Maintenance.MasterArea obj)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            string currentUser = sUser;
            obj.OperatorID = currentUser;

            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return Json(new { msg = "Session timeout. Please relogin", success = false });
            }

            if (FormAction == 1)
            {
                if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
                {
                    return Json(new { msg = "You have no access to Create Data", success = false });
                }
            }
            if (FormAction == 2)
            {
                if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
                {
                    return Json(new { msg = "You have no access to Edit Data", success = false });
                }
            }

            string message = "";
            try
            {
                int result = obj.InsUpDel((int)FormAction, ref message);

                if (result > 0)
                {
                    if ((int)FormAction == 1)
                    {

                        return Json(new { msg = "New Master Area has been saved.", success = true });
                    }
                    else
                    {
                        return Json(new { msg = "Master Area has been edited.", success = true });
                    }

                }
                else
                    return Json(new { msg = message, success = false });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, success = false });
            }
        }

        [HttpPost]
        public ActionResult Delete(IDS.Maintenance.MasterArea obj)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup, this.ControllerContext.RouteData.Values["controller"].ToString());

            string currentUser = sUser;
            obj.OperatorID = currentUser;

            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return Json(new { msg = "Session timeout. Please relogin", success = false });
            }

            if (AccessLevel.ReadAccess == -1 || AccessLevel.DeleteAccess == 0)
            {
                return Json(new { msg = "You have no access to Delete Data", success = false });
            }
            
            string message = "";
            try
            {
                int result = obj.Delete(ref message);

                if (result > 0)
                {
                    return Json(new { msg = "Master Area has been deleted.", success = true });
                }
                else
                    return Json(new { msg = message, success = false });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, success = false });
            }
        }


        public JsonResult GetDataForEdit(string AreaCode)
        {
            if (sUser == null)
            {
                return Json(new { success = false });
            }
            IDS.Maintenance.MasterArea obj = IDS.Maintenance.MasterArea.GetDataForEdit(AreaCode);
           
            return Json(new { obj = obj, success = true });
        }
    }
}
