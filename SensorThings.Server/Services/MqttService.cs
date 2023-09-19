using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Newtonsoft.Json;
using SensorThings.Entities;
using SensorThings.Server.Mqtt;
using TinyIoC;

namespace SensorThings.Server.Services
{
    public class MqttService : IMqttService { 

        private MqttServer _mqtt;
        private MqttRouter _router;

        public MqttService()
        {
        }

        private async void OnAppMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            // If ClientId is null, the message was produced by the server and we can skip
            if (e.ClientId == null) return;
            _ = await _router.Route(e.ApplicationMessage.Topic, e.ApplicationMessage.PayloadSegment.ToArray()); // Convert from ArraySegment to byte[]
        }

        public Task HandleClientConnectedAsync(ClientConnectedEventArgs eventArgs)
        {
            Console.WriteLine($"Client connected. ID: {eventArgs.ClientId}");

            return Task.CompletedTask;

        }

        public Task HandleClientDisconnectedAsync(ClientDisconnectedEventArgs eventArgs)
        {
            Console.WriteLine($"Client disconnected. ID: {eventArgs.ClientId}");

            return Task.CompletedTask;
        }

        public async Task PublishAsync(MqttApplicationMessageBuilder message)
        {
            //var result = await _mqtt.PublishAsync(message.Build()); 
            var msg = new InjectedMqttApplicationMessage(message.Build()); // replacement for PublishAsync
            await _mqtt.InjectApplicationMessage(msg);

            // Console.WriteLine("Publish Result: " ); // Is there a way to get the result in 4.0?
        }

        public async Task PublishObservationAsync(Observation observation)
        {
            var json = JsonConvert.SerializeObject(observation);

            var rootTopic = $"Datastreams({observation.Datastream.ID})/Observations";

            var msg = new MqttApplicationMessageBuilder()
                .WithTopic(rootTopic)
                .WithPayload(json);

            // Publish for SensorThings 1.0
            await PublishAsync(msg);

            // Publish for SensorThings 1.1
            await PublishAsync(msg.WithTopic($"v1.0/{rootTopic}"));
        }


        public void Configure(MqttServer mqtt)
        {
            // If we constructor inject the router we will end up in a circular dependency
            _router = TinyIoCContainer.Current.Resolve<MqttRouter>();

            _mqtt = mqtt;
            _mqtt.ClientConnectedAsync += this.HandleClientConnectedAsync;
            _mqtt.ClientDisconnectedAsync += this.HandleClientDisconnectedAsync;
            _mqtt.InterceptingPublishAsync += async e => { // ApplicationMessageReceivedHandler was removed in 4.0
                if (e.ClientId == null) return;
                _ = await _router.Route(e.ApplicationMessage.Topic, e.ApplicationMessage.PayloadSegment.ToArray()); // Convert from ArraySegment to byte[]
            };
        }
    }
}
