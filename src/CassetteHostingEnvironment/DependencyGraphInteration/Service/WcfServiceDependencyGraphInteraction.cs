using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web;
using Cassette;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;
using Cassette.DependencyGraphInteration.InterationResults;
using Cassette.HtmlTemplates;
using Cassette.ScriptAndTemplate;
using Cassette.Scripts;
using Cassette.Stylesheets;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostingEnvironment.DependencyGraphInteration.Service
{
    public class WcfServiceDependencyGraphInteraction : IInteractWithDependencyGraph
    {
        readonly ICachePerRequestProvider application;

        public WcfServiceDependencyGraphInteraction(ICachePerRequestProvider provider)
        {
            this.application = application;
        }

        private HttpContext GetHttpContext()
        {
            return HttpContext.Current;
        }

        public BundleContainerInteractionResult CreateBundleContainer(CassetteSettings settings, IEnumerable<ICassetteConfiguration> configs)
        {
            return PerformInteraction(hostService =>
            {
                var ret = hostService.AppStart();

                if(ret.Exception != null)
                {
                    throw ret.Exception;
                }

                return new BundleContainerInteractionResult
                {
                    BundleContainer = new BundleContainer(new List<Bundle>())
                };
            });   
        }

        const string ReferencedBundlesContextKey = "Bundle_References_Per_Request";
        public SimpleInteractionResult ReferenceBundle(string path, string location)
        {
            return PerformInteraction(hostService =>
            {
                var itemsBag = GetHttpContext().Items;
                var bundlesForThisRequest = itemsBag[ReferencedBundlesContextKey] as List<BundleRequest> ??  new List<BundleRequest>();

                bundlesForThisRequest.Add(new BundleRequest
                {
                    Path = path,
                    Location = location
                });

                itemsBag[ReferencedBundlesContextKey] = bundlesForThisRequest;

                return new SimpleInteractionResult();
            });
        }

        private readonly static Dictionary<Type, BundleType>  TypeToBundleType = new Dictionary<Type, BundleType>
        {
            {  typeof(StylesheetBundle), BundleType.StyleSheet },
            {  typeof(CombinedStylesheetBundle), BundleType.StyleSheet },
            {  typeof(CombinedScriptBundle), BundleType.Script },
            {  typeof(ScriptBundle), BundleType.Script },
            {  typeof(HtmlTemplateBundle), BundleType.HtmlTemplate },
        };

        public StringInterationResult Render<T>(string location) where T : Bundle
        {
            return PerformInteraction(hostService =>
            {
                var type = typeof(T);
                var bundleType = TypeToBundleType[type];
                return hostService.Render(bundleType, location);
            });
        }

        public StreamInterationResult GetAsset(string path)
        {
            return PerformInteraction(hostService =>
            {
                var stream = hostService.GetAsset(path);
                return new StreamInterationResult(stream, metaData: null);
            });
        }

        public StreamInterationResult GetBundle<T>(string path)
            where T: Bundle
        {
            return PerformInteraction(hostService =>
            {
                var type = typeof(T);
                var bundleType = TypeToBundleType[type];
                var stream = hostService.GetBundle(bundleType, path);

                return new StreamInterationResult(stream, metaData: null);
            });
        }

        private T PerformInteraction<T>(Func<ICassetteHost, T> action)
            where T : IInterationResult, new()
        {
            try
            {
                using(var pipeFactory = new ChannelFactory<ICassetteHost>(new NetNamedPipeBinding { TransferMode = TransferMode.Streamed},
                                                                new EndpointAddress("net.pipe://localhost/HostingService")))
                {
                    var proxy = pipeFactory.CreateChannel();
                    return action(proxy);
                }
            }
            catch (Exception exception)
            {
                return new T
                {
                    Exception = exception
                };
            }
        }
    }
}
