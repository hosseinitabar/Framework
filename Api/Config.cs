using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Holism.Api
{
    public class Config : Framework.Config
    {
        public static string Roles = "";

        public static bool DelayApiResponsesGlobally = false;

        internal static List<Type> Enumerations = new List<Type>();

        public static void AddEnumeration(Type enumType)
        {
            if (Enumerations.Any(i => i.FullName == enumType.FullName))
            {
                return;
            }
            Enumerations.Add(enumType);
        }

        public static void ConfigureEverything()
        {
            //Startup.AddControllerSearchAssembly(typeof(Mvc.MvcConfig).Assembly);
            Startup.AddControllerSearchAssembly(typeof(Config).Assembly);
        }

        public static bool RedirectAllHttpRequestsToHttps
        {
            get
            {
                var key = "RedirectAllHttpRequestsToHttps";
                if (HasSetting(key))
                {
                    var value = GetSetting(key);
                    return value.ToBoolean();
                }
                return false;
            }
        }
    }
}
