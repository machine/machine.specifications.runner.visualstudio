using System;
using System.Collections.Generic;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio
{
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [ExtensionUri(ExecutorUri)]
    [DefaultExecutorUri(ExecutorUri)]
    public class MSpecTestAdapter : ITestDiscoverer, ITestExecutor
    {
        private const string ExecutorUri = "executor://machine.vstestadapter";

        public static readonly Uri Uri = new Uri(ExecutorUri);

        private readonly MSpecTestAdapterDiscoverer testAdapterDiscoverer;

        private readonly MSpecTestAdapterExecutor testAdapterExecutor;

        public MSpecTestAdapter()
            : this(new BuiltInSpecificationDiscoverer(), new SpecificationExecutor(), new SpecificationFilterProvider())
        {
        }

        public MSpecTestAdapter(ISpecificationDiscoverer discoverer, ISpecificationExecutor executor, ISpecificationFilterProvider specificationFilterProvider)
        {
            testAdapterDiscoverer = new MSpecTestAdapterDiscoverer(discoverer);
            testAdapterExecutor = new MSpecTestAdapterExecutor(executor, testAdapterDiscoverer, specificationFilterProvider);
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            testAdapterDiscoverer.DiscoverTests(sources, discoveryContext, logger, discoverySink);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            testAdapterExecutor.RunTests(tests, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            testAdapterExecutor.RunTests(sources, runContext, frameworkHandle);
        }

        public void Cancel()
        {
        }
    }
}
