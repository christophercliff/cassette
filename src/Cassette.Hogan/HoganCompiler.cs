using Cassette.IO;
using Jurassic;

namespace Cassette.HtmlTemplates
{
    public class HoganCompiler : ICompiler
    {
        private static object lockObject = new object();
        private static readonly ScriptEngine _scriptEngine = new ScriptEngine();
        static bool needToCreateHogan = true;

        public HoganCompiler()
        {
            if (needToCreateHogan)
            {
                lock(lockObject)
                {
                    if (needToCreateHogan)
                    {
                        needToCreateHogan = false;
                        _scriptEngine.Execute(Properties.Resources.hogan);
                    }
                }
            }
        }

        public string Compile(string source, IFile sourceFile)
        {
            return _scriptEngine.CallGlobalFunction<string>("compile", source);
        }
    }
}