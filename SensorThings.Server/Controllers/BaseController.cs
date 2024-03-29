﻿using EmbedIO.WebApi;
using SensorThings.Server.Repositories;

namespace SensorThings.Server.Controllers
{
    public abstract class BaseController : WebApiController
    {
        protected IRepositoryFactory RepoFactory { get; private set; }

        public BaseController(IRepositoryFactory repositoryFactory)
        {
            RepoFactory = repositoryFactory;
        }

        protected string GetBaseUrl()
        {
            var fullUrl = HttpContext.Request.Url.AbsoluteUri;
            var path = HttpContext.Route.SubPath;
            var index = fullUrl.LastIndexOf(path);
            return fullUrl.Remove(index, path.Length);
        }
    }
}
