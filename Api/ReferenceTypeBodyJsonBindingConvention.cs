using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Api
{
    public class ReferenceTypeBodyJsonBindingConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            var controllerName = action.Controller.ControllerType.FullName;
            var actionName = action.ActionName;
            if (actionName == "OkJson" || actionName == "ErrorJson")
            {
                return;
            }
            foreach (var parameter in action.Parameters)
            {
                if (parameter.ParameterInfo.ParameterType.IsFileUploadParameter())
                {
                    parameter.BindingInfo = parameter.BindingInfo ?? new BindingInfo();
                    parameter.BindingInfo.BindingSource = BindingSource.FormFile;
                }
                else if (!parameter.ParameterInfo.ParameterType.IsValueType && parameter.ParameterInfo.ParameterType.FullName != typeof(string).FullName)
                {
                    parameter.BindingInfo = parameter.BindingInfo ?? new BindingInfo();
                    parameter.BindingInfo.BindingSource = BindingSource.Body;
                }
            }
        }
    }
}