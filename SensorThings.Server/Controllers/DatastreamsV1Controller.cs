using System;
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
        public async Task<string> CreateDatastream()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var datastream = JsonConvert.DeserializeObject<Datastream>(data);

            var service = new DatastreamsService(RepoFactory);
            datastream = await service.AddDatastream(datastream);
            datastream.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return JsonConvert.SerializeObject(datastream);
        }

        [Route(HttpVerbs.Get, "/Datastreams({id})")]
        public string GetDatastream(int id)
        {
            return $"Datastream with id: {id}";
        }
    }
}
