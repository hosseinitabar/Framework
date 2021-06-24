using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Holism.Framework
{
    public static class LongExtensions
    {
        public static T ToEnum<T>(this long value)
        {
            if (typeof(T).BaseType.Name != typeof(Enum).Name)
            {
                throw new Exception("Input type of generic method ToEnum<T>() is not an Enum");
            }
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static bool IsValidFor<T>(this long value, bool enumHasZero = false)
        {
            if (!enumHasZero && value == 0)
            {
                return false;
            }
            long all = 0;
            foreach (string name in Enum.GetNames(typeof(T)))
            {
                all |= Convert.ToInt64(Enum.Parse(typeof(T), name));
            }
            var result = value & ~all;
            return result == 0;
        }

        public static string DigitGroup(this long value)
        {
            return ((decimal)value).DigitGroup();
        }

        public static string Abbreviate(this long value)
        {
            if (value < 10_000)
            {
                return value.DigitGroup();
            }
            else if (value >= 10_000 && value < 1_000_000)
            {
                var result = Math.Round((decimal)value / (decimal)1000, 1) + "k";
                return result;
            }
            else if (value >= 1_000_000 && value < 1_000_000_000)
            {
                var result = Math.Round((decimal)value / (decimal)1_000_000, 1) + "m";
                return result;
            }
            else
            {
                var result = Math.Round((decimal)value / (decimal)1_000_000_000, 1) + "b";
                return result;
            }
        }
    }
}
