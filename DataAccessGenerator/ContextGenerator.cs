using System;
using Holism.Generation;
using System.IO;
using System.Text.RegularExpressions;

namespace Holism.DataAccessGenerator
{
    public class ContextGenerator: ProviderGenerator 
    {
        public ContextGenerator()
        {
            SaveModels();            
        }  

        public void SaveModels()
        {
            var filename = $"{Repository}Context.cs";
            var modelsFolder = PrepareOutputFile(filename, "DataAccess");
            var targetPath = Path.Combine(modelsFolder, filename);
            var targetDirectory = Path.GetDirectoryName(targetPath);
            var classContent = ProperClass();
            TrySave(classContent, targetPath, targetDirectory);
        }
        
        private string ProperClass()
        {

            string @class = $@"public class {Repository}Context : DatabaseContext 
    {{
        public override string ConnectionStringName => ""{Repository}"";   

         {ProperProperty()} 
        
        protected override void OnModelCreating(ModelBuilder builder)
        {{
            base.OnModelCreating(builder);
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
        public DbSet<{item.SingularName}> {item.SingularName}s {{ get; set; }} 
    ";
            }

            return ListProperty;

        }
   
    }
}