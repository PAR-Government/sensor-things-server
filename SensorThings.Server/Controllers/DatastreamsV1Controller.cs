using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using SensorThings.Entities;
using SensorThings.Server.Repositories;

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

            return $"name: {datastream.Name}, description: {datastream.Description}";
        }

        [Route(HttpVerbs.Get, "/Datastreams({id})")]
        public string GetDatastream(int id)
        {
            return $"Datastream with id: {id}";
        }
    }
}
