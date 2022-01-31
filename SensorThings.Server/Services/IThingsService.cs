using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;

namespace SensorThings.Server.Services
{
    public interface IThingsService
    {
        Task<Thing> AddThing(Thing thing);
        Task AssociateDatastreamAsync(long thingId, long datastreamId);
        Task AssociateHistoricalLocation(long thingId, long historicalLocationId);
        Task AssociateLocation(long thingId, long locationId);
        Task<IEnumerable<Datastream>> GetAssociatedDatastreamsAsync(long thingId);
        Task<IEnumerable<HistoricalLocation>> GetAssociatedHistoricalLocations(long thingId);
        Task<IEnumerable<Location>> GetAssociatedLocations(long thingId);
        Task<IEnumerable<Location>> GetLocationsAsync(int id);
        Task<Thing> GetThingById(int id);
        Task<IEnumerable<Thing>> GetThings();
        Task RemoveThing(int id);
        Task UnassociateAllLocations(long thingId);
        Task UnassociateDatastream(long thingId, long datastreamId);
        Task UnassociateDatastreams(long thingId);
        Task UnassociateHistoricalLocation(long thingId, long historicalLocationId);
        Task UnassociateHistoricalLocations(long thingId);
        Task UnassociateLocation(long thingId, long locationId);
        Task<Thing> UpdateThing(JObject updates, int id);
    }
}