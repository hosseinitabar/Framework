using Holism.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Api.Controllers
{
    public class JavaScriptController : DefaultController
    {
        [HttpGet]
        public IActionResult Enums()
        {
            return GeneralController.Enums(Request, Config.Enumerations);
        }
    }
}
