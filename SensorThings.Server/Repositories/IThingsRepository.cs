﻿using SensorThings.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorThings.Server.Repositories
{
    public interface IThingsRepository : IRepository<Thing>
    {
        public Task<IEnumerable<Location>> GetLinkedLocations(long thingId);
        
        public Task AddLocationLinkAsync(long thingId, long locationId);

        public Task RemoveLocationLinkAsync(long thingId, long locationId);

        public Task RemoveLocationLinksAsync(long thingId);

        public Task<IEnumerable<HistoricalLocation>> GetLinkedHistoricalLocationsAsync(long thingId);

        public Task AddHistoricalLocationLinkAsync(long thingId, long historicalLocationId);

        public Task RemoveHistoricalLocationLinkAsync(long thingId, long historicalLocationId);

        public Task RemoveHistoricalLocationLinksAsync(long thingId);

        public Task<IEnumerable<Datastream>> GetLinkedDatastreamsAsync(long thingId);

        public Task AddDatastreamLinkAsync(long thingId, long datastreamId);

        public Task RemoveDatastreamLinkAsync(long thingId, long datastreamId);

        public Task RemoveDatastreamLinksAsync(long thingId);

        public Task<Thing> GetLinkedThingForDatastream(long datastreamId);
    }
}
