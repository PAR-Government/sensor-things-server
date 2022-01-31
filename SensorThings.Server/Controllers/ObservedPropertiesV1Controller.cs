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
    public class ObservedPropertiesV1Controller : BaseController
    {
        public ObservedPropertiesV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/ObservedProperties")]
        public async Task<ObservedProperty> CreateObservedPropertyAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var property = JsonConvert.DeserializeObject<ObservedProperty>(data);

            var service = uow.ObservedPropertiesService;
            property = await service.AddObservedProperty(property);
            property.BaseUrl = GetBaseUrl();

            uow.Commit();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return property;
        }

        [Route(HttpVerbs.Get, "/ObservedProperties({id})")]
        public async Task<ObservedProperty> GetObservedPropertyAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.ObservedPropertiesService;
            var property = await service.GetObservedPropertyById(id);
            property.BaseUrl = GetBaseUrl();

            return property;
        }

        [Route(HttpVerbs.Get, "/ObservedProperties")]
        public async Task<Listing<ObservedProperty>> GetObservedPropertiesAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var baseUrl = GetBaseUrl();
            var service = uow.ObservedPropertiesService;
            var properties = await service.GetObservedProperties();

            foreach (var property in properties)
            {
                property.BaseUrl = baseUrl;
            }

            var listing = new Listing<ObservedProperty>() { Items = properties.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Patch, "/ObservedProperties({id})")]
        public async Task UpdateObservedPropertyAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = uow.ObservedPropertiesService;
            await service.UpdateObservedProperty(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Delete, "/ObservedProperties({id})")]
        public async Task RemoveObservedPropertyAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.ObservedPropertiesService;
            await service.RemoveObservedProperty(id);
            uow.Commit();
        }
    }
}
