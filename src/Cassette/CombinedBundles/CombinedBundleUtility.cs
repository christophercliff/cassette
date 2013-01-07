using System.Collections.Generic;
using System.Linq;
using Cassette.BundleProcessing;
using Cassette.Configuration;
using Cassette.HtmlTemplates;
using Cassette.Manifests;
using Cassette.Scripts;
using Microsoft.Ajax.Utilities;

namespace Cassette.ScriptAndTemplate
{
    public static class CombinedBundleUtility 
    {
        public static byte[] ComputeCombinedHash(List<Bundle> bundles)
        {
            var hashLength = 0;
            foreach (var bundle in bundles)
            {
                hashLength += bundle.Hash.Length;
            }

            var hash = new byte[hashLength];
            var copiedlength = 0;
            foreach (var bundle in bundles)
            {
                bundle.Hash.CopyTo(hash, copiedlength);
                copiedlength += bundle.Hash.Length;
            }

            return hash;
        }

        public static void RemoveAssetReferences(IEnumerable<string> bundleNames, List<IAsset> assets)
        {
            foreach (var bundleName in bundleNames)
            {
                var name = bundleName;
                if (!name.StartsWith("~/"))
                {
                    if (!name.StartsWith("/"))
                    {
                        name = "~/" + name;
                    }
                    else
                    {
                        name = "~" + name;
                    }
                }

                foreach (var asset in assets)
                {
                    var fileAsset = asset as FileAsset;

                    if(fileAsset != null)
                    {
                        var toRemove = fileAsset.References.FirstOrDefault(x => x.Path == name);
                        if (toRemove != null)
                        {
                            fileAsset.RemoveReference(toRemove);
                        }
                    }
                }
            }
        }

        public static void CompressBundle(Bundle bundle, IAssetTransformer minifier, CassetteSettings settings)
        {
            if (!settings.IsDebuggingEnabled)
            {
                bundle.ConcatenateAssets();
                new MinifyAssets(minifier).Process(bundle, settings);
            }
        }

    }
}
