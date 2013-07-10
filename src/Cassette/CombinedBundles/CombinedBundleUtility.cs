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
        public static void RemoveAssetReferences(IEnumerable<string> bundleNames, List<IAsset> assets, CassetteSettings settings)
        {
            if(settings.IsDebuggingEnabled)
            {
                return;
            }

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
            new AssignHash().Process(bundle, settings);

            if (!settings.IsDebuggingEnabled)
            {
                bundle.ConcatenateAssets();
                new MinifyAssets(minifier).Process(bundle, settings);
            }
        }

    }
}
