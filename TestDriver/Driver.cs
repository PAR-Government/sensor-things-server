using System;
using SensorThings.Server;
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
            // Create our MQTT pieces
            var factory = new MqttFactory();
            var mqttServer = factory.CreateMqttServer();
            var mqttClient = factory.CreateMqttClient();

            // Figure out our listening interfaces
            //IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPAddress mqttIpAddress = ipAddress;

            Console.WriteLine($"Server IP Address: {ipAddress}");
            Console.WriteLine($"MQTT Broker IP Address: {mqttIpAddress}");

            // Configure MQTT broker. If you are reusing an existing broker, then you don't need
            // to run this one. Instead just point the client to your existing broker
            var mqttServerOptionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionValidator(c =>
                {
                    c.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
                });

            // Please not that localhost or loopback may not work on Android per the MQTTnet project
            if (mqttIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                mqttServerOptionsBuilder.WithDefaultEndpointBoundIPV6Address(mqttIpAddress);
            } 
            else
            {
                mqttServerOptionsBuilder.WithDefaultEndpointBoundIPAddress(mqttIpAddress);
            }

            var mqttServerOptions = mqttServerOptionsBuilder.Build();

            // Start up MQTT broker
            await mqttServer.StartAsync(mqttServerOptions);

            // Configure MQTT Client. The server will connect it later
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttIpAddress.ToString())
                .Build();

            // Our server will use SQLite for the storage backend
            var dbPath = Path.Combine(Environment.CurrentDirectory, "sensorthings.db");
            IRepositoryFactory sqlLiteRepoFactory = new SqliteRepositoryFactory(dbPath);

            // Create our server
            var server = new Server($"http://{ipAddress}:8080", sqlLiteRepoFactory, mqttClient, mqttClientOptions);

            // Init the server and then start it
            server.Configure();
            await server.RunAsync();

            Console.ReadLine();

            // Shutdown
            await server.StopAsync();
            await mqttServer.StopAsync();
        }
    }
}
