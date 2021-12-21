using Dapper;
using System.Data;
using System.Text;
using System.Threading.Tasks;

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

        public static async Task ExportIntoFile(IDbConnection connection, string file)
        {
            var sql = @"VACUUM INTO @file;";
            await connection.ExecuteScalarAsync(sql, new { file = file });
        }
    }
}
