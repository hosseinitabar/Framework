using Holism.Framework;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Holism.Api
{
    public class ModelChecker : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }
            var errors = context.ModelState.Values.SelectMany(i => i.Errors).Where(i => i.Exception != null).Select(i => i.Exception).Select(i => i.Message).ToList();
            errors.AddRange(context.ModelState.Values.SelectMany(i => i.Errors).Where(i => i.Exception == null).Select(i => i.ErrorMessage).ToList());
            throw new ClientException(errors.Merge());
        }
    }
}
