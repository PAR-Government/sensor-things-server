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
        public async Task<string> CreateLocationAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var location = JsonConvert.DeserializeObject<Location>(data);

            var service = new LocationsService(RepoFactory);
            location = await service.AddLocation(location);

            location.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return JsonConvert.SerializeObject(location);
        }

        [Route(HttpVerbs.Get, "/Locations({id})")]
        public async Task<string> GetLocationAsync(int id)
        {
            var service = new LocationsService(RepoFactory);
            var location = await service.GetLocationById(id);
            location.BaseUrl = GetBaseUrl();

            return JsonConvert.SerializeObject(location);
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
        public async Task<string> GetLocationsAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new LocationsService(RepoFactory);
            var locations = await service.GetLocations();

            foreach (var location in locations)
            {
                location.BaseUrl = baseUrl;
            }

            var listing = new Listing<Location>() { Items = locations.ToList() };

            return JsonConvert.SerializeObject(listing);
        }
    }
}
