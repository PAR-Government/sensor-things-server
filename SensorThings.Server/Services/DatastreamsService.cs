using Newtonsoft.Json.Linq;
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

        public async Task<Datastream> UpdateDatastream(JObject updates, int id)
        {
            var datastream = await GetDatastreamById(id);

            // Convert to JSON to make it easier to merge updates
            var datastreamJson = JObject.FromObject(datastream);
            foreach (var property in updates.Properties())
            {
                datastreamJson[property.Name] = property.Value;
            }

            // Convert back
            datastream = datastreamJson.ToObject<Datastream>();

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.DatastreamsRepository.UpdateAsync(datastream);
            uow.Commit();

            return datastream;
        }
    }
}
