using System;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using SensorThings.Entities;

namespace SensorThings.Server.Controllers
{
    public class FeaturesOfInterestV1Controller : WebApiController
    {
        [Route(HttpVerbs.Post, "/FeaturesOfInterest")]
        public async Task<string> CreateFeatureOfInterest()
        {
            var data = await HttpContext.GetRequestBodyAsStringAsync();
            var featureOfInterest = JsonConvert.DeserializeObject<Thing>(data);

            return $"name: {featureOfInterest.Name}, description: {featureOfInterest.Description}";
        }

        [Route(HttpVerbs.Get, "/FeaturesOfInterest({id})")]
        public string GetFeatureOfInterest(int id)
        {
            return $"FeatureOfInterest with id: {id}";
        }
    }
}
