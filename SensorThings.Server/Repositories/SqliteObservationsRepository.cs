using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteObservationsRepository : IRepository<Observation>
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
    }
}
