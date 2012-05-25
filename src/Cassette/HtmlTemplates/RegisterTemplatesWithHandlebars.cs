using Cassette.BundleProcessing;

namespace Cassette.HtmlTemplates
{
    public class RegisterTemplatesWithHandlebars : AddTransformerToAssets
    {
        public RegisterTemplatesWithHandlebars(HtmlTemplateBundle bundle, string javaScriptVariableName)
            : base(new RegisterTemplateWithHandlebars(bundle, javaScriptVariableName))
        {
        }
    }
}