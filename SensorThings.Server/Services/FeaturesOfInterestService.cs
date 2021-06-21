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
            throw new NotImplementedException();
        }

        public async Task<FeatureOfInterest> GetFeatureById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FeatureOfInterest>> GetFeatures()
        {
            throw new NotImplementedException();
        }

        public async Task<FeatureOfInterest> UpdateFeature(JObject updates, int id)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveFeature(int id)
        {
            throw new NotImplementedException();
        }
    }
}
