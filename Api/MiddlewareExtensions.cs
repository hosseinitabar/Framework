using Holism.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Dynamic;
using System.Net;
using System.Threading;

namespace Holism.Api
{
    public static class MiddlewareExtensions
    {
        public static void UseExceptionHandler(this IApplicationBuilder application)
        {
            application.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    await LogAndCreateErrorJson(context);
                });
            });
        }

        private static async System.Threading.Tasks.Task LogAndCreateErrorJson(HttpContext context)
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>().Error;
            if (exception.GetType().FullName != typeof(ClientException).FullName)
            {
                Logger.LogException(exception);
            }
            dynamic response = new ExpandoObject();
            response.Type = MessageType.Error.ToString();
            var message = ExceptionHelper.TranslateToFriendlyMessage(exception);
            response.Text = message;
            if (exception is ClientException && ((ClientException)exception).Code.IsSomething())
            {
                response.Code = ((ClientException)exception).Code;
            }
            if (exception is ClientException && ((ClientException)exception).Data != null)
            {
                response.Data = ((ClientException)exception).Data;
            }
            if (Config.IsDeveloping)
            {
                response.StackTrace = ExceptionHelper.BuildExceptionString(exception);
            }
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            string result = ((object)response).JsonSerialize(Casing.CamelCase);
            context.EnableCors();
            await context.Response.WriteAsync(result);
        }

        public static void UseApiDelayedResponse(this IApplicationBuilder application)
        {
            application.Use(async (context, next) =>
            {
                Thread.Sleep(5000);
                await next.Invoke();
            });
        }
    }
}