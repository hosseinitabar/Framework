// using Holism.Framework;
// using System.Text.Json;
// using System;
// using System.Diagnostics;
// using System.Linq;

// namespace Holism.Api
// {
//     public class RelatedItemsFlattenerJsonConverter : JsonConverter
//     {
//         public override bool CanRead => false;

//         public override bool CanConvert(Type objectType)
//         {
//             return true;
//         }

//         public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//         {
//             return null;
//         }

//         public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//         {
//             var stopwatch = new Stopwatch();
//             stopwatch.Start();
//             JToken token = JToken.FromObject(value);
//             if (token.Type == JTokenType.Array)
//             {
//                 var array = (JArray)token;
//                 foreach (var item in token)
//                 {
//                     FlattenRelatedItems(item);
//                 }
//             }
//             else if (token.Type == JTokenType.Object)
//             {
//                 var @object = (JObject)token;
//                 FlattenRelatedItems(@object);

//             }
//             token.WriteTo(writer);
//             stopwatch.Stop();
//         }

//         private void FlattenRelatedItems(JToken token)
//         {
//             if (token.Type == JTokenType.Array)
//             {
//                 var array = (JArray)token;
//                 foreach (var item in token)
//                 {
//                     FlattenRelatedItems(item);
//                 }
//             }
//             else if (token.Type == JTokenType.Object)
//             {
//                 var @object = (JObject)token;
//                 var relatedItemsProperty = @object.Properties().FirstOrDefault(i => i.Name.ToLower() == "RelatedItems".ToLower());
//                 if (relatedItemsProperty != null)
//                 {
//                     var relatedItems = @object[relatedItemsProperty.Name];
//                     @object.Remove(relatedItemsProperty.Name);
//                     var keys = ((JObject)relatedItems).Properties().Select(i => i.Name).ToList();
//                     foreach (var key in keys)
//                     {
//                         @object.Add(key, relatedItems[key]);
//                     }
//                 }
//                 var properties = @object.Properties().ToList();
//                 foreach (var property in properties)
//                 {
//                     FlattenRelatedItems(@object[property.Name]);
//                 }
//             }
//         }
//     }
// }
