using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using System;

namespace SensorThings.Server.Test.TestObjects
{

    public class TestRepoFactory : IRepositoryFactory
    {
        #region Low level repositories
        public IThingsRepository ThingsRepository { get; set; }

        public IRepository<Location> LocationsRepository { get; set; }

        public IHistoricalLocationsRepository HistoricalLocationsRepository { get; set; }

        public IDatastreamsRepository DatastreamsRepository { get; set; }

        public IRepository<Sensor> SensorsRepository { get; set; }

        public IRepository<ObservedProperty> ObservedPropertiesRepository { get; set; }

        public IObservationsRepository ObservationsRepository { get; set; }

        public IRepository<FeatureOfInterest> FeaturesOfInterestRepository { get; set; }
        #endregion

        #region high level repository services
        public IDatastreamsService DatastreamsService { get; set; }

        public IFeaturesOfInterestService FeaturesOfInterestService { get; set; }

        public IHistoricalLocationsService HistoricalLocationsService { get; set; }

        public ILocationsService LocationsService { get; set; }

        public IObservationsService ObservationsService { get; set; }

        public IObservedPropertiesService ObservedPropertiesService { get; set; }

        public ISensorsService SensorsService { get; set; }

        public IThingsService ThingsService { get; set; }
        #endregion

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
                FeaturesOfInterestRepository = FeaturesOfInterestRepository,

                DatastreamsService = DatastreamsService,
                FeaturesOfInterestService = FeaturesOfInterestService,
                HistoricalLocationsService = HistoricalLocationsService,
                LocationsService = LocationsService,
                ObservationsService = ObservationsService,
                ObservedPropertiesService = ObservedPropertiesService,
                SensorsService = SensorsService,
                ThingsService = ThingsService
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

            public IObservationsRepository ObservationsRepository { get; set; }

            public IRepository<FeatureOfInterest> FeaturesOfInterestRepository { get; set; }

            public IDatastreamsService DatastreamsService { get; set; }

            public IFeaturesOfInterestService FeaturesOfInterestService { get; set; }

            public IHistoricalLocationsService HistoricalLocationsService { get; set; }

            public ILocationsService LocationsService { get; set; }

            public IObservationsService ObservationsService { get; set; }

            public IObservedPropertiesService ObservedPropertiesService { get; set; }

            public ISensorsService SensorsService { get; set; }

            public IThingsService ThingsService { get; set; }

            public void Commit()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
