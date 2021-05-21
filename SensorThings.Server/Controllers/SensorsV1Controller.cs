using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using SensorThings.Entities;
using SensorThings.Server.Repositories;

namespace SensorThings.Server.Controllers
{
    public class SensorsV1Controller : BaseController
    {
        public SensorsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

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
