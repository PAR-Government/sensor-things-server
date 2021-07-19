using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.WebApi;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using SensorThings.Server.Controllers;
using SensorThings.Server.Repositories;

namespace SensorThings.Server
{
    public class Server
    {
        private IRepositoryFactory RepoFactory { get; set; }

        private WebServer _server;
        private readonly IMqttClient _mqttClient;
        private readonly IMqttClientOptions _mqttClientOptions;

        public Server(string url, IRepositoryFactory repoFactory, IMqttClient mqttClient, IMqttClientOptions mqttClientOptions)
        {
            RepoFactory = repoFactory;
            _server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager();

            _mqttClient = mqttClient;
            _mqttClientOptions = mqttClientOptions;
        }

        public void Configure()
        {
            _server.WithWebApi("/echo", m => m.WithController<EchoController>());
            _server.WithModule(
                new WebApiModule("/v1.0")
                .WithController(() => new ResourceV1Controller(RepoFactory))
                .WithController(() => new ThingsV1Controller(RepoFactory))
                .WithController(() => new LocationsV1Controller(RepoFactory))
                .WithController(() => new HistoricalLocationsV1Controller(RepoFactory))
                .WithController(() => new SensorsV1Controller(RepoFactory))
                .WithController(() => new ObservedPropertiesV1Controller(RepoFactory))
                .WithController(() => new FeaturesOfInterestV1Controller(RepoFactory))
                .WithController(() => new ObservationsV1Controller(RepoFactory, _mqttClient))
                .WithController(() => new DatastreamsV1Controller(RepoFactory)));
        }

        public Task RunAsync()
        {
            if (_mqttClient != null)
            {
                return Task.WhenAll(_mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None), _server.RunAsync());
            }
            else
            {
                return _server.RunAsync();
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
