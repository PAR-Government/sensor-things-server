﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using SensorThings.Server.Utils;

namespace SensorThings.Server.Controllers
{
    public class ThingsV1Controller : BaseController
    {
        public ThingsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) {}

        [Route(HttpVerbs.Post, "/Things")]
        public async Task<Thing> CreateThingAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var thing = JsonConvert.DeserializeObject<Thing>(data);

            var service = new ThingsService(RepoFactory);
            thing = await service.AddThing(thing);

            thing.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int) HttpStatusCode.Created;
            //return JsonConvert.SerializeObject(thing);
            return thing;
        }

        [Route(HttpVerbs.Get, "/Things")]
        public async Task<Listing<Thing>> GetThingsAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new ThingsService(RepoFactory);
            var things = await service.GetThings();

            foreach (var thing in things)
            {
                thing.BaseUrl = baseUrl;
            }

            var listing = new Listing<Thing>() { Items = things.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Get, "/Things({id})")]
        public async Task<Thing> GetThingAsync(int id)
        {
            var service = new ThingsService(RepoFactory);
            var thing = await service.GetThingById(id);

            if (thing == null) 
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return null;
            }

            thing.BaseUrl = GetBaseUrl();

            return thing;
        }

        [Route(HttpVerbs.Get, "/Things({id})/Locations")]
        public async Task<Listing<Location>> GetLocationsForThingAsync(int id)
        {
            var baseUrl = GetBaseUrl();
            var service = new ThingsService(RepoFactory);
            var locations = await service.GetLocationsAsync(id);

            foreach (var location in locations)
            {
                location.BaseUrl = baseUrl;
            }

            var listing = new Listing<Location> { Items = locations.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Patch, "/Things({id})")]
        public async Task UpdateThingAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new ThingsService(RepoFactory);
            await service.UpdateThing(updates, id);
        }

        [Route(HttpVerbs.Delete, "/Things({id})")]
        public async Task RemoveThingAsync(int id)
        {
            var service = new ThingsService(RepoFactory);
            await service.RemoveThing(id);
        }

        [Route(HttpVerbs.Get, "/Things({id})/Datastreams")]
        public async Task<Listing<Datastream>> GetDatastreamsAsync(int id)
        {
            var service = new ThingsService(RepoFactory);
            var datastreams = await service.GetAssociatedDatastreamsAsync(id);
            var listing = new Listing<Datastream> { Items = datastreams.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Get, "/Things({id})/Locations")]
        public async Task<Listing<Location>> GetLocationsAsync(int id)
        {
            var service = new ThingsService(RepoFactory);
            var locations = await service.GetAssociatedLocations(id);
            var listing = new Listing<Location> { Items = locations.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Get, "/Things({id})/HistoricalLocations")]
        public async Task<Listing<HistoricalLocation>> GetHistoricalLocationsAsync(int id)
        {
            var service = new ThingsService(RepoFactory);
            var locations = await service.GetAssociatedHistoricalLocations(id);
            var listing = new Listing<HistoricalLocation> { Items = locations.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Get, "/Things({id})/{propertyName}")]
        public async Task<JObject> GetThingPropertyAsync(int id, string propertyName)
        {
            var service = new ThingsService(RepoFactory);
            var thing = await service.GetThingById(id);

            var jsonThing = JObject.FromObject(thing);

            var propertyValue = jsonThing.GetValue(propertyName);

            if (propertyValue == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            var resultObject = new JObject();
            resultObject.Add(propertyName, propertyValue);

            return resultObject;
        }

        [Route(HttpVerbs.Get, "/Things({id})/{propertyName}/$value")]
        public async Task<object> GetThingPropertyValueAsync(int id, string propertyName)
        {
            var service = new ThingsService(RepoFactory);
            var thing = await service.GetThingById(id);

            var jsonThing = JObject.FromObject(thing);

            var propertyValue = jsonThing.GetValue(propertyName);

            if (propertyValue == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return null;
            }

            return propertyValue;
        }
    }
}
