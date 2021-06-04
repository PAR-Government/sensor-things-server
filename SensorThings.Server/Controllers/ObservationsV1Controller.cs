using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorThings.Entities;
using SensorThings.Server.Repositories;
using SensorThings.Server.Services;

namespace SensorThings.Server.Controllers
{
    public class ObservationsV1Controller : BaseController
    {
        public ObservationsV1Controller(IRepositoryFactory repoFactory) : base(repoFactory) { }

        [Route(HttpVerbs.Post, "/Observations")]
        public async Task<string> CreateObservationAsync()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var observation = JsonConvert.DeserializeObject<Observation>(data);

            var service = new ObservationsService(RepoFactory);
            observation = await service.AddObservation(observation);

            observation.BaseUrl = GetBaseUrl();

            Response.StatusCode = (int)HttpStatusCode.Created;

            return JsonConvert.SerializeObject(observation);
        }

        [Route(HttpVerbs.Get, "/Observations({id})")]
        public async Task<string> GetObservationAsync(int id)
        {
            var service = new ObservationsService(RepoFactory);
            var observation = await service.GetObservationById(id);
            observation.BaseUrl = GetBaseUrl();

            return JsonConvert.SerializeObject(observation);
        }

        [Route(HttpVerbs.Get, "/Observations")]
        public async Task<string> GetObservationsAsync()
        {
            var baseUrl = GetBaseUrl();
            var service = new ObservationsService(RepoFactory);
            var observations = await service.GetObservations();

            foreach (var observation in observations)
            {
                observation.BaseUrl = baseUrl;
            }

            var listing = new Listing<Observation>() { Items = observations.ToList() };

            return JsonConvert.SerializeObject(listing);
        }

        [Route(HttpVerbs.Patch, "/Observations({id})")]
        public async Task UpdateObservationAsync(int id)
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var updates = JObject.Parse(data);

            var service = new ObservationsService(RepoFactory);
            await service.UpdateObservation(updates, id);
        }

        [Route(HttpVerbs.Delete, "/Observations({id})")]
        public async Task RemoveObservationAsync(int id)
        {
            var service = new ObservationsService(RepoFactory);
            await service.RemoveObservation(id);
        }
    }
}
