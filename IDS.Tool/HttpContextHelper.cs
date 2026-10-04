using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public static class HttpContextHelper
    {
        public static IHttpContextAccessor Accessor;

        public static HttpContext Current => Accessor?.HttpContext;

        public static string GetIp()
        {
            //string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            var context = HttpContextHelper.Current;

            string ip = context?.Request?.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {
                //ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                ip = context?.Connection?.RemoteIpAddress?.ToString();
            }
            return ip;
        }
    }
}
