//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Web;
//using System.Web.Configuration;
//

//namespace IDS.Web.UI.Areas.Maintenance.Controllers
//{
//    public class ExpiredController : IDS.Web.UI.Controllers.MenuController
//    {
//        //public JsonResult ChangePass()
//        //{
//        //    var result = new { status = "error", msg = "ErrDesc" };
//        //    System.IO.Stream req = Request.InputStream;
//        //    req.Seek(0, System.IO.SeekOrigin.Begin);
//        //    string json = new System.IO.StreamReader(req).ReadToEnd();
//        //    if (Tool.GeneralHelper.ValidateJSON(json))
//        //    {
//        //        var o = Newtonsoft.Json.Linq.JObject.Parse(json);
//        //        var oldpass = o.SelectToken("oldpass").ToString();
//        //        var newpass = o.SelectToken("newpass").ToString();
//        //        var confirmpass = o.SelectToken("confirmpass").ToString();
//        //        if (IDS.Maintenance.User.UserPassChange(Convert.ToString(sUser), new Tool.clsCryptho().Encrypt(oldpass, "ids"), new Tool.clsCryptho().Encrypt(newpass, "ids"), new Tool.clsCryptho().Encrypt(confirmpass, "ids")))
//        //        {
//        //            result = new { status = "success", msg = "Sucess Change Password" };
//        //        }
//        //        else
//        //        {
//        //            result = new { status = "error", msg = "Wrong Password" };
//        //        }
//        //    }
//        //    return Json(result);
//        //}

//        //Add By Renaldi 24 August 2024
//        public ActionResult CheckMaintenanceExpired()
//        {
//            ////Ambil tanggal expired, untuk sekarang ditembak
//            //DateTime expiredDate = DateTime.Now.AddDays(-1);

//            //Ambil dari web config
//            string expiredDateString = ConfigurationManager.AppSettings["ExpiredDateValue"];
//            DateTime expiredDate = DateTime.Parse(expiredDateString);
//            DateTime weekBeforeExpiredDate = expiredDate.AddDays(-7);
//            DateTime now = DateTime.Now;


//            if (weekBeforeExpiredDate <= now)
//            {
//                return Json("expired,"+expiredDate.ToString("dd/MMM/yyyy"));
//            }

//            return Json("");
//        }
//        //End Add

//        //Add By Renaldi 26 August 2024
//        public ActionResult InputMaintenanceExpired(string maintenanceCode)
//        {
//            try
//            {
//                //Ambil kode
//                //Edited By Renaldi 7 April 2025 Update Maintenance
//                //string code = "INTIDATA";
//                string code = DateTime.Now.ToString("yyyyMM");
//                //End Edited

//                Tool.clsCryptho crypt = new Tool.clsCryptho();
//                string decryptCode = crypt.Decrypt(maintenanceCode, "ids");

//                //Generate Code
//                //string GeneratedCode = crypt.Encrypt("87654321INTIDATA1234qwerty", "ids");

//                if (decryptCode.Contains(code))
//                {
//                    // Update expired date
//                    string expiredDateString = ConfigurationManager.AppSettings["ExpiredDateValue"];
//                    DateTime expiredDate = DateTime.Parse(expiredDateString);
//                    DateTime newExpiredDate = expiredDate.AddYears(1);
//                    string newExpiredDateString = newExpiredDate.ToString("yyyy-MM-ddTHH:mm:ss");

//                    var config = WebConfigurationManager.OpenWebConfiguration("~");
//                    var appSettings = (AppSettingsSection)config.GetSection("appSettings");

//                    if (appSettings.SectionInformation.IsProtected)
//                    {
//                        appSettings.SectionInformation.UnprotectSection();
//                    }
//                    appSettings.Settings["ExpiredDateValue"].Value = newExpiredDateString;
//                    appSettings.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");

//                    config.Save(ConfigurationSaveMode.Modified);
//                    ConfigurationManager.RefreshSection("appSettings");

//                    return Json(new { msg = "success", date = newExpiredDate });
//                }
//                else
//                {
//                    return Json(new { msg = "failed"});
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(ex.Message);
//            }
            
//        }
//        //End Add
//    }
//}