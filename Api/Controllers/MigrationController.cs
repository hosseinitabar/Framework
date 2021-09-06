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
using System.Data.SqlClient;
using Holism.Framework;

namespace Holism.Api
{
    [Authorize(Roles="Admin")]
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
                    ApplyMigration(type);
                }
            }
            return OkJson();
        }

        public static void ApplyMigration(Type type)
        {
            try
            {
                var context = (DatabaseContext)Activator.CreateInstance(type);
                Logger.LogInfo($"Applying migration for {type.Name}");
                TestConnection(context);
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static void TestConnection(DatabaseContext context)
        {
            var connectionString = context.Database.GetDbConnection().ConnectionString;
            var connection = new SqlConnection(connectionString);
            Logger.LogInfo($"Connection string: {connectionString}");
            connection.Open();
            connection.Close();
        }
    }
}
