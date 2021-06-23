using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SensorThings.Server.Services
{
    public class ThingsService
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public ThingsService(IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
        }

        public async Task<Thing> AddThing(Thing thing)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var id = await uow.ThingsRepository.AddAsync(thing);
            uow.Commit();

            thing.ID = id;

            return thing;
        }

        public async Task<Thing> GetThingById(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ThingsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Thing>> GetThings()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ThingsRepository.GetAllAsync();
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

            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.UpdateAsync(thing);
            uow.Commit();

            return thing;
        }

        public async Task RemoveThing(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.RemoveLocationLinksAsync(id);
            await uow.ThingsRepository.Remove(id);
            uow.Commit();
        }

        public async Task<IEnumerable<Location>> GetLocationsAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ThingsRepository.GetLinkedLocations(id);
        }

        public async Task AssociateLocation(long thingId, long locationId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.AddLocationLinkAsync(thingId, locationId);
            uow.Commit();
        }

        public async Task<IEnumerable<Location>> GetAssociatedLocations(long thingId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ThingsRepository.GetLinkedLocations(thingId);
        }

        public async Task UnassociateAllLocations(long thingId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.RemoveLocationLinksAsync(thingId);
            uow.Commit();
        }

        public async Task UnassociateLocation(long thingId, long locationId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.RemoveLocationLinkAsync(thingId, locationId);
            uow.Commit();
        }

        public async Task AssociateHistoricalLocation(long thingId, long historicalLocationId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.AddHistoricalLocationLinkAsync(thingId, historicalLocationId);
            uow.Commit();
        }

        public async Task<IEnumerable<HistoricalLocation>> GetAssociatedHistoricalLocations(long thingId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ThingsRepository.GetLinkedHistoricalLocationsAsync(thingId);
        }

        public async Task UnassociateHistoricalLocations(long thingId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.RemoveHistoricalLocationLinksAsync(thingId);
            uow.Commit();
        }

        public async Task UnassociateHistoricalLocation(long thingId, long historicalLocationId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.RemoveHistoricalLocationLinkAsync(thingId, historicalLocationId);
            uow.Commit();
        }

        public async Task AssociateDatastreamAsync(long thingId, long datastreamId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.AddDatastreamLinkAsync(thingId, datastreamId);
            uow.Commit();
        }

        public async Task<IEnumerable<Datastream>> GetAssociatedDatastreamsAsync(long thingId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            return await uow.ThingsRepository.GetLinkedDatastreamsAsync(thingId);
        }

        public async Task UnassociateDatastreams(long thingId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.RemoveDatastreamLinksAsync(thingId);
            uow.Commit();
        }

        public async Task UnassociateDatastream(long thingId, long datastreamId)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            await uow.ThingsRepository.RemoveDatastreamLinkAsync(thingId, datastreamId);
            uow.Commit();
        }
    }
}
