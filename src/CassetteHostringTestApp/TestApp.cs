using System.ServiceModel;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostringTestApp
{
    class TestApp
    {
        static void Main(string[] args)
        {
            var pipeFactory = new ChannelFactory<ICassetteHost>(new NetNamedPipeBinding(),
                                                                new EndpointAddress("net.pipe://localhost/HostingService"));

            var proxy = pipeFactory.CreateChannel();
            var result = proxy.GetAsset("Path");
            var result2 = proxy.GetBundle(BundleType.HtmlTemplate, "Path");
        }
    }
}
