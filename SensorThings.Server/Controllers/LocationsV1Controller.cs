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
    public class LocationsV1Controller : BaseController
    {
        public LocationsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/Locations")]
        public async Task<Location> CreateLocationAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var location = JsonConvert.DeserializeObject<Location>(data);

            var service = uow.LocationsService;
            location = await service.AddLocation(location);

            location.BaseUrl = GetBaseUrl();

            uow.Commit();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return location;
        }

        [Route(HttpVerbs.Get, "/Locations({id})")]
        public async Task<Location> GetLocationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.LocationsService;
            var location = await service.GetLocationById(id);
            location.BaseUrl = GetBaseUrl();

            return location;
        }

        [Route(HttpVerbs.Patch, "/Locations({id})")]
        public async Task UpdateLocationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = uow.LocationsService;
            await service.UpdateLocation(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Get, "/Locations")]
        public async Task<Listing<Location>> GetLocationsAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var baseUrl = GetBaseUrl();
            var service = uow.LocationsService;
            var locations = await service.GetLocations();

            foreach (var location in locations)
            {
                location.BaseUrl = baseUrl;
            }

            var listing = new Listing<Location>() { Items = locations.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Delete, "/Locations({id})")]
        public async Task RemoveLocationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.LocationsService;
            await service.RemoveLocation(id);
            uow.Commit();
        }
    }
}
