using System.IO;

namespace Cassette.DependencyGraphInteration.InterationResults
{
    public class StreamInterationResult : SimpleInteractionResult
    {
        public bool NotFound { get; set; }
        public Stream ResourceStream { get; set; }
        public string Hash { get; set; }
        public string ContentType { get; set; }
    }
}
