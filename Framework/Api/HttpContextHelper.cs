using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Api
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextHelper.httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext Current => httpContextAccessor.HttpContext;
    }
}
