using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Holism.Framework
{
    public static class DecimalExtensions
    {
        public static string DigitGroup(this decimal value)
        {
            var tokens = value.ToString().Split('.');
            if (tokens.Length > 0 && tokens[0].Length > 3)
            {
                tokens[0] = Regex.Replace(tokens[0], @"(\d)(?=(\d{3})+$)", "$1,");
            }
            var result = string.Join('.', tokens);
            return result;
        }
    }
}
