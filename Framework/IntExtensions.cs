using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Holism.Framework
{
    public static class IntExtensions
    {
        public static T ToEnum<T>(this int value)
        {
            return ((long)value).ToEnum<T>();
        }

        public static bool IsValidFor<T>(this int value, bool enumHasZero = false)
        {
            return ((long)value).IsValidFor<T>(enumHasZero);
        }

        public static string DigitGroup(this int value)
        {
            return ((decimal)value).DigitGroup();
        }

        public static string Abbreviate(this int value)
        {
            return ((long)value).Abbreviate();
        }
    }
}
