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
            Startup.AddControllerSearchAssembly(typeof(Startup).Assembly);
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
        
        public static string[] CorsOrigins
        {
            get
            {
                var key = "CorsOrigins";
                if (HasSetting(key))
                {
                    var origins = GetSetting(key).SplitCsv().Select(i => i.TrimEnd('/')).ToArray();
                    return origins;
                }
                return new string[] { };
            }
        }

        public static bool CorsOriginsSpecified
        {
            get
            {
                return CorsOrigins.Length > 0;
            }
        }
    }
}
