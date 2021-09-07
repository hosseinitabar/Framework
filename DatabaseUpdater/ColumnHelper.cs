using Holism.Framework;
using Holism.DataAccess;
using System.Collections.Generic;

namespace Holism.DatabaseUpdater
{
    public class ColumnHelper
    {
        public static void CreateColumns(string connectionString, Table table)
        {
            if (table.Columns == null)
            {
                if (table.IsEnum)
                {
                    table.Columns = new List<Column>();
                    table.Columns.Add(new Column
                    {
                        Name = "Key",
                        SqlType = "varchar(100)"
                    });
                    table.Columns.Add(new Column
                    {
                        Name = "Order",
                        SqlType = "int",
                        Nullable = true
                    });
                }
                else 
                {
                    throw new ClientException($"Columns are not defined for table {table.Name}");
                }
            }
            if (table.HasGuid)
            {
                CreateGuidColumn(connectionString, table.Name);
            }
            foreach (var column in table.Columns)
            {
                CreateColumn(connectionString, table.Name, column);
            }
        }

        public static void CreateGuidColumn(string connectionString, string tableName)
        {
            var query = @$"
                if not exists (select * from sys.columns where [object_id] in (select [object_id] from sys.tables where [name] = '{tableName}') and [name] = 'Guid')
                begin
                    alter table {tableName}
                    add Guid uniqueidentifier default('newid()')
                end
                else
            ";
            Holism.DataAccess.Database.Open(connectionString).Run(query);
        }

        public static void CreateColumn(string connectionString, string tableName, Column column)
        {
            var query = @$"
                if not exists (select * from sys.columns where [object_id] in (select [object_id] from sys.tables where [name] = '{tableName}') and [name] = '{column.Name}')
                begin
                    alter table [{tableName}]
                    add [{column.Name}] {GetColumnSqlType(column)} {(column.Nullable ? "null" : "not null")}
                end
                else
                begin
                    alter table [{tableName}]
                    alter column [{column.Name}] {GetColumnSqlType(column)} {(column.Nullable? "null" : "not null")}
                end
            ";
            Holism.DataAccess.Database.Open(connectionString).Run(query);
        }

        public static string GetColumnSqlType(Column column)
        {
            if (column.SqlType.IsSomething())
            {
                return column.SqlType;
            }
            if (column.Name.EndsWith("Guid"))
            {
                return "uniqueidentifier";
            }
            if (column.Name.EndsWith("Id"))
            {
                return "bigint";
            }
            if (column.Name.Contains("Date"))
            {
                return "datetime";
            }
            return "nvarchar(400)";
        }
    }
}