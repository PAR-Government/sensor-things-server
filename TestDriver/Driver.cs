using System;
using Newtonsoft.Json;
using SensorThings.Server;
using SensorThings.Entities;
using System.IO;
using SensorThings.Server.Repositories;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using System.Net;
using System.Threading.Tasks;

namespace TestDriver
{
    public class Driver
    {
        public static async Task Main()
        {
            var factory = new MqttFactory();
            var mqttServer = factory.CreateMqttServer();
            var mqttClient = factory.CreateMqttClient();

            IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            Console.WriteLine($"IP Address: {ipAddress}");

            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(ipAddress)
                .WithDefaultEndpointBoundIPV6Address(IPAddress.None)
                .Build();
            await mqttServer.StartAsync(mqttServerOptions);

            //var mqttServerTask = mqttServer.StartAsync(new MqttServerOptions());

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("127.0.0.1")
                .Build();

            var dbPath = Path.Combine(Environment.CurrentDirectory, "sensorthings.db");
            var server = new Server("http://127.0.0.1:8080", new SqliteRepositoryFactory(dbPath), mqttClient, mqttClientOptions);

            server.Configure();
            var t = server.RunAsync();
            Console.ReadLine();
        }
    }
}
