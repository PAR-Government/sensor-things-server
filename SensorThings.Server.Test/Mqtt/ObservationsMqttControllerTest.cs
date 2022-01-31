using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Mqtt;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using SensorThings.Server.Test.TestObjects;
using Xunit;

namespace SensorThings.Server.Test.Mqtt
{
    public class ObservationsMqttControllerTest
    {
        private ServerConfig serverConfig = new ServerConfig { BaseUrl = "http://127.0.0.1/v1/" };
        private Mock<IMqttService> mqttService = new Mock<IMqttService>();
        private Mock<IObservationsService> observationsService = new Mock<IObservationsService>();
        private TestRepoFactory repoFactory = new TestRepoFactory();

        [Fact]
        public async Task Test_CreateObservation()
        {
            var datastream = new Datastream { ID = 5 };
            var observation = new Observation
            {
                ID = 3,
                Result = JObject.Parse("{'foo': 42}"),
                Datastream = datastream,
                BaseUrl = serverConfig.BaseUrl
            };
            observationsService.Setup(m => m.AddObservation(It.IsAny<Observation>(), observation.Datastream.ID)).ReturnsAsync(observation);
            repoFactory.ObservationsService = observationsService.Object;

            var observationJson = JsonConvert.SerializeObject(observation);
            var controller = new ObservationsMqttController(serverConfig, repoFactory, mqttService.Object);

            var createdObservation = await controller.Create(observationJson);

            Assert.Equal(observation, createdObservation);
            observationsService.Verify(m => m.AddObservation(It.IsAny<Observation>(), observation.Datastream.ID));
            mqttService.Verify(m => m.PublishObservationAsync(observation));
        }

        [Fact]
        public async Task Test_CreateObservationNullDatastream()
        {
            var observation = new Observation
            {
                ID = 3,
                Result = JObject.Parse("{'foo': 42}"),
                BaseUrl = serverConfig.BaseUrl
            };
            repoFactory.ObservationsService = observationsService.Object;

            var observationJson = JsonConvert.SerializeObject(observation);
            var controller = new ObservationsMqttController(serverConfig, repoFactory, mqttService.Object);

            var createdObservation = await controller.Create(observationJson);

            Assert.Null(createdObservation);
            observationsService
                .Verify(m => m.AddObservation(It.IsAny<Observation>(), It.IsAny<long>()), Times.Never);
            mqttService.Verify(m => m.PublishObservationAsync(observation), Times.Never);
        }
    }
}
