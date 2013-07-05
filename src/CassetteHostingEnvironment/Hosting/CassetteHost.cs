using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using Cassette;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration.InterationResults;
using Cassette.HtmlTemplates;
using Cassette.Scripts;
using Cassette.Stylesheets;
using Cassette.Utilities;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;
using CassetteHostingEnvironment.DependencyGraphInteration.Settings;

namespace CassetteHostingEnvironment.Hosting
{
    [ServiceBehaviorAttribute(InstanceContextMode = InstanceContextMode.PerCall,
                              ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CassetteHost : ICassetteHost
    {
        private static CassetteApplicationContainer<CassetteServiceApplication> _container;

        public SimpleInteractionResult AppStart(HostedCassetteSettings settings)
        {
            return PerformInteraction(() =>
            {
                var newAssemblyLocation = DiskBackedBundleCache.CacheDirectory + Guid.NewGuid().ToString();
                File.Copy(settings.AssemblyPath, newAssemblyLocation);

                var assembly = Assembly.LoadFile(newAssemblyLocation);

                var factory = new CassetteServiceContainerFactory(
                    new AssemblyScanningCassetteConfigurationFactory(new[] { assembly }),
                    new CassetteConfigurationSection
                    {
                        RewriteHtml = false,
                        WatchFileSystem = settings.IsDebug
                    },
                    settings.AppDomainAppPath,
                    settings.AppDomainAppVirtualPath,
                    settings.IsDebug,
                    new HostedDependencyGraphInteractionFactory(null)
                    );

                _container = factory.CreateContainer();
                CassetteApplicationContainer.SetApplicationAccessor(() => _container.Application);
                var lazyEvalulatedApp = _container.Application;
                _container.Application.SetDependencyInteractionFactory(new HostedDependencyGraphInteractionFactory(lazyEvalulatedApp));
                return new SimpleInteractionResult();
            });
        }

        public StringInterationResult Render(IEnumerable<BundleRequest> referencedBundles, BundleType type, string location)
        {
            return PerformInteraction(() =>
            {
                var referenceBuilder = _container.Application.GetReferenceBuilder();
                foreach(var bundleToReference in referencedBundles)
                {
                    referenceBuilder.Reference(bundleToReference.Path, bundleToReference.Location);
                }

                string ret;
                switch(type)
                {
                    case BundleType.Script:
                        ret = referenceBuilder.Render<ScriptBundle>(location);
                        break;
                    case BundleType.StyleSheet:
                        ret = referenceBuilder.Render<StylesheetBundle>(location);
                        break;
                    case BundleType.HtmlTemplate:
                        ret = referenceBuilder.Render<HtmlTemplateBundle>(location);
                        break;
                    default:
                        throw new Exception("Unknown bundle type: " + type.ToString());
                }

                return new StringInterationResult
                {
                    ResourceString = ret
                };
            });
        }

        public Stream GetAsset(string path)
        {
            IAsset asset;
            Bundle bundle;
            if (!_container.Application.Bundles.TryGetAssetByPath(path, out asset, out bundle))
            {
                throw new Exception("Path not found.");
            }

            return asset.OpenStream();
        }


        public StreamMetaDataResult GetAssetMetaData(string path)
        {
            return PerformInteraction(() =>
            {
                IAsset asset;
                Bundle bundle;
                if (!_container.Application.Bundles.TryGetAssetByPath(path, out asset, out bundle))
                {
                    return new StreamMetaDataResult
                    {
                        NotFound = true
                    };
                }

                return new StreamMetaDataResult
                {
                    Hash = asset.Hash.ToHexString(),
                    ContentType = bundle.ContentType
                };
            });
        }

        public Stream GetBundle(BundleType type, string path)
        {
            
            var bundle = FindBundle(type, path);
            if (bundle == null)
            {
                throw new Exception("Unknown path: " + path);
            }

            return bundle.OpenStream();   
        }

        private Bundle FindBundle(BundleType type, string path)
        {
            switch (type)
            {
                case BundleType.Script:
                    return  _container.Application.FindBundleContainingPath<ScriptBundle>(path);
                case BundleType.StyleSheet:
                    return _container.Application.FindBundleContainingPath<StylesheetBundle>(path);
                case BundleType.HtmlTemplate:
                    return _container.Application.FindBundleContainingPath<HtmlTemplateBundle>(path);
                default:
                    throw new Exception("Unknown bundle type: " + type.ToString());
            }
        }

        public StreamMetaDataResult GetBundleMetaData(BundleType type, string path)
        {
            return PerformInteraction(() =>
            {
                var bundle = FindBundle(type, path);
                if (bundle == null)
                {
                    return new StreamMetaDataResult
                    {
                        NotFound = true
                    };
                }

                return new StreamMetaDataResult
                {
                    Hash = bundle.Hash.ToHexString(),
                    ContentType = bundle.ContentType
                };
            });
        }

        private T PerformInteraction<T>(Func<T> action)
            where T : IInterationResult, new()
        {
            try
            {
                return action();
            }
            catch (Exception exception)
            {
                return new T
                {
                    Exception = exception
                };
            }
        }

        public ImageExistsInteractionResult ImageExists(string path)
        {
            return PerformInteraction(() =>
            {
                var rawFileReferenceFinder = new RawFileReferenceFinder(path);
                foreach (var bundle in _container.Application.Bundles)
                {
                    bundle.Accept(rawFileReferenceFinder);
                    if (rawFileReferenceFinder.IsRawFileReferenceFound)
                    {
                        return new ImageExistsInteractionResult
                        {
                            ImageExists = true
                        };
                    }
                }
                return new ImageExistsInteractionResult
                {
                    ImageExists = false
                };
            });
        }
    }
}
