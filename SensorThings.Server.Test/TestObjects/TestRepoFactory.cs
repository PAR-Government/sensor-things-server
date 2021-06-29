using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;

namespace SensorThings.Server.Test.TestObjects
{

    public class TestRepoFactory : IRepositoryFactory
    {
        public IThingsRepository ThingsRepository { get; set; }

        public IRepository<Location> LocationsRepository { get; set; }

        public IHistoricalLocationsRepository HistoricalLocationsRepository { get; set; }

        public IDatastreamsRepository DatastreamsRepository { get; set; }

        public IRepository<Sensor> SensorsRepository { get; set; }

        public IRepository<ObservedProperty> ObservedPropertiesRepository { get; set; }

        public IRepository<Observation> ObservationsRepository { get; set; }

        public IRepository<FeatureOfInterest> FeaturesOfInterestRepository { get; set; }

        public IRepositoryUnitOfWork CreateUnitOfWork()
        {
            return new TestUOW()
            {
                ThingsRepository = ThingsRepository,
                LocationsRepository = LocationsRepository,
                HistoricalLocationsRepository = HistoricalLocationsRepository,
                DatastreamsRepository = DatastreamsRepository,
                SensorsRepository = SensorsRepository,
                ObservedPropertiesRepository = ObservedPropertiesRepository,
                ObservationsRepository = ObservationsRepository,
                FeaturesOfInterestRepository = FeaturesOfInterestRepository
            };
        }

        public class TestUOW : IRepositoryUnitOfWork
        {
            public IThingsRepository ThingsRepository { get; set; }

            public IRepository<Location> LocationsRepository { get; set; }

            public IHistoricalLocationsRepository HistoricalLocationsRepository { get; set; }

            public IDatastreamsRepository DatastreamsRepository { get; set; }

            public IRepository<Sensor> SensorsRepository { get; set; }

            public IRepository<ObservedProperty> ObservedPropertiesRepository { get; set; }

            public IRepository<Observation> ObservationsRepository { get; set; }

            public IRepository<FeatureOfInterest> FeaturesOfInterestRepository { get; set; }

            public void Commit()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
