using System;
using System.Collections.Generic;
#if !NETSTANDARD
using System.IO;
#endif
using System.Linq;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.BuiltIn;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter
{
    public class MSpecTestAdapterDiscoverer
    {
        readonly ISpecificationDiscoverer discoverer;

        public MSpecTestAdapterDiscoverer()
            : this(new BuiltInSpecificationDiscoverer())
        {
        }

        public MSpecTestAdapterDiscoverer(ISpecificationDiscoverer discoverer)
        {
            this.discoverer = discoverer;
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            Settings settings = Settings.Parse(discoveryContext.RunSettings?.SettingsXml);
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
#if !NETSTANDARD
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
