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
        [FaultContract(typeof(GeneralFault))]
        SimpleInteractionResult AppStart(HostedCassetteSettings settings);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        StringInterationResult Render(IEnumerable<BundleRequest> referencedBundles, BundleType type, string location);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Stream GetAsset(string path);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        StreamMetaDataResult GetAssetMetaData(string path);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Stream GetBundle(BundleType type, string path);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        StreamMetaDataResult GetBundleMetaData(BundleType type, string path);

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        ImageExistsInteractionResult ImageExists(string path);

    }
}
