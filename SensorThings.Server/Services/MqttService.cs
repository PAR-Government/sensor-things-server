using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using Newtonsoft.Json;
using SensorThings.Entities;
using SensorThings.Server.Mqtt;
using TinyIoC;

namespace SensorThings.Server.Services
{
    public interface IMqttService
    {
        public Task PublishObservationAsync(Observation observation);
        public void Configure(IMqttServer mqtt);
    }

    public class MqttService : IMqttService, IMqttServerClientConnectedHandler, IMqttServerClientDisconnectedHandler, IMqttApplicationMessageReceivedHandler
    {
        private IMqttServer _mqtt;
        private MqttRouter _router;

        public MqttService()
        {
        }

        public void Configure(IMqttServer mqtt)
        {
            // If we constructor inject the router we will end up in a circular dependency
            _router = TinyIoCContainer.Current.Resolve<MqttRouter>();

            _mqtt = mqtt;
            _mqtt.ClientConnectedHandler = this;
            _mqtt.ClientDisconnectedHandler = this;
            _mqtt.ApplicationMessageReceivedHandler = this;
        }

        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            // If ClientId is null, the message was produced by the server and we can skip
            if (eventArgs.ClientId == null) return;
            _ = await _router.Route(eventArgs.ApplicationMessage.Topic, eventArgs.ApplicationMessage.Payload);
        }

        public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
        {
            Console.WriteLine($"Client connected. ID: {eventArgs.ClientId}");

            return Task.CompletedTask;
        }

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            Console.WriteLine($"Client disconnected. ID: {eventArgs.ClientId}");

            return Task.CompletedTask;
        }

        public async Task PublishAsync(MqttApplicationMessageBuilder message)
        {
            await _mqtt.PublishAsync(message.Build());
        }

        public async Task PublishObservationAsync(Observation observation)
        {
            var json = JsonConvert.SerializeObject(observation);

            var rootTopic = $"Datastreams({observation.Datastream.ID})/Observations";

            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(rootTopic)
                .WithPayload(json);

            // Publish for SensorThings 1.0
            await _mqtt.PublishAsync(msg.Build());

            // Publish for SensorThings 1.1
            _ = await _mqtt.PublishAsync(msg.WithTopic($"v1.0/{rootTopic}").Build());
        }
    }
}