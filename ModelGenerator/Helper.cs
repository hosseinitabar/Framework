using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Holism.Framework;
using Holism.Generation;

namespace Holism.ModelGenerator
{
    public class Helper : Generator
    {
        public override List<Table> Generate()
        {
            foreach (var table in Tables)
            {
                if (table.IsEnum)
                {
                    table.GeneratedCode = GenerateEnumForTable(table);
                }
                else
                {
                    table.GeneratedCode = GenerateClassForTable(table);
                }
            }
            return Tables;
        }

        private string GenerateEnumForTable(Table table)
        {
            var @enum = @$"this is enum for {table.Name}";
            return @enum;
        }

        private string GenerateClassForTable(Table table)
        {
            var properties = "";
            foreach (var column in table.Columns)
            {
                properties += GeneratePropertyForColumn(column) + "\n" + "\n";
            }
            properties += "        public dynamic RelatedItems { get; set; }" + "\n" + "\n";
            var @namespace = Namespace;
            @namespace += table.IsView ? ".Views" : "";
            string interfaces = "";
            if (Interfaces.Count > 0)
            {
                interfaces = ", " + Interfaces.ToCsv();
            }
            var usings = $"{NtsUsingStatement(table)}{UsingStatements}";
            if (table.Columns.Any(i => i.DotNetType.StartsWith("Guid")) || table.Columns.Any(i => i.DotNetType.StartsWith("DateTime")))
            {
                usings += "\nusing System;";
            }
            usings = usings.Trim();
            if (usings.IsSomething())
            {
                usings += "\n\n";
            }
            var entityInterfaceInheritance = "Holism.Models.";
            var isGuidEntity = table.HasGuid || table.Columns.Any(i => i.Name == "Guid");
            if (isGuidEntity)
            {
                entityInterfaceInheritance += "IGuidEntity";
            }
            else
            {
                entityInterfaceInheritance += "IEntity";
            }
            string @class = $@"{usings}{GetNamespaceDeclaration(@namespace)}
{{
    public class {table.SingularName} : {entityInterfaceInheritance}{interfaces}
    {{
        public {table.SingularName}()
        {{
            RelatedItems = new System.Dynamic.ExpandoObject();
        }}

{properties}
    }}
}}
";
            @class = Regex.Replace(@class, @"(\n){2}\n", "$1");
            return @class;
        }

        private string GeneratePropertyForColumn(Column column)
        {
            string property;
            property = $@"        public {column.DotNetType} {column.Name} {{ get; set; }}";
            return property;
        }

        public string UsingStatements
        {
            get
            {
                return "";
            }
        }

        public string Namespace 
        { 
            get 
            {
                return $"{OrganizationPrefix}.{Repository}";
            }
        }

        public List<string> Interfaces
        {
            get
            {
                return new List<string> { };
            }
        }

        public void SaveModels()
        {
            var modelsFolder = PrepareOutputFolder("Models");
            foreach (var model in Tables)
            {
                var targetPath = Path.Combine(modelsFolder, (model.IsView ? @"Views" : ""), model.SingularName + ".cs");
                var targetDirectory = Path.GetDirectoryName(targetPath);
                TrySave(model, targetPath, targetDirectory);
            }
        }

        public string OutputFolder { get; }
    }
}