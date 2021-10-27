using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.IO;

namespace Machine.Specifications.Runner.VisualStudio
{
    public class MSpecTestAdapterDiscoverer
    {
        private readonly ISpecificationDiscoverer discoverer;

        public MSpecTestAdapterDiscoverer(ISpecificationDiscoverer discoverer)
        {
            this.discoverer = discoverer;
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            var settings = Settings.Parse(discoveryContext.RunSettings?.SettingsXml);

            DiscoverTests(sources, settings, logger, discoverySink.SendTestCase);
        }

        public void DiscoverTests(IEnumerable<string> sources, Settings settings, IMessageLogger logger, Action<TestCase> discoverySinkAction)
        {
            logger.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Discovering Specifications.");

            var discoveredSpecCount = 0;
            var sourcesWithSpecs = 0;

            var sourcesArray = sources.Distinct().ToArray();

            foreach (var assemblyPath in sourcesArray)
            {
                try
                {
#if NETFRAMEWORK
                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(assemblyPath)), "Machine.Specifications.dll")))
                        continue;
#endif

                    sourcesWithSpecs++;

                    logger.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Discovering...looking in {assemblyPath}");

                    var specs = discoverer.DiscoverSpecs(assemblyPath)
                        .Select(spec => SpecTestHelper.GetVSTestCaseFromMSpecTestCase(assemblyPath, spec, settings.DisableFullTestNameInIDE, MSpecTestAdapter.Uri))
                        .ToList();

                    foreach (var discoveredTest in specs)
                    {
                        discoveredSpecCount++;
                        discoverySinkAction(discoveredTest);
                    }
                }
                catch (Exception discoverException)
                {
                    logger.SendMessage(TestMessageLevel.Error, $"Machine Specifications Visual Studio Test Adapter - Error while discovering specifications in assembly {assemblyPath}." + Environment.NewLine + discoverException);
                }
            }

            logger.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Discovery Complete - {discoveredSpecCount} specifications in {sourcesWithSpecs} of {sourcesArray.Length} assemblies scanned.");
        }
    }
}
