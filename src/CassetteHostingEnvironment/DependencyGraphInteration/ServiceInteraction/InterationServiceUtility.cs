using System;
using System.ServiceModel;
using System.Xml;
using Cassette.DependencyGraphInteration.InterationResults;
using CassetteHostingEnvironment.DependencyGraphInteration.ServiceInteraction;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostingEnvironment.DependencyGraphInteration.Service
{
    public class InterationServiceUtility : IInterationServiceUtility
    {
        public T PerformInteraction<T>(Func<ICassetteHost, T> action)
            where T : IInterationResult, new()
        {
            try
            {
                using (var pipeFactory = new ChannelFactory<ICassetteHost>(GetBinding(),
                                                                          GetEndPoint()))
                {
                    var proxy = pipeFactory.CreateChannel();
                    return action(proxy);
                }
            }
            catch (FaultException<GeneralFault> e)
            {
                return new T
                {
                    Exception = new Exception(e.Message)
                };
            }
        }

        public NetNamedPipeBinding GetBinding()
        {
            return new NetNamedPipeBinding
            {
                TransferMode = TransferMode.Buffered,
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxConnections = 100,
                ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxBytesPerRead = int.MaxValue,
                    MaxStringContentLength = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue,
                    MaxDepth = int.MaxValue
                    
                }
            };
        }

        public EndpointAddress GetEndPoint()
        {
            var address = GetServiceUri() + GetServiceName();
            return new EndpointAddress(address);
        }

        public string GetServiceUri()
        {
            return "net.pipe://localhost/";
        }

        public string GetServiceName()
        {
            return "HostingService";
        }

    }
}
