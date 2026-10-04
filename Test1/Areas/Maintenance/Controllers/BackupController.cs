//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//
//using Microsoft.SqlServer.Management.Smo;
//using Microsoft.SqlServer.Management.Common;

//namespace IDS.Web.UI.Areas.Maintenance.Controllers
//{
//    public class BackupController :   IDS.Web.UI.Controllers.MenuController
//    {
//        private static Microsoft.SqlServer.Management.Smo.Server svr;
//        // GET: Maintenance/Backup
//        public ActionResult Index()
//        {
//            if (sUser == null)
//                return RedirectToAction("index", "Main", new { area = "" });
            
//            ViewBag.UserMenu = MainMenu;
//            ViewBag.UserLogin = Session[Tool.GlobalVariable.SESSION_USER_ID].ToString();
//            ViewData["SelectAllDatabase"] = new SelectList(IDS.Maintenance.UserMntMainProject.GEtAllDatabase(), "Value", "Text");
//            ViewData["SelectALLServerName"] = new SelectList(GetServerName(), "Value", "Text");
//            ViewData["GEtAllBackUpFile"] = new SelectList(GetFileDatabaseName(), "Value", "Text");
//            return View();
//        }

//        public JsonResult BackuPDatabase()
//        {
//            var result = new { status = "error", msg = "Invalid Backup" };
//            System.IO.Stream req = Request.InputStream;
//            req.Seek(0, System.IO.SeekOrigin.Begin);
//            string json = new System.IO.StreamReader(req).ReadToEnd();
//            if (Tool.GeneralHelper.ValidateJSON(json)) {
//                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
//                var dbnamefurename = o.SelectToken("dbnamefurename").ToString();
//                var dbfilename = o.SelectToken("dbfilename").ToString();
//                //  System.Diagnostics.Debug.WriteLine(dbnamefurename + "\n" + dbfilename);
//                string filePath = "C:\\Finance System\\DbBackUp\\";
//               // string strConn = Convert.ToString(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Conn"]);
//                if (System.IO.Directory.Exists(filePath))
//                {
//                    if (System.IO.File.Exists(filePath + dbfilename + ".bak"))
//                    {
//                        System.IO.File.Delete(filePath + dbfilename + ".bak");
//                    }
//                }
//                else
//                {
//                    System.IO.Directory.CreateDirectory(filePath);
//                }
//                string strConn = Convert.ToString(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Conn"]);
//                Microsoft.SqlServer.Management.Smo.Backup dbBackup = new Microsoft.SqlServer.Management.Smo.Backup();
//                System.Data.SqlClient.SqlConnection sqlConn = new System.Data.SqlClient.SqlConnection(strConn);
//                Microsoft.SqlServer.Management.Common.ServerConnection connection = new Microsoft.SqlServer.Management.Common.ServerConnection(sqlConn);
//                Server sqlServer = new Server(connection);
//                dbBackup.Action = BackupActionType.Database;
//                dbBackup.Database = dbnamefurename;
//                BackupDeviceItem deviceItem = new BackupDeviceItem(filePath + dbfilename + ".bak", DeviceType.File);
//                dbBackup.Devices.Add(deviceItem);
//                dbBackup.Complete += ((sende, eveb) =>
//                    result = new { status = "success", msg = "Success Backup" }
//                );
//                try {
//                    dbBackup.SqlBackup(sqlServer);
//                    result = new { status = "success", msg = "Success Backup" };
//                }
//                catch (Exception e)
//                {
//                    result = new { status = "error", msg = e.Message};
//                }
//             }
//            return Json(result);
//        }

//        private static List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetServerName()
//        {
//            string namaDatabase = IDS.Maintenance.UserMntMainProject.NamaDataBase();
//            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> RP = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
//            RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = namaDatabase, Value = namaDatabase });
//            return RP;
//        }

//        private  List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> GetFileDatabaseName()
//        {
//            List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> RP = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();//C:\\Finance System\\DbBackUp\\
//            string filePath = "C:\\Finance System\\DbBackUp\\";
//            // string strConn = Convert.ToString(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Conn"]);
//            if (!System.IO.Directory.Exists(filePath))
//            {
//                System.IO.Directory.CreateDirectory(filePath);
//            }
//            string[] filePaths = System.IO.Directory.GetFiles(@"C:\\Finance System\\DbBackUp\\");
//            foreach (string x in filePaths)
//            {
//                RP.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text =System.IO.Path.GetFileName(x), Value = System.IO.Path.GetFileName(x) });
//            }
//            return RP;
//        }

//        public JsonResult RestoreDatabaseWithName() {
//            var result = new { status = "error", msg = "Invalid Backup" };
//            System.IO.Stream req = Request.InputStream;
//            req.Seek(0, System.IO.SeekOrigin.Begin);
//            string json = new System.IO.StreamReader(req).ReadToEnd();
//            if (Tool.GeneralHelper.ValidateJSON(json)) {
//                var o = Newtonsoft.Json.Linq.JObject.Parse(json);
//                var dbnamefurename = o.SelectToken("dbname").ToString();
//                var dbusername = o.SelectToken("username").ToString();
//                var dbpassword = o.SelectToken("password").ToString();
//                var dbhost = o.SelectToken("host").ToString();
//                ////var hasil = "dbnamefurename-->" + dbnamefurename + " ,dbusername-->" + dbusername + " ,dbpassword-->" + dbpassword+ "" + "dbhost-->"+ dbhost;
//                Microsoft.SqlServer.Management.Common.ServerConnection connection = new Microsoft.SqlServer.Management.Common.ServerConnection(dbhost, dbusername, dbpassword);
//                Server sqlServer = new Server(connection);
//                Restore sqlRestore = new Restore();
//                sqlRestore.Devices.AddDevice(@"C:\\Finance System\\DbBackUp\\" + dbnamefurename, DeviceType.File);
//                sqlRestore.Database = dbnamefurename;
//                sqlRestore.Action = RestoreActionType.Database;
//                sqlRestore.ReplaceDatabase = true;
//                sqlRestore.PercentCompleteNotification = 10;
//                sqlRestore.Complete += new ServerMessageEventHandler(sqlRestore_Complete);
//                try
//                {
//                    sqlRestore.SqlRestore(sqlServer);
//                    result = new { status = "success", msg = "Success Restore" };
//                }
//                catch (Exception e)
//                {
//                    result = new { status = "error", msg = e.Message };
//                }
//            }
//            return Json(result);
//        }

//        public void RestoreDatabase(String databaseName, String filePath, String serverName, String userName, String password)
//        {
//            Microsoft.SqlServer.Management.Common.ServerConnection connection = new Microsoft.SqlServer.Management.Common.ServerConnection(serverName, userName, password);
//            Server sqlServer = new Server(connection);
//            Restore sqlRestore = new Restore();
//            sqlRestore.Devices.AddDevice(@"C:\\Finance System\\DbBackUp\\"+databaseName, DeviceType.File);
//            sqlRestore.Database = databaseName;
//            sqlRestore.Action = RestoreActionType.Database;
//            sqlRestore.ReplaceDatabase = true;
//            sqlRestore.PercentCompleteNotification = 10;
//            sqlRestore.Complete += new ServerMessageEventHandler(sqlRestore_Complete);
//            sqlRestore.SqlRestore(sqlServer);
//        }
//        private void sqlRestore_Complete(object sender, ServerMessageEventArgs e)
//        {
//            System.Diagnostics.Debug.WriteLine("Restore Complete");
//        }
//    }
//}