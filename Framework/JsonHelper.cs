using System.Text.Json;

namespace Holism.Framework 
{
    public class JsonHelper 
    {
        public static JsonSerializerOptions Options
        {
            get
            {
                var options = new JsonSerializerOptions {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return options;
            }
        }
    }
}