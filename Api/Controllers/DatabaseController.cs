using System.Linq;
using System;
using System.Reflection;
using Holism.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using Holism.Framework;
using Holism.DatabaseUpdater;

namespace Holism.Api
{
    [Authorize(Roles="Admin")]
    public class DatabaseController : HolismController
    {
        [HttpGet]
        public IActionResult Update()
        {
            var databaseJsonFiles = Directory.GetFiles(System.IO.Path.Combine(AppContext.BaseDirectory, "Database"), "*.json", SearchOption.AllDirectories);
            foreach (var jsonFile in databaseJsonFiles)
            {
            }
            return OkJson();
        }

        public static void UpdateDatabase(string jsonFile)
        {
            try
            {
                var json = System.IO.File.ReadAllText(jsonFile);
                var database = json.Deserialize<Database>();
                DatabaseHelper.CreateDatabase(database);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
