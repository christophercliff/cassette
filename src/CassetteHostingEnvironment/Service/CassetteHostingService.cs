using System;
using System.ServiceModel;
using System.ServiceProcess;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;
using CassetteHostingEnvironment.DependencyGraphInteration.Settings;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostingEnvironment
{
    /// <summary>
    /// Trivial windows service to host the cassette WCF entry point. 
    /// </summary>
    public class CassetteHostingService : ServiceBase
    {
        public const string CassetteServiceName = "CassetteHostingService";

        public static void Main()
        {
            //Run();\
            var service = new CassetteHostingService();
            service.OnStart(null);
            Console.ReadLine();
        }

        public ServiceHost ServiceHost = null;

        public CassetteHostingService()
        {
            ServiceName = CassetteServiceName;
        }

        protected override void OnStart(string[] args)
        {
            if (ServiceHost != null)
            {
                ServiceHost.Close();
            }

            ServiceHost = new ServiceHost(typeof(CassetteHost), new Uri("net.pipe://localhost"));

            ServiceHost.AddServiceEndpoint(typeof(ICassetteHost),
                new NetNamedPipeBinding
                {
                    TransferMode = TransferMode.Streamed,
                    MaxReceivedMessageSize = 256000
                },
                "HostingService");

            ServiceHost.Open();
        }

        protected override void OnStop()
        {
            if (ServiceHost != null)
            {
                ServiceHost.Close();
                ServiceHost = null;
            }
        }
    }
}
