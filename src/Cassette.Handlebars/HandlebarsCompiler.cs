using Cassette.IO;
using Jurassic;

namespace Cassette.HtmlTemplates
{
    public class HandlebarsCompiler : ICompiler
    {
        public HandlebarsCompiler()
        {
            scriptEngine = new ScriptEngine();
            scriptEngine.Execute(Properties.Resources.handlebars
                + Properties.Resources.handlebars_i18n
                + Properties.Resources.compile);
        }
        
        readonly ScriptEngine scriptEngine;
        
        public string Compile(string source, IFile sourceFile)
        {
            return scriptEngine.CallGlobalFunction<string>("compile", source);
        }
    }
}