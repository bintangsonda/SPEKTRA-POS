using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace IDS.Web.UI.Controllers
{
    public class ErrorController : Controller
    {
        // Error akses ke page yang tidak ada
        public ActionResult Error404()
        {
            return View();
        }

        // Error No Access
        public ActionResult Error403()
        {
            return View();
        }

        // Error Internal
        public ActionResult Error500()
        {
            return View();
        }
        public ActionResult ErrorLicense()
        {
            ViewData["Message"] = "You don't have permission to access on this server. Please contact PT. Intidata Anugrah Pratama at (+62-21) 5595-2979";
            return View();
        }
    }
}