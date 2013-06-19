using Cassette.DependencyGraphInteration.InMemory;

namespace Cassette.DependencyGraphInteration
{
    public class DependencyGraphInteractionFactory : IDependencyGraphInteractionFactory
    {
        readonly ICassetteApplication application;
        public DependencyGraphInteractionFactory(ICassetteApplication application)
        {
            this.application = application;
        }

        public IInteractWithDependencyGraph GetDependencyGraphInteration()
        {
            return new InMemoryDependencyGraphInteraction(application);
        }
    }
}
