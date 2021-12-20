using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteHistoricalLocationsRepository : IHistoricalLocationsRepository
    { 
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection { get { return _transaction.Connection; } }

        public SqliteHistoricalLocationsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public static void CheckForTables(IDbConnection connection, IDbTransaction transaction)
        {
            if (!SqliteUtil.CheckForTable(connection, "historical_locations"))
            {
                CreateTable(connection, transaction);
            }

            if (!SqliteUtil.CheckForTable(connection, "historical_locations_locations"))
            {
                CreateAssociationTable(connection, transaction);
            }
        }

        public static async Task DropAssociationTables(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = @"DROP TABLE IF EXISTS historical_locations_locations";
            await connection.ExecuteScalarAsync(sql, transaction);
        }

        public static async Task DropTables(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = @"DROP TABLE IF EXISTS historical_locations;";
            await connection.ExecuteScalarAsync(sql, transaction);
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

        public async Task<IEnumerable<Location>> GetLinkedLocations(long historicalLocationId)
        {
            var sql =
                @"SELECT
                    locations.ID as ID,
                    locations.Name as Name,
                    locations.Description as Description,
                    locations.EncodingType as EncodingType,
                    locations.Location as FeatureLocation
                FROM historical_locations
                INNER JOIN historical_locations_locations on (historical_locations.id = historical_locations_locations.historical_location_id)
                INNER JOIN locations on (locations.id = historical_locations_locations.historical_location_id)
                WHERE historical_locations.id = @historicalLocationId;";
            return await Connection.QueryAsync<Location>(sql, new { historicalLocationId }, _transaction);
        }

        public async Task LinkLocationAsync(long historicalLocationId, long locationId)
        {
            var sql =
                @"INSERT INTO historical_locations_locations(historical_location_id, location_id)
                    VALUES(@historicalLocationId, @locationId);";
            await Connection.ExecuteAsync(sql, new { historicalLocationId, locationId }, _transaction);
        }

        public async Task UnlinkLocationAsync(long historicalLocationId, long locationId)
        {
            var sql =
                @"DELETE FROM historical_locations_locations
                    WHERE historical_location_id = @historicalLocationId AND location_id = @locationId";
            await Connection.ExecuteAsync(sql, new { historicalLocationId, locationId }, _transaction);
        }

        public async Task UnlinkLocationsAsync(long historicalLocationId)
        {
            var sql =
                @"DELETE FROM historical_locations_locations
                    WHERE historical_location_id = @historicalLocationId";
            await Connection.ExecuteAsync(sql, new { historicalLocationId }, _transaction);
        }

        public async Task UpdateAsync(HistoricalLocation item)
        {
            var sql =
                @"UPDATE historical_locations
                    SET Time = @Time
                WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        private static void CreateTable(IDbConnection connection, IDbTransaction transaction)
        {
            var sql =
                @"CREATE TABLE historical_locations (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Time TEXT);";
            connection.ExecuteScalar(sql, transaction);
        }

        private static void CreateAssociationTable(IDbConnection connection, IDbTransaction transaction)
        {
            var sql =
                @"CREATE TABLE historical_locations_locations(
                    historical_location_id int NOT NULL,
                    location_id int NOT NULL,
                    FOREIGN KEY(historical_location_id) REFERENCES historical_locations(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(location_id) REFERENCES locations(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(historical_location_id, location_id)
                );";
            connection.ExecuteScalar(sql, transaction);
        }
    }
}
