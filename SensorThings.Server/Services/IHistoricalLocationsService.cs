using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface IHistoricalLocationsService
    {
        Task<HistoricalLocation> AddHistoricalLocation(HistoricalLocation location);
        Task<HistoricalLocation> GetHistoricalLocationById(int id);
        Task<IEnumerable<HistoricalLocation>> GetHistoricalLocations();
        Task<IEnumerable<Location>> GetLinkedLocations(long id);
        Task LinkLocation(long historicalLocationId, long locationId);
        Task RemoveHistoricalLocation(int id);
        Task UnlinkLocation(long historicalLocationId, long locationId);
        Task UnlinkLocations(long historicalLocationId);
        Task<HistoricalLocation> UpdateHistoricalLocation(JObject updates, int id);
    }
}