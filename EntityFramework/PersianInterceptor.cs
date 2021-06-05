using Holism.Normalization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Holism.EntityFramework
{
    public class PersianInterceptor : DbCommandInterceptor
    {
        static DbType[] stringTypes = new DbType[]
        {
                    DbType.AnsiString,
                    DbType.AnsiStringFixedLength,
                    DbType.String,
                    DbType.StringFixedLength
        };

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            SafeEncode(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            SafeEncode(command);
            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            SafeEncode(command);
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            SafeEncode(command);
            return base.ReaderExecuting(command, eventData, result);
        }

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            SafeEncode(command);
            return base.NonQueryExecuting(command, eventData, result);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            SafeEncode(command);
            return base.ScalarExecuting(command, eventData, result);
        }

        private static void SafeEncode(DbCommand command)
        {
            command.CommandText = command.CommandText.SafePersianEncode();
            var parameters = command.Parameters.OfType<DbParameter>().ToList();
            foreach (var parameter in parameters)
            {
                if (stringTypes.Contains(parameter.DbType))
                {
                    if (parameter.Value is DBNull)
                    {

                    }
                    else
                    {
                        parameter.Value = (object)parameter.Value.ToString().SafePersianEncode();
                    }
                }
            }
        }
    }
}
