using Cassette.BundleProcessing;

namespace Cassette.HtmlTemplates
{
    public class CompileHandlebars : AddTransformerToAssets
    {
        public CompileHandlebars()
            : base(new CompileAsset(new HandlebarsCompiler()))
        {
        }
    }
}
