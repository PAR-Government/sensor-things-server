using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface IDatastreamsService
    {
        Task<Datastream> AddDatastream(Datastream datastream);
        Task<Datastream> GetDatastreamById(int id);
        Task<IEnumerable<Datastream>> GetDatastreams();
        Task<IEnumerable<Observation>> GetLinkedObservations(long datastreamId);
        Task<ObservedProperty> GetLinkedObservedPropertyAsync(long datastreamId);
        Task<Sensor> GetLinkedSensorAsync(long datastreamId);
        Task<Thing> GetLinkedThing(long datastreamId);
        Task LinkObservationAsync(long datastreamId, long observationId);
        Task LinkObservedPropertyAsync(long datastreamId, long propertyId);
        Task LinkSensorAsync(long datastreamId, long sensorId);
        Task RemoveDatastream(int id);
        Task UnlinkObservation(long datastreamId, long observationId);
        Task UnlinkObservedPropertyAsync(long datastreamId, long propertyId);
        Task UnlinkSensorAsync(long datastreamId, long sensorId);
        Task<Datastream> UpdateDatastream(JObject updates, int id);
    }
}