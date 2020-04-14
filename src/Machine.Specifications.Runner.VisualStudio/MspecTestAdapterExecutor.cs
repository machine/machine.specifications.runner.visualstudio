using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter
{
    public class MSpecTestAdapterExecutor
    {
        readonly ISpecificationExecutor executor;
        readonly MSpecTestAdapterDiscoverer discover;
        readonly ISpecificationFilterProvider specificationFilterProvider;

        public MSpecTestAdapterExecutor(ISpecificationExecutor executor, MSpecTestAdapterDiscoverer discover, ISpecificationFilterProvider specificationFilterProvider)
        {
            this.executor = executor;
            this.discover = discover;
            this.specificationFilterProvider = specificationFilterProvider;
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Source Specifications.");
            var testsToRun = new List<TestCase>();
            DiscoverTests(sources, runContext, frameworkHandle, testsToRun);
            RunTests(testsToRun, runContext, frameworkHandle);
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Source Specifications Complete.");
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Test Specifications.");
            var totalSpecCount = 0;
            var executedSpecCount = 0;
            var settings = Settings.Parse(runContext.RunSettings?.SettingsXml);
            var currentAssembly = string.Empty;

            try
            {
                var testCases = tests.ToArray();
                foreach (var grouping in testCases.GroupBy(x => x.Source))
                {
                    currentAssembly = grouping.Key;
                    totalSpecCount += grouping.Count();

                    var filteredTests = specificationFilterProvider.FilteredTests(grouping.AsEnumerable(), runContext, frameworkHandle);

                    var testsToRun = filteredTests
                        .Select(test => test.ToVisualStudioTestIdentifier())
                        .ToArray();

                    frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Executing {testsToRun.Length} tests in '{currentAssembly}'");

                    executor.RunAssemblySpecifications(grouping.Key, testsToRun, settings, MSpecTestAdapter.Uri, frameworkHandle);
                    executedSpecCount += testsToRun.Length;
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Execution Complete - {executedSpecCount} of {totalSpecCount} specifications in {testCases.GroupBy(x => x.Source).Count()} assemblies.");
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, $"Machine Specifications Visual Studio Test Adapter - Error while executing specifications in assembly '{currentAssembly}' - {ex}");
            }
        }

        void DiscoverTests(IEnumerable<string> sources, IRunContext discoveryContext, IMessageLogger logger, List<TestCase> testsToRun)
        {
            Settings settings = Settings.Parse(discoveryContext.RunSettings?.SettingsXml);
            discover.DiscoverTests(sources, settings, logger, testsToRun.Add);
        }
    }
}
