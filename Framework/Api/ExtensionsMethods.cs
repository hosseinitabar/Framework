using Holism.Framework;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Holism.Api
{
    public static class ExtensionMethods
    {
        public static string ToAbsolute(this string relativePath)
        {
            relativePath = relativePath.Replace("/", @"\").TrimStart('\\');
            var path = AppContext.BaseDirectory;
            path = path.Replace(@"\bin\Debug\netcoreapp2.2", "");
            path = Path.Combine(path, relativePath);
            return path;
        }

        public static void EnableCors(this HttpContext context)
        {
            var originHeaders = context.Request.Headers["Origin"];
            if (originHeaders.Count == 0)
            {
                return;
            }
            var originHeader = originHeaders.First();
            var corsOriginsContainOrigin = Config.CorsOrigins.ToList().Contains(originHeader);
            if (corsOriginsContainOrigin)
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", originHeader);
                context.Response.Headers.Add("Access-Control-Allow-Headers", "returnokwithjsononunauthorization,returnokwithjsonresponseonexceptions");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            }
            else
            {
                Logger.LogWarning($"CORS list is provided, but does not contain {originHeader}. Please check your SettingsOverride.json file, and make sure that the CORS is configured properly. CORS origins are {Config.CorsOrigins.ToList().Merge()}");
            }
        }

        public static bool IsFileUploadParameter(this Type parameterType)
        {
            return parameterType == typeof(IFormFile) || parameterType == typeof(IFormFileCollection) || typeof(IEnumerable<IFormFile>).IsAssignableFrom(parameterType);
        }
    }
}
