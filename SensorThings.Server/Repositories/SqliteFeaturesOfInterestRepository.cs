using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteFeaturesOfInterestRepository : IRepository<FeatureOfInterest>
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection {  get { return _transaction.Connection; } }

        public SqliteFeaturesOfInterestRepository(IDbTransaction transaction)
        {
            SqlMapper.AddTypeHandler(DapperJObjectHandler.Instance);

            _transaction = transaction;

            if (!SqliteUtil.CheckForTable(Connection, "featuresOfInterest"))
            {
                CreateTable();
            }
        }

        public async Task<long> AddAsync(FeatureOfInterest item)
        {
            var sql =
                @"INSERT INTO featuresOfInterest
                    (Name, Description, EncodingType, Feature)
                    VALUES(@Name, @Description, @EncodingType, @Feature);
                    SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<FeatureOfInterest>> GetAllAsync()
        {
            var sql =
                @"SELECT
                    ID, Name, Description, EncodingType, Feature
                    FROM featuresOfInterest;";
            var features = await Connection.QueryAsync<FeatureOfInterest>(sql, _transaction);

            return features;
        }

        public async Task<FeatureOfInterest> GetByIdAsync(long id)
        {
            var sql =
                @"SELECT
                    ID, Name, Description, EncodingType, Feature
                    FROM featuresOfInterest
                    WHERE id = @ID";
            var feature = await Connection.QueryFirstAsync<FeatureOfInterest>(sql, new { ID = id }, _transaction);

            return feature;
        }

        public async Task Remove(long id)
        {
            var sql =
                @"DELETE FROM featuresOfInterest WHERE id = @Id";
            await Connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        }

        public async Task UpdateAsync(FeatureOfInterest item)
        {
            var sql =
                @"UPDATE featuresOfInterest
                    SET Name = @Name,
                        Description = @Description,
                        EncodingType = @EncodingType,
                        Feature = @Feature
                    WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        private void CreateTable()
        {
            var sql =
                @"Create Table featuresOfInterest (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT NULL,
                    EncodingType TEXT,
                    Feature TEXT);";
            Connection.Execute(sql, _transaction);
        }
    }
}
