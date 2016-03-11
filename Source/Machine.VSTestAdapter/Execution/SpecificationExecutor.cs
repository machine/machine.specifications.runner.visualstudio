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
        public void RunAssembly(string source, Settings settings, Uri executorUri, IFrameworkHandle frameworkHandle)
        {
            source = Path.GetFullPath(source);

            using (var scope = new IsolatedAppDomainExecutionScope<AppDomainExecutor>(source)) {
                VSProxyAssemblySpecificationRunListener listener = new VSProxyAssemblySpecificationRunListener(source, frameworkHandle, executorUri, settings);

                AppDomainExecutor executor = scope.CreateInstance();
                executor.RunAllTestsInAssembly(source, listener);
            }
        }

        public void RunAssemblySpecifications(string assemblyPath,
                                              IEnumerable<VisualStudioTestIdentifier> specifications,
                                              Settings settings,
                                              Uri executorUri,
                                              IFrameworkHandle frameworkHandle)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);

            using (var scope = new IsolatedAppDomainExecutionScope<AppDomainExecutor>(assemblyPath))
            {
                VSProxyAssemblySpecificationRunListener listener = new VSProxyAssemblySpecificationRunListener(assemblyPath, frameworkHandle, executorUri, settings);

                AppDomainExecutor executor = scope.CreateInstance();

                executor.RunTestsInAssembly(assemblyPath, specifications, listener);
            }
        }
    }
}