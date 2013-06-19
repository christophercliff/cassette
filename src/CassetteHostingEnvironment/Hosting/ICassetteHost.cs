using System.ServiceModel;
using Cassette;
using Cassette.DependencyGraphInteration.InterationResults;

namespace CassetteHostingEnvironment.Hosting
{
    [ServiceContract(Namespace = "CassetteHostingService")]
    public interface ICassetteHost
    {
        [OperationContract]
        SimpleInteractionResult AppStart();

        [OperationContract]
        SimpleInteractionResult ReferenceBundle(string path, string location);

        [OperationContract]
        StringInterationResult Render(BundleType type, string location);

        [OperationContract]
        StreamInterationResult GetAsset(string path);

        [OperationContract]
        StreamInterationResult GetBundle(BundleType type, string path);
    }
}
