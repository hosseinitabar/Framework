using System;
using Holism.Generation;
using System.IO;
using System.Text.RegularExpressions;

namespace Holism.DataAccessGenerator
{
    public class RepositoryGenerator: ProviderGenerator 
    {
        public RepositoryGenerator()
        {
            SaveModels();            
        }  

        public void SaveModels()
        {
            var filename = $"Repository.cs";
            var modelsFolder = PrepareOutputFile(filename, "DataAccess");
            var targetPath = Path.Combine(modelsFolder, filename);
            var targetDirectory = Path.GetDirectoryName(targetPath);
            var classContent = ProperClass();
            TrySave(classContent, targetPath, targetDirectory);
        }
        
        private string ProperClass()
        {

            string @class = $@"using {OrganizationPrefix}.{Repository}.Models;
using Holism.DataAccess;

namespace {OrganizationPrefix}.{Repository}.DataAccess
{{
    public class Repository
    {{
        {ProperProperty()}
    }}
}}";

            @class = Regex.Replace(@class, @"(\n){2}\n", "$1");
            return @class;
        
        }

        private string ProperProperty(){
            var ListProperty ="";
            foreach (var item in Database.Tables)
            {
                
                ListProperty += $@"
        public static Repository<{item.SingularName}> {item.SingularName}
        {{
            get
            {{
                return new Holism.DataAccess.Repository<{item.SingularName}>(new {Repository}sContext());
            }}
        }}
    "+ "\n" + "\n";;
            }

            return ListProperty;

        }
   
    }
}