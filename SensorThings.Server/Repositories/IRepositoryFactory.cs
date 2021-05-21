using SensorThings.Entities;
using System;
using System.Data;

namespace SensorThings.Server.Repositories
{
    public interface IRepositoryFactory
    {
        public IDbConnection CreateConnection();
        public IThingsRepository CreateThingsRepository(IDbTransaction transaction);
        public IRepository<Location> CreateLocationsRepository(IDbTransaction transaction);
    }
}
