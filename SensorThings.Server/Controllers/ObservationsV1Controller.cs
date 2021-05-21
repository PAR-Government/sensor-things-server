using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using SensorThings.Entities;

namespace SensorThings.Server.Controllers
{
    public class ObservationsV1Controller : WebApiController
    {
        [Route(HttpVerbs.Post, "/Observations")]
        public async Task<string> CreateObservation()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var observation = JsonConvert.DeserializeObject<Observation>(data);

            return $"name: {observation.PhenomenonTime}";
        }

        [Route(HttpVerbs.Get, "/Observations({id})")]
        public string GetObservation(int id)
        {
            return $"Observation with id: {id}";
        }
    }
}
