using System;
using System.Collections.Generic;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;

namespace Cassette
{
    abstract class CassetteApplicationContainerFactoryBase<T>
        where T : ICassetteApplication
    {
        readonly ICassetteConfigurationFactory cassetteConfigurationFactory;
        readonly CassetteConfigurationSection configurationSection;
        readonly string physicalDirectory;
        readonly string virtualDirectory;
        readonly object creationLock = new object();
        IEnumerable<ICassetteConfiguration> cassetteConfigurations;

        protected CassetteApplicationContainerFactoryBase(ICassetteConfigurationFactory cassetteConfigurationFactory, CassetteConfigurationSection configurationSection, string physicalDirectory, string virtualDirectory)
        {
            this.cassetteConfigurationFactory = cassetteConfigurationFactory;
            this.configurationSection = configurationSection;
            this.physicalDirectory = physicalDirectory;
            this.virtualDirectory = virtualDirectory;
        }

        protected abstract bool ShouldWatchFileSystem { get; }

        protected abstract string PhysicalApplicationDirectory { get; }

        protected abstract string GetConfigurationVersion();

        protected abstract T CreateCassetteApplicationCore(IBundleContainer bundleContainer, CassetteSettings settings);

        public virtual CassetteApplicationContainer<T> CreateContainer()
        {
            cassetteConfigurations = CreateCassetteConfigurations();
            return CreateContainerFromConfiguration();
        }

        CassetteApplicationContainer<T> CreateContainerFromConfiguration()
        {
            var container = new CassetteApplicationContainer<T>(CreateApplication);
            if (ShouldWatchFileSystem)
            {
                container.CreateNewApplicationWhenFileSystemChanges(PhysicalApplicationDirectory);
            }
            return container;
        }

        protected virtual IEnumerable<ICassetteConfiguration> CreateCassetteConfigurations()
        {
            return cassetteConfigurationFactory.CreateCassetteConfigurations();
        }

        T CreateApplication()
        {
            lock (creationLock)
            {
                var cacheVersion = GetConfigurationVersion();
                var settings = new CassetteSettings(cacheVersion);
                var dependencyInteractionFactory = new DependencyGraphInteractionFactory(null);
                var dependencyInteractor = dependencyInteractionFactory.GetDependencyGraphInteration();
                var result = dependencyInteractor.CreateBundleContainer(settings, CassetteConfigurations);

                if(result.Exception != null)
                {
                    throw new Exception();
                }

                Trace.Source.TraceInformation("IsDebuggingEnabled: {0}", settings.IsDebuggingEnabled);
                Trace.Source.TraceInformation("Cache version: {0}", cacheVersion);
                Trace.Source.TraceInformation("Creating Cassette application object");

                return CreateCassetteApplicationCore(result.BundleContainer, settings);
            }
        }

        protected IEnumerable<ICassetteConfiguration> CassetteConfigurations
        {
            get { return cassetteConfigurations; }
        }

    }
}