using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteObservedPropertiesRepository : IRepository<ObservedProperty>
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection {  get { return _transaction.Connection; } }

        public SqliteObservedPropertiesRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public static void CheckForTables(IDbConnection connection)
        {
            if (!SqliteUtil.CheckForTable(connection, "observed_properties"))
            {
                CreateTable(connection);
            }
        }

        public async Task<long> AddAsync(ObservedProperty item)
        {
            var sql =
                @"INSERT INTO observed_properties (Name, Definition, Description)
                    VALUES(@Name, @Definition, @Description);
                    SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<ObservedProperty>> GetAllAsync()
        {
            var sql =
                @"SELECT ID, Name, Definition, Description
                    FROM observed_properties;";
            var observedProperties = await Connection.QueryAsync<ObservedProperty>(sql, _transaction);

            return observedProperties;
        }

        public async Task<ObservedProperty> GetByIdAsync(long id)
        {
            var sql =
                @"SELECT ID, Name, Definition, Description
                    FROM observed_properties
                    WHERE id = @Id;";
            var observedProperty = await Connection.QueryFirstAsync<ObservedProperty>(sql, new { Id = id }, _transaction);

            return observedProperty;
        }

        public async Task Remove(long id)
        {
            var sql =
                @"DELETE FROM observed_properties
                    WHERE id = @Id";
            await Connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        }

        public async Task UpdateAsync(ObservedProperty item)
        {
            var sql =
                @"UPDATE observed_properties
                    SET Name = @Name,
                        Definition = @Definition,
                        Description = @Description
                    WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);
        }

        private static void CreateTable(IDbConnection connection)
        {
            var sql =
                @"CREATE TABLE observed_properties (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Definition TEXT,
                    Description TEXT
                );";
            connection.Execute(sql);
        }
    }
}
