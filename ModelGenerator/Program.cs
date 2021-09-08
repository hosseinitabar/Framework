using System;
using Holism.Framework;
using Holism.Generation;

namespace Holism.ModelGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Logger.LogWarning("Wrong arguments are specified for the model generator");
                return;
            }
            
            Generator.RepositoryPath = args[0];
            Generator.Organization = args[1];
            Generator.OrganizationPrefix = args[2];
            Generator.Repository = args[3];

            var helper = new Helper();
            helper.Generate();
            helper.SaveModels();
        }
    }
}
