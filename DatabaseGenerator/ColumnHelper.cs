using Holism.Framework;
using Holism.DataAccess;
using Holism.Generation;
using System.Collections.Generic;

namespace Holism.DatabaseUpdater
{
    public class ColumnHelper
    {
        public static void CreateColumns(string connectionString, Table table)
        {
            if (table.Columns == null)
            {
                if (!table.IsEnum)
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
                    add [{column.Name}] {GetColumnSqlType(column)} {(column.IsNullable ? "null" : "not null")}
                end
                else
                begin
                    alter table [{tableName}]
                    alter column [{column.Name}] {GetColumnSqlType(column)} {(column.IsNullable? "null" : "not null")}
                end
            ";
            Holism.DataAccess.Database.Open(connectionString).Run(query);
        }
    }
}