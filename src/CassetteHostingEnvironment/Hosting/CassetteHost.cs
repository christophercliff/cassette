using System;
using Cassette.DependencyGraphInteration.InterationResults;

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

        public StringInterationResult Render(BundleType type, string location)
        {
            return new StringInterationResult();
        }

        public StreamInterationResult GetAsset(string path)
        {
            return new StreamInterationResult
            {
                NotFound = true,
                Hash = "bwuh!"
            };
        }

        public StreamInterationResult GetBundle(BundleType type, string path)
        {
            return new StreamInterationResult
            {
                ContentType = "Bwuh"
            };
        }
    }
}
