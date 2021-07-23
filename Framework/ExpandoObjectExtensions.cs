using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Holism.Framework
{
    public static class ExpandoObjectExtensions
    {
        public static T To<T>(this ExpandoObject expandoObject) where T : class, new()
        {
            var result = new T();
            IDictionary<string, object> expando = (IDictionary<string, object>)expandoObject;
            var properties = result.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (expando.ContainsKey(property.Name))
                {
                    property.SetValue(result, expando[property.Name]);
                }
            }
            return result;
        }

        public static dynamic CopyProperties(this ExpandoObject source, ExpandoObject target)
        {
            IDictionary<string, object> expando = (IDictionary<string, object>)source;
            foreach (var item in expando)
            {
                target.AddProperty(item.Key, item.Value);
            }
            return target;
        }

        public static object GetProperty(this ExpandoObject expandoObject, string propertyName)
        {
            var dictionary = expandoObject as IDictionary<string, object>;
            if (dictionary.ContainsKey(propertyName))
            {
                return dictionary[propertyName];
            }
            throw new ServerException($"ExpandoObject {expandoObject.GetType().FullName} does not have property {propertyName}");
        }

        public static ExpandoObject AddProperty(this ExpandoObject expandoObject, string propertyName, object propertyValue)
        {
            var dictionary = expandoObject as IDictionary<string, object>;
            if (dictionary.ContainsKey(propertyName))
                dictionary[propertyName] = propertyValue;
            else
                dictionary.Add(propertyName, propertyValue);
            return dictionary as ExpandoObject;
        }

        public static bool Has(this ExpandoObject expandoObject, string propertyName)
        {
            var dictionary = expandoObject as IDictionary<string, object>;
            if (dictionary.ContainsKey(propertyName))
                return true;
            else
                return false;
        }

        public static ExpandoObject Remove(this ExpandoObject expandoObject, string field)
        {
            IDictionary<string, object> expando = (IDictionary<string, object>)expandoObject;
            expando.Remove(field);
            return expando as ExpandoObject;
        }
    }
}
