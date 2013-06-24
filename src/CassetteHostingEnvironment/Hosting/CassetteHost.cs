using System;
using System.Collections.Generic;
using System.IO;
using Cassette.DependencyGraphInteration.InterationResults;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;

namespace CassetteHostingEnvironment.Hosting
{
    public class CassetteHost : ICassetteHost
    {
        public SimpleInteractionResult AppStart()
        {
            return new SimpleInteractionResult();
        }

        public SimpleInteractionResult ReferenceBundle(string path, string location)
        {
            return new SimpleInteractionResult();
        }

        public StringInterationResult Render(IEnumerable<BundleRequest> referencedBundles, BundleType type, string location)
        {
            return new StringInterationResult();
        }

        public Stream GetAsset(string path)
        {
            return File.Open(@"c:\test1.txt", FileMode.Open);
        }

        public StreamMetaDataResult GetAssetMetaData(string path)
        {
            throw new NotImplementedException();
        }

        public Stream GetBundle(BundleType type, string path)
        {
            return File.Open(@"c:\test2.txt", FileMode.Open);
        }

        public StreamMetaDataResult GetBundleMetaData(BundleType type, string path)
        {
            throw new NotImplementedException();
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
    }
}
