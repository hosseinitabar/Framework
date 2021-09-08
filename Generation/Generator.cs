using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Holism.Framework;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace Holism.Generation
{
    /*
             * get the database name as parameter
             * get the output directory as parameter
             * Create models from tables
             * Create view models from views, and inherit from tables (only specify extrac columns as members of the derived enttiy)
             * write files to the output directory
             * create a bat file of this program with appropriat parameters to be used in other project services
             */

    /*
    Other things this generator, or other generators, or even technical debt detectors can do:

    1: Ensure that all views are inherited from their base table
    2: Create DataContext and ViewContext classes. In DataContext class, all tables should exist, with plural name for properties (name of the table). This is a little tricky. I might need an external library or EF's pluralization service to get this.
    3: Create mappings for all entities. I don't like the idea of separate maps. I prefer to map entities to their tables and schemas in simple way in OnModelCreation method, and then via reflection get all extra maps that could possibly exist and inherit from EntityTypeConfiguration class, or even all classes that exist in a folder during code generation.
    4: Creating manager classes for all entities
    5: Creating view manager classes for all views
    6: Creating simple services
    */
    public abstract class Generator
    {
        protected static string ConnectionString { get; set; }

        public static string OrganizationPrefix { get; set; }

        public static string Organization { get; set; }

        public static string Repository { get; set; }

        public static string RepositoryPath { get; set; }

        public Generator()
        {
            ConnectionString = Config.GetConnectionString(Repository);
            FetchTables();
            FetchColumns();
            FetchUniqueKeys();
        }

        private void PopulateUniqueKeys(Table table)
        {
            var uniqueKeysQuery = $@"
                        select object_schema_name(i.[object_id]) as [Schema],
	                        object_name(i.[object_id]) as [Table],
	                        i.name as [Index],
	                        stuff((
		                        select '_And_' + c.name
		                        from sys.index_columns ic
		                        inner join sys.columns c
		                        on ic.[object_id] = c.[object_id]
		                        and ic.column_id = c.column_id
		                        where ic.index_id = i.index_id
		                        and ic.[object_id] = i.[object_id]
		                        order by c.name
		                        for xml path('')
	                        ), 1, 5, '') as Columns
                        from sys.indexes i 
                        where is_unique = 1
                        and [type] = 2
                        and object_name(i.[object_id]) not in ('sysdiagrams')
                        and i.[object_id] in 
                        (
	                        select [object_id]
	                        from sys.tables 
	                        where [type] = 'U'
                        )
                        and i.name like N'%Unique%'
                        and i.[object_id] = {table.ObjectId}
                        order by [Schema], [Table], [Index]
                        ";
            var uniqueKeys = Holism.DataAccess.Database
                .Open(ConnectionString)
                .Get(uniqueKeysQuery);
            if (uniqueKeys.Rows.Count == 0)
            {
                table.UniqueKeys = new List<UniqueKey>();
                return;
            }
            table.UniqueKeys = new List<UniqueKey>();
            for (int i = 0; i < uniqueKeys.Rows.Count; i++)
            {
                var uniqueKey = new UniqueKey();
                uniqueKey.Name = uniqueKeys.Rows[i]["index"].ToString();
                uniqueKey.Columns = GetUniqueKeyColumns(table, uniqueKeys.Rows[i]["columns"].ToString());
                uniqueKey.Table = table;
                table.UniqueKeys.Add(uniqueKey);
            }
        }

        private List<Column> GetUniqueKeyColumns(Table table, string delimitedColumns)
        {
            var columns = new List<Column>();
            var columnNames = delimitedColumns.Split(new string[] { "_And_" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var columnName in columnNames)
            {
                columns.Add(table.Columns.Single(i => i.Name == columnName));
            }
            return columns;
        }

        private void PopulateColumns(Table table)
        {
            var columnsQuery = $@"
                    select (select name from sys.types where system_type_id = sys.columns.[system_type_id] and user_type_id = sys.columns.user_type_id) as [type],
                           (select [definition] from sys.computed_columns where [object_id] = sys.columns.[object_id] and column_id = sys.columns.column_id) as [definition], 
                           (select [definition] from sys.default_constraints where [parent_object_id] = sys.columns.[object_id] and parent_column_id = sys.columns.column_id) as [DefaultDefinition],
                            *
                    from sys.columns
                    where [object_id] = {table.ObjectId}
                        ";
            table.Columns = new List<Column>();
            var definitionsTable = Holism.DataAccess.Database
                .Open(ConnectionString)
                .Get(columnsQuery);
            for (int i = 0; i < definitionsTable.Rows.Count; i++)
            {
                var column = new Column();
                column.Name = definitionsTable.Rows[i]["name"].ToString();
                column.SqlType = definitionsTable.Rows[i]["type"].ToString();
                column.ClrType = MapSqlTypeToClrType(definitionsTable.Rows[i]["type"].ToString());
                column.IsNullable = definitionsTable.Rows[i]["is_nullable"].ToBoolean();
                column.IsComputed = definitionsTable.Rows[i]["is_computed"].ToBoolean();
                column.IsIdentity = definitionsTable.Rows[i]["is_identity"].ToBoolean();
                column.MaxLength = definitionsTable.Rows[i]["max_length"].ToString().ToInt();
                column.Precision = (byte?)definitionsTable.Rows[i]["precision"];
                column.Scale = (byte?)definitionsTable.Rows[i]["scale"];
                if (column.IsComputed)
                {
                    column.ComputedColumnFormula = definitionsTable.Rows[i]["definition"].ToString();
                }
                column.HasDefault = definitionsTable.Rows[i]["DefaultDefinition"] != DBNull.Value;
                if (column.HasDefault)
                {
                    column.DefaultSqlText = definitionsTable.Rows[i]["DefaultDefinition"].ToString();
                }
                table.Columns.Add(column);
            }
            var nonNullableColumns = new List<string> { "string", "byte[]", "Geometry" };
            foreach (var column in table.Columns)
            {
                if (column.IsNullable && !nonNullableColumns.Contains(column.ClrType))
                {
                    column.ClrType += "?";
                }
                if (column.SqlType == "nvarchar" && column.MaxLength != -1)
                {
                    column.MaxLength = column.MaxLength / 2;
                }
            }
        }

        private string MapSqlTypeToClrType(string sqlType)
        {
            switch (sqlType)
            {
                case "varchar":
                case "nvarchar":
                case "nchar":
                case "char":
                case "text":
                case "ntext":
                    return "string";
                case "decimal":
                case "numeric":
                case "float":
                    return "decimal";
                case "bigint":
                    return "long";
                case "int":
                    return "int";
                case "smallint":
                    return "Int16";
                case "tinyint":
                    return "byte";
                case "bit":
                    return "bool";
                case "varbinary":
                    return "byte[]";
                case "datetime2":
                case "datetime":
                case "date":
                    return "DateTime";
                case "time":
                    return "TimeSpan";
                case "uniqueidentifier":
                    return "Guid";
                case "timestamp":
                    return "byte[]";
                case "geography":
                    return "Geography";
                case "geometry":
                    return "Geometry";
                case "datetimeoffset":
                    return "DateTimeOffset";
                default:
                    throw new ServerException($"SQL data type '{sqlType}' is not mapped to .NET data type");
            }
        }

        private void FetchTables()
        {
            var query = @"
                        select name, [object_id], schema_name([schema_id]) as [schema], 0 as IsView
                        from sys.tables 
                        where name not in ('sysdiagrams')
						
						union
						
                        select name, [object_id], schema_name([schema_id]) as [schema], 1 as IsView
                        from sys.views 
                        where name not in ('sysdiagrams');
                        ";
            var tables = new List<Table>();
            var definitionsTable = Holism.DataAccess.Database
                .Open(ConnectionString)
                .Get(query);

            for (int i = 0; i < definitionsTable.Rows.Count; i++)
            {
                var table = new Table();
                table.Name = definitionsTable.Rows[i]["name"].ToString();
                table.ObjectId = definitionsTable.Rows[i]["object_id"].ToString().ToInt();
                table.IsView = Convert.ToBoolean(definitionsTable.Rows[i]["IsView"]);
                tables.Add(table);
            }
            foreach (var table in Tables)
            {
                // if (CodeGeneratorConfig.TablesToSkipTotalCount.Contains(table.Name.ToLower()) || CodeGeneratorConfig.TablesToSkipTotalCount.Contains(table.FullyQualifiedName.ToLower()) || CodeGeneratorConfig.TablesToSkipTotalCount.Contains(table.FullyQualifiedName.Replace("[", "").Replace("]", "").ToLower()))
                // {
                //     table.SkipTotalCount = true;
                // }
            }
        }

        private void FetchColumns()
        {
            foreach (var table in Tables)
            {
                PopulateColumns(table);
            }
        }

        private void FetchUniqueKeys()
        {
            foreach (var table in Tables)
            {
                PopulateUniqueKeys(table);
            }
        }

        public Database Database { get; private set; }

        public List<Table> Tables { get; set; }

        public abstract List<Table> Generate();

        protected string PrepareOutputFolder(string path)
        {
            var outputFolder = Config.ExpandEnvironmentVariables(path);
            if (Directory.Exists(outputFolder))
            {
                Directory.Delete(outputFolder, true);
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

        protected void TrySave(Table model, string targetPath, string targetDirectory)
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
                    File.WriteAllText(targetPath, model.GeneratedCode);
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