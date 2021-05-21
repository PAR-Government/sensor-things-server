using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using SensorThings.Entities;

namespace SensorThings.Server.Controllers
{
    public class SensorsV1Controller : WebApiController
    {
        [Route(HttpVerbs.Post, "/Sensors")]
        public async Task<string> CreateSensor()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var sensor = JsonConvert.DeserializeObject<Sensor>(data);

            return $"name: {sensor.Name}, description: {sensor.Description}";
        }

        [Route(HttpVerbs.Get, "/Sensors({id})")]
        public string GetSensor(int id)
        {
            return $"Sensor with id: {id}";
        }
    }
}
