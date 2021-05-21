using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SensorThings.Server.Repositories
{
    public class RepositoryUnitOfWork : IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        private IThingsRepository _thingsRepository;
        private IRepository<Location> _locationsRepository;


        public IThingsRepository ThingsRepository
        {
            get
            {
                return _thingsRepository ??= Server.RepoFactory.CreateThingsRepository(_transaction);
            }
        }

        public IRepository<Location> LocationsRepository
        {
            get
            {
                return _locationsRepository ??= Server.RepoFactory.CreateLocationsRepository(_transaction);
            }
        }

        public RepositoryUnitOfWork()
        {
            _connection = Server.RepoFactory.CreateConnection();
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
        }
    }
}
