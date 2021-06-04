using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;

namespace SensorThings.Server.Test.TestObjects
{

    public class TestRepoFactory : IRepositoryFactory
    {
        public IThingsRepository ThingsRepository { get; set; }

        public IRepository<Location> LocationsRepository { get; set; }

        public IRepository<Datastream> DatastreamsRepository { get; set; }

        public IRepository<Sensor> SensorsRepository { get; set; }

        public IRepository<ObservedProperty> ObservedPropertiesRepository { get; set; }

        public IRepositoryUnitOfWork CreateUnitOfWork()
        {
            return new TestUOW()
            {
                ThingsRepository = ThingsRepository,
                LocationsRepository = LocationsRepository,
                DatastreamsRepository = DatastreamsRepository,
                SensorsRepository = SensorsRepository,
                ObservedPropertiesRepository = ObservedPropertiesRepository
            };
        }

        public class TestUOW : IRepositoryUnitOfWork
        {
            public IThingsRepository ThingsRepository { get; set; }

            public IRepository<Location> LocationsRepository { get; set; }

            public IRepository<Datastream> DatastreamsRepository { get; set; }

            public IRepository<Sensor> SensorsRepository { get; set; }

            public IRepository<ObservedProperty> ObservedPropertiesRepository { get; set; }

            public void Commit()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
