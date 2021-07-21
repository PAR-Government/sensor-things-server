using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SensorThings.Server.Controllers
{
    public class HistoricalLocationsV1Controller : BaseController
    {
        public HistoricalLocationsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Get, "/HistoricalLocations({id})")]
        public async Task<HistoricalLocation> GetHistoricalLocationAsync(int id)
        {
            var service = new HistoricalLocationsService(RepoFactory);
            var location = await service.GetHistoricalLocationById(id);
            location.BaseUrl = GetBaseUrl();

            return location;
        }

        [Route(HttpVerbs.Get, "/HistoricalLocations")]
        public async Task<Listing<HistoricalLocation>> GetHistoricalLocationsAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new HistoricalLocationsService(RepoFactory);
            var locations = await service.GetHistoricalLocations();

            foreach (var location in locations)
            {
                location.BaseUrl = baseUrl;
            }

            var listing = new Listing<HistoricalLocation>() { Items = locations.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Patch, "/HistoricalLocations({id})")]
        public async Task UpdateHistoricalLocationAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new HistoricalLocationsService(RepoFactory);
            await service.UpdateHistoricalLocation(updates, id);
        }

        [Route(HttpVerbs.Delete, "/HistoricalLocations({id})")]
        public async Task RemoveHistoricalLocationAsync(int id)
        {
            var service = new HistoricalLocationsService(RepoFactory);
            await service.RemoveHistoricalLocation(id);
        }
    }
}
