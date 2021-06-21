using Holism.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Holism.Api
{
    public class FileUploadChecker : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var fileUploadParameter = context.ActionDescriptor.Parameters.FirstOrDefault(i => i.ParameterType.IsFileUploadParameter());
            if (fileUploadParameter == null)
            {
                return;
            }
            EnsureFileIsUploaded(context.HttpContext.Request);
            EnsureUploadedFileMatchesParameterName(context.HttpContext.Request, fileUploadParameter);
        }

        private void EnsureFileIsUploaded(HttpRequest request)
        {
            if (request.Form == null || request.Form.Files.Count == 0)
            {
                throw new FrameworkException("No file is uploaded in HTTP request");
            }
        }

        private void EnsureUploadedFileMatchesParameterName(HttpRequest request, ParameterDescriptor parameter)
        {
            var parameterName = parameter.Name;
            var file = request.Form.Files.FirstOrDefault(i => i.Name == parameterName);
            if (file == null)
            {
                throw new FrameworkException($"Parameter name, and form field name should match for binding to happen. You've named your parameter {parameterName}, but file is uploaded with the name {request.Form.Files[0].Name}.");
            }
        }
    }
}
