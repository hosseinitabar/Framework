using Microsoft.AspNetCore.Authorization;
using Holism.Framework;
using Holism.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;

namespace Holism.Api
{
    // [Authorize(Policy = "HasRole")]
    // [Authorize]
    public class DefaultController : Controller
    {
        public static Func<long, Guid> UserGuidProvider;

        public static JavaScriptResult Enums(HttpRequest request, Type type)
        {
            var result = EnumExtractor.Extract(type);
            return new JavaScriptResult(result);
        }

        public static JavaScriptResult Enums(HttpRequest request, List<Type> types)
        {
            var result = EnumExtractor.Extract(types);
            return new JavaScriptResult(result);
        }

        public IActionResult OkJson(string message = null, object data = null, string code = null)
        {
            message = message ?? "Done";
            return JsonMessage(message, MessageType.Success, data, code);
        }

        public ActionResult ErrorJson(string message, object data = null, string code = null)
        {
            return JsonMessage(message, MessageType.Error, data, code);
        }

        private ActionResult JsonMessage(string message, MessageType messageType, object data = null, string code = null)
        {
            dynamic @object = new ExpandoObject();
            @object.type = messageType.ToString();
            @object.message = message;
            if (data != null)
            {
                @object.data = data;
            }
            if (code.IsSomething())
            {
                @object.code = code;
            }
            var result = new JsonResult(@object, JsonHelper.Options);
            result.StatusCode = (int)HttpStatusCode.OK;
            return result;
        }

        public string GetParameter(string name)
        {
            var parameter = Request.Query[name];
            if (parameter.Count > 0)
            {
                return parameter.First();
            }
            throw new ClientException($"{name} doesn't exist in query string parameters");
        }

        public string GetParameterOrNull(string name)
        {
            var parameter = Request.Query[name];
            if (parameter.Count > 0)
            {
                return parameter.First();
            }
            return null;
        }

        public T Extract<T>() where T : class, new()
        {
            var instance = new T();
            var properties = instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                ExtractProperty(instance, property);
            }
            return instance;
        }

        private void ExtractProperty(object instance, PropertyInfo property)
        {
            var values = HttpContext.Request.Form[property.Name];
            if (values.Count > 0)
            {
                if (property.PropertyType.Name == typeof(long).Name || property.PropertyType.FullName == typeof(long?).FullName)
                {
                    property.SetValue(instance, Convert.ToInt64(values[0]));
                }
                else if (property.PropertyType.Name == typeof(int).Name || property.PropertyType.FullName == typeof(int?).FullName)
                {
                    property.SetValue(instance, Convert.ToInt32(values[0]));
                }
                else if (property.PropertyType.Name == typeof(Guid).Name || property.PropertyType.FullName == typeof(Guid?).FullName)
                {
                    var guid = values[0];
                    if (guid.IsNonEmptyGuid())
                    {
                        property.SetValue(instance, Guid.Parse(values[0]));
                    }
                    else
                    {
                        Logger.LogWarning($"Property {property.PropertyType.Name} of model {instance.GetType().FullName} requires a Guid value, but in HTTP request we received {guid}.");
                    }
                }
                else if (property.PropertyType.Name == typeof(string).Name)
                {
                    property.SetValue(instance, values[0]);
                }
            }
        }

        public long UserId
        {
            get
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier).Value.ToLong();
                return id;
            }
        }

        public Guid UserGuid
        {
            get
            {
                var guid = UserGuidProvider?.Invoke(UserId);
                return guid.Value;
            }
        }

        [HttpGet]
        public IActionResult CheckAuthenticationToken()
        {
            return OkJson();
        }
    }
}
