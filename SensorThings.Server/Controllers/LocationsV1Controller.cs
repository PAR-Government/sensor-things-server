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

namespace SensorThings.Server.Controllers
{
    public class LocationsV1Controller : BaseController
    {
        public LocationsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/Locations")]
        public async Task<string> CreateLocationAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var location = JsonConvert.DeserializeObject<Location>(data);
            location.BaseUrl = GetBaseUrl(HttpContext);

            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.LocationsRepository.AddAsync(location);
            uow.Commit();

            location.ID = id;
            Response.StatusCode = (int)HttpStatusCode.Created;

            return JsonConvert.SerializeObject(location);
        }

        [Route(HttpVerbs.Get, "/Locations({id})")]
        public async Task<string> GetLocationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var location = await uow.LocationsRepository.GetByIdAsync(id);
            location.BaseUrl = GetBaseUrl(HttpContext);

            return JsonConvert.SerializeObject(location);
        }

        [Route(HttpVerbs.Patch, "/Locations({id})")]
        public async Task UpdateLocationAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            using var uow = RepoFactory.CreateUnitOfWork();
            var location = await uow.LocationsRepository.GetByIdAsync(id);

            // Convert the Location to JSON to make it easier to merge the updates
            var locationJson = JObject.FromObject(location);
            foreach (var property in updates.Properties())
            {
                locationJson[property.Name] = property.Value;
            }

            // Go back to an actual Thing instance
            location = locationJson.ToObject<Location>();

            await uow.LocationsRepository.UpdateAsync(location);
            uow.Commit();
        }

        [Route(HttpVerbs.Get, "/Locations")]
        public async Task<string> GetLocationsAsync()
        {
            var baseUrl = GetBaseUrl(HttpContext);

            using var uow = RepoFactory.CreateUnitOfWork();
            var locations = await uow.LocationsRepository.GetAllAsync();

            foreach (var location in locations)
            {
                location.BaseUrl = baseUrl;
            }

            var listing = new Listing<Location>() { Items = locations.ToList() };

            return JsonConvert.SerializeObject(listing);
        }
    }
}
