using System.ServiceModel;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostringTestApp
{
    class TestApp
    {
        static void Main(string[] args)
        {
            var pipeFactory = new ChannelFactory<ICassetteHost>(new NetNamedPipeBinding { TransferMode = TransferMode.Streamed},
                                                                new EndpointAddress("net.pipe://localhost/HostingService"));

            var proxy = pipeFactory.CreateChannel();
            using(var result = proxy.GetAsset("Path"))
            {
                var buffer = new byte[1024];
                var read = 1;
                var sum = 0;
                while (read > 0)
                {
                    read = result.Read(buffer, sum, buffer.Length - sum);
                    sum += read;
                }
                var text = System.Text.Encoding.UTF8.GetString(buffer, 0, sum);
            }

            using (var result = proxy.GetBundle(BundleType.HtmlTemplate, "Path"))
            {
                var buffer = new byte[1024];
                var read = 1;
                var sum = 0;
                while (read > 0)
                {
                    read = result.Read(buffer, sum, buffer.Length - sum);
                    sum += read;
                }
                var text = System.Text.Encoding.UTF8.GetString(buffer, 0, sum);
            }

            //var result2 = proxy.GetBundle(BundleType.HtmlTemplate, "Path");
            

        }
    }
}
