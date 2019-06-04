using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Helpers;

namespace Machine.Specifications.Runner.VisualStudio.Discovery.BuiltIn
{
    public class BuiltInSpecificationDiscoverer : ISpecificationDiscoverer
    {
        public IEnumerable<MSpecTestCase> DiscoverSpecs(string assemblyFilePath)
        {
#if !NETSTANDARD
            using (var scope = new IsolatedAppDomainExecutionScope<TestDiscoverer>(assemblyFilePath)) {
                var discoverer = scope.CreateInstance();
#else
                var discoverer = new TestDiscoverer();
#endif
                return discoverer.DiscoverTests(assemblyFilePath).ToArray();

#if !NETSTANDARD
            }
#endif
        }
    }
}
