using System;

namespace Cassette.DependencyGraphInteration.InterationResults
{
    public interface IInterationResult
    {
        Exception Exception { get; set; }
    }
}
