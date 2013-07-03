using System;
using System.Collections.Generic;
using Cassette;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;
using Cassette.DependencyGraphInteration.InterationResults;
using Cassette.HtmlTemplates;
using Cassette.ScriptAndTemplate;
using Cassette.Scripts;
using Cassette.Stylesheets;
using CassetteHostingEnvironment.DependencyGraphInteration.ServiceInteraction;
using CassetteHostingEnvironment.DependencyGraphInteration.Settings;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostingEnvironment.DependencyGraphInteration.Service
{
    public class WcfServiceDependencyGraphInteraction : IInteractWithDependencyGraph
    {
        readonly ICachePerRequestProvider<List<BundleRequest>> _provider;
        readonly IInterationServiceUtility _utility;
        readonly HostedCassetteSettings _settings;

        public WcfServiceDependencyGraphInteraction(CassetteSettings settings,
                                                    ICachePerRequestProvider<List<BundleRequest>> provider,
                                                    IInterationServiceUtility utility)
        {
            _settings = new HostedCassetteSettings
            {
                AppDomainAppPath = settings.AppDomainAppPath,
                AppDomainAppVirtualPath = settings.AppDomainAppVirtualPath,
                AssemblyPath = settings.AssemblyPath,
                IsDebug = settings.IsDebuggingEnabled
            };

            _provider = provider;
            _utility = utility;
        }

        public BundleContainerInteractionResult CreateBundleContainer(CassetteSettings settings, 
                                                                      IEnumerable<ICassetteConfiguration> configs)
        {
            return _utility.PerformInteraction(hostService =>
            {
                var ret = hostService.AppStart(_settings);

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

        public SimpleInteractionResult ReferenceBundle(string path, string location)
        {
            return _utility.PerformInteraction(hostService =>
            {
                var bundlesForThisRequest = _provider.GetCachedValue() ??  new List<BundleRequest>();

                bundlesForThisRequest.Add(new BundleRequest
                {
                    Path = path,
                    Location = location
                });

                _provider.SetCachedValue(bundlesForThisRequest);

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
            return _utility.PerformInteraction(hostService =>
            {
                var type = typeof(T);
                var bundleType = TypeToBundleType[type];
                var bundles = _provider.GetCachedValue();
                return hostService.Render(bundles, bundleType, location);
            });
        }

        public StreamInterationResult GetAsset(string path)
        {
            return _utility.PerformInteraction(hostService =>
            {
                var metaData = GetMetaData(() => hostService.GetAssetMetaData(path));
                var stream = hostService.GetAsset(path);
                return new StreamInterationResult(stream, metaData);
            });
        }

        public StreamInterationResult GetBundle<T>(string path)
            where T: Bundle
        {
            return _utility.PerformInteraction(hostService =>
            {
                var type = typeof(T);
                var bundleType = TypeToBundleType[type];
                var metaData = GetMetaData(() => hostService.GetAssetMetaData(path));
                var stream = hostService.GetBundle(bundleType, path);

                return new StreamInterationResult(stream, metaData);
            });
        }

        private StreamMetaDataResult GetMetaData(Func<StreamMetaDataResult> getMetaData)
        {
            var metaData = getMetaData();

            if (metaData.Exception != null)
            {
                throw metaData.Exception;
            }

            return metaData;
        }


        public ImageExistsInteractionResult ImageExists(string path)
        {
            return _utility.PerformInteraction(hostService =>
            {
                return hostService.ImageExists(path);
            });
        }
    }
}
