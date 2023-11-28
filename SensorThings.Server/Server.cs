using System.Data;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Utilities;
using EmbedIO.WebApi;
using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json;
using SensorThings.Server.Controllers;
using SensorThings.Server.Mqtt;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;
using TinyIoC;

namespace SensorThings.Server
{
    public class Server
    {
        private IRepositoryFactory RepoFactory { get; set; }

        private ServerConfig _serverConfig = new ServerConfig();
        private WebServer _server;
        private Task _webServerTask;

        private MqttServer _mqttServer;

        private readonly MqttServerOptionsBuilder _mqttOptions;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ"
        };

        public Server(string url, IRepositoryFactory repoFactory, MqttServerOptionsBuilder mqttOptions)
        {
            _serverConfig.BaseUrl = $"{url}/v1.0";
            _mqttOptions = mqttOptions;

            // TODO: Intending these to be Singletons, but they may not be
            TinyIoCContainer.Current.Register<ServerConfig>(_serverConfig);
            TinyIoCContainer.Current.Register<IRepositoryFactory>(repoFactory);

            RepoFactory = repoFactory;
            _server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager();
        }

        public void Configure()
        {
            // Create and register our MQTT controller and router
            TinyIoCContainer.Current.Register<IObservationsMqttController, ObservationsMqttController>();
            TinyIoCContainer.Current.Register<MqttRouter>();
            TinyIoCContainer.Current.Register<IMqttService, MqttService>();

            // Configure our Mqtt Service
            var mqttService = TinyIoCContainer.Current.Resolve<IMqttService>();
            _mqttServer = new MqttFactory().CreateMqttServer(_mqttOptions.WithDefaultEndpoint().Build());
            _mqttServer.ValidatingConnectionAsync += this.ValidateConnectionAsync;
            mqttService.Configure(_mqttServer);

            // Configure our embedded web server
            _server.WithWebApi("/Health", m => m.WithController<HealthController>());

            var apiModule = new WebApiModule("/v1.0", SerializeWithNewtonsoft)
                .WithController(() => new ResourceV1Controller(RepoFactory))
                .WithController(() => new ThingsV1Controller(RepoFactory))
                .WithController(() => new LocationsV1Controller(RepoFactory))
                .WithController(() => new HistoricalLocationsV1Controller(RepoFactory))
                .WithController(() => new SensorsV1Controller(RepoFactory))
                .WithController(() => new ObservedPropertiesV1Controller(RepoFactory))
                .WithController(() => new FeaturesOfInterestV1Controller(RepoFactory))
                .WithController(() => new ObservationsV1Controller(RepoFactory))
                .WithController(() => new DatastreamsV1Controller(RepoFactory))
                .HandleHttpException(HttpExceptionHandler.DataResponse(SerializeWithNewtonsoft));

            _server.WithModule(apiModule);
        }

        public async Task SerializeWithNewtonsoft(IHttpContext context, object data)
        {
            Validate.NotNull(nameof(context), context).Response.ContentType = MimeType.Json;
            using var text = context.OpenResponseText(new UTF8Encoding(true));
            await text.WriteAsync(JsonConvert.SerializeObject(data, _jsonSettings)).ConfigureAwait(false);
        }

        public async Task RunAsync()
        {
            _webServerTask = _server.RunAsync(_cancellationTokenSource.Token);
            await _mqttServer.StartAsync();
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();

            if (_webServerTask != null)
            {
                await _webServerTask;
            }

            if (_mqttServer != null)
            {
                await _mqttServer.StopAsync();
            }
        }

        private Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
        {
            return Task.Run(() =>
            {
                // replaces this, because WithConnectionValidator got removed
                // Relevant discussion: https://github.com/dotnet/MQTTnet/discussions/1521 

                //var mqttServerOptionsBuilder = new MqttServerOptionsBuilder()
                //    .WithConnectionValidator(c =>
                //    {
                //        c.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
                //    });
                return args.ReasonCode;
            });
        }
    }
}
