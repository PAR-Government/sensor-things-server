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
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var location = JsonConvert.DeserializeObject<Location>(data);

            var service = new LocationsService(RepoFactory);
            location = await service.AddLocation(location);

            location.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return location;
        }

        [Route(HttpVerbs.Get, "/Locations({id})")]
        public async Task<Location> GetLocationAsync(int id)
        {
            var service = new LocationsService(RepoFactory);
            var location = await service.GetLocationById(id);
            location.BaseUrl = GetBaseUrl();

            return location;
        }

        [Route(HttpVerbs.Patch, "/Locations({id})")]
        public async Task UpdateLocationAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new LocationsService(RepoFactory);
            await service.UpdateLocation(updates, id);
        }

        [Route(HttpVerbs.Get, "/Locations")]
        public async Task<Listing<Location>> GetLocationsAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new LocationsService(RepoFactory);
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
            var service = new LocationsService(RepoFactory);
            await service.RemoveLocation(id);
        }
    }
}
