using EmbedIO;
using EmbedIO.Routing;
using System.Net;
using EmbedIO.WebApi;

namespace SensorThings.Server.Controllers
{
	public class HealthController : WebApiController
    {
		public HealthController()
        {
		}

        [Route(HttpVerbs.Get, "/")]
        public string GetHealth()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;

            return "Healthy";
        }
    }
}
