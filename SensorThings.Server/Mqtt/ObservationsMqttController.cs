using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
    }

    public class ObservationsMqttController: IObservationsMqttController
    {
        public IRepositoryFactory RepoFactory { get; }
        private readonly IMqttService _mqttService;
        public string BaseUrl { get; }

        public ObservationsMqttController(ServerConfig serverConfig, IRepositoryFactory repoFactory, IMqttService mqttService)
        {
            _mqttService = mqttService;
            BaseUrl = serverConfig.BaseUrl;
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

            var service = uow.ObservationsService;
            observation = await service.AddObservation(observation, ds.ID);
            observation.BaseUrl = BaseUrl;
            ds.BaseUrl = BaseUrl;

            uow.Commit();

            await _mqttService.PublishObservationAsync(observation);

            return observation;
        }
    }
}
