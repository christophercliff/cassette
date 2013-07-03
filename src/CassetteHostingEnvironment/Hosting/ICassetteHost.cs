using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Cassette.DependencyGraphInteration.InterationResults;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;
using CassetteHostingEnvironment.DependencyGraphInteration.Settings;

namespace CassetteHostingEnvironment.Hosting
{
    [ServiceContract(Namespace = "CassetteHostingService")]
    public interface ICassetteHost
    {
        [OperationContract]
        SimpleInteractionResult AppStart(HostedCassetteSettings settings);

        [OperationContract]
        StringInterationResult Render(IEnumerable<BundleRequest> referencedBundles, BundleType type, string location);

        [OperationContract]
        Stream GetAsset(string path);

        [OperationContract]
        StreamMetaDataResult GetAssetMetaData(string path);

        [OperationContract]
        Stream GetBundle(BundleType type, string path);

        [OperationContract]
        StreamMetaDataResult GetBundleMetaData(BundleType type, string path);

        [OperationContract]
        ImageExistsInteractionResult ImageExists(string path);

    }
}
