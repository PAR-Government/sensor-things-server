using System;
using Xunit;
using SensorThings.Server.Mqtt;
using MQTTnet;
using Moq;
using SensorThings.Entities;
using Newtonsoft.Json;
using SensorThings.Server.Utils;

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
        public void TestWithPrefix()
        {
            var observationController = new Mock<IObservationsMqttController>();
            var router = new MqttRouter(observationController.Object);
            var topic = $"v1.0/Observations";

            router.Route(topic, observationBytes);

            observationController.Verify(m => m.Create(observationJson));
        }

        [Fact]
        public void TestWithoutPrefix()
        {
            var observationController = new Mock<IObservationsMqttController>();
            var router = new MqttRouter(observationController.Object);
            var topic = $"Observations";

            router.Route(topic, observationBytes);

            observationController.Verify(m => m.Create(observationJson));
        }

        [Fact]
        public void TestWithBadPrefix()
        {
            var observationController = new Mock<IObservationsMqttController>();
            var router = new MqttRouter(observationController.Object);
            var topic = $"foo/Observations";

            router.Route(topic, observationBytes);

            observationController.Verify(m => m.Create(observationJson), Times.Never);
        }
    }
}
