using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public static class SessionHelper
    {
        private static IHttpContextAccessor? _contextAccessor;

        public static void Configure(IHttpContextAccessor accessor)
        {
            _contextAccessor = accessor;
        }

        public static string? Get(string key)
        {
            return _contextAccessor?.HttpContext?.Session.GetString(key);
        }

        public static void Set(string key, string value)
        {
            _contextAccessor?.HttpContext?.Session.SetString(key, value);
        }
    }
}
