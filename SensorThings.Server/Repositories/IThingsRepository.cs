using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public interface IThingsRepository : IRepository<Thing>
    {
        public Task<IEnumerable<Location>> GetLinkedLocations(long thingId);

        public Task AddLocationLinkAsync(Thing thing, Location location);
        
        public Task AddLocationLinkAsync(long thingId, long locationId);

        public Task RemoveLocationLinkAsync(long thingId, long locationId);

        public Task RemoveLocationLinksAsync(long thingId);
    }
}
