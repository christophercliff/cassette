using System;
using Cassette;
using Cassette.Configuration;

namespace CassetteHostingEnvironment.DependencyGraphInteration.Service
{
    public class CassetteServiceApplication : CassetteApplicationBase
    {
        public CassetteServiceApplication(IBundleContainer bundleContainer, CassetteSettings settings)
            : base(bundleContainer, settings) { }

        public void OnPostMapRequestHandler()
        {
            throw new NotImplementedException();
        }

        public void OnPostRequestHandlerExecute()
        {
            throw new NotImplementedException();
        }

        protected override IReferenceBuilder GetOrCreateReferenceBuilder(Func<IReferenceBuilder> create)
        {
            return CreateReferenceBuilder();
        }

        protected override IPlaceholderTracker GetPlaceholderTracker()
        {
            return new NullPlaceholderTracker();
        }
    }
}