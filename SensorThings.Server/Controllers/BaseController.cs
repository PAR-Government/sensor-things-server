using System;
using EmbedIO;
using EmbedIO.WebApi;

namespace SensorThings.Server.Controllers
{
    public abstract class BaseController : WebApiController
    {
        protected string GetBaseUrl(IHttpContext ctx)
        {
            var fullUrl = ctx.Request.Url.AbsoluteUri;
            var path = ctx.Route.Path;

            var index = fullUrl.LastIndexOf(path);
            return fullUrl.Remove(index, path.Length);
        }
    }
}
