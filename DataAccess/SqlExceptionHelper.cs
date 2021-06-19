﻿using Holism.Framework;
using Holism.Framework.Extensions;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace Holism.DataAccess
{
    public class SqlExceptionHelper
    {
        public static void HandleSqlException(Exception exception, string typeName)
        {
            SqlException sqlException = exception as SqlException;
            var ex = exception;
            while (ex != null)
            {
                if (sqlException is SqlException)
                {
                    break;
                }
                sqlException = ex.InnerException as SqlException;
                ex = ex.InnerException;
            }
            if (sqlException == null)
            {
                throw exception;
            }
            var message = GetMessage(sqlException, typeName);
            throw new FrameworkException(message, sqlException);
        }

        private static string GetMessage(SqlException ex, string typeName)
        {
            switch (ex.Number)
            {
                case 2601: // Cannot insert duplicate key row in object '%.*ls' with unique index '%.*ls'. The duplicate key value is %ls.
                case 2627: // Violation of %ls constraint '%.*ls'. Cannot insert duplicate key in object '%.*ls'. The duplicate key value is %ls.
                    return $"{typeName} is duplicate";
                case 547:
                    if (ex.Message.Contains("FOREIGN KEY"))
                    {
                        var match = new Regex(@"(?<=foreign key constraint ""FK_)(.*)_Id_(.*)_Id", RegexOptions.IgnoreCase).Match(ex.Message);
                        var from = match.Groups[1].Value;
                        var to = match.Groups[2].Value;
                        return $"relation error: from {from} to {to}";
                    }
                    return "db error";
                default:
                    return "db error";
            }
        }
    }
}
