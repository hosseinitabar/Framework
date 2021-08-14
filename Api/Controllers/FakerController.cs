using System.Linq;
using System;
using System.Reflection;
using Holism.DataAccess;
using Holism.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Holism.Api
{
    [Authorize(Role="Admin")]
    public class MigrationController : HolismController
    {
        [HttpGet]
        public IActionResult Apply()
        {
            if (!Config.IsDeveloping) 
            {
                throw new ClientException("Faker data can not be run in production.")
            }
            // run fakers
        }
    }
}
