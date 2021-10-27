using System.Collections.Generic;

namespace Machine.Specifications.Runner.VisualStudio.Discovery
{
    public interface ISpecificationDiscoverer
    {
        IEnumerable<MSpecTestCase> DiscoverSpecs(string assemblyPath);
    }
}
