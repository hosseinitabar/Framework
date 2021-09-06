using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text.Json;

namespace Holism.Framework
{
    public static class StringExtensions
    {
        public static bool IsNothing(this string text)
        {
            var isNothing = string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
            return isNothing;
        }

        public static bool IsSomething(this string text)
        {
            var isSomething = !text.IsNothing();
            return isSomething;
        }

        public static T ToEnum<T>(this string value)
        {
            if (typeof(T).BaseType.Name != typeof(Enum).Name)
            {
                throw new Exception("Input type of generic method ToEnum<T>() is not an Enum");
            }
            if (value.IsNumeric())
            {
                var number = (long)value.ToLong();
                if (!number.IsValidFor<T>())
                {
                    throw new Exception($"Enum {typeof(T).Name} does not have a numeric value of {value}");
                }
            }
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static int ToInt(this string text)
        {
            int result = 0;
            int.TryParse(text, out result);
            return result;
        }

        public static long ToLong(this string text)
        {
            long result = 0;
            long.TryParse(text, out result);
            return result;
        }

        public static decimal ToDecimal(this string text)
        {
            decimal result = 0;
            decimal.TryParse(text, out result);
            return result;
        }

        public static object Deserialize(this string json)
        {
            return JsonSerializer.Deserialize(json, typeof(object), JsonHelper.Options);
        }

        public static T Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonHelper.Options);
        }

        public static List<T> SplitCsv<T>(this string text)
        {
            List<T> result = new List<T>();
            if (text.IsNothing())
            {
                return result;
            }
            List<string> tokens = text.Split(',').Select(i => i.Trim()).ToList();
            tokens.ForEach(t =>
            {
                try
                {
                    result.Add((T)Convert.ChangeType(t, typeof(T)));
                }
                catch
                {
                    Logger.LogError($"Error in converting item {t} to type {typeof(T)} in SplitCsv<T>() method");
                }
            });
            return result;
        }

        public static List<string> SplitCsv(this string text)
        {
            return SplitCsv<string>(text);
        }
    }
}