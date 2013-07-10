using System.Runtime.Serialization;

namespace CassetteHostingEnvironment.Hosting
{
    [DataContract(Namespace = "CassetteHostingService")]
    public class GeneralFault
    {
        [DataMember]
        public string Message { get; set; }
    }
}
