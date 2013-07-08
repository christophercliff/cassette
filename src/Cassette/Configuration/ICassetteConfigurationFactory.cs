using System.Collections.Generic;

namespace Cassette.Configuration
{
    public interface ICassetteConfigurationFactory
    {
        IEnumerable<ICassetteConfiguration> CreateCassetteConfigurations();
    }
}