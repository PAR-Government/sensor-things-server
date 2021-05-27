using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;

namespace SensorThings.Server.Controllers
{
    public class DatastreamsV1Controller : BaseController
    {
        public DatastreamsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/Datastreams")]
        public async Task<string> CreateDatastreamAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var datastream = JsonConvert.DeserializeObject<Datastream>(data);

            var service = new DatastreamsService(RepoFactory);
            datastream = await service.AddDatastream(datastream);
            datastream.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return JsonConvert.SerializeObject(datastream);
        }

        [Route(HttpVerbs.Get, "/Datastreams")]
        public async Task<string> GetDatastreamsAsync()
        {
            var service = new DatastreamsService(RepoFactory);
            var datastreams = await service.GetDatastreams();
            var baseUrl = GetBaseUrl();

            foreach(var datastream in datastreams)
            {
                datastream.BaseUrl = baseUrl;
            }

            var listing = new Listing<Datastream> { Items = datastreams.ToList() };
            return JsonConvert.SerializeObject(listing);
        }

        [Route(HttpVerbs.Get, "/Datastreams({id})")]
        public async Task<String> GetDatastreamAsync(int id)
        {
            var service = new DatastreamsService(RepoFactory);
            var datastream = await service.GetDatastreamById(id);

            datastream.BaseUrl = GetBaseUrl();

            return JsonConvert.SerializeObject(datastream);
        }

        [Route(HttpVerbs.Delete, "/Datastreams({id})")]
        public async Task RemoveDatastreamAsync(int id)
        {
            var service = new DatastreamsService(RepoFactory);
            await service.RemoveDatastream(id);
        }
    }
}
