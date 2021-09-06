using Holism.Framework;

namespace Holism.DatabaseUpdater
{
    public class DatabaseHelper
    {
        public static void CreateDatabase(Database database)
        {
            if (database.Name.IsNothing())
            {
                throw new ClientException($"Please provide database name");
            }
            CreateDatabase(database.Name);
        }

        private static void CreateDatabase(string name)
        {
            var query = @$"
            if not exists (select * from sys.databases where name='{name}')
            begin
                create database [{name}]
            end
            ";
            Holism.DataAccess.Database.Open(Config.GetConnectionString(name)).Run(query);
        }
    }
}