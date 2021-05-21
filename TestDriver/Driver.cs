using System;
using Newtonsoft.Json;
using SensorThings.Server;
using SensorThings.Entities;
using System.IO;
using SensorThings.Server.Repositories;

namespace TestDriver
{
    public class Driver
    {
        public static void Main()
        {
            var dbPath = Path.Combine(Environment.CurrentDirectory, "sensorthings.db");
            var server = new Server("http://localhost:8080", new SqliteRepositoryFactory(dbPath));

            server.Configure();
            var t = server.RunAsync();
            Console.ReadLine();
        }
    }
}
