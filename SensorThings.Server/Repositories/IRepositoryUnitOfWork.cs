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

        IRepository<Datastream> DatastreamsRepository { get; }

        IRepository<Sensor> SensorsRepository { get; }

        IRepository<ObservedProperty> ObservedPropertiesRepository { get; }

        IRepository<Observation> ObservationsRepository { get; }

        public void Commit();
    }
}
