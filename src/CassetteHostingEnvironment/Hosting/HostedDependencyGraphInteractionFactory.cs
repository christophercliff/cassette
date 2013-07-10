using Cassette;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;
using Cassette.DependencyGraphInteration.InMemory;

namespace CassetteHostingEnvironment.Hosting
{
    public class HostedDependencyGraphInteractionFactory : IDependencyGraphInteractionFactory
    {
        readonly ICassetteApplication application;
        public HostedDependencyGraphInteractionFactory(ICassetteApplication application = null)
        {
            this.application = application;
        }

        public IInteractWithDependencyGraph GetDependencyGraphInteration(CassetteSettings settings)
        {
            return new InMemoryDependencyGraphInteraction(application);
        }
    }
}
