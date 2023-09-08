using System;
using SensorThings.Server;
using System.IO;
using SensorThings.Server.Repositories;
using MQTTnet.Server;
using System.Net;
using System.Threading.Tasks;

namespace TestDriver
{
    public class Driver
    {
        public static async Task Main()
        {
            // Figure out our listening interfaces
            //IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPAddress mqttIpAddress = ipAddress;

            Console.WriteLine($"Server IP Address: {ipAddress}");
            Console.WriteLine($"MQTT Broker IP Address: {mqttIpAddress}");

            // Configure MQTT broker. If you are reusing an existing broker, then you don't need
            // to run this one. Instead just point the client to your existing broker
            var mqttServerOptionsBuilder = new MqttServerOptionsBuilder(); // Removed WithConnectionValidator.

            // Please not that localhost or loopback may not work on Android per the MQTTnet project
            if (mqttIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                mqttServerOptionsBuilder.WithDefaultEndpointBoundIPV6Address(mqttIpAddress);
            } 
            else
            {
                mqttServerOptionsBuilder.WithDefaultEndpointBoundIPAddress(mqttIpAddress);
            }

            var mqttServerOptions = mqttServerOptionsBuilder.WithDefaultEndpointPort(8080).Build();


            // Our server will use SQLite for the storage backend
            var dbPath = Path.Combine(Environment.CurrentDirectory, "sensorthings.db");
            IRepositoryFactory sqlLiteRepoFactory = new SqliteRepositoryFactory(dbPath);

            // Create our server
            //public Server(string url, IRepositoryFactory repoFactory, MqttServerOptionsBuilder mqttOptions)

            var server = new Server($"http://{ipAddress}:8080", sqlLiteRepoFactory, mqttServerOptionsBuilder);

            // Init the server and then start it
            server.Configure();
            await server.RunAsync();

            Console.ReadLine();

            // Shutdown
            await server.StopAsync();
        }
    }
}
