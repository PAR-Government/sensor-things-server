using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using SensorThings.Entities;

namespace SensorThings.Server.Controllers
{
    public class ObservedPropertiesV1Controller : WebApiController
    {
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
