using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SensorThings.Server.Repositories
{
    public class SqliteRepositoryUnitOfWork : IRepositoryUnitOfWork
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        private IThingsRepository _thingsRepository;
        private IRepository<Location> _locationsRepository;
        private IRepository<Datastream> _datastreamsRepository;
        private IRepository<Sensor> _sensorsRepository;

        public IThingsRepository ThingsRepository
        {
            get => _thingsRepository ??= new SqliteThingsRepository(_transaction);
        }

        public IRepository<Location> LocationsRepository
        {
            get => _locationsRepository ??= new SqliteLocationsRepository(_transaction);
        }

        public IRepository<Datastream> DatastreamsRepository
        {
            get => _datastreamsRepository ??= new SqliteDatastreamsRepository(_transaction);
        }

        public IRepository<Sensor> SensorsRepository
        {
            get => _sensorsRepository ??= new SqliteSensorsRepository(_transaction);
        }

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
            _datastreamsRepository = null;
            _sensorsRepository = null;
        }
    }
}
