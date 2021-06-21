using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class FeaturesOfInterestService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public FeaturesOfInterestService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<FeatureOfInterest> AddFeature(FeatureOfInterest feature)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.FeaturesOfInterestRepository.AddAsync(feature);
            uow.Commit();

            feature.ID = id;

            return feature;
        }

        public async Task<FeatureOfInterest> GetFeatureById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.FeaturesOfInterestRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<FeatureOfInterest>> GetFeatures()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.FeaturesOfInterestRepository.GetAllAsync();
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

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.FeaturesOfInterestRepository.UpdateAsync(feature);
            uow.Commit();

            return feature;
        }

        public async Task RemoveFeature(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.FeaturesOfInterestRepository.Remove(id);
            uow.Commit();
        }
    }
}
