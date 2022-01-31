using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface IFeaturesOfInterestService
    {
        Task<FeatureOfInterest> AddFeature(FeatureOfInterest feature);
        Task<FeatureOfInterest> GetFeatureById(int id);
        Task<IEnumerable<FeatureOfInterest>> GetFeatures();
        Task<IEnumerable<Observation>> GetLinkedObservations(long featureId);
        Task RemoveFeature(int id);
        Task<FeatureOfInterest> UpdateFeature(JObject updates, int id);
    }
}