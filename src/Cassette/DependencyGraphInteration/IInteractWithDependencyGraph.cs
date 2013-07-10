using System.Collections.Generic;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration.InterationResults;

namespace Cassette.DependencyGraphInteration
{
    public interface IInteractWithDependencyGraph
    {
        BundleContainerInteractionResult CreateBundleContainer(CassetteSettings settings, IEnumerable<ICassetteConfiguration> configs);
        SimpleInteractionResult ReferenceBundle(string path, string location);
        StringInterationResult Render<T>(string location) 
              where T: Bundle;
        StreamInterationResult GetAsset(string path);
        StreamInterationResult GetBundle<T>(string path)
            where T : Bundle;
        ImageExistsInteractionResult ImageExists(string path);
    }
}
