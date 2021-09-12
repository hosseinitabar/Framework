using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Holism.Framework;
using Holism.Validation;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace Holism.Generation
{
    public abstract class ProviderGenerator
    {
        public static string OrganizationPrefix { get; set; }

        public static string Organization { get; set; }

        public static string Repository { get; set; }

        public static string RepositoryPath { get; set; }

        public Database Database { get; private set; }

        public ProviderGenerator()
        {
            var databaseJsonFile = Path.Combine(RepositoryPath, "Database.json");
            if (!File.Exists(databaseJsonFile))
            {
                throw new ServerException($"Database.json file does not exist {RepositoryPath}");
            }
            var databaseJson = File.ReadAllText(databaseJsonFile);
            if (!databaseJson.IsJson())
            {
                throw new ServerException($"Database.json does not containe valid JSON in {RepositoryPath}");
            }
            Database = databaseJson.Deserialize<Database>();
        }

        protected string PrepareOutputFolder(string directory)
        {
            var outputFolder = $"/{Organization}/{Repository}/{directory}";
            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
                Directory.CreateDirectory(outputFolder);
            }
            return outputFolder;
        }

        protected string PrepareOutputFile(string filename, string directory)
        {
            var outputFolder= $"/{Organization}/{Repository}/{directory}";
            var targetDirectory= $"/{Organization}/{Repository}/{directory}/{filename}";
            if (File.Exists(targetDirectory))
            {
                File.Delete(targetDirectory);
            }
            else if (!File.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            return outputFolder;
        }


        public static string GetNamespaceDeclaration(string @namespace)
        {
            return "namespace " + @namespace;
        }

        public static string GetUsingStatement(string @namespace)
        {
            return "using " + @namespace;
        }

        public static string MakeSafeConsideringReservedKeywords(string name)
        {
            var reservedKeywords = new List<string> { "operator", "event", "catch" }.Select(i => i.ToLower()).ToList();
            if (reservedKeywords.Contains(name.ToLower()))
            {
                return $"@{name}";
            }
            return name;
        }

        public string NtsUsingStatement(Table table)
        {
            if (table.UsesNts)
            {
                return "using NetTopologySuite.Geometries;\r\n";
            }
            return "";
        }

        protected void TrySave(string contentOfClass, string targetPath, string targetDirectory)
        {
            var retryCount = 0;
            bool isSaved = false;
            while (!isSaved)
            {
                try
                {
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }
                    File.WriteAllText(targetPath, contentOfClass);
                    isSaved = true;
                }
                catch (Exception)
                {
                    Logger.LogWarning($"Trying to save {targetPath}. Waiting for a second...");
                    Thread.Sleep(1000);
                    retryCount++;
                }
            }
        }
    }
}