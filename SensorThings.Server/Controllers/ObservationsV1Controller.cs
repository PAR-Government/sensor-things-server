using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using SensorThings.Entities;
using SensorThings.Server.Repositories;

namespace SensorThings.Server.Controllers
{
    public class ObservationsV1Controller : BaseController
    {
        public ObservationsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/Observations")]
        public async Task<string> CreateObservation()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var observation = JsonConvert.DeserializeObject<Observation>(data);

            return $"name: {observation.PhenomenonTime}";
        }

        [Route(HttpVerbs.Get, "/Observations({id})")]
        public string GetObservation(int id)
        {
            return $"Observation with id: {id}";
        }
    }
}
