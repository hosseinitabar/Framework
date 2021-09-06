using Holism.Framework;
using Holism.DataAccess;

namespace Holism.DatabaseUpdater
{
    public class ColumnHelper
    {
        public static void CreateColumns(string connectionString, Table table)
        {
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
                if not exists (select * from sys.columns where [object_id] in (select * from sys.tables where [name] = {tableName}))
                begin
                    alter table {tableName}
                    add Guid uniqueidentifier default('newid()')
                end
            ";
        }

        public static void CreateColumn(string connectionString, string tableName, Column column)
        {
            var query = @$"
                if not exists (select * from sys.columns where [object_id] in (select * from sys.tables where [name] = {tableName}))
                begin
                    alter atble {tableName}
                    add {column.Name} {GetColumnSqlType(column)} {(column.Nullable ? "null" : "not null")}
                end
                else
                begin
                end
            ";
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