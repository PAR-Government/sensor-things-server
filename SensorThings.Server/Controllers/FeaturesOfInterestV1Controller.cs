using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;

namespace SensorThings.Server.Controllers
{
    public class FeaturesOfInterestV1Controller : BaseController
    {
        public FeaturesOfInterestV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/FeaturesOfInterest")]
        public async Task<FeatureOfInterest> CreateFeatureOfInterestAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var featureOfInterest = JsonConvert.DeserializeObject<FeatureOfInterest>(data);

            var service = new FeaturesOfInterestService(RepoFactory);
            featureOfInterest = await service.AddFeature(featureOfInterest);

            featureOfInterest.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return featureOfInterest;
        }

        [Route(HttpVerbs.Get, "/FeaturesOfInterest({id})")]
        public async Task<FeatureOfInterest> GetFeatureOfInterestAsync(int id)
        {
            var service = new FeaturesOfInterestService(RepoFactory);
            var feature = await service.GetFeatureById(id);
            feature.BaseUrl = GetBaseUrl();

            return feature;
        }

        [Route(HttpVerbs.Patch, "/FeaturesOfInterest({id})")]
        public async Task UpdateFeatureOfInterestAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new FeaturesOfInterestService(RepoFactory);
            await service.UpdateFeature(updates, id);
        }

        [Route(HttpVerbs.Get, "/FeaturesOfInterest")]
        public async Task<Listing<FeatureOfInterest>> GetFeaturesOfInterestAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new FeaturesOfInterestService(RepoFactory);
            var features = await service.GetFeatures();

            foreach (var feature in features)
            {
                feature.BaseUrl = baseUrl;
            }

            var listing = new Listing<FeatureOfInterest>() { Items = features.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Delete, "/FeaturesOfInterest({id})")]
        public async Task RemoveFeatureOfInterest(int id)
        {
            var service = new FeaturesOfInterestService(RepoFactory);
            await service.RemoveFeature(id);
        }
    }
}
