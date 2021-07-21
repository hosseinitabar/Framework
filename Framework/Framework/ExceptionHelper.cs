using Holism.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Holism.Framework
{
    public class ExceptionHelper
    {
        public static string GetSpecificInfo(Exception ex)
        {
            if (ex is ReflectionTypeLoadException)
            {
                var loaderExceptions = ((ReflectionTypeLoadException)ex).LoaderExceptions;
                var result = "";
                foreach (var loaderException in loaderExceptions)
                {
                    result += $"\r\n{loaderException.Message}";
                }
                return result;
            }
            else if (ex is TypeLoadException)
            {
                var typeLoadException = (TypeLoadException)ex;
                return $"\r\n{typeLoadException.Message} => {typeLoadException.TypeName}";
            }
            //else if (ex is HttpResponseException)
            //{
            //    var exception = ((HttpResponseException)ex);
            //    var result = "{0} - {1}".Fill(exception.Response.StatusCode, exception.Response.ReasonPhrase);
            //}
            else if (ex is FileNotFoundException)
            {
                return ((FileNotFoundException)ex).FileName;
            }
            return "";
        }

        public static string TranslateToFriendlyMessage(Exception ex)
        {
            string result = "";
            while (ex != null)
            {
                if (ex is ServerException || ex is ClientException)
                {
                    return ex.Message;
                }
                else if (ex is FileNotFoundException)
                {
                    return $"Fil {Path.GetFileNameWithoutExtension(((FileNotFoundException)ex).FileName)} is not found";
                }
                if (result.IsSomething())
                {
                    break;
                }
                ex = ex.InnerException;
            }
            return "An error occured. Please notify the adminstrator.";
        }

        private static void BuildExceptionString(Exception ex, ref string message)
        {
            message += "\r\n***********\r\n";
            message += ex.Message;
            message += "\r\n***********\r\n";
            message += ExceptionHelper.GetSpecificInfo(ex);
            message += "\r\n***********\r\n";
            message += FilterStackTrace(ex.StackTrace);
            if (ex.InnerException != null)
            {
                BuildExceptionString(ex.InnerException, ref message);
            }
            if (ex is AggregateException)
            {
                var innerExceptions = ((AggregateException)ex).InnerExceptions;
                foreach (var innerException in innerExceptions)
                {
                    message += "\r\n***********\r\n";
                    BuildExceptionString(innerException, ref message);
                }
            }
            if (ex is FileLoadException)
            {
                message += "\r\n************\r\n" + ((FileLoadException)ex).FusionLog;
                message += "\r\n************\r\n" + ((FileLoadException)ex).FileName;
            }
        }

        public static string BuildExceptionString(Exception ex)
        {
            string error = "";
            BuildExceptionString(ex, ref error);
            return error;
        }

        private static string FilterStackTrace(string stackTrace)
        {
            if (stackTrace.IsNothing())
            {
                return "";
            }
            var newStackTrace = Regex.Replace(stackTrace ,@"^(\s*at\s*(System|Microsoft)+|-*\s*End|\s*at\s*lambda).*$", "", RegexOptions.Multiline);
            newStackTrace = Regex.Replace(newStackTrace, @"\n\n", "", RegexOptions.Multiline);
            return newStackTrace;
        }
    }
}
