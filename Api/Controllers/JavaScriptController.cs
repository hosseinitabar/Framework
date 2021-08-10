using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Api
{
    public class JavaScriptController : HolismController
    {
        [HttpGet]
        public IActionResult Enums()
        {
            return HolismController.Enums(Request, Config.Enumerations);
        }
    }
}
