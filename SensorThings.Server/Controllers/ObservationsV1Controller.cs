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
using TinyIoC;

namespace SensorThings.Server.Controllers
{
    public class ObservationsV1Controller : BaseController
    {
        private readonly IMqttService mqttService;

        public ObservationsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) 
        {
            mqttService = TinyIoCContainer.Current.Resolve<IMqttService>();
        }

        [Route(HttpVerbs.Post, "/Observations")]
        public async Task<Observation> CreateObservationAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var observation = JsonConvert.DeserializeObject<Observation>(data);
            var ds = observation.Datastream;

            if (ds == null)
            {
                var errorDoc = new JObject
                {
                    { "error", "Missing datastream link" }
                };

                Response.ContentType = MimeType.Json;

                throw HttpException.BadRequest("Missing Datastream link", errorDoc);
            }

            var service = new ObservationsService(uow);
            observation = await service.AddObservation(observation, ds.ID);
            observation.BaseUrl = GetBaseUrl();
            ds.BaseUrl = GetBaseUrl();

            uow.Commit();

            Response.StatusCode = (int)HttpStatusCode.Created;

            if (mqttService != null)
            {
                await mqttService?.PublishObservationAsync(observation);
            }

            return observation;
        }

        [Route(HttpVerbs.Get, "/Observations({id})")]
        public async Task<Observation> GetObservationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new ObservationsService(uow);
            var observation = await service.GetObservationById(id);
            observation.BaseUrl = GetBaseUrl();

            return observation;
        }

        [Route(HttpVerbs.Get, "/Observations")]
        public async Task<Listing<Observation>> GetObservationsAsync()
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var baseUrl = GetBaseUrl();
            var service = new ObservationsService(uow);
            var observations = await service.GetObservations();

            foreach (var observation in observations)
            {
                observation.BaseUrl = baseUrl;
            }

            var listing = new Listing<Observation>() { Items = observations.ToList() };

            return listing;
        }

        [Route(HttpVerbs.Patch, "/Observations({id})")]
        public async Task UpdateObservationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new ObservationsService(uow);
            await service.UpdateObservation(updates, id);
            uow.Commit();
        }

        [Route(HttpVerbs.Delete, "/Observations({id})")]
        public async Task RemoveObservationAsync(int id)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var service = new ObservationsService(uow);
            await service.RemoveObservation(id);
            uow.Commit();
        }
    }
}