using SensorThings.Entities;
using System;
using System.Data;

namespace SensorThings.Server.Repositories
{
    public interface IRepositoryFactory
    {
        public IRepositoryUnitOfWork CreateUnitOfWork();
    }
}
