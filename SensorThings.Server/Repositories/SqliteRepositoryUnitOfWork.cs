using SensorThings.Entities;
using SensorThings.Server.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SensorThings.Server.Repositories
{
    public class SqliteRepositoryUnitOfWork : IRepositoryUnitOfWork
    {
        internal IDbConnection _connection;
        internal IDbTransaction _transaction;

        #region Low level repositories
        private IThingsRepository _thingsRepository;
        private IRepository<Location> _locationsRepository;
        private IHistoricalLocationsRepository _historicalLocationsRepository;
        private IDatastreamsRepository _datastreamsRepository;
        private IRepository<Sensor> _sensorsRepository;
        private IRepository<ObservedProperty> _observedPropertiesRepository;
        private IObservationsRepository _observationsRepository;
        private IRepository<FeatureOfInterest> _featuresOfInterestRepository;

        public IThingsRepository ThingsRepository
        {
            get => _thingsRepository ??= new SqliteThingsRepository(_transaction);
        }

        public IRepository<Location> LocationsRepository
        {
            get => _locationsRepository ??= new SqliteLocationsRepository(_transaction);
        }

        public IHistoricalLocationsRepository HistoricalLocationsRepository
        {
            get => _historicalLocationsRepository ??= new SqliteHistoricalLocationsRepository(_transaction);
        }

        public IDatastreamsRepository DatastreamsRepository
        {
            get => _datastreamsRepository ??= new SqliteDatastreamsRepository(_transaction);
        }

        public IRepository<Sensor> SensorsRepository
        {
            get => _sensorsRepository ??= new SqliteSensorsRepository(_transaction);
        }

        public IRepository<ObservedProperty> ObservedPropertiesRepository
        {
            get => _observedPropertiesRepository ??= new SqliteObservedPropertiesRepository(_transaction);
        }

        public IObservationsRepository ObservationsRepository
        {
            get => _observationsRepository ??= new SqliteObservationsRepository(_transaction);
        }

        public IRepository<FeatureOfInterest> FeaturesOfInterestRepository
        {
            get => _featuresOfInterestRepository ??= new SqliteFeaturesOfInterestRepository(_transaction);
        }
        #endregion

        #region high level repository services
        public IDatastreamsService DatastreamsService
        {
            get => new DatastreamsService(this);
        }

        public IFeaturesOfInterestService FeaturesOfInterestService
        {
            get => new FeaturesOfInterestService(this);
        }

        public IHistoricalLocationsService HistoricalLocationsService
        {
            get => new HistoricalLocationsService(this);
        }

        public ILocationsService LocationsService
        {
            get => new LocationsService(this);
        }

        public IObservationsService ObservationsService
        {
            get => new ObservationsService(this);
        }

        public IObservedPropertiesService ObservedPropertiesService
        {
            get => new ObservedPropertiesService(this);
        }

        public ISensorsService SensorsService
        {
            get => new SensorsService(this);
        }

        public IThingsService ThingsService
        {
            get => new ThingsService(this);
        }
        #endregion

        public SqliteRepositoryUnitOfWork(IDbConnection connection)
        {
            _connection = connection;
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                Reset();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        private void Reset()
        {
            _thingsRepository = null;
            _locationsRepository = null;
            _historicalLocationsRepository = null;
            _datastreamsRepository = null;
            _sensorsRepository = null;
            _observedPropertiesRepository = null;
            _observationsRepository = null;
            _featuresOfInterestRepository = null;
        }
    }
}
