using System;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Routing;
using Cassette.DependencyGraphInteration;
using Cassette.DependencyGraphInteration.InterationResults;
using Cassette.Utilities;

namespace Cassette.Web
{
    class BundleRequestHandler<T> : IHttpHandler
        where T : Bundle
    {
        readonly ICassetteApplication application;
        readonly RouteData routeData;
        readonly HttpResponseBase response;
        readonly HttpRequestBase request;
        readonly HttpContextBase httpContext;
        readonly IInteractWithDependencyGraph interaction;

        public BundleRequestHandler(ICassetteApplication application, RequestContext requestContext, IInteractWithDependencyGraph interaction)
        {
            this.application = application;
            
            routeData = requestContext.RouteData;
            response = requestContext.HttpContext.Response;
            request = requestContext.HttpContext.Request;
            httpContext = requestContext.HttpContext;
            this.interaction = interaction;
        }

        public void ProcessRequest()
        {
            httpContext.DisableHtmlRewriting();
            var result = FindBundle();

            if (result.Exception != null)
            {
                throw result.Exception;
            }

            if (result.NotFound)
            {
                Trace.Source.TraceInformation("Bundle not found \"{0}\".", Path.Combine("~", routeData.GetRequiredString("path")));
                response.StatusCode = 404;
            }
            else
            {
                var actualETag = "\"" + result.Hash + "\"";
                var givenETag = request.Headers["If-None-Match"];
                if (givenETag == actualETag)
                {
                    SendNotModified(actualETag);
                }
                else
                {
                    SendBundle(result, actualETag);
                }
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        void IHttpHandler.ProcessRequest(HttpContext unused)
        {
            // The HttpContext is unused because the constructor accepts a more test-friendly RequestContext object.
            ProcessRequest();
        }

        StreamInterationResult FindBundle()
        {
            var path = "~/" + routeData.GetRequiredString("path");
            Trace.Source.TraceInformation("Handling bundle request for \"{0}\".", path);
            path = RemoveTrailingHashFromPath(path);
            return interaction.GetBundle<T>(path);
        }

        /// <summary>
        /// A Bundle URL has the hash appended after an underscore character. This method removes the underscore and hash from the path.
        /// </summary>
        string RemoveTrailingHashFromPath(string path)
        {
            var index = path.LastIndexOf('_');
            if (index >= 0)
            {
                return path.Substring(0, index);
            }
            return path;
        }

        void SendNotModified(string actualETag)
        {
            CacheLongTime(actualETag); // Some browsers seem to require a reminder to keep caching?!
            response.StatusCode = 304; // Not Modified
            response.SuppressContent = true;
        }

        void SendBundle(StreamInterationResult stream, string actualETag)
        {
            response.ContentType = stream.ContentType;
            CacheLongTime(actualETag);

            var encoding = request.Headers["Accept-Encoding"];
            response.Filter = EncodeStreamAndAppendResponseHeaders(response.Filter, encoding);

            using (stream)
            {
                stream.CopyTo(response.OutputStream);
            }
        }

        void CacheLongTime(string actualETag)
        {
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.SetExpires(DateTime.UtcNow.AddYears(1));
            response.Cache.SetETag(actualETag);
        }

        Stream EncodeStreamAndAppendResponseHeaders(Stream stream, string encoding)
        {
            return stream;
        }
    }
}
