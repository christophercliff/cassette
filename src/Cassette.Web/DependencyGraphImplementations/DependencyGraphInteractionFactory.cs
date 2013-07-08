using Cassette.Configuration;
using Cassette.DependencyGraphInteration.InMemory;
using Cassette.Web.DependencyGraphImplementations;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;

namespace Cassette.DependencyGraphInteration
{
    public class DependencyGraphInteractionFactory : IDependencyGraphInteractionFactory
    {
        readonly ICassetteApplication application;
        public DependencyGraphInteractionFactory(ICassetteApplication application = null)
        {
            this.application = application;
        }

        public IInteractWithDependencyGraph GetDependencyGraphInteration(CassetteSettings settings)
        {
            if (settings.UseOutOfProcessCassette && settings.IsDebuggingEnabled)
            {
                return new WcfServiceDependencyGraphInteraction(settings,
                                                                new BundleRequestedPerRequestProvider(),
                                                                new InterationServiceUtility());
            }

            return new InMemoryDependencyGraphInteraction(application);
        }
    }
}
