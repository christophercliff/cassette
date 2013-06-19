using System;
using System.Collections.Generic;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration.InterationResults;
using Cassette.Utilities;

namespace Cassette.DependencyGraphInteration.InMemory
{
    public class InMemoryDependencyGraphInteraction : IInteractWithDependencyGraph
    {
        readonly ICassetteApplication application;

        public InMemoryDependencyGraphInteraction(ICassetteApplication application)
        {
            this.application = application;
        }

        public BundleContainerInteractionResult CreateBundleContainer(CassetteSettings settings, IEnumerable<ICassetteConfiguration> configs)
        {
            return PerformInteraction(() =>
            {
                var bundleContainerFactory = settings.GetBundleContainerFactory(configs);
                return new BundleContainerInteractionResult
                {
                    BundleContainer = bundleContainerFactory.CreateBundleContainer()
                };
            });   
        }

        public SimpleInteractionResult ReferenceBundle(string path, string location)
        {
            return PerformInteraction(() =>
            {
                var referenceBuilder = application.GetReferenceBuilder();
                referenceBuilder.Reference(path, location);
                return new SimpleInteractionResult();
            });
        }

        public StringInterationResult Render<T>(string location) where T : Bundle
        {
            return PerformInteraction(() =>
            {
                var referenceBuilder = application.GetReferenceBuilder();
                return new StringInterationResult
                {
                    ResourceString = referenceBuilder.Render<T>(location)
                };
            });
        }

        public StreamInterationResult GetAsset(string path)
        {
            return PerformInteraction(() =>
            {
                IAsset asset;
                Bundle bundle;
                if (!application.Bundles.TryGetAssetByPath(path, out asset, out bundle))
                {
                    return new StreamInterationResult
                    {
                        NotFound = true
                    };
                }

                return new StreamInterationResult
                {
                    Hash = asset.Hash.ToHexString(),
                    ResourceStream = asset.OpenStream(),
                    ContentType = bundle.ContentType
                };

            });

            
        }

        public StreamInterationResult GetBundle<T>(string path)
            where T: Bundle
        {
            return PerformInteraction(() =>
            {
                var bundle = application.FindBundleContainingPath<T>(path);
                if (bundle == null)
                {
                    return new StreamInterationResult
                    {
                        NotFound = true
                    };
                }

                return new StreamInterationResult
                {
                    Hash = bundle.Hash.ToHexString(),
                    ResourceStream = bundle.OpenStream(),
                    ContentType = bundle.ContentType
                };

            });
        }

        private T PerformInteraction<T>(Func<T> action)
            where T : SimpleInteractionResult, new()
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
    }
}
