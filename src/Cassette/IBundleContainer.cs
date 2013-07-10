using System;
using System.Collections.Generic;

namespace Cassette
{
    public interface IBundleContainer : IDisposable
    {
        IEnumerable<Bundle> Bundles { get; }
        Bundle[] FindBundlesContainingPath(string path);
        IEnumerable<Bundle> IncludeReferencesAndSortBundles(IEnumerable<Bundle> bundles);
    }
}