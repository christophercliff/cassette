using Cassette.Configuration;

namespace Cassette.DependencyGraphInteration
{
    public interface IDependencyGraphInteractionFactory
    {
        IInteractWithDependencyGraph GetDependencyGraphInteration(CassetteSettings settings);
    }
}
