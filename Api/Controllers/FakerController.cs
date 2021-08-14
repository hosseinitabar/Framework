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
using Holism.Framework;

namespace Holism.Api
{
    [Authorize(Roles="Admin")]
    public class FakerController : HolismController
    {
        [HttpGet]
        public IActionResult Fake()
        {
            if (!Config.IsDeveloping) 
            {
                throw new ClientException("Faker data can not be run in production.");
            }
            // run fakers
            return OkJson("Faked");
        }
    }
}
