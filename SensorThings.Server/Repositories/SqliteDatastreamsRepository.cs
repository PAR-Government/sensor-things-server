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

        public static void CheckForTables(IDbConnection connection)
        {
            if (!SqliteUtil.CheckForTable(connection, "datastreams"))
            {
                CreateTable(connection);
            }

            if (!SqliteUtil.CheckForTable(connection, "datastreams_sensors"))
            {
                CreateDatastreamSensorsTable(connection);
            }
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
                INNER JOIN datastreams_sensors on (datastreams.id = datastreams_sensors.datastream_id)
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

        private static void CreateTable(IDbConnection connection)
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
            connection.Execute(sql);
        }

        private static void CreateDatastreamSensorsTable(IDbConnection connection)
        {
            var sql =
                @"CREATE TABLE datastreams_sensors(
                    datastream_id int NOT NULL,
                    sensor_id int NOT NULL,
                    FOREIGN KEY(datastream_id) REFERENCES datastreams(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(sensor_id) REFERENCES sensors(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(datastream_id, sensor_id)
                );";
            connection.Execute(sql);
        }
    }
}
