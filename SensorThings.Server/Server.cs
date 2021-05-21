using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.WebApi;
using SensorThings.Server.Controllers;
using SensorThings.Entities;
using SensorThings.Server.Repositories;

namespace SensorThings.Server
{
    public class Server
    {
        internal static IRepositoryFactory RepoFactory;

        private WebServer _server;

        public Server(string url, IRepositoryFactory repoFactory)
        {
            RepoFactory = repoFactory;
            _server = new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
                .WithLocalSessionManager();
        }

        public void Configure()
        {
            _server.WithWebApi("/echo", m => m.WithController<EchoController>());
            _server.WithModule(
                new WebApiModule("/v1.0")
                .WithController<ResourceV1Controller>()
                .WithController<ThingsV1Controller>()
                .WithController<LocationsV1Controller>()
                .WithController<SensorsV1Controller>()
                .WithController<ObservedPropertiesV1Controller>()
                .WithController<FeaturesOfInterestV1Controller>()
                .WithController<ObservationsV1Controller>()
                .WithController<DatastreamsV1Controller>());
        }

        public Task RunAsync()
        {
            return _server.RunAsync();
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
