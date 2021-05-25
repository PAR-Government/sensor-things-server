using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;

namespace SensorThings.Server.Test.TestObjects
{

    public class TestRepoFactory : IRepositoryFactory
    {
        public IThingsRepository ThingsRepository { get; set; }

        public IRepository<Location> LocationsRepository { get; set; }

        public IRepositoryUnitOfWork CreateUnitOfWork()
        {
            return new TestUOW()
            {
                ThingsRepository = ThingsRepository,
                LocationsRepository = LocationsRepository
            };
        }

        public class TestUOW : IRepositoryUnitOfWork
        {
            public IThingsRepository ThingsRepository { get; set; }

            public IRepository<Location> LocationsRepository { get; set; }

            public void Commit()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
