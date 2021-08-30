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

            var service = new DatastreamsService(uow);
            datastream = await service.AddDatastream(datastream);
            datastream.BaseUrl = GetBaseUrl();
            uow.Commit();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return datastream;
        }

        [Route(HttpVerbs.Get, "/Datastreams")]
        public async Task<Listing<Datastream>> GetDatastreamsAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new DatastreamsService(uow);
            var datastreams = await service.GetDatastreams();
            var baseUrl = GetBaseUrl();

            foreach(var datastream in datastreams)
            {
                datastream.BaseUrl = baseUrl;
            }

            var listing = new Listing<Datastream> { Items = datastreams.ToList() };
            return listing;
        }

        [Route(HttpVerbs.Get, "/Datastreams({id})")]
        public async Task<Datastream> GetDatastreamAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new DatastreamsService(uow);
            var datastream = await service.GetDatastreamById(id);

            datastream.BaseUrl = GetBaseUrl();

            return datastream;
        }

        [Route(HttpVerbs.Patch, "/Datastreams({id})")]
        public async Task UpdateDatastreamAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new DatastreamsService(uow);
            await service.UpdateDatastream(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Delete, "/Datastreams({id})")]
        public async Task RemoveDatastreamAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new DatastreamsService(uow);
            await service.RemoveDatastream(id);
            uow.Commit();
        }
    }
}
