using SensorThings.Entities;
using SensorThings.Server.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorThings.Server.Repositories
{
    public interface IRepositoryUnitOfWork : IDisposable
    {
        #region low level repositories
        IThingsRepository ThingsRepository { get; }

        IRepository<Location> LocationsRepository { get; }

        IHistoricalLocationsRepository HistoricalLocationsRepository { get; }

        IDatastreamsRepository DatastreamsRepository { get; }

        IRepository<Sensor> SensorsRepository { get; }

        IRepository<ObservedProperty> ObservedPropertiesRepository { get; }

        IObservationsRepository ObservationsRepository { get; }

        IRepository<FeatureOfInterest> FeaturesOfInterestRepository { get; }

        #endregion

        #region high level repository services
        // Higher level interfaces for typical interactions
        IDatastreamsService DatastreamsService { get; }

        IFeaturesOfInterestService FeaturesOfInterestService { get; }

        IHistoricalLocationsService HistoricalLocationsService { get; }

        ILocationsService LocationsService { get; }

        IObservationsService ObservationsService { get; }

        IObservedPropertiesService ObservedPropertiesService { get; }

        ISensorsService SensorsService { get; }

        IThingsService ThingsService { get; }
        #endregion

        void Commit();
    }
}
