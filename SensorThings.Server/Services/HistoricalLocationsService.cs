using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class HistoricalLocationsService : IHistoricalLocationsService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public HistoricalLocationsService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<HistoricalLocation> AddHistoricalLocation(HistoricalLocation location)
        {
            var id = await UOW.HistoricalLocationsRepository.AddAsync(location);

            location.ID = id;

            return location;
        }

        public async Task<HistoricalLocation> GetHistoricalLocationById(int id)
        {
            return await UOW.HistoricalLocationsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<HistoricalLocation>> GetHistoricalLocations()
        {
            return await UOW.HistoricalLocationsRepository.GetAllAsync();
        }

        public async Task<HistoricalLocation> UpdateHistoricalLocation(JObject updates, int id)
        {
            var location = await GetHistoricalLocationById(id);

            var locationJson = JObject.FromObject(location);
            foreach (var property in updates.Properties())
            {
                locationJson[property.Name] = property.Value;
            }

            location = locationJson.ToObject<HistoricalLocation>();

            await UOW.HistoricalLocationsRepository.UpdateAsync(location);

            return location;
        }

        public async Task RemoveHistoricalLocation(int id)
        {
            await UOW.HistoricalLocationsRepository.Remove(id);
        }

        public async Task LinkLocation(long historicalLocationId, long locationId)
        {
            await UOW.HistoricalLocationsRepository.LinkLocationAsync(historicalLocationId, locationId);
        }

        public async Task<IEnumerable<Location>> GetLinkedLocations(long id)
        {
            return await UOW.HistoricalLocationsRepository.GetLinkedLocations(id);
        }

        public async Task UnlinkLocation(long historicalLocationId, long locationId)
        {
            await UOW.HistoricalLocationsRepository.UnlinkLocationAsync(historicalLocationId, locationId);
        }

        public async Task UnlinkLocations(long historicalLocationId)
        {
            await UOW.HistoricalLocationsRepository.UnlinkLocationsAsync(historicalLocationId);
        }
    }
}
