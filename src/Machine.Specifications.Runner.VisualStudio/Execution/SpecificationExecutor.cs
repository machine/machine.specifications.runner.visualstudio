using System;
using System.IO;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class SpecificationExecutor : ISpecificationExecutor
    {
        public void RunAssembly(string source, Settings settings, Uri executorUri, IFrameworkHandle frameworkHandle)
        {
            source = Path.GetFullPath(source);

#if !NETSTANDARD
            using (var scope = new IsolatedAppDomainExecutionScope<TestExecutor>(source))
            {
                var executor = scope.CreateInstance();
#else
                var executor = new TestExecutor();
#endif

                var listener = new VSProxyAssemblySpecificationRunListener(source, frameworkHandle, executorUri, settings);
                executor.RunAllTestsInAssembly(source, listener);
#if !NETSTANDARD
            }
#endif
        }

        public void RunAssemblySpecifications(
            string assemblyPath,
            VisualStudioTestIdentifier[] specifications,
            Settings settings,
            Uri executorUri,
            IFrameworkHandle frameworkHandle)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);

#if !NETSTANDARD
            using (var scope = new IsolatedAppDomainExecutionScope<TestExecutor>(assemblyPath))
            {
                var executor = scope.CreateInstance();
#else
                var executor = new TestExecutor();
#endif
                var listener = new VSProxyAssemblySpecificationRunListener(assemblyPath, frameworkHandle, executorUri, settings);

                executor.RunTestsInAssembly(assemblyPath, specifications, listener);
#if !NETSTANDARD
            }
#endif
        }
    }
}
