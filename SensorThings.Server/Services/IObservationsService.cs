using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface IObservationsService
    {
        Task<Observation> AddObservation(Observation observation, long datastreamId);
        Task<Datastream> GetLinkedDatastream(long observationId);
        Task<FeatureOfInterest> GetLinkedFeatureOfInterest(long observationId);
        Task<Observation> GetObservationById(int id);
        Task<IEnumerable<Observation>> GetObservations();
        Task LinkFeatureOfInterestAsync(long observationId, long featureId);
        Task RemoveObservation(int id);
        Task UnlinkFeatureOfInterestAsync(long observationId, long featureId);
        Task<Observation> UpdateObservation(JObject updates, int id);
    }
}