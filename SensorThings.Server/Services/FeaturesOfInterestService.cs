using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class FeaturesOfInterestService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public FeaturesOfInterestService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<FeatureOfInterest> AddFeature(FeatureOfInterest feature)
        {
            var id = await UOW.FeaturesOfInterestRepository.AddAsync(feature);

            feature.ID = id;

            return feature;
        }

        public async Task<FeatureOfInterest> GetFeatureById(int id)
        {
            return await UOW.FeaturesOfInterestRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<FeatureOfInterest>> GetFeatures()
        {
            return await UOW.FeaturesOfInterestRepository.GetAllAsync();
        }

        public async Task<FeatureOfInterest> UpdateFeature(JObject updates, int id)
        {
            var feature = await GetFeatureById(id);

            var featureJson = JObject.FromObject(feature);
            foreach (var property in updates.Properties())
            {
                featureJson[property.Name] = property.Value;
            }

            feature = featureJson.ToObject<FeatureOfInterest>();

            await UOW.FeaturesOfInterestRepository.UpdateAsync(feature);

            return feature;
        }

        public async Task RemoveFeature(int id)
        {
            await UOW.FeaturesOfInterestRepository.Remove(id);
        }

        public async Task<IEnumerable<Observation>> GetLinkedObservations(long featureId)
        {
            return await UOW.ObservationsRepository.GetLinkedObservationsForFeatureOfInterestAsync(featureId);
        }
    }
}
