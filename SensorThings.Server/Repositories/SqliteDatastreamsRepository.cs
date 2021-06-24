using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteDatastreamsRepository : IRepository<Datastream>
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
    }
}
