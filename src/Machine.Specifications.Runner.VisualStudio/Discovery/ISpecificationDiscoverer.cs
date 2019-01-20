using System.Collections.Generic;

namespace Machine.VSTestAdapter.Discovery
{
    public interface ISpecificationDiscoverer
    {
        IEnumerable<MSpecTestCase> DiscoverSpecs(string assemblyPath);
    }
}