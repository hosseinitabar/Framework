using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Holism.Validation
{
    public static class Extensions
    {
        public static Ensure Ensure(this object @object)
        {
            return new Ensure(@object);
        }

        public static EnsureNumber Ensure(this int number)
        {
            return new EnsureNumber(number);
        }

        public static EnsureNumber Ensure(this long number)
        {
            return new EnsureNumber(number);
        }

        public static EnsureString Ensure(this string text)
        {
            return new EnsureString(text);
        }

        public static EnsureGuid Ensure(this Guid guid)
        {
            return new EnsureGuid(guid);
        }

        public static bool IsGuid(this string text)
        {
            Guid guid;
            if (Guid.TryParse(text, out guid))
            {
                return true;
            }
            return false;
        }

        public static bool IsEmptyGuid(this string text)
        {
            if (!text.IsGuid())
            {
                return false;
            }
            if (text == Guid.Empty.ToString())
            {
                return true;
            }
            return false;
        }

        public static bool IsNonEmptyGuid(this string text)
        {
            return text.IsGuid() && !text.IsEmptyGuid();
        }

        public static bool IsJson(this string text)
        {
            text = text.Trim();
            try
            {
                if (text.StartsWith("["))
                {
                    var json = JArray.Parse(text);
                    return true;
                }
                if (text.StartsWith("{"))
                {
                    var json = JObject.Parse(text);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsJsonArray(this string text)
        {
            text = text.Trim();
            try
            {
                if (text.StartsWith("["))
                {
                    var json = JArray.Parse(text);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsJsonObject(this string text)
        {
            text = text.Trim();
            try
            {
                if (text.StartsWith("{"))
                {
                    var json = JObject.Parse(text);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}