using System.Collections.Generic;
using System.Web;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;

namespace Cassette.Web.DependencyGraphImplementations
{
    public class BundleRequestedPerRequestProvider : ICachePerRequestProvider<List<BundleRequest>> 
    {
        const string ReferencedBundlesContextKey = "Bundle_References_Per_Request";
        public List<BundleRequest> GetCachedValue()
        {
            return HttpContext.Current.Items[ReferencedBundlesContextKey] as List<BundleRequest>;
        }

        public void SetCachedValue(List<BundleRequest> value)
        {
            HttpContext.Current.Items[ReferencedBundlesContextKey] = value;
        }
    }
}
