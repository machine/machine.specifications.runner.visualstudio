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
        public SpecificationExecutor()
        {
        }

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

        public void RunAssemblySpecifications(string source, Uri executorUri, IRunContext runContext, IFrameworkHandle frameworkHandle, IEnumerable<TestCase> specifications)
        {
            source = Path.GetFullPath(source);

            using (var scope = new IsolatedAppDomainExecutionScope<AppDomainExecutor>(source))
            {
                VSProxyAssemblySpecificationRunListener listener = new VSProxyAssemblySpecificationRunListener(source, frameworkHandle, executorUri);

                AppDomainExecutor executor = scope.CreateInstance();

                List<VisualStudioTestIdentifier> testsToRun = specifications.Select(x => x.ToVisualStudioTestIdentifier()).ToList();
                executor.RunTestsInAssembly(source, testsToRun, listener);
            }
        }
    }
}