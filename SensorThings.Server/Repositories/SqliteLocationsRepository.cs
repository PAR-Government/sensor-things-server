using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteLocationsRepository : IRepository<Location>
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection { get { return _transaction.Connection; } }

        public SqliteLocationsRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public static void CheckForTables(IDbConnection connection)
        {
            if (!SqliteUtil.CheckForTable(connection, "locations"))
            {
                CreateTable(connection);
            }
        }

        public async Task<long> AddAsync(Location item)
        {
            var sql = @"INSERT INTO locations (Name, Description, EncodingType, Location)
                        VALUES(@Name, @Description, @EncodingType, @FeatureLocation); SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            var sql = @"SELECT ID, Name, Description, EncodingType, Location as FeatureLocation FROM locations;";
            var locations = await Connection.QueryAsync<Location>(sql, _transaction);

            return locations;
        }

        public async Task<Location> GetByIdAsync(long id)
        {
            var sql = @"SELECT ID, Name, Description, EncodingType, Location as FeatureLocation FROM locations WHERE id=@ID";
            var location = await Connection.QueryFirstAsync<Location>(sql, new { ID = id }, _transaction);

            return location;
        }

        public async Task Remove(long id)
        {
            var sql = @"DELETE FROM locations WHERE id = @Id";
            await Connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        }

        public async Task UpdateAsync(Location item)
        {
            var sql = @"UPDATE locations
                        SET Name = @Name,
                            Description = @Description,
                            EncodingType = @EncodingType,
                            Location = @FeatureLocation
                        WHERE id = @ID";

            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        private static void CreateTable(IDbConnection connection)
        {
            var sql =
                @"Create Table locations (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name VARCHAR(100) NOT NULL,
                    Description VARCHAR(1000) NULL,
                    EncodingType VARCHAR(100) NULL,
                    Location VARCHAR(1000) NULL
                );";
            connection.Execute(sql);
        }
    }
}
