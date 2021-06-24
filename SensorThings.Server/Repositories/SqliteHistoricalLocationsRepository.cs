using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteHistoricalLocationsRepository : IRepository<HistoricalLocation>
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection { get { return _transaction.Connection; } }

        public SqliteHistoricalLocationsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public static void CheckForTables(IDbConnection connection)
        {
            if (!SqliteUtil.CheckForTable(connection, "historical_locations"))
            {
                CreateTable(connection);
            }
        }

        public async Task<long> AddAsync(HistoricalLocation item)
        {
            var sql =
                @"INSERT INTO historical_locations
                    (Time)
                    VALUES (@Time);
                SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<HistoricalLocation>> GetAllAsync()
        {
            var sql =
                @"SELECT
                    ID, Time
                FROM historical_locations";
            var locations = await Connection.QueryAsync<HistoricalLocation>(sql, _transaction);

            return locations;
        }

        public async Task<HistoricalLocation> GetByIdAsync(long id)
        {
            var sql =
                @"SELECT
                    ID, Time
                FROM historical_locations
                WHERE id = @ID";
            var location = await Connection.QueryFirstAsync<HistoricalLocation>(sql, new { ID = id }, _transaction);

            return location;
        }

        public async Task Remove(long id)
        {
            var sql =
                @"DELETE FROM historical_locations WHERE id = @ID";
            await Connection.ExecuteAsync(sql, new { ID = id }, _transaction);
        }

        public async Task UpdateAsync(HistoricalLocation item)
        {
            var sql =
                @"UPDATE historical_locations
                    SET Time = @Time
                WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        private static void CreateTable(IDbConnection connection)
        {
            var sql =
                @"CREATE TABLE historical_locations (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Time TEXT);";
            connection.Execute(sql);
        }
    }
}
