using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter.Execution
{
    public class SpecificationExecutor : ISpecificationExecutor
    {
        public void RunAssembly(string source, Uri executorUri, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            source = Path.GetFullPath(source);

            using (var scope = new IsolatedAppDomainExecutionScope<AppDomainExecutor>(source))
            {
                VSProxyAssemblySpecificationRunListener listener = new VSProxyAssemblySpecificationRunListener(source, frameworkHandle, executorUri);

                AppDomainExecutor executor = scope.CreateInstance();
                executor.RunAllTestsInAssembly(source, listener);
            }
        }

        public void RunAssemblySpecifications(string assemblyPath, IEnumerable<VisualStudioTestIdentifier> specifications, Uri executorUri, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);

            using (var scope = new IsolatedAppDomainExecutionScope<AppDomainExecutor>(assemblyPath))
            {
                VSProxyAssemblySpecificationRunListener listener = new VSProxyAssemblySpecificationRunListener(assemblyPath, frameworkHandle, executorUri);

                AppDomainExecutor executor = scope.CreateInstance();

                executor.RunTestsInAssembly(assemblyPath, specifications, listener);
            }
        }
    }
}