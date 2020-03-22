using System;
using System.Collections.Generic;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.BuiltIn;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter
{
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [ExtensionUri(ExecutorUri)]
    [DefaultExecutorUri(ExecutorUri)]
    public class MSpecTestAdapter : ITestDiscoverer, ITestExecutor
    {
        const string ExecutorUri = "executor://machine.vstestadapter";
        public static readonly Uri Uri = new Uri(ExecutorUri);

        readonly MSpecTestAdapterDiscoverer testAdapterDiscoverer;
        readonly MSpecTestAdapterExecutor testAdapterExecutor;

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
            //Debugger.Launch();
            testAdapterDiscoverer.DiscoverTests(sources, discoveryContext, logger, discoverySink);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //Debugger.Launch();
            testAdapterExecutor.RunTests(tests, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //Debugger.Launch();
            testAdapterExecutor.RunTests(sources, runContext, frameworkHandle);
        }

        public void Cancel()
        {
        }
    }
}
