using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter.Execution
{
    public class SpecificationExecutor : ISpecificationExecutor
    {
        public void RunAssemblySpecifications(string assemblyPath,
                                              VisualStudioTestIdentifier[] specifications,
                                              Settings settings,
                                              Uri executorUri,
                                              IFrameworkHandle frameworkHandle)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);

#if !NETSTANDARD
            using (var scope = new IsolatedAppDomainExecutionScope<TestExecutor>(assemblyPath)) {
                TestExecutor executor = scope.CreateInstance();
#else
                TestExecutor executor = new TestExecutor();
#endif
                VSProxyAssemblySpecificationRunListener listener = new VSProxyAssemblySpecificationRunListener(assemblyPath, frameworkHandle, executorUri, settings);

                executor.RunTestsInAssembly(assemblyPath, specifications, listener);
#if !NETSTANDARD
           }
#endif
        }
    }
}
