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
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var sensor = JsonConvert.DeserializeObject<Sensor>(data);

            var service = new SensorsService(uow);
            sensor = await service.AddSensor(sensor);
            sensor.BaseUrl = GetBaseUrl();

            uow.Commit();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return sensor;
        }

        [Route(HttpVerbs.Get, "/Sensors({id})")]
        public async Task<Sensor> GetSensorAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new SensorsService(uow);
            var sensor = await service.GetSensorById(id);
            sensor.BaseUrl = GetBaseUrl();

            return sensor;
        }

        [Route(HttpVerbs.Patch, "/Sensors({id})")]
        public async Task UpdateSensorAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new SensorsService(uow);
            await service.UpdateSensor(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Get, "/Sensors")]
        public async Task<Listing<Sensor>> GetSensorsAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var baseUrl = GetBaseUrl();
            var service = new SensorsService(uow);
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
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new SensorsService(uow);
            await service.RemoveSensor(id);
            uow.Commit();
        }
    }
}
