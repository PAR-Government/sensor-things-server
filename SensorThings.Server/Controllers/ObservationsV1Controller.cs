using System;
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
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var document = JObject.Parse(data);
            var observation = JsonConvert.DeserializeObject<Observation>(data);
            long dsId;
            // Extract the datastream reference
            try 
            {
                var dsDoc = document.GetValue("Datastream");
                dsId = dsDoc.Value<int>("@iot.id");
            }
            catch (Exception)
            {
                var errorDoc = new JObject
                {
                    { "error", "Missing datastream link" }
                };

                Response.ContentType = MimeType.Json;

                throw HttpException.BadRequest("Missing Datastream link", errorDoc);
            }

            var service = new ObservationsService(RepoFactory);
            observation = await service.AddObservation(observation, dsId);

            observation.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            var json = JsonConvert.SerializeObject(observation);

            if (_mqttClient != null)
            {
                await PublishObservation(dsId, json);
            }
            return observation;
        }

        [Route(HttpVerbs.Get, "/Observations({id})")]
        public async Task<Observation> GetObservationAsync(int id)
        {
            var service = new ObservationsService(RepoFactory);
            var observation = await service.GetObservationById(id);
            observation.BaseUrl = GetBaseUrl();

            return observation;
        }

        [Route(HttpVerbs.Get, "/Observations")]
        public async Task<Listing<Observation>> GetObservationsAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new ObservationsService(RepoFactory);
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
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new ObservationsService(RepoFactory);
            await service.UpdateObservation(updates, id);
        }

        [Route(HttpVerbs.Delete, "/Observations({id})")]
        public async Task RemoveObservationAsync(int id)
        {
            var service = new ObservationsService(RepoFactory);
            await service.RemoveObservation(id);
        }

        private async Task PublishObservation(long datastreamId, string observation)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"Datastreams({datastreamId})/Observations")
                .WithPayload(observation)
                .Build();
            await _mqttClient.PublishAsync(message, CancellationToken.None);
        }
    }
}