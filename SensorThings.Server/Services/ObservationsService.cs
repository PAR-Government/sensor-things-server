using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class ObservationsService : IObservationsService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public ObservationsService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<Observation> AddObservation(Observation observation, long datastreamId)
        {
            var id = await UOW.ObservationsRepository.AddAsync(observation);
            await UOW.DatastreamsRepository.LinkObservationAsync(datastreamId, id);

            observation.ID = id;

            return observation;
        }

        public async Task<Observation> GetObservationById(int id)
        {
            return await UOW.ObservationsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Observation>> GetObservations()
        {
            return await UOW.ObservationsRepository.GetAllAsync();
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

            await UOW.ObservationsRepository.UpdateAsync(observation);

            return observation;
        }

        public async Task RemoveObservation(int id)
        {
            await UOW.ObservationsRepository.Remove(id);
        }

        public async Task<Datastream> GetLinkedDatastream(long observationId)
        {
            var datastream = await UOW.DatastreamsRepository.GetLinkedDatastreamForObservationAsync(observationId);
            return datastream;
        }

        public async Task LinkFeatureOfInterestAsync(long observationId, long featureId)
        {
            await UOW.ObservationsRepository.LinkFeatureOfInterestAsync(observationId, featureId);
        }

        public async Task<FeatureOfInterest> GetLinkedFeatureOfInterest(long observationId)
        {
            return await UOW.ObservationsRepository.GetLinkedFeatureOfInterestAsync(observationId);
        }

        public async Task UnlinkFeatureOfInterestAsync(long observationId, long featureId)
        {
            await UOW.ObservationsRepository.UnlinkFeatureOfInterest(observationId, featureId);
        }
    }
}
