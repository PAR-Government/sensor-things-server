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

            cs = new SqliteConnectionStringBuilder()
            {
                DataSource = dbFile,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();
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
