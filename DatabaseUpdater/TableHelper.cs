using Holism.Framework;
using System;

namespace Holism.DatabaseUpdater
{
    public class TableHelper
    {
        public void CreateTables(Database database)
        {
            var connectionString = Config.GetConnectionString(database.Name);
            foreach(var table in database.Tables)
            {
                try
                {
                     CreateTable(connectionString, table);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public void CreateTable(string connectionString, Table table)
        {
            var query = @$"
            if not exists (select * from sys.tables where [name] = '{table.Name}')
            begin
                create table {table.Name}
                (
                    Id bigint not null primary key identity(1,1),
                    [Guid] uniqueidentifier default('newid()')
                )
            end
            ";
        }

        public void IsOneToOneRelationship(Table table)
        {
            
        }
    }
}