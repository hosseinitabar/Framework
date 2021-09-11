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
                properties += GeneratePropertyForColumn(column) + "\r\n" + "\r\n";
            }
            properties += "        public dynamic RelatedItems { get; set; }" + "\r\n" + "\r\n";
            var @namespace = Namespace;
            @namespace += table.IsView ? ".Views" : "";
            string interfaces = "";
            if (Interfaces.Count > 0)
            {
                interfaces = ", " + Interfaces.ToCsv();
            }
            var usings = $"{NtsUsingStatement(table)}{UsingStatements}";
            if (table.Columns.Any(i => i.ClrType.StartsWith("Guid")) || table.Columns.Any(i => i.ClrType.StartsWith("DateTime")))
            {
                usings += "\r\nusing System;";
            }
            usings = usings.Trim();
            if (usings.IsSomething())
            {
                usings += "\r\n\r\n";
            }
            var entityInterfaceInheritance = "Holism.DataAccess.";
            var isGuidEntity = table.Columns.Any(i => i.ClrType == "Guid" && i.Name == "Guid" && i.DefaultSqlText == "(newid())") || (table.IsView && table.Columns.Any(i => i.ClrType == "Guid" && i.Name == "Guid"));
            if (isGuidEntity)
            {
                entityInterfaceInheritance += "IGuidEntity";
            }
            else
            {
                var shouldBeGuidEntity = table.Columns.Any(i => i.ClrType == "Guid" && i.Name == "Guid" && i.DefaultSqlText != "(newid())");
                if (shouldBeGuidEntity)
                {
                    Logger.LogWarning($"Table {table.SqlQualifiedName} has a GUID column, called Guid, but its default value is not newid(). Thus its model can't inherit from IGuidEntity. Please fix this issue.");
                }
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
            @class = Regex.Replace(@class, @"(\r\n){2}\r\n", "$1");
            return @class;
        }

        private string GeneratePropertyForColumn(Column column)
        {
            string property;
            if (column.IsComputed)
            {
                property = $@"        public {column.ClrType} {column.Name} {{ get; private set; }}";
            }
            else
            {
                property = $@"        public {column.ClrType} {column.Name} {{ get; set; }}";
            }
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
            var modelsFolder = PrepareOutputFolder(OutputFolder);
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