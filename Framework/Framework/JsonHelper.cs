using System.Text.Json;

namespace Holism.Framework 
{
    public class JsonHelper 
    {
        public static JsonSerializerOptions Options
        {
            get
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                return options;
            }
        }
    }
}