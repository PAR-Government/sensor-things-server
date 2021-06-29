using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteObservationsRepository : IObservationsRepository
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection { get { return _transaction.Connection; } }

        public SqliteObservationsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public static void CheckForTables(IDbConnection connection)
        {
            if (!SqliteUtil.CheckForTable(connection, "observations"))
            {
                CreateTable(connection);
            }

            if (!SqliteUtil.CheckForTable(connection, "observations_featuresofinterest"))
            {
                CreateObservationsFeaturesTable(connection);
            }
        }

        public async Task<long> AddAsync(Observation item)
        {
            var sql =
                @"INSERT INTO observations
                    (PhenomenonTime, ResultTime, Result, ValidTime, Parameters)
                    VALUES(@PhenomenonTime, @ResultTime, @Result, @ValidTime, @Parameters);
                SELECT last_insert_rowid();";

            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<Observation>> GetAllAsync()
        {
            var sql =
                @"SELECT ID, PhenomenonTime, ResultTime, Result, ValidTime, Parameters
                    FROM observations";
            var observations = await Connection.QueryAsync<Observation>(sql, _transaction);

            return observations;
        }

        public async Task<Observation> GetByIdAsync(long id)
        {
            var sql =
                @"SELECT ID, PhenomenonTime, ResultTime, Result, ValidTime, Parameters
                    FROM observations
                    WHERE id = @ID";
            var observation = await Connection.QueryFirstAsync<Observation>(sql, new { ID = id }, _transaction);

            return observation;
        }

        public async Task Remove(long id)
        {
            var sql =
                @"DELETE FROM observations WHERE id = @Id";
            await Connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        }

        public async Task UpdateAsync(Observation item)
        {
            var sql =
                @"UPDATE observations
                    SET PhenomenonTime = @PhenomenonTime,
                        ResultTime = @ResultTime,
                        Result = @Result,
                        ValidTime = @ValidTime,
                        Parameters = @Parameters
                    WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        public async Task LinkFeatureOfInterestAsync(long observationId, long featureId)
        {
            var sql =
                @"INSERT INTO observations_featuresofinterest(observation_id, feature_id)
                    VALUES(@observationId, @featureId)";
            await Connection.ExecuteAsync(sql, new { observationId, featureId }, _transaction);
        }

        public async Task<FeatureOfInterest> GetLinkedFeatureOfInterestAsync(long observationId)
        {
            var sql =
                @"SELECT
                    featuresOfInterest.id as ID,
                    featuresOfInterest.Name as Name,
                    featuresOfInterest.Description as Description,
                    featuresOfInterest.EncodingType as EncodingType,
                    featuresOfInterest.Feature as Feature
                FROM observations
                INNER JOIN observations_featuresofinterest on (observations.id = observations_featuresofinterest.observation_id)
                INNER JOIN featuresOfInterest on (featuresOfInterest.id = observations_featuresofinterest.feature_id)
                WHERE observations.id = @observationId";
            var feature = await Connection.QueryFirstAsync<FeatureOfInterest>(sql, new { observationId }, _transaction);
            return feature;
        }

        public async Task<IEnumerable<Observation>> GetLinkedObservationsForFeatureOfInterestAsync(long featureId)
        {
            var sql =
                @"SELECT
                    observations.id as ID,
                    oservations.PhenomenonTime as PhenomenonTime, 
                    observations.ResultTime as ResultTime,
                    observations.Result as Result,
                    observations.ValidTime as ValidTime,
                    observations.Parameters as Parameters
                FROM featuresOfInterest
                INNER JOIN observations_featuresofinterest on (featuresOfInterest.id = observations_featuresofinterest.feature_id)
                INNER JOIN observations on (observations.id = observations_featuresofinterest.observation_id)
                WHERE featuresOfInterest.id = @featureId";
            var observations = await Connection.QueryAsync<Observation>(sql, new { featureId }, _transaction);
            return observations;
        }

        public async Task UnlinkFeatureOfInterest(long observationId, long featureId)
        {
            var sql =
                @"DELETE FROM observations_featuresofinterest
                    WHERE observation_id = @observationId AND feature_id = @featureId";
            await Connection.ExecuteAsync(sql, new { observationId, featureId }, _transaction);
        }

        private static void CreateTable(IDbConnection connection)
        {
            var sql =
                @"CREATE TABLE observations (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PhenomenonTime TEXT,
                    ResultTime TEXT,
                    Result TEXT,
                    ValidTime TEXT,
                    Parameters TEXT);";
            connection.Execute(sql);
        }

        private static void CreateObservationsFeaturesTable(IDbConnection connection)
        {
            var sql =
                @"CREATE TABLE observations_featuresofinterest (
                    observation_id int NOT NULL,
                    feature_id int NOT NULL,
                    FOREIGN KEY(observation_id) REFERENCES observations(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    FOREIGN KEY(feature_id) REFERENCES featuresOfInterest(id) ON DELETE RESTRICT ON UPDATE CASCADE,
                    PRIMARY KEY(observation_id, feature_id)";
            connection.Execute(sql);
        }
    }
}
