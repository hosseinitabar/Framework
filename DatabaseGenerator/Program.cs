using System;
using Holism.Framework;
using System.IO;
using Holism.Validation;
using Holism.Generation;

namespace Holism.DatabaseUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Logger.LogWarning("No argument is specified for Holism.Database");
                return;
            }
            var file = Path.Combine(args[0], args[1]);
            var json = File.ReadAllText(file);
            if (json.IsNothing()) 
            {
                throw new ClientException($"{file} is empty");
            }
            if (!json.IsJson()) 
            {
                throw new ClientException($"{file} is not a valid JSON");
            }
            var database = json.Deserialize<Database>();
            DatabaseHelper.CreateDatabase(database);
            Logger.LogSuccess(database.Name);
        }
    }
}
