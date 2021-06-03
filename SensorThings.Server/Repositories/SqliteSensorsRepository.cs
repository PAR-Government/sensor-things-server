using Dapper;
using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public class SqliteSensorsRepository : IRepository<Sensor>
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection Connection {  get { return _transaction.Connection;  } }

        public SqliteSensorsRepository(IDbTransaction transaction)
        {
            SqlMapper.AddTypeHandler(DapperJObjectHandler.Instance);
            _transaction = transaction;

            if (!SqliteUtil.CheckForTable(Connection, "sensors"))
            {
                CreateTable();
            }
        }

        public async Task<long> AddAsync(Sensor item)
        {
            var sql =
                @"INSERT INTO sensors (Name, Description, EncodingType, Metadata)
                    VALUES(@Name, @Description, @EncodingType, @Metadata);
                    SELECT last_insert_rowid();";
            var id = await Connection.ExecuteScalarAsync<long>(sql, item, _transaction);

            return id;
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync()
        {
            var sql =
                @"SELECT ID, Name, Description, EncodingType, Metadata FROM sensors";
            var sensors = await Connection.QueryAsync<Sensor>(sql, _transaction);

            return sensors;
        }

        public async Task<Sensor> GetByIdAsync(long id)
        {
            var sql =
                @"SELECT ID, Name, Description, EncodingType, Metadata 
                    FROM sensors
                    WHERE id = @ID";
            var sensor = await Connection.QueryFirstAsync<Sensor>(sql, new { ID = id }, _transaction);

            return sensor;
        }

        public async Task Remove(long id)
        {
            var sql = @"DELETE FROM sensors WHERE id = @Id";
            await Connection.ExecuteAsync(sql, new { Id = id }, _transaction);
        }

        public async Task UpdateAsync(Sensor item)
        {
            var sql =
                @"UPDATE sensors
                    SET
                        Name = @Name,
                        Description = @Description,
                        EncodingType = @EncodingType,
                        Metadata = @Metadata
                    WHERE id = @ID";
            await Connection.ExecuteAsync(sql, item, _transaction);

            throw new NotImplementedException();
        }

        private void CreateTable()
        {
            var sql =
                @"CREATE TABLE sensors (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    EncodingType TEXT,
                    Metadata TEXT);";

            Connection.Execute(sql, _transaction);
        }
    }
}
