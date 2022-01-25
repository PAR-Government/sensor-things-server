using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Utilities;
using EmbedIO.WebApi;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using SensorThings.Server.Controllers;
using SensorThings.Server.Repositories;

namespace SensorThings.Server
{
    public class Server
    {
        private IRepositoryFactory RepoFactory { get; set; }

        private WebServer _server;
        private readonly IMqttClient _mqttPublishClient;
        private readonly IMqttClient _mqttSubscribeClient;
        private readonly IMqttClientOptionsFactory _mqttClientOptionsFactory;
        private readonly IMqttClientOptions _mqttClientOptions;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public Server(string url, IRepositoryFactory repoFactory, IMqttClientOptionsFactory mqttClientOptionsFactory)
        {
            RepoFactory = repoFactory;
            _server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager();

            _mqttClientOptionsFactory = mqttClientOptionsFactory;

            var factory = new MqttFactory();
            _mqttPublishClient = factory.CreateMqttClient();
            _mqttSubscribeClient = factory.CreateMqttClient();
        }

        public void Configure()
        {
            _server.WithWebApi("/echo", m => m.WithController<EchoController>());


            var apiModule = new WebApiModule("/v1.0", SerializeWithNewtonsoft)
                .WithController(() => new ResourceV1Controller(RepoFactory))
                .WithController(() => new ThingsV1Controller(RepoFactory))
                .WithController(() => new LocationsV1Controller(RepoFactory))
                .WithController(() => new HistoricalLocationsV1Controller(RepoFactory))
                .WithController(() => new SensorsV1Controller(RepoFactory))
                .WithController(() => new ObservedPropertiesV1Controller(RepoFactory))
                .WithController(() => new FeaturesOfInterestV1Controller(RepoFactory))
                .WithController(() => new ObservationsV1Controller(RepoFactory, _mqttPublishClient))
                .WithController(() => new DatastreamsV1Controller(RepoFactory))
                .HandleHttpException(HttpExceptionHandler.DataResponse(SerializeWithNewtonsoft));

            _server.WithModule(apiModule);
        }

        public async Task SerializeWithNewtonsoft(IHttpContext context, object data)
        {
            Validate.NotNull(nameof(context), context).Response.ContentType = MimeType.Json;
            using var text = context.OpenResponseText(new UTF8Encoding(true));
            await text.WriteAsync(JsonConvert.SerializeObject(data)).ConfigureAwait(false);
        }

        public async Task RunAsync()
        {
            if (_mqttPublishClient != null)
            {
                await _mqttPublishClient
                    .ConnectAsync(_mqttClientOptionsFactory.NewPublisherOptions(), CancellationToken.None)
                    .ConfigureAwait(false);
            }

            if (_mqttSubscribeClient != null)
            {
                await _mqttSubscribeClient
                    .ConnectAsync(_mqttClientOptionsFactory.NewSubscriberOptions(), CancellationToken.None)
                    .ConfigureAwait(false);
            }

            await _server.RunAsync(_cancellationTokenSource.Token);
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            await _mqttPublishClient?.DisconnectAsync();
        }
    }
}
