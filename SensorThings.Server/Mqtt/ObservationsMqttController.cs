using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;

namespace SensorThings.Server.Mqtt
{
    public interface IObservationsMqttController
    {
        public IRepositoryFactory RepoFactory { get; }

        public string BaseUrl { get; }

        public Task<Observation> Create(string body);

        public Task Publish(Observation observation);
    }

    public class ObservationsMqttController: IObservationsMqttController
    {
        public readonly IMqttClient _mqttClient;
        public IRepositoryFactory RepoFactory { get; }
        public string BaseUrl { get; }

        public ObservationsMqttController(string baseUrl, IMqttClient publishClient, IRepositoryFactory repoFactory)
        {
            _mqttClient = publishClient;
            BaseUrl = baseUrl;
            RepoFactory = repoFactory;
        }

        public async Task<Observation> Create(string body)
        {
            using var uow = RepoFactory.CreateUnitOfWork();
            var observation = JsonConvert.DeserializeObject<Observation>(body);
            var ds = observation.Datastream;
      
            if (ds == null)
            {
                return null;
            }

            var service = new ObservationsService(uow);
            observation = await service.AddObservation(observation, ds.ID);
            observation.BaseUrl = BaseUrl;
            ds.BaseUrl = BaseUrl;

            uow.Commit();

            await PublishAsync(null, observation);

            return observation;
        }

        public async Task Publish(Observation observation)
        {
            await PublishAsync(_mqttClient, observation);
        }

        public static async Task PublishAsync(IMqttClient client, Observation observation)
        {
            var json = JsonConvert.SerializeObject(observation);

            var v1_0message = new MqttApplicationMessageBuilder()
                .WithTopic($"Datastreams({observation.Datastream.ID})/Observations")
                .WithPayload(json)
                .Build();

            var v1_1message = new MqttApplicationMessageBuilder()
                .WithTopic($"v1.0/Datastreams({observation.ID})/Observations")
                .WithPayload(json)
                .Build();

            await client.PublishAsync(v1_0message, CancellationToken.None);
            await client.PublishAsync(v1_1message, CancellationToken.None);
        }
    }
}
