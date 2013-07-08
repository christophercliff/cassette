using System;
using System.Collections.Generic;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;

namespace Cassette
{
    public interface ICassetteApplication : IDisposable
    {
        CassetteSettings Settings { get; }
        IEnumerable<Bundle> Bundles { get; }
        T FindBundleContainingPath<T>(string path) where T : Bundle;
        IReferenceBuilder GetReferenceBuilder();
        IInteractWithDependencyGraph GetInteration();
        void SetDependencyInteractionFactory(IDependencyGraphInteractionFactory factory);
    }
}