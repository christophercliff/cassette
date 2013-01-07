using System;
using Cassette.BundleProcessing;
using Cassette.Configuration;
using Cassette.HtmlTemplates;
using Cassette.Scripts;

namespace Cassette.ScriptAndTemplate
{
    public class ScriptAndTemplateBundle : ScriptBundle
    {
        public ScriptAndTemplateBundle(string name, ScriptBundle bundle, HtmlTemplateBundle templateBundle, Func<IBundleProcessor<HtmlTemplateBundle>> templateProcessor)
            : base(name)
        {
            ScriptBundle = bundle;
            HtmlTemplateBundle = templateBundle;
            ContentType = "text/javascript";
            TemplateProcessor = templateProcessor();
            ScriptProcessor = new ScriptPipeline();
        }

        public HtmlTemplateBundle HtmlTemplateBundle { get; private set; }
        public ScriptBundle ScriptBundle { get; private set; }

        public IBundleProcessor<ScriptBundle> ScriptProcessor { get; set; } 
        public IBundleProcessor<HtmlTemplateBundle> TemplateProcessor { get; set; }

        protected override void ProcessCore(CassetteSettings settings)
        {
            //force the internal bundles to be processed in debug mode.  Otherwise we prematurely get concatenated assets.
            bool isDebug = settings.IsDebuggingEnabled;
            settings.IsDebuggingEnabled = true;

            ScriptProcessor.Process(ScriptBundle, settings);
            TemplateProcessor.Process(HtmlTemplateBundle, settings);
           
            var hash = new byte[ScriptBundle.Hash.Length + HtmlTemplateBundle.Hash.Length];
            ScriptBundle.Hash.CopyTo(hash, 0);
            HtmlTemplateBundle.Hash.CopyTo(hash, ScriptBundle.Hash.Length);
            Hash = hash;

            assets.AddRange(HtmlTemplateBundle.Assets);
            assets.AddRange(ScriptBundle.Assets);

            foreach (var reference in ScriptBundle.References)
            {
                AddReference(reference);
            }

            settings.IsDebuggingEnabled = isDebug;
            new AssignScriptRenderer().Process(this, settings);
            if (!settings.IsDebuggingEnabled)
            {
                ConcatenateAssets();
                new MinifyAssets(new MicrosoftJavaScriptMinifier()).Process(this, settings);
            }

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
