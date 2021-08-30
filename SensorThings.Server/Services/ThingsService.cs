using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class ThingsService
    {
        protected IRepositoryUnitOfWork UOW { get; private set; }

        public ThingsService(IRepositoryUnitOfWork uow)
        {
            UOW = uow;
        }

        public async Task<Thing> AddThing(Thing thing)
        {
            var id = await UOW.ThingsRepository.AddAsync(thing);
            thing.ID = id;

            return thing;
        }

        public async Task<Thing> GetThingById(int id)
        {
            return await UOW.ThingsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Thing>> GetThings()
        {
            return await UOW.ThingsRepository.GetAllAsync();
        }

        public async Task<Thing> UpdateThing(JObject updates, int id)
        {
            var thing = await GetThingById(id);

            // Convert the Thing to JSON to make it easier to merge the updates
            var thingJson = JObject.FromObject(thing);
            foreach (var property in updates.Properties())
            {
                thingJson[property.Name] = property.Value;
            }

            // Go back to an actual Thing instance
            thing = thingJson.ToObject<Thing>();

            await UOW.ThingsRepository.UpdateAsync(thing);

            return thing;
        }

        public async Task RemoveThing(int id)
        {
            await UOW.ThingsRepository.RemoveLocationLinksAsync(id);
            await UOW.ThingsRepository.Remove(id);
        }

        public async Task<IEnumerable<Location>> GetLocationsAsync(int id)
        {
            return await UOW.ThingsRepository.GetLinkedLocations(id);
        }

        public async Task AssociateLocation(long thingId, long locationId)
        {
            await UOW.ThingsRepository.AddLocationLinkAsync(thingId, locationId);
        }

        public async Task<IEnumerable<Location>> GetAssociatedLocations(long thingId)
        {
            return await UOW.ThingsRepository.GetLinkedLocations(thingId);
        }

        public async Task UnassociateAllLocations(long thingId)
        {
            await UOW.ThingsRepository.RemoveLocationLinksAsync(thingId);
        }

        public async Task UnassociateLocation(long thingId, long locationId)
        {
            await UOW.ThingsRepository.RemoveLocationLinkAsync(thingId, locationId);
        }

        public async Task AssociateHistoricalLocation(long thingId, long historicalLocationId)
        {
            await UOW.ThingsRepository.AddHistoricalLocationLinkAsync(thingId, historicalLocationId);
        }

        public async Task<IEnumerable<HistoricalLocation>> GetAssociatedHistoricalLocations(long thingId)
        {
            return await UOW.ThingsRepository.GetLinkedHistoricalLocationsAsync(thingId);
        }

        public async Task UnassociateHistoricalLocations(long thingId)
        {
            await UOW.ThingsRepository.RemoveHistoricalLocationLinksAsync(thingId);
        }

        public async Task UnassociateHistoricalLocation(long thingId, long historicalLocationId)
        {
            await UOW.ThingsRepository.RemoveHistoricalLocationLinkAsync(thingId, historicalLocationId);
        }

        public async Task AssociateDatastreamAsync(long thingId, long datastreamId)
        {
            await UOW.ThingsRepository.AddDatastreamLinkAsync(thingId, datastreamId);
        }

        public async Task<IEnumerable<Datastream>> GetAssociatedDatastreamsAsync(long thingId)
        {
            return await UOW.ThingsRepository.GetLinkedDatastreamsAsync(thingId);
        }

        public async Task UnassociateDatastreams(long thingId)
        {
            await UOW.ThingsRepository.RemoveDatastreamLinksAsync(thingId);
        }

        public async Task UnassociateDatastream(long thingId, long datastreamId)
        {
            await UOW.ThingsRepository.RemoveDatastreamLinkAsync(thingId, datastreamId);
        }
    }
}
