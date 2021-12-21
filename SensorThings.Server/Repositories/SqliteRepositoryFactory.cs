using Dapper;
using Microsoft.Data.Sqlite;
using SensorThings.Entities;
using System;
using System.Data;
using System.Threading.Tasks;

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

            using var uow = (SqliteRepositoryUnitOfWork)CreateUnitOfWork();
            var connection = uow._connection;
            var transaction = uow._transaction;
            CheckForTables(connection, transaction);
            uow.Commit();
        }

        public IDbConnection CreateConnection()
        {
            return new SqliteConnection(cs);
        }

        public IRepositoryUnitOfWork CreateUnitOfWork()
        {
            return new SqliteRepositoryUnitOfWork(CreateConnection());
        }

        public async Task ResetRepositories()
        {
            using var uow = (SqliteRepositoryUnitOfWork)CreateUnitOfWork();
            var connection = uow._connection;
            var transaction = uow._transaction;

            // Order matters and most coincide with ensuring foreign key contraints in the underlying tables are removed first
            await SqliteDatastreamsRepository.DropAssociationTables(connection, transaction);
            await SqliteFeaturesOfInterestRepository.DropAssociationTables(connection, transaction);
            await SqliteHistoricalLocationsRepository.DropAssociationTables(connection, transaction);
            await SqliteLocationsRepository.DropAssociationTables(connection, transaction);
            await SqliteObservationsRepository.DropAssociationTables(connection, transaction);
            await SqliteObservedPropertiesRepository.DropAssociationTables(connection, transaction);
            await SqliteSensorsRepository.DropAssociationTables(connection, transaction);
            await SqliteThingsRepository.DropAssociationTables(connection, transaction);

            await SqliteThingsRepository.DropTables(connection, transaction);
            await SqliteSensorsRepository.DropTables(connection, transaction);
            await SqliteObservedPropertiesRepository.DropTables(connection, transaction);
            await SqliteObservationsRepository.DropTables(connection, transaction);
            await SqliteLocationsRepository.DropTables(connection, transaction);
            await SqliteHistoricalLocationsRepository.DropTables(connection, transaction);
            await SqliteFeaturesOfInterestRepository.DropTables(connection, transaction);
            await SqliteDatastreamsRepository.DropTables(connection, transaction);

            CheckForTables(connection, transaction);
            uow.Commit();
        }

        public async Task ExportToFile(string filepath)
        {
            var connection = CreateConnection();
            await SqliteUtil.ExportIntoFile(connection, filepath);
        }

        private void CheckForTables(IDbConnection connection, IDbTransaction transaction)
        {
            SqliteDatastreamsRepository.CheckForTables(connection, transaction);
            SqliteFeaturesOfInterestRepository.CheckForTables(connection, transaction);
            SqliteHistoricalLocationsRepository.CheckForTables(connection, transaction);
            SqliteLocationsRepository.CheckForTables(connection, transaction);
            SqliteObservationsRepository.CheckForTables(connection, transaction);
            SqliteObservedPropertiesRepository.CheckForTables(connection, transaction);
            SqliteSensorsRepository.CheckForTables(connection, transaction);
            SqliteThingsRepository.CheckForTables(connection, transaction);
        }
    }
}
