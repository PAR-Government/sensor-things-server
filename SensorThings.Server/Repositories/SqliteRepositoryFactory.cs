using Dapper;
using Microsoft.Data.Sqlite;
using SensorThings.Entities;
using System;
using System.Data;

namespace SensorThings.Server.Repositories
{
    public class SqliteRepositoryFactory : IRepositoryFactory
    {
        private readonly string cs;

        public SqliteRepositoryFactory(string dbFile)
        {
            SQLitePCL.Batteries.Init();
            SqlMapper.AddTypeHandler(DapperJObjectHandler.Instance);
            SqlMapper.AddTypeHandler(DapperOGCTimeHandler.Instance);
            SqlMapper.AddTypeHandler(DapperURIMapper.Instance);

            cs = new SqliteConnectionStringBuilder()
            {
                DataSource = dbFile,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();

            var connection = CreateConnection();

            SqliteDatastreamsRepository.CheckForTables(connection);
            SqliteFeaturesOfInterestRepository.CheckForTables(connection);
            SqliteHistoricalLocationsRepository.CheckForTables(connection);
            SqliteLocationsRepository.CheckForTables(connection);
            SqliteObservationsRepository.CheckForTables(connection);
            SqliteObservedPropertiesRepository.CheckForTables(connection);
            SqliteSensorsRepository.CheckForTables(connection);
            SqliteThingsRepository.CheckForTables(connection);
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(cs);
        }

        public IRepositoryUnitOfWork CreateUnitOfWork()
        {
            return new SqliteRepositoryUnitOfWork(CreateConnection());
        }
    }
}
