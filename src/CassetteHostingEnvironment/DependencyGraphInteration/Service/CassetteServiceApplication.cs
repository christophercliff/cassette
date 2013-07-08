using System;
using Cassette;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;

namespace CassetteHostingEnvironment.DependencyGraphInteration.Service
{
    public class CassetteServiceApplication : CassetteApplicationBase
    {
        public CassetteServiceApplication(IBundleContainer bundleContainer,
                                          CassetteSettings settings,
                                          IDependencyGraphInteractionFactory dependencyGraphFactory)
            : base(bundleContainer, settings, dependencyGraphFactory) { }


        protected override IReferenceBuilder GetOrCreateReferenceBuilder(Func<IReferenceBuilder> create)
        {
            return CreateReferenceBuilder();
        }

        protected override IPlaceholderTracker GetPlaceholderTracker()
        {
            return new NullPlaceholderTracker();
        }

        public void OnPostMapRequestHandler()
        {
            throw new NotImplementedException();
        }

        public void OnPostRequestHandlerExecute()
        {
            throw new NotImplementedException();
        }

    }
}