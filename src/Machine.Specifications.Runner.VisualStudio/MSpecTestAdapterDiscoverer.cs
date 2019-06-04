using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Discovery.BuiltIn;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio
{
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [ExtensionUri("executor://machine.vstestadapter")]
    [DefaultExecutorUri("executor://machine.vstestadapter")]
    public partial class MSpecTestAdapter : ITestDiscoverer
    {
        public const string ExecutorUri = "executor://machine.vstestadapter";

        private static Uri uri = new Uri(ExecutorUri);

        public MSpecTestAdapter()
            : this(new BuiltInSpecificationDiscoverer(), new SpecificationExecutor())
        {
        }

        readonly ISpecificationDiscoverer discoverer;
        readonly ISpecificationExecutor executor;

        public MSpecTestAdapter(ISpecificationDiscoverer discoverer, ISpecificationExecutor executor)
        {
            this.executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this.discoverer = discoverer ?? throw new ArgumentNullException(nameof(discoverer));
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            logger.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Discovering Specifications.");

            var discoveredSpecCount = 0;
            var sourcesWithSpecs = 0;

            var settings = GetSettings(discoveryContext);
            
            foreach (var assemblyPath in sources.Distinct())
            {
                try
                {
#if !NETSTANDARD
                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(assemblyPath)), "Machine.Specifications.dll")))
                        continue;
#endif

                    sourcesWithSpecs++;

                    logger.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Discovering...looking in {assemblyPath}");

                    List<TestCase> specs = discoverer.DiscoverSpecs(assemblyPath)
                        .Select(x => SpecTestHelper.GetVSTestCaseFromMSpecTestCase(assemblyPath, x, settings.DisableFullTestNameInIDE, uri))
                        .ToList();

                    foreach (TestCase discoveredTest in specs)
                    {
                        discoveredSpecCount++;
                        discoverySink?.SendTestCase(discoveredTest);
                    }
                }
                catch (Exception discoverException)
                {
                    logger.SendMessage(TestMessageLevel.Error, $"Machine Specifications Visual Studio Test Adapter - Error while discovering specifications in assembly {assemblyPath} - {discoverException.Message}");
                }
            }

            logger.SendMessage(TestMessageLevel.Informational, $"Machine Specifications Visual Studio Test Adapter - Discovery Complete - {discoveredSpecCount} specifications in {sourcesWithSpecs} of {sources.Count()} assemblies scanned.");
        }
    }

}
