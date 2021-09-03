using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;

namespace SensorThings.Server.Controllers
{
    public class ObservationsV1Controller : BaseController
    {
        private readonly IMqttClient _mqttClient;

        public ObservationsV1Controller(IRepositoryFactory repoFactory, IMqttClient mqttClient) : base(repoFactory) 
        {
            _mqttClient = mqttClient;
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

            var json = JsonConvert.SerializeObject(observation);

            if (_mqttClient != null)
            {
                await PublishObservation(ds.ID, json);
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

        private async Task PublishObservation(long datastreamId, string observation)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"v1.0/Datastreams({datastreamId})/Observations")
                .WithPayload(observation)
                .Build();
            await _mqttClient.PublishAsync(message, CancellationToken.None);
        }
    }
}