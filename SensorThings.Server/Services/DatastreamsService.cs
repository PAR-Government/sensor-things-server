using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class DatastreamsService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public DatastreamsService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<Datastream> AddDatastream(Datastream datastream)
        {
            var id = await UOW.DatastreamsRepository.AddAsync(datastream);

            datastream.ID = id;

            return datastream;
        }

        public async Task<Datastream> GetDatastreamById(int id)
        {
            var datastream = await UOW.DatastreamsRepository.GetByIdAsync(id);

            return datastream;
        }

        public async Task<IEnumerable<Datastream>> GetDatastreams()
        {
            return await UOW.DatastreamsRepository.GetAllAsync();
        }

        public async Task RemoveDatastream(int id)
        {
            await UOW.DatastreamsRepository.Remove(id);
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

            await UOW.DatastreamsRepository.UpdateAsync(datastream);

            return datastream;
        }

        public async Task LinkSensorAsync(long datastreamId, long sensorId)
        {
            await UOW.DatastreamsRepository.LinkSensorAsync(datastreamId, sensorId);
        }

        public async Task<Sensor> GetLinkedSensorAsync(long datastreamId)
        {
            var sensor = await UOW.DatastreamsRepository.GetLinkedSensorAsync(datastreamId);
            return sensor;
        }

        public async Task UnlinkSensorAsync(long datastreamId, long sensorId)
        {
            await UOW.DatastreamsRepository.UnlinkSensorAsync(datastreamId, sensorId);
        }

        public async Task LinkObservedPropertyAsync(long datastreamId, long propertyId)
        {
            await UOW.DatastreamsRepository.LinkObservedPropertyAsync(datastreamId, propertyId);
        }

        public async Task<ObservedProperty> GetLinkedObservedPropertyAsync(long datastreamId)
        {
            var property = await UOW.DatastreamsRepository.GetLinkedObservedPropertyAsync(datastreamId);
            return property;
        }

        public async Task UnlinkObservedPropertyAsync(long datastreamId, long propertyId)
        {
            await UOW.DatastreamsRepository.UnlinkObservedPropertyAsync(datastreamId, propertyId);
        }

        public async Task LinkObservationAsync(long datastreamId, long observationId)
        {
            await UOW.DatastreamsRepository.LinkObservationAsync(datastreamId, observationId);
        }

        public async Task<IEnumerable<Observation>> GetLinkedObservations(long datastreamId)
        {
            return await UOW.DatastreamsRepository.GetLinkedObservationsAsync(datastreamId);
        }

        public async Task UnlinkObservation(long datastreamId, long observationId)
        {
            await UOW.DatastreamsRepository.UnlinkObservationAsync(datastreamId, observationId);
        }
    }
}
