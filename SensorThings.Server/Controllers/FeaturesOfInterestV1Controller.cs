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
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var featureOfInterest = JsonConvert.DeserializeObject<FeatureOfInterest>(data);

            var service = uow.FeaturesOfInterestService;
            featureOfInterest = await service.AddFeature(featureOfInterest);

            featureOfInterest.BaseUrl = GetBaseUrl();

            uow.Commit();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return featureOfInterest;
        }

        [Route(HttpVerbs.Get, "/FeaturesOfInterest({id})")]
        public async Task<FeatureOfInterest> GetFeatureOfInterestAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.FeaturesOfInterestService;
            var feature = await service.GetFeatureById(id);
            feature.BaseUrl = GetBaseUrl();

            return feature;
        }

        [Route(HttpVerbs.Patch, "/FeaturesOfInterest({id})")]
        public async Task UpdateFeatureOfInterestAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = uow.FeaturesOfInterestService;
            await service.UpdateFeature(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Get, "/FeaturesOfInterest")]
        public async Task<Listing<FeatureOfInterest>> GetFeaturesOfInterestAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var baseUrl = GetBaseUrl();
            var service = uow.FeaturesOfInterestService;
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
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.FeaturesOfInterestService;
            await service.RemoveFeature(id);
            uow.Commit();
        }
    }
}
