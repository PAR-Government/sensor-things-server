using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public interface IHistoricalLocationsRepository : IRepository<HistoricalLocation>
    {
        public Task<IEnumerable<Location>> GetLinkedLocations(long id);

        public Task LinkLocationAsync(long historicalLocationId, long locationId);

        public Task UnlinkLocationAsync(long historicalLocationId, long locationId);

        public Task UnlinkLocationsAsync(long historicalLocationId);
    }
}
