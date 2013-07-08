using Cassette.IO;

namespace Cassette.Configuration
{
    public class InitialConfiguration : ICassetteConfiguration
    {
        readonly CassetteConfigurationSection configurationSection;
        readonly bool isAspNetDebuggingEnabled;
        readonly string sourceDirectory;
        readonly string virtualDirectory;

        public InitialConfiguration()
        {
            
        }

        public InitialConfiguration(CassetteConfigurationSection configurationSection, bool isAspNetDebuggingEnabled, string sourceDirectory, string virtualDirectory)
        {
            this.configurationSection = configurationSection;
            this.isAspNetDebuggingEnabled = isAspNetDebuggingEnabled;
            this.sourceDirectory = sourceDirectory;
            this.virtualDirectory = virtualDirectory;
        }

        public void Configure(BundleCollection bundles, CassetteSettings settings)
        {
            if (configurationSection == null)
            {
                return;
            }

            settings.IsDebuggingEnabled = configurationSection.Debug.HasValue ? configurationSection.Debug.Value : isAspNetDebuggingEnabled;
            settings.IsHtmlRewritingEnabled = configurationSection.RewriteHtml;
            settings.AllowRemoteDiagnostics = configurationSection.AllowRemoteDiagnostics;
            settings.SourceDirectory = new FileSystemDirectory(sourceDirectory);
            settings.CacheDirectory = new IsolatedStorageDirectory(() => IsolatedStorageContainer.IsolatedStorageFile);
            settings.UrlModifier = new VirtualDirectoryPrepender(virtualDirectory);
        }
    }
}