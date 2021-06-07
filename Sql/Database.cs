using Holism.Framework.Extensions;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using Holism.Normalization;

namespace Holism.Sql
{
    public class Database
    {
        string connectionString;

        static List<Func<string, string>> QueryProcessors = new List<Func<string, string>>();

        private Database(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public static Database Open(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "context connection=true;";
            }
            return new Database(connectionString);
        }

        public void Run(string sql, int? timeoutInSeconds = null)
        {
            sql = ApplyQueryProcessors(sql);
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = sql;
                if (timeoutInSeconds.HasValue)
                {
                    command.CommandTimeout = timeoutInSeconds.Value;
                }
                command.ExecuteNonQuery();
            }
        }

        public DataTable Get(string sql)
        {
            sql = ApplyQueryProcessors(sql);
            var table = Get(sql, null);
            return table;
        }

        public DataTable Get(string sql, int? timeoutInSeconds)
        {
            sql = ApplyQueryProcessors(sql);
            var dataTable = new DataTable();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = sql;
                if (timeoutInSeconds.HasValue)
                {
                    command.CommandTimeout = timeoutInSeconds.Value;
                }
                var dataReader = command.ExecuteReader();
                dataTable.Load(dataReader);
            }
            return dataTable;
        }

        public List<dynamic> GetList(string sql)
        {
            sql = ApplyQueryProcessors(sql);
            var table = Get(sql);
            var list = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                dynamic item = new ExpandoObject();
                foreach (DataColumn column in table.Columns)
                {
                    ExpandoObjectExtensions.AddProperty(item, column.Caption, row[column].ToString());
                }
                list.Add(item);
            }
            return list;
        }

        private ApplyQueryProcessors(string sql)
        {
            foreach (var queryProcessor in QueryProcessors)
            {
                sql = queryProcessor(sql);
            }
        }
    }
}
