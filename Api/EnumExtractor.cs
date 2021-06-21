using Holism.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Holism.Api
{
    public class EnumExtractor
    {
        public static string Extract(List<Type> types)
        {
            var enums = types.SelectMany(i => i.Assembly.DefinedTypes.Where(x => x.IsEnum).ToList());
            var result = @"var enums=enums||{};";
            foreach (var @enum in enums)
            {
                result += $"enums.{@enum.Name.Camelize()}={{}};";
                var names = @enum.GetEnumNames();
                foreach (var name in names)
                {
                    result += $"enums.{@enum.Name.Camelize()}.{name.Camelize()}={{value:{(int)Enum.Parse(@enum, name)},key:'{name.Camelize()}',translation:'{string.Empty}'}};";
                }
            }
            //result += "Object.freeze(enums);Object.seal(enums);";
            //foreach (var @enum in enums)
            //{
            //    result += "Object.freeze(enums.{0});Object.seal(enums.{0});".Fill(@enum.Name);
            //    var names = @enum.GetEnumNames();
            //    foreach (var name in names)
            //    {
            //        result += "Object.freeze(enums.{0}.{1});Object.seal(enums.{0}.{1});".Fill(@enum.Name, name, (int)Enum.Parse(@enum, name));
            //    }
            //}
            return result;
        }

        public static string Extract(Type type)
        {
            return Extract(new List<Type> { type });
        }
    }
}
