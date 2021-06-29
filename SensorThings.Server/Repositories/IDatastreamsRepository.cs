using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public interface IDatastreamsRepository : IRepository<Datastream>
    {
        public Task LinkSensorAsync(long datastreamId, long sensorId);

        public Task<Sensor> GetLinkedSensorAsync(long datastreamId);

        public Task UnlinkSensorAsync(long datastreamId, long sensorId);

        public Task<IEnumerable<Datastream>> GetLinkedDatastreamsForSensorAsync(long sensorId);

        public Task LinkObservedPropertyAsync(long datastreamId, long propertyId);

        public Task<ObservedProperty> GetLinkedObservedPropertyAsync(long datastreamId);

        public Task<IEnumerable<Datastream>> GetLinkedDatastreamsForObservedPropertyAsync(long propertyId);

        public Task UnlinkObservedPropertyAsync(long datastreamId, long propertyId);

        public Task LinkObservationAsync(long datastreamId, long observationId);

        public Task<IEnumerable<Observation>> GetLinkedObservationsAsync(long datastreamId);

        public Task UnlinkObservationAsync(long datastreamId, long observationId);

        public Task<Datastream> GetLinkedDatastreamForObservationAsync(long observationId);
    }
}
