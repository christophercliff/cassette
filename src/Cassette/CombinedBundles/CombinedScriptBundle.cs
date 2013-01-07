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
    public class CombinedScriptBundle : ScriptBundle
    {
        public CombinedScriptBundle(string name, IEnumerable<ScriptBundle> bundles, IEnumerable<string> bundleNames)
            : base(name)
        {
            Bundles = bundles.Cast<Bundle>().ToList();
            BundleNames = new HashSet<string>();
            foreach (var bundleName in bundleNames)
            {
                BundleNames.Add(bundleName);
            }
            ContentType = "text/javascript";
        }

        public List<Bundle> Bundles { get; private set; }
        public HashSet<string> BundleNames { get; private set; }

        protected override void ProcessCore(CassetteSettings settings)
        {
            //force the internal bundles to be processed in debug mode.  Otherwise we prematurely get concatenated assets.
            bool isDebug = settings.IsDebuggingEnabled;
            settings.IsDebuggingEnabled = true;

            foreach (var bundle in Bundles)
            {
                bundle.Process(settings);
                assets.AddRange(bundle.Assets);
            }

            CombinedBundleUtility.RemoveAssetReferences(BundleNames, assets);
            Hash = CombinedBundleUtility.ComputeCombinedHash(Bundles);

            settings.IsDebuggingEnabled = isDebug;
            new AssignScriptRenderer().Process(this, settings);

            CombinedBundleUtility.CompressBundle(this, new MicrosoftJavaScriptMinifier(), settings);
        }

        internal override string Render()
        {
            return Renderer.Render(this);
        }

        protected override string UrlBundleTypeArgument
        {
            get { return "scriptbundle"; }
        }
    }
}
