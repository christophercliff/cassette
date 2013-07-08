using System;
using System.ServiceModel;
using Cassette.DependencyGraphInteration.InterationResults;
using CassetteHostingEnvironment.Hosting;

namespace CassetteHostingEnvironment.DependencyGraphInteration.ServiceInteraction
{
    public interface IInterationServiceUtility
    {
        T PerformInteraction<T>(Func<ICassetteHost, T> action)
            where T : IInterationResult, new();

        NetNamedPipeBinding GetBinding();
        EndpointAddress GetEndPoint();
        string GetServiceUri();
        string GetServiceName();
    }
}
