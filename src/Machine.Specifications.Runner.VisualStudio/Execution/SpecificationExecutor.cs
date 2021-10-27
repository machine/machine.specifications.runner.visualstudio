using System;
using System.Collections.Generic;
using System.IO;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class SpecificationExecutor : ISpecificationExecutor
    {
        public void RunAssemblySpecifications(string assemblyPath,
                                              IEnumerable<VisualStudioTestIdentifier> specifications,
                                              Settings settings,
                                              Uri adapterUri,
                                              IFrameworkHandle frameworkHandle)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);

#if NETFRAMEWORK
            using (var scope = new IsolatedAppDomainExecutionScope<TestExecutor>(assemblyPath))
            {
                var executor = scope.CreateInstance();
#else
                var executor = new TestExecutor();
#endif
                var listener = new VSProxyAssemblySpecificationRunListener(assemblyPath, frameworkHandle, adapterUri, settings);

                executor.RunTestsInAssembly(assemblyPath, specifications, listener);
#if NETFRAMEWORK
            }
#endif
        }
    }
}
