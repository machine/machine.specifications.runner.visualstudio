using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio
{
    public partial class MSpecTestAdapter : ITestExecutor
    {
        public void Cancel()
        {
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Specifications.");

            var settings = GetSettings(runContext);

            foreach (var assembly in sources.Distinct())
            {
                try
                {
#if !NETSTANDARD
                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(assembly)), "Machine.Specifications.dll")))
                    {
                        frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine.Specifications.dll not found for {assembly}");
                        continue;
                    }
#endif

                    frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Executing tests in {assembly}");

                    executor.RunAssembly(assembly, settings, uri, frameworkHandle);
                }
                catch (Exception ex)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Error, $"Machine Specifications Visual Studio Test Adapter - Error while executing specifications in assembly {assembly} - {ex.Message}");
                }
            }

            frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Complete on {sources.Count()} assemblies ");
            
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Executing Specifications.");

            var executedSpecCount = 0;

            var settings = GetSettings(runContext);

            var assembly = string.Empty;

            try
            {

                foreach (var grouping in tests.GroupBy(x => x.Source))
                {
                    assembly = grouping.Key;
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Executing tests in {assembly}");

                    var testsToRun = grouping
                        .Select(test => test.ToVisualStudioTestIdentifier())
                        .ToArray();

                    executor.RunAssemblySpecifications(assembly, testsToRun, settings, uri, frameworkHandle);
                    executedSpecCount += grouping.Count();
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Execution Complete - {executedSpecCount} specifications in {tests.GroupBy(x => x.Source).Count()} assemblies.");
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, $"Machine Specifications Visual Studio Test Adapter - Error while executing specifications in assembly {assembly} - {ex.Message}");
            }
        }

        private static Settings GetSettings(IDiscoveryContext runContext)
        {
            return Settings.Parse(runContext?.RunSettings?.SettingsXml);
        }
    }
}
