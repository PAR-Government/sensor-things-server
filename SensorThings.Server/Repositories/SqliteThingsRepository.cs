using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SensorThings.Entities;

namespace SensorThings.Server.Repositories
{
    public class SqliteThingsRepository : IThingsRepository
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection { get { return _transaction.Connection; } }

        public SqliteThingsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public static void CheckForTables(IDbConnection connection)
        {
            if (!SqliteUtil.CheckForTable(connection, "things"))
            {
                CreateTable(connection);
            }

            if (!SqliteUtil.CheckForTable(connection, "things_locations"))
            {
                CreateThingLocationTable(connection);
            }

            if (!SqliteUtil.CheckForTable(connection, "things_historical_locations"))
            {
                CreateThingHistoricalLocationTable(connection);
            }

            if (!SqliteUtil.CheckForTable(connection, "things_datastreams"))
            {
                CreateThingDatastreamsTable(connection);
            }
        }

        public async Task<long> AddAsync(Thing item)
        {
            var sql = @"INSERT INTO things (Name, Description) 
                        Values(@Name, @Description); 
                        SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<Thing>> GetAllAsync()
        {
            var sql = @"SELECT ID, Name, Description FROM things;";
            var things = await Connection.QueryAsync<Thing>(sql, _transaction);

            return things;
        }

        public async Task<Thing> GetByIdAsync(long id)
        {
            var sql = @"SELECT ID, Name, Description FROM things WHERE id=@ID;";
            var thing = await Connection.QueryFirstAsync<Thing>(sql, new { ID = id }, _transaction);

            return thing;
        }

        public async Task Remove(long id)
        {
            var sql = @"DELETE FROM things WHERE id = @Id";
            await Connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        }

        public async Task UpdateAsync(Thing item)
        {
            var sql = @"UPDATE things
                        SET Name = @Name,
                            Description = @Description
                        WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        public async Task AddLocationLinkAsync(long thingId, long locationId)
        {
            var sql = @"INSERT INTO things_locations(thing_id, location_id) VALUES(@ThingId, @LocationId);";
            await Connection.ExecuteAsync(sql, new { ThingId = thingId, LocationId = locationId }, _transaction);
        }

        public async Task RemoveLocationLinkAsync(long thingId, long locationId)
        {
            var sql = @"DELETE FROM things_locations 
                        WHERE thing_id = @thingId AND location_id = @locationId";
            await Connection.ExecuteAsync(sql, new { thingId, locationId }, _transaction);
        }

        public async Task RemoveLocationLinksAsync(long thingId)
        {
            var sql = @"DELETE FROM things_locations WHERE thing_id = @thingId";
            await Connection.ExecuteAsync(sql, new { thingId }, _transaction);
        }

        public async Task<IEnumerable<Location>> GetLinkedLocations(long thingId)
        {
            var sql = @"SELECT 
                            locations.ID as ID, 
                            locations.Name as Name, 
                            locations.Description as Description, 
                            locations.EncodingType as EncodingType, 
                            locations.Location as FeatureLocation 
                        FROM things
                        INNER JOIN things_locations on (things.id = things_locations.thing_id)
                        INNER JOIN locations on (locations.id = things_locations.location_id)
                        WHERE things.id = @thingId;";
            return await Connection.QueryAsync<Location>(sql, new { thingId }, _transaction);
        }

        public async Task AddHistoricalLocationLinkAsync(long thingId, long historicalLocationId)
        {
            var sql = @"INSERT INTO things_historical_locations(thing_id, historical_location_id) 
                        VALUES(@thingId, @historicalLocationId);";
            await Connection.ExecuteAsync(sql, new { thingId, historicalLocationId }, _transaction);
        }

        public async Task<IEnumerable<HistoricalLocation>> GetLinkedHistoricalLocationsAsync(long thingId)
        {
            var sql = @"SELECT 
                            historical_locations.ID as ID, 
                            historical_locations.Time as Time 
                        FROM things
                        INNER JOIN things_historical_locations on (things.id = things_historical_locations.thing_id)
                        INNER JOIN historical_locations on (historical_locations.id = things_historical_locations.historical_location_id)
                        WHERE things.id = @thingId;";
            return await Connection.QueryAsync<HistoricalLocation>(sql, new { thingId }, _transaction);
        }

        public async Task RemoveHistoricalLocationLinkAsync(long thingId, long historicalLocationId)
        {
            var sql = 
                @"DELETE FROM things_historical_locations 
                    WHERE thing_id = @thingId AND historical_location_id = @historicalLocationId";
            await Connection.ExecuteAsync(sql, new { thingId, historicalLocationId }, _transaction);
        }

        public async Task RemoveHistoricalLocationLinksAsync(long thingId)
        {
            var sql = @"DELETE FROM things_locations WHERE thing_id = @thingId";
            await Connection.ExecuteAsync(sql, new { thingId }, _transaction);
        }

        public async Task<IEnumerable<Datastream>> GetLinkedDatastreamsAsync(long thingId)
        {
            var sql =
                @"SELECT 
                        datastreams.ID as ID, 
                        datastreams.Name as Name,
                        datastreams.Description as Description,
                        datastreams.ObservationType as ObservationType,
                        datastreams.UnitOfMeasurement as UnitOfMeasurement,
                        datastreams.ObservedArea as ObservedArea,
                        datastreams.PhenomenonTime as PhenomenonTime,
                        datastreams.ResultTime as ResultTime
                    FROM things
                    INNER JOIN things_datstreams on (things.id = things_datastreams.thing_id)
                    INNER JOIN datastreams on (datastreams.id = things_datadatstreams.datastream_id)
                    WHERE things.id = @thingId;";
            return await Connection.QueryAsync<Datastream>(sql, new { thingId }, _transaction);
        }

        public async Task AddDatastreamLinkAsync(long thingId, long datastreamId)
        {
            var sql = @"INSERT INTO things_datastreams(thing_id, datastream_id) 
                        VALUES(@thingId, @datastreamId);";
            await Connection.ExecuteAsync(sql, new { thingId, datastreamId }, _transaction);
        }

        public async Task RemoveDatastreamLinkAsync(long thingId, long datastreamId)
        {
            var sql = @"DELETE FROM things_datastreams 
                        WHERE thing_id = @thingId and datastream_id = @datastreamId";
            await Connection.ExecuteAsync(sql, new { thingId, datastreamId }, _transaction);
        }

        public async Task RemoveDatastreamLinksAsync(long thingId)
        {
            var sql = @"DELETE FROM things_datastreams WHERE thing_id = @thingId";
            await Connection.ExecuteAsync(sql, new { thingId }, _transaction);
        }

        private static void CreateTable(IDbConnection connection)
        {
            var sql =
                @"Create Table things (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT NULL);";
            connection.Execute(sql);
        }

        private static void CreateThingLocationTable(IDbConnection connection)
        {
            var sql =
                @"CREATE Table things_locations(
                    thing_id int NOT NULL,
                    location_id int NOT NULL,
                    FOREIGN KEY(thing_id) REFERENCES things(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(location_id) REFERENCES locations(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(thing_id, location_id)
                );";

            connection.Execute(sql);
        }

        private static void CreateThingHistoricalLocationTable(IDbConnection connection)
        {
            var sql =
                @"CREATE Table things_historical_locations (
                    thing_id int NOT NULL,
                    historical_location_id int NOT NULL,
                    FOREIGN KEY(thing_id) REFERENCES things(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(historical_location_id) REFERENCES historical_locations(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(thing_id, historical_location_id)
                );";
            connection.Execute(sql);
        }

        private static void CreateThingDatastreamsTable(IDbConnection connection)
        {
            var sql =
                @"CREATE TABLE things_datastreams (
                    thing_id int NOT NULL,
                    datastream_id int NOT NULL,
                    FOREIGN KEY(thing_id) REFERENCES things(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(datastream_id) REFERENCES datastreams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(thing_id, datastream_id)
                );";
            connection.Execute(sql);
        }
    }
}
