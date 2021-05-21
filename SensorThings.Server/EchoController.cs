using System;
using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;

namespace SensorThings.Server
{
    public class EchoController : WebApiController
    {
        public EchoController()
        {
        }

        [Route(HttpVerbs.Get, "/{phrase}")]
        public string Echo(string phrase)
        {
            return $"Your phrase was: {phrase}";
        }
    }
}
