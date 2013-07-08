using System.Web;
using System.Web.Routing;
using Cassette.DependencyGraphInteration;
using Cassette.DependencyGraphInteration.InterationResults;

namespace Cassette.Web
{
    class AssetRequestHandler : IHttpHandler
    {
        public AssetRequestHandler(RequestContext requestContext, IInteractWithDependencyGraph interaction)
        {
            this.interaction = interaction;
            this.requestContext = requestContext;
        }

        readonly IInteractWithDependencyGraph interaction;
        readonly RequestContext requestContext;

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext _)
        {
            var path = "~/" + requestContext.RouteData.GetRequiredString("path");
            Trace.Source.TraceInformation("Handling asset request for path \"{0}\".", path);
            requestContext.HttpContext.DisableHtmlRewriting();
            var response = requestContext.HttpContext.Response;

            var result = interaction.GetAsset(path);

            if(result.Exception != null)
            {
                throw result.Exception;
            }

            if (result.NotFound)
            {
                Trace.Source.TraceInformation("Bundle asset not found with path \"{0}\".", path);
                NotFound(response);
                return;
            }

            var request = requestContext.HttpContext.Request;
            SendAsset(request, response, result);
        }

        void SendAsset(HttpRequestBase request, HttpResponseBase response, StreamInterationResult stream)
        {
            response.ContentType = stream.ContentType;

            var actualETag = "\"" + stream.Hash + "\"";
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetETag(actualETag);

            var givenETag = request.Headers["If-None-Match"];
            if (givenETag == actualETag)
            {
                SendNotModified(response);
            }
            else
            {
                using (stream)
                {
                    stream.CopyTo(response.OutputStream); 
                }
            }
        }

        void SendNotModified(HttpResponseBase response)
        {
            response.StatusCode = 304; // Not Modified
            response.SuppressContent = true;
        }

        void NotFound(HttpResponseBase response)
        {
            response.StatusCode = 404;
            response.SuppressContent = true;
        }
    }
}