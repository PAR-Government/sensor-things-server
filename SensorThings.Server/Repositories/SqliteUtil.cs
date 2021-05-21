using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SensorThings.Server.Repositories
{
    public static class SqliteUtil
    {
        public static bool CheckForTable(IDbConnection conn, string name)
        {
            var sql = @"SELECT name FROM sqlite_master WHERE type='table' AND name = @TableName;";
            var tableName = conn.QueryFirstOrDefault<string>(sql, new { TableName = name });

            return !string.IsNullOrEmpty(tableName) && tableName == name;
        }
    }
}
