using System.Collections.Generic;
using System.Linq;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using SensorThings.Server.Repositories;

namespace SensorThings.Server.Controllers
{
    public class ResourceV1Controller : BaseController
    {
        public ResourceV1Controller(IRepositoryFactory repositoryFactory) : base(repositoryFactory) { }

        private ResourceEntries BuildEntries(string url)
        {
            var endPoints = new List<string> {
                "Things", "Locations", "HistoricalLocations", "Datastreams",
                "Sensors", "Observations", "ObservedProperties",
                "FeaturesOfInterest" };

            var resources = endPoints.Select(x => new ResourceEntry() { Name = x, Url = $"{url}{x}"});

            var e = new ResourceEntries() { Entries = resources };
            

            return e;
        }

        [Route(HttpVerbs.Get, "/")]
        public ResourceEntries GetResourceIndex()
        {
            return BuildEntries(Request.Url.AbsoluteUri);
        }

        public class ResourceEntry
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }
        }

        public class ResourceEntries
        {
            [JsonProperty("value")]
            public IEnumerable<ResourceEntry> Entries { get; set; }
        }
    }
}
