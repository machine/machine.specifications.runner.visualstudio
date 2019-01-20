using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter.Discovery.BuiltIn
{
    public class BuiltInSpecificationDiscoverer : ISpecificationDiscoverer
    {
        public IEnumerable<MSpecTestCase> DiscoverSpecs(string assemblyFilePath)
        {

#if !NETSTANDARD
            using (IsolatedAppDomainExecutionScope<TestDiscoverer> scope = new IsolatedAppDomainExecutionScope<TestDiscoverer>(assemblyFilePath)) {
                TestDiscoverer discoverer = scope.CreateInstance();
#else
                TestDiscoverer discoverer = new TestDiscoverer();
#endif
                return discoverer.DiscoverTests(assemblyFilePath).ToList();

#if !NETSTANDARD
            }
#endif
        }
    }
}