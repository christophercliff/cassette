using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Cassette;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;

namespace CassetteHostingEnvironment.DependencyGraphInteration.Service
{
    public class CassetteServiceContainerFactory : CassetteApplicationContainerFactoryBase<CassetteServiceApplication>
    {
        readonly CassetteConfigurationSection configurationSection;
        readonly string physicalDirectory;
        readonly string virtualDirectory;
        readonly bool isAspNetDebuggingEnabled;

        public CassetteServiceContainerFactory(ICassetteConfigurationFactory cassetteConfigurationFactory,
                                               CassetteConfigurationSection configurationSection,
                                               string physicalDirectory,
                                               string virtualDirectory,
                                               bool isAspNetDebuggingEnabled,
                                               IDependencyGraphInteractionFactory dependencyGraphFactory)
            : base(cassetteConfigurationFactory, configurationSection, physicalDirectory, virtualDirectory, dependencyGraphFactory, isAspNetDebuggingEnabled)
        {
            this.configurationSection = configurationSection;
            this.physicalDirectory = physicalDirectory;
            this.virtualDirectory = virtualDirectory;
            this.isAspNetDebuggingEnabled = isAspNetDebuggingEnabled;
        }

        public override CassetteApplicationContainer<CassetteServiceApplication> CreateContainer()
        {
            var container = base.CreateContainer();
            container.IgnoreFileSystemChange(
                new Regex(
                    "^" + Regex.Escape(Path.Combine(PhysicalApplicationDirectory, "App_Data")),
                    RegexOptions.IgnoreCase
                )
            );
            return container;
        }

        protected override IEnumerable<ICassetteConfiguration> CreateCassetteConfigurations()
        {
            yield return CreateInitialConfiguration();
            foreach (var configuration in base.CreateCassetteConfigurations())
            {
                yield return configuration;
            }
            yield return new AssignUrlGenerator();
        }

        InitialConfiguration CreateInitialConfiguration()
        {
            return new InitialConfiguration(
                configurationSection,
                isAspNetDebuggingEnabled,
                physicalDirectory,
                virtualDirectory
            );
        }

        protected override string GetConfigurationVersion()
        {
            var assemblyVersion = CassetteConfigurations
                .Select(configuration => configuration.GetType().Assembly.FullName)
                .Distinct()
                .Select(name => new AssemblyName(name).Version.ToString());

            var parts = assemblyVersion.Concat(new[] { virtualDirectory });
            return string.Join("|", parts.ToArray());
        }

        protected override CassetteServiceApplication CreateCassetteApplicationCore(IBundleContainer bundleContainer, CassetteSettings settings)
        {
            return new CassetteServiceApplication(bundleContainer, settings, _dependencyGraphFactory);
        }

        protected override bool ShouldWatchFileSystem
        {
            get
            {
                if (configurationSection.WatchFileSystem.HasValue)
                {
                    return configurationSection.WatchFileSystem.Value;
                }
                else
                {
                    return isAspNetDebuggingEnabled;
                }
            }
        }

        protected override string PhysicalApplicationDirectory
        {
            get { return physicalDirectory; }
        }
    }
}
