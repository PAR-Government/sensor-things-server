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

        public async Task<Datastream> GetDatastreamById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var datastream = await uow.DatastreamsRepository.GetByIdAsync(id);

            return datastream;
        }

        public async Task<IEnumerable<Datastream>> GetDatastreams()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.DatastreamsRepository.GetAllAsync();
        }

        public async Task RemoveDatastream(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.DatastreamsRepository.Remove(id);
            uow.Commit();
        }
    }
}
