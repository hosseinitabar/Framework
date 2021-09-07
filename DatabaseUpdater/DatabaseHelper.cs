using Holism.Framework;
using System.Text.RegularExpressions;
using System;

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
            try
            {
                 CreateDatabase(database.Name);
                 TableHelper.CreateTables(database);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static string GetMasterConnectionString(string databaseName)
        {
            var connectionString = Config.GetConnectionString(databaseName);
            connectionString = Regex.Replace(connectionString, @"(?<=initial\s*catalog\s*=)[^;]*", "master");
            return connectionString;
        }

        private static void CreateDatabase(string databaseName)
        {
            var query = @$"
            if not exists (select * from sys.databases where name='{databaseName}')
            begin
                create database [{databaseName}]
            end
            ";
            Holism.DataAccess.Database.Open(GetMasterConnectionString(databaseName)).Run(query);
        }
    }
}