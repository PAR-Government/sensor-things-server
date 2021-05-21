using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Utils;
using Swan;

namespace SensorThings.Server.Controllers
{
    public class ThingsV1Controller : BaseController
    {
        [Route(HttpVerbs.Post, "/Things")]
        public async Task<string> CreateThingAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var thing = JsonConvert.DeserializeObject<Thing>(data);

            thing.BaseUrl = GetBaseUrl(HttpContext);

            using var uow = new RepositoryUnitOfWork();
            var id = await uow.ThingsRepository.AddAsync(thing);
            uow.Commit();

            thing.ID = id;
            Response.StatusCode = (int) HttpStatusCode.Created;
            return JsonConvert.SerializeObject(thing);
        }

        [Route(HttpVerbs.Get, "/Things")]
        public async Task<string> GetThingsAsync()
        {
            var baseUrl = GetBaseUrl(HttpContext);

            using var uow = new RepositoryUnitOfWork();
            var things = await uow.ThingsRepository.GetAllAsync();

            foreach (var thing in things)
            {
                thing.BaseUrl = baseUrl;
            }

            var listing = new Listing<Thing>() { Items = things.ToList() };

            return JsonConvert.SerializeObject(listing);
        }

        [Route(HttpVerbs.Get, "/Things({id})")]
        public async Task<string> GetThingAsync(int id)
        {
            using var uow = new RepositoryUnitOfWork();
            var thing = await uow.ThingsRepository.GetByIdAsync(id);
            thing.BaseUrl = GetBaseUrl(HttpContext);

            return ThingsSerializer.Serialize(thing, null);
        }

        [Route(HttpVerbs.Get, "/Things({id})/Locations")]
        public async Task<string> GetLocationsForThingAsync(int id)
        {
            using var uow = new RepositoryUnitOfWork();
            var locations = await uow.ThingsRepository.GetLinkedLocations(id);
            var baseUrl = GetBaseUrl(HttpContext);

            foreach (var location in locations)
            {
                location.BaseUrl = baseUrl;
            }

            var listing = new Listing<Location> { Items = locations.ToList() };

            return JsonConvert.SerializeObject(listing);
        }

        [Route(HttpVerbs.Patch, "/Things({id})")]
        public async Task UpdateThingAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            using var uow = new RepositoryUnitOfWork();
            var thing = await uow.ThingsRepository.GetByIdAsync(id);

            // Convert the Thing to JSON to make it easier to merge the updates
            var thingJson = JObject.FromObject(thing);
            foreach (var property in updates.Properties())
            {
                thingJson[property.Name] = property.Value;
            }

            // Go back to an actual Thing instance
            thing = thingJson.ToObject<Thing>();

            await uow.ThingsRepository.UpdateAsync(thing);
            uow.Commit();
        }

        [Route(HttpVerbs.Delete, "/Things({id})")]
        public async Task RemoveThingAsync(int id)
        {
            using var uow = new RepositoryUnitOfWork();
            await uow.ThingsRepository.RemoveLocationLinksAsync(id);
            await uow.ThingsRepository.Remove(id);
            uow.Commit();
        }
    }
}
