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
    public class ObservedPropertiesV1Controller : BaseController
    {
        public ObservedPropertiesV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/ObservedProperties")]
        public async Task<ObservedProperty> CreateObservedPropertyAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var property = JsonConvert.DeserializeObject<ObservedProperty>(data);

            var service = new ObservedPropertiesService(RepoFactory);
            property = await service.AddObservedProperty(property);

            property.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return property;
        }

        [Route(HttpVerbs.Get, "/ObservedProperties({id})")]
        public async Task<ObservedProperty> GetObservedPropertyAsync(int id)
        {
            var service = new ObservedPropertiesService(RepoFactory);
            var property = await service.GetObservedPropertyById(id);
            property.BaseUrl = GetBaseUrl();

            return property;
        }

        [Route(HttpVerbs.Get, "/ObservedProperties")]
        public async Task<Listing<ObservedProperty>> GetObservedPropertiesAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new ObservedPropertiesService(RepoFactory);
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
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new ObservedPropertiesService(RepoFactory);
            await service.UpdateObservedProperty(updates, id);
        }

        [Route(HttpVerbs.Delete, "/ObservedProperties({id})")]
        public async Task RemoveObservedPropertyAsync(int id)
        {
            var service = new ObservedPropertiesService(RepoFactory);
            await service.RemoveObservedProperty(id);
        }
    }
}
