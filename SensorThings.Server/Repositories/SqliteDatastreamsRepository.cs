using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteDatastreamsRepository : IDatastreamsRepository
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection { get { return _transaction.Connection; } }

        public SqliteDatastreamsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public static void CheckForTables(IDbConnection connection, IDbTransaction transaction)
        {
            if (!SqliteUtil.CheckForTable(connection, "datastreams"))
            {
                CreateTable(connection, transaction);
            }

            if (!SqliteUtil.CheckForTable(connection, "datastreams_sensors"))
            {
                CreateDatastreamSensorsTable(connection, transaction);
            }

            if (!SqliteUtil.CheckForTable(connection, "datastreams_observedproperties"))
            {
                CreateDatastreamsObservedPropertiesTable(connection, transaction);
            }

            if (!SqliteUtil.CheckForTable(connection, "datastreams_observations"))
            {
                CreateDatastreamsObservationsTable(connection, transaction);
            }
        }

        public static async Task DropAssociationTables(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = @"DROP TABLE IF EXISTS datastreams_observations;
                        DROP TABLE IF EXISTS datastreams_observedproperties;
                        DROP TABLE IF EXISTS datastreams_sensors;";
            await connection.ExecuteScalarAsync(sql, transaction);
        }

        public static async Task DropTables(IDbConnection connection, IDbTransaction transaction)
        {
            var sql = @"DROP TABLE IF EXISTS datastreams;";
            await connection.ExecuteScalarAsync(sql, transaction);
        }

        public async Task<long> AddAsync(Datastream item)
        {
            var sql = @"INSERT INTO datastreams
                        (Name, Description, ObservationType, UnitOfMeasurement, ObservedArea, PhenomenonTime, ResultTime)
                        VALUES(@Name, @Description, @ObservationType, @UnitOfMeasurement, @ObservedArea, @PhenomenonTime, @ResultTime);
                        SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<Datastream>> GetAllAsync()
        {
            var sql = @"SELECT
                        ID, Name, Description, ObservationType, UnitOfMeasurement, ObservedArea, PhenomenonTime, ResultTime
                        FROM datastreams;";
            var datastreams = await Connection.QueryAsync<Datastream>(sql, _transaction);

            return datastreams;
        }

        public async Task<Datastream> GetByIdAsync(long id)
        {
            var sql = @"SELECT
                        ID, Name, Description, ObservationType, UnitOfMeasurement, ObservedArea, PhenomenonTime, ResultTime
                        FROM datastreams
                        WHERE id = @ID;";
            var datastream = await Connection.QueryFirstAsync<Datastream>(sql, new { ID = id }, _transaction);

            return datastream;
        }

        public async Task Remove(long id)
        {
            var sql = @"DELETE FROM datastreams WHERE id = @Id";
            await Connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        }

        public async Task UpdateAsync(Datastream item)
        {
            var sql = @"UPDATE datastreams
                        SET Name = @Name,
                            Description = @Description,
                            ObservationType = @ObservationType
                            UnitOfMeasurement = @UnitOfMeasurement,
                            ObservedArea = @ObservedArea,
                            PhenomenonTime = @PhenomenonTime,
                            ResultTime = @ResultTime
                        WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        public async Task LinkSensorAsync(long datastreamId, long sensorId)
        {
            var sql =
                @"INSERT INTO datastreams_sensors(datastream_id, sensor_id)
                    VALUES(@datastreamId, @sensorId);";
            await Connection.ExecuteAsync(sql, new { datastreamId, sensorId }, _transaction);
        }

        public async Task<Sensor> GetLinkedSensorAsync(long datastreamId)
        {
            // Datastreams have a single sensor but a sensor may be associated with multiple datastreams
            var sql =
                @"SELECT
                    sensors.id as ID,
                    sensors.Name as Name,
                    sensors.Description as Description,
                    sensors.EncodingType as EncodingType,
                    sensors.Metadata as Metadata
                FROM datastreams
                INNER JOIN datastreams_sensors on (datastreams.id = datastreams_sensors.datastream_id)
                INNER JOIN sensors on (sensors.id = datastreams_sensors.sensor_id)
                WHERE datastreams.id = @datastreamId;";
            var sensor = await Connection.QueryFirstAsync<Sensor>(sql, new { datastreamId }, _transaction);

            return sensor;
        }

        public async Task<IEnumerable<Datastream>> GetLinkedDatastreamsForSensorAsync(long sensorId)
        {
            var sql =
                @"SELECT
                    datastreams.id as ID,
                    datastreams.Name as Name,
                    datastreams.Description as Description,
                    datastreams.ObservationType as ObservationType,
                    datastreams.UnitOfMeasurement as UnitOfMeasurement,
                    datastreams.ObservedArea as ObservedArea,
                    datastreams.PhenomenonTime as PhenomenonTime,
                    datastreams.ResultTime as ResultTime
                FROM sensors
                INNER JOIN datastream_sensors on (sensors.id = datastreams_sensors.sensor_id)
                INNER JOIN datastreams on (datastreams.id = datastreams_sensors.datastream_id)
                WHERE sensors.id = @sensorId;";
            var datastreams = await Connection.QueryAsync<Datastream>(sql, new { sensorId }, _transaction);

            return datastreams;
        }

        public async Task UnlinkSensorAsync(long datastreamId, long sensorId)
        {
            var sql =
                @"DELETE FROM datastreams_sensors
                    WHERE datastream_id = @datastreamId AND sensor_id = @sensorId";
            await Connection.ExecuteAsync(sql, new { datastreamId, sensorId }, _transaction);
        }

        public async Task LinkObservedPropertyAsync(long datastreamId, long propertyId)
        {
            var sql =
                @"INSERT INTO datastreams_observedproperties(datastream_id, observed_property_id)
                    VALUES(@datastreamId, @propertyId)";
            await Connection.ExecuteAsync(sql, new { datastreamId, propertyId }, _transaction);
        }

        public async Task<ObservedProperty> GetLinkedObservedPropertyAsync(long datastreamId)
        {
            // Datastreams have a single ObservedProperty but an ObservedProperty may be associated with multiple datastreams
            var sql =
                @"SELECT
                    observed_properties.id as ID,
                    observed_properties.Name as Name,
                    observed_properties.Definition as Definition,
                    observed_properties.Description as Description
                FROM datastreams
                INNER JOIN datastreams_observedproperties on (datastreams.id = datastreams_observedproperties.datastream_id)
                INNER JOIN observed_properties on (observed_properties.id = datastreams_observedproperties.observed_property_id)
                WHERE datastreams.id = @datastreamId;";
            var property = await Connection.QueryFirstAsync<ObservedProperty>(sql, new { datastreamId }, _transaction);

            return property;
        }

        public async Task<IEnumerable<Datastream>> GetLinkedDatastreamsForObservedPropertyAsync(long propertyId)
        {
            var sql =
                @"SELECT
                    datastreams.id as ID,
                    datastreams.Name as Name,
                    datastreams.Description as Description,
                    datastreams.ObservationType as ObservationType,
                    datastreams.UnitOfMeasurement as UnitOfMeasurement,
                    datastreams.ObservedArea as ObservedArea,
                    datastreams.PhenomenonTime as PhenomenonTime,
                    datastreams.ResultTime as ResultTime
                FROM observed_properties
                INNER JOIN datastreams_observedproperties on (observed_properties.id = datastreams_observedproperties.observed_property_id)
                INNER JOIN datastreams on (datastreams.id = datastreams_observedproperties.datastream_id)
                WHERE observed_properties.id = @propertyId;";
            var datastreams = await Connection.QueryAsync<Datastream>(sql, new { propertyId }, _transaction);

            return datastreams;
        }

        public async Task UnlinkObservedPropertyAsync(long datastreamId, long propertyId)
        {
            var sql =
                @"DELETE FROM datastreams_observedproperties
                    WHERE datastream_id = @datastreamId AND observed_property_id = @propertyId";
            await Connection.ExecuteAsync(sql, new { datastreamId, propertyId }, _transaction);
        }

        public async Task LinkObservationAsync(long datastreamId, long observationId)
        {
            var sql =
                @"INSERT INTO datastreams_observations(datastream_id, observation_id)
                    VALUES(@datastreamId, @observationId)";
            await Connection.ExecuteAsync(sql, new { datastreamId, observationId }, _transaction);
        }

        public async Task<IEnumerable<Observation>> GetLinkedObservationsAsync(long datastreamId)
        {
            var sql =
                @"SELECT
                    observations.id as ID,
                    observations.PhenomenonTime as PhenomenonTime,
                    observations.ResultTime as ResultTime,
                    observations.Result as Result,
                    observations.ValidTime as ValidTime,
                    observations.Parameters as Parameters
                FROM datastreams
                INNER JOIN datastreams_observations on (datastreams.id = datastreams_observations.datastream_id)
                INNER JOIN observations on (observations.id = datastreams_observations.observation_id)
                WHERE datastreams.id = @datastreamId;";
            var observations = await Connection.QueryAsync<Observation>(sql, new { datastreamId }, _transaction);

            return observations;
        }

        public async Task UnlinkObservationAsync(long datastreamId, long observationId)
        {
            var sql =
                @"DELETE FROM datastreams_observations
                    WHERE datastream_id = @datastreamId AND observation_id = @observationId";
            await Connection.ExecuteAsync(sql, new { datastreamId, observationId }, _transaction);
        }

        public async Task<Datastream> GetLinkedDatastreamForObservationAsync(long observationId)
        {
            var sql =
            @"SELECT
                    datastreams.id as ID,
                    datastreams.Name as Name,
                    datastreams.Description as Description,
                    datastreams.ObservationType as ObservationType,
                    datastreams.UnitOfMeasurement as UnitOfMeasurement,
                    datastreams.ObservedArea as ObservedArea,
                    datastreams.PhenomenonTime as PhenomenonTime,
                    datastreams.ResultTime as ResultTime
                FROM observations
                INNER JOIN datastreams_observations on (observations.id = datastreams_observations.observation_id)
                INNER JOIN datastreams on (datastreams.id = datastreams_observations.datastream_id)
                WHERE observations.id = @observationId;";
            var datastreams = await Connection.QueryFirstAsync<Datastream>(sql, new { observationId }, _transaction);

            return datastreams;
        }

        private static void CreateTable(IDbConnection connection, IDbTransaction transaction)
        {
            var sql =
                @"Create Table datastreams (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name VARCHAR(100) NOT NULL,
                    Description VARCHAR(1000) NULL,
                    ObservationType VARCHAR(100) NULL,
                    UnitOfMeasurement VARCHAR(100) NULL,
                    ObservedArea VARCHAR(1000),
                    PhenomenonTime TEXT,
                    ResultTime TEXT);";
            connection.ExecuteScalar(sql, transaction);
        }

        private static void CreateDatastreamSensorsTable(IDbConnection connection, IDbTransaction transaction)
        {
            var sql =
                @"CREATE TABLE datastreams_sensors(
                    datastream_id int NOT NULL,
                    sensor_id int NOT NULL,
                    FOREIGN KEY(datastream_id) REFERENCES datastreams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(sensor_id) REFERENCES sensors(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(datastream_id, sensor_id)
                );";
            connection.ExecuteScalar(sql, transaction);
        }

        private static void CreateDatastreamsObservedPropertiesTable(IDbConnection connection, IDbTransaction transaction)
        {
            var sql =
               @"CREATE TABLE datastreams_observedproperties(
                    datastream_id int NOT NULL,
                    observed_property_id int NOT NULL,
                    FOREIGN KEY(datastream_id) REFERENCES datastreams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(observed_property_id) REFERENCES observed_properties(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(datastream_id, observed_property_id)
                );";
            connection.ExecuteScalar(sql, transaction);
        }

        public static void CreateDatastreamsObservationsTable(IDbConnection connection, IDbTransaction transaction)
        {
            var sql =
                @"CREATE TABLE datastreams_observations(
                    datastream_id int NOT NULL,
                    observation_id int NOT NULL,
                    FOREIGN KEY(datastream_id) REFERENCES datastreams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(observation_id) REFERENCES observations(id) ON DELETE RESTRICT ON UPDATE  CASCADE,
                    PRIMARY KEY(datastream_id, observation_id)
                );";
            connection.ExecuteScalar(sql, transaction);
        }
    }
}
