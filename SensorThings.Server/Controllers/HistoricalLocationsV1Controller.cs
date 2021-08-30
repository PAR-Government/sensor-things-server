using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
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
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new HistoricalLocationsService(uow);
            var location = await service.GetHistoricalLocationById(id);
            location.BaseUrl = GetBaseUrl();

            return location;
        }

        [Route(HttpVerbs.Get, "/HistoricalLocations")]
        public async Task<Listing<HistoricalLocation>> GetHistoricalLocationsAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var baseUrl = GetBaseUrl();
            var service = new HistoricalLocationsService(uow);
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
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new HistoricalLocationsService(uow);
            await service.UpdateHistoricalLocation(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Delete, "/HistoricalLocations({id})")]
        public async Task RemoveHistoricalLocationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new HistoricalLocationsService(uow);
            await service.RemoveHistoricalLocation(id);
            uow.Commit();
        }
    }
}
