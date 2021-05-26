using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class DatastreamsService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public DatastreamsService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<Datastream> AddDatastream(Datastream datastream)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.DatastreamsRepository.AddAsync(datastream);
            uow.Commit();

            datastream.ID = id;

            return datastream;
        }
    }
}
