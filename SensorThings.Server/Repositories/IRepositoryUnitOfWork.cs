using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorThings.Server.Repositories
{
    public interface IRepositoryUnitOfWork : IDisposable
    {
        IThingsRepository ThingsRepository { get; }
        IRepository<Location> LocationsRepository { get; }

        public void Commit();
    }
}
