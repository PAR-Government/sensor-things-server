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
using SensorThings.Server.Services;

namespace SensorThings.Server.Controllers
{
    public class DatastreamsV1Controller : BaseController
    {
        public DatastreamsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/Datastreams")]
        public async Task<Datastream> CreateDatastreamAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var datastream = JsonConvert.DeserializeObject<Datastream>(data);
            var thing = datastream.Thing;

            var service = uow.DatastreamsService;
            var thingService = uow.ThingsService;

            datastream = await service.AddDatastream(datastream);

            if (thing == null)
            {
                // TODO: Inform user that invariant wasn't met, DS must have a Thing
            }
            else
            {
                await thingService.AssociateDatastreamAsync(thing.ID, datastream.ID);
            }

            datastream.BaseUrl = GetBaseUrl();
            uow.Commit();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return datastream;
        }

        // TODO: Add support for expand attribute
        [Route(HttpVerbs.Get, "/Datastreams")]
        public async Task<Listing<Datastream>> GetDatastreamsAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.DatastreamsService;
            var datastreams = await service.GetDatastreams();
            var baseUrl = GetBaseUrl();

            foreach(var datastream in datastreams)
            {
                datastream.BaseUrl = baseUrl;

                try
                {
                    var thing = await service.GetLinkedThing(datastream.ID);
                    thing.BaseUrl = baseUrl;
                    datastream.Thing = thing;
                }
                catch (Exception) { }
            }

            var listing = new Listing<Datastream> { Items = datastreams.ToList() };
            return listing;
        }

        // TODO: Add support for expand attribute
        [Route(HttpVerbs.Get, "/Datastreams({id})")]
        public async Task<Datastream> GetDatastreamAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.DatastreamsService;

            var datastream = await service.GetDatastreamById(id);
            datastream.BaseUrl = GetBaseUrl();

            try
            {
                var thing = await service.GetLinkedThing(id);
                thing.BaseUrl = GetBaseUrl();
                datastream.Thing = thing;
            }
            catch (Exception) { }

            return datastream;
        }

        [Route(HttpVerbs.Patch, "/Datastreams({id})")]
        public async Task UpdateDatastreamAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = uow.DatastreamsService;
            await service.UpdateDatastream(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Delete, "/Datastreams({id})")]
        public async Task RemoveDatastreamAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = uow.DatastreamsService;
            await service.RemoveDatastream(id);
            uow.Commit();
        }

        [Route(HttpVerbs.Get, "/Datastreams({id})/Observations")]
        public async Task<Listing<Observation>> GetObservationsForDatastreamAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var baseUrl = GetBaseUrl();
            var service = uow.DatastreamsService;
            var observations = await service.GetLinkedObservations(id);

            foreach (Observation o in observations)
            {
                o.BaseUrl = baseUrl;
            }

            var listing = new Listing<Observation> { Items = observations.ToList() };

            return listing;
        }
    }
}
