using System;
using System.Collections.Generic;
using System.Linq;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;

namespace Cassette
{
    public abstract class CassetteApplicationBase : ICassetteApplication
    {
        readonly CassetteSettings _settings;
        readonly IBundleContainer _bundleContainer;
        IDependencyGraphInteractionFactory _factory;

        protected CassetteApplicationBase(IBundleContainer bundleContainer,
                                          CassetteSettings settings,
                                          IDependencyGraphInteractionFactory factory)
        {
            _settings = settings;
            _bundleContainer = bundleContainer;
            _factory = factory;
        }

        public CassetteSettings Settings
        {
            get { return _settings; }
        }

        public IEnumerable<Bundle> Bundles
        {
            get { return _bundleContainer.Bundles; }
        }

        public virtual T FindBundleContainingPath<T>(string path)
            where T : Bundle
        {
            return _bundleContainer.FindBundlesContainingPath(path).OfType<T>().FirstOrDefault();
        }

        public IReferenceBuilder GetReferenceBuilder()
        {
            return GetOrCreateReferenceBuilder(CreateReferenceBuilder);
        }

        public IInteractWithDependencyGraph GetInteration()
        {
            return _factory.GetDependencyGraphInteration(_settings);
        }

        protected abstract IReferenceBuilder GetOrCreateReferenceBuilder(Func<IReferenceBuilder> create);
        protected abstract IPlaceholderTracker GetPlaceholderTracker();

        public void Dispose()
        {
            _bundleContainer.Dispose();
        }

        protected IReferenceBuilder CreateReferenceBuilder()
        {
            return new ReferenceBuilder(
                _bundleContainer,
                _settings.BundleFactories,
                GetPlaceholderTracker(),
                _settings
            );
        }

        protected IPlaceholderTracker CreatePlaceholderTracker()
        {
            return Settings.IsHtmlRewritingEnabled
                       ? (IPlaceholderTracker)new PlaceholderTracker()
                       : new NullPlaceholderTracker();
        }

        public void SetDependencyInteractionFactory(IDependencyGraphInteractionFactory factory)
        {
            _factory = factory;
        }
    }
}