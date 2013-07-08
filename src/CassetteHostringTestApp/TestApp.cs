using System;
using Cassette.DependencyGraphInteration.InterationResults;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;
using CassetteHostingEnvironment.DependencyGraphInteration.Settings;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostringTestApp
{
    class TestApp
    {
        static void Main(string[] args)
        {
            var util = new InterationServiceUtility();
            util.PerformInteraction(host =>
            {
                host.AppStart(new HostedCassetteSettings
                {
                    IsDebug = true,
                    AppDomainAppPath = @"C:\src\zocdoc.web\zocdoc_web\ZocDoc.Web\",
                    AppDomainAppVirtualPath = "/",
                    AssemblyPath = @"C:\src\zocdoc.web\zocdoc_web\ZocDoc.Web\Bin\ZocDoc.WebApp.dll"
                });

                var renderedTest = host.Render(new[]
                {
                    new BundleRequest() { Path = "app_scripts/modules/framework", },
                    new BundleRequest() { Path = "app_scripts/modules/ui", }
                },
                                               BundleType.Script, null);

                var assetPath =
                    renderedTest.ResourceString.Split(new[] { "src=\"" }, StringSplitOptions.None)[1].Split(
                        new[] { "\" type=" }, StringSplitOptions.None)[0];
                assetPath = "~/" + assetPath.Replace("/_cassette/asset/", "").Split('?')[0];

                using (var asset = host.GetAsset(assetPath))
                {
                    var buffer = new byte[1024];
                    var read = 1;
                    var sum = 0;
                    while (read > 0)
                    {
                        read = asset.Read(buffer, sum, buffer.Length - sum);
                        sum += read;
                    }
                    var text = System.Text.Encoding.UTF8.GetString(buffer, 0, sum);

                    Console.Write(text);
                }

                Console.WriteLine("***************");
                Console.WriteLine("***************");
                Console.WriteLine("***************");
                Console.WriteLine("***************");
                Console.WriteLine("***************");

                using (var asset = host.GetAsset(assetPath))
                {
                    var buffer = new byte[1024];
                    var read = 1;
                    var sum = 0;
                    while (read > 0)
                    {
                        read = asset.Read(buffer, sum, buffer.Length - sum);
                        sum += read;
                    }
                    var text = System.Text.Encoding.UTF8.GetString(buffer, 0, sum);

                    Console.Write(text);
                }

                Console.ReadLine();
                return new BundleContainerInteractionResult();
            });
        }
    }
}
