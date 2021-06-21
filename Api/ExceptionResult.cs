using Holism.Framework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;

namespace Holism.Api
{
    public class ExceptionResult : ContentResult
    {
        public ExceptionResult(Exception exception)
        {
            dynamic response = new ExpandoObject();
            response.Type = MessageType.Error.ToString();
            var message = ExceptionHelper.TranslateToFriendlyMessage(exception);
            response.Text = message;
            if (exception is ClientException && ((ClientException)exception).Code.IsSomething())
            {
                response.Code = ((ClientException)exception).Code;
            }
            if (exception is ClientException && ((ClientException)exception).Data.IsNotNull())
            {
                response.Data = ((ClientException)exception).Data;
            }
            this.ContentType = "application/json; charset=utf-8";
            this.StatusCode = (int)HttpStatusCode.OK;
            this.Content = ((object)response).JsonSerialize(Casing.CamelCase);
        }
    }
}
