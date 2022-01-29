using System;
using Xunit;
using SensorThings.Server.Mqtt;
using MQTTnet;
using Moq;
using SensorThings.Entities;
using Newtonsoft.Json;
using SensorThings.Server.Utils;
using System.Threading.Tasks;

namespace SensorThings.Server.Test.Mqtt
{
    public class MqttRouterTest
    {
        private readonly Observation observation = new Observation { ID = 1 };
        private string observationJson;
        private byte[] observationBytes;

        public MqttRouterTest()
        {
            observationJson = JsonConvert.SerializeObject(observation);
            observationBytes = PayloadUtils.ConvertStringToUTF8Bytes(observationJson);
        }

        [Fact]
        public async Task TestWithPrefix()
        {
            var observationController = new Mock<IObservationsMqttController>();
            var router = new MqttRouter(observationController.Object);
            var topic = $"v1.0/Observations";

            var handled = await router.Route(topic, observationBytes);

            Assert.True(handled);
            observationController.Verify(m => m.Create(observationJson));
        }

        [Fact]
        public async Task TestWithoutPrefix()
        {
            var observationController = new Mock<IObservationsMqttController>();
            var router = new MqttRouter(observationController.Object);
            var topic = $"Observations";

            var handled = await router.Route(topic, observationBytes);

            Assert.True(handled);
            observationController.Verify(m => m.Create(observationJson));
        }

        [Fact]
        public async Task TestWithBadPrefix()
        {
            var observationController = new Mock<IObservationsMqttController>();
            var router = new MqttRouter(observationController.Object);
            var topic = $"foo/Observations";

            var handled = await router.Route(topic, observationBytes);

            Assert.False(handled);
            observationController.Verify(m => m.Create(observationJson), Times.Never);
        }
    }
}
