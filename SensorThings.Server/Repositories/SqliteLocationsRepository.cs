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
            SqlMapper.AddTypeHandler(DapperJObjectHandler.Instance);

            _transaction = transaction;

            if (!SqliteUtil.CheckForTable(Connection, "locations"))
            {
                CreateTable();
            }
        }

        public async Task<long> AddAsync(Location item)
        {
            var sql = @"INSERT INTO locations (Name, Description, EncodingType, Location)
                        VALUES(@Name, @Description, @EncodingType, @FeatureLocation); SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public Task<IEnumerable<Location>> GetAllAsync()
        {
            var sql = @"SELECT ID, Name, Description, EncodingType, Location as FeatureLocation FROM locations;";
            var locations = Connection.QueryAsync<Location>(sql, _transaction);

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
            //var param = new
            //{
            //    Name = item.Name,
            //    Description = item.Description,
            //    EncodingType = item.EncodingType,
            //    FeatureLocation = item.FeatureLocation.ToString()
            //};
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        private void CreateTable()
        {
            var sql =
                @"Create Table locations (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name VARCHAR(100) NOT NULL,
                    Description VARCHAR(1000) NULL,
                    EncodingType VARCHAR(100) NULL,
                    Location VARCHAR(1000) NULL);";
            Connection.Execute(sql, _transaction);
        }
    }
}
