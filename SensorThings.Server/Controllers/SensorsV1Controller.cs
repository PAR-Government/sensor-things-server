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
    public class SensorsV1Controller : BaseController
    {
        public SensorsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/Sensors")]
        public async Task<Sensor> CreateSensorAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var sensor = JsonConvert.DeserializeObject<Sensor>(data);

            var service = new SensorsService(RepoFactory);
            sensor = await service.AddSensor(sensor);

            sensor.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return sensor;
        }

        [Route(HttpVerbs.Get, "/Sensors({id})")]
        public async Task<Sensor> GetSensorAsync(int id)
        {
            var service = new SensorsService(RepoFactory);
            var sensor = await service.GetSensorById(id);
            sensor.BaseUrl = GetBaseUrl();

            return sensor;
        }

        [Route(HttpVerbs.Patch, "/Sensors({id})")]
        public async Task UpdateSensorAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new SensorsService(RepoFactory);
            await service.UpdateSensor(updates, id);
        }

        [Route(HttpVerbs.Get, "/Sensors")]
        public async Task<Listing<Sensor>> GetSensorsAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new SensorsService(RepoFactory);
            var sensors = await service.GetSensors();

            foreach (var sensor in sensors)
            {
                sensor.BaseUrl = baseUrl;
            }

            var listing = new Listing<Sensor>() { Items = sensors.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Delete, "/Sensors({id})")]
        public async Task RemoveSensorAsync(int id)
        {
            var service = new SensorsService(RepoFactory);
            await service.RemoveSensor(id);
        }
    }
}
