using Holism.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Holism.Api
{
    public class DebugController : DefaultController
    {
        IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

        public DebugController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public DebugController()
        {

        }

        [HttpGet]
        public object ControllersAndActions()
        {
            if (!Config.IsDeveloping)
            {
                return ErrorJson("Not available in production mode");
            }
            var result = new List<DebuggingInfo>();
            var controllerActions = actionDescriptorCollectionProvider.ActionDescriptors.Items.ToList();
            foreach (var action in controllerActions)
            {
                var descriptor = action as ControllerActionDescriptor;
                if (descriptor.ControllerName == "Debug")
                {
                    continue;
                }
                if (descriptor.ActionName == "OkJson" || descriptor.ActionName == "ErrorJson")
                {
                    continue;
                }
                var debuggingInfo = new DebuggingInfo();
                debuggingInfo.Controller = descriptor.ControllerName;
                debuggingInfo.Action = descriptor.ActionName;
                debuggingInfo.TypeFqn = descriptor.ControllerTypeInfo.FullName;
                //debuggingInfo.Area = descriptor.ControllerTypeInfo.GetCustomAttribute<AreaAttribute>().RouteValue;
                result.Add(debuggingInfo);
            }
            return result;
        }

        [HttpGet]
        public object GetAllRoutes()
        {
            var result = new List<dynamic>();
            var routes = actionDescriptorCollectionProvider.ActionDescriptors.Items
                .Where(i => i.AttributeRouteInfo != null)
                .Select(i => new 
                {
                    i.AttributeRouteInfo.Name,
                    i.AttributeRouteInfo.Template
                }).ToList();
            return result;
        }
    }
}
