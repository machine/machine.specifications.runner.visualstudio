using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter.Execution
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

#if !NETSTANDARD
            using (var scope = new IsolatedAppDomainExecutionScope<TestExecutor>(assemblyPath)) {
                TestExecutor executor = scope.CreateInstance();
#else
                TestExecutor executor = new TestExecutor();
#endif
                VSProxyAssemblySpecificationRunListener listener = new VSProxyAssemblySpecificationRunListener(assemblyPath, frameworkHandle, adapterUri, settings);

                executor.RunTestsInAssembly(assemblyPath, specifications, listener, frameworkHandle);
#if !NETSTANDARD
            }
#endif
        }
    }
}
