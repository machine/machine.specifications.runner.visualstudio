using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Helpers;

namespace Machine.Specifications.Runner.VisualStudio.Discovery
{
    public class BuiltInSpecificationDiscoverer : ISpecificationDiscoverer
    {
        public IEnumerable<MSpecTestCase> DiscoverSpecs(string assemblyFilePath)
        {
#if NETFRAMEWORK
            using (IsolatedAppDomainExecutionScope<TestDiscoverer> scope = new IsolatedAppDomainExecutionScope<TestDiscoverer>(assemblyFilePath))
            {
                TestDiscoverer discoverer = scope.CreateInstance();
#else
                TestDiscoverer discoverer = new TestDiscoverer();
#endif
                return discoverer.DiscoverTests(assemblyFilePath).ToList();
#if NETFRAMEWORK
            }
#endif
        }
    }
}
