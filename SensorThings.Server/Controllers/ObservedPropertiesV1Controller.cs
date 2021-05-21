using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using SensorThings.Entities;
using SensorThings.Server.Repositories;

namespace SensorThings.Server.Controllers
{
    public class ObservedPropertiesV1Controller : BaseController
    {
        public ObservedPropertiesV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/ObservedProperties")]
        public async Task<string> CreateObservedProperty()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var property = JsonConvert.DeserializeObject<ObservedProperty>(data);

            return $"name: {property.Name}, definition: {property.Definition}, description: {property.Description}";
        }

        [Route(HttpVerbs.Get, "/ObservedProperties({id})")]
        public string GetObservedProperty(int id)
        {
            return $"ObservedProperty with id: {id}";
        }
    }
}
