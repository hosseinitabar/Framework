using System.Linq;
using System;
using System.Reflection;
using Holism.DataAccess;
using Holism.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Holism.Api
{
    [Authorize(Role="Admin")]
    public class MigrationController : HolismController
    {
        [HttpGet]
        public IActionResult Apply()
        {
            var assemblies = Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
            assemblies = assemblies
                .Where(i => i.GetName().Name.Contains("DataAccess"))
                .ToList();
            var result = "";
            foreach (var assembly in assemblies) 
            {
                var types = assembly
                    .GetTypes()
                    .Where(i => i.Name.Contains("Context"))
                    .Where(i => !i.Name.Contains("Snapshot"))
                    .Where(i => i.Name != "DatabaseContext")
                    .ToList();
                foreach (var type in types) 
                {
                    result += $"{type.Name}\n";
                    var context = (DatabaseContext)Activator.CreateInstance(type);
                    // result += context.Database.GetDbConnection().ConnectionString;
                    context.Database.Migrate();
                }
            }
            return OkJson(result);
        }
    }
}
