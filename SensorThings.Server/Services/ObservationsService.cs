using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class ObservationsService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public ObservationsService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<Observation> AddObservation(Observation observation)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.ObservationsRepository.AddAsync(observation);
            uow.Commit();

            observation.ID = id;

            return observation;
        }

        public async Task<Observation> GetObservationById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ObservationsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Observation>> GetObservations()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ObservationsRepository.GetAllAsync();
        }

        public async Task<Observation> UpdateObservation(JObject updates, int id)
        {
            var observation = await GetObservationById(id);

            var observationJson = JObject.FromObject(observation);
            foreach (var property in updates.Properties())
            {
                observationJson[property.Name] = property.Value;
            }

            observation = observationJson.ToObject<Observation>();

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ObservationsRepository.UpdateAsync(observation);
            uow.Commit();

            return observation;
        }

        public async Task RemoveObservation(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ObservationsRepository.Remove(id);
            uow.Commit();
        }
    }
}
