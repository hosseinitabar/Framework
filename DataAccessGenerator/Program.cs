using System;
using Holism.Framework;
using System.IO;
using Holism.Validation;
using Holism.Generation;

namespace Holism.DataAccessGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new string[] {
                "/HolismDotNet/Ticketing",
                "HolismDotNet",
                "Holism",
                "Ticketing"
            };
            if (args.Length != 4)
            {
                Logger.LogWarning("Wrong arguments are specified for the model generator");
                return;
            }
            
            ProviderGenerator.RepositoryPath = args[0];
            ProviderGenerator.Organization = args[1];
            ProviderGenerator.OrganizationPrefix = args[2];
            ProviderGenerator.Repository = args[3];

            var GenerateRepo = new RepositoryGenerator();
            var GenerateContext = new ContextGenerator();

        }
    }
}
