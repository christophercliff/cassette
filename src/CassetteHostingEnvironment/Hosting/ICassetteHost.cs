using System.IO;
using System.ServiceModel;
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
        Stream GetAsset(string path);

        [OperationContract]
        Stream GetBundle(BundleType type, string path);
    }
}
