using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Discovery.BuiltIn;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Configuration;
using System.IO;

namespace Machine.VSTestAdapter
{
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [ExtensionUri("executor://machine.vstestadapter")]
    [DefaultExecutorUri("executor://machine.vstestadapter")]
    public partial class MSpecTestAdapter : ITestDiscoverer
    {
        public const string ExecutorUri = "executor://machine.vstestadapter";
        public const string VSObjectModelAssemblyName = "Microsoft.VisualStudio.TestPlatform.ObjectModel";
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
            // indicate start of discovery
            logger.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Adapter - Discovering Specifications.");
            int discoveredSpecCount = 0;
            int sourcesWithSpecs = 0;

            Settings settings = GetSettings(discoveryContext);
            
            foreach (string assemblyPath in sources.Distinct())
            {
                try
                {
#if !NETSTANDARD
                    if (!File.Exists(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(assemblyPath)), "Machine.Specifications.dll")))
                        continue;
#endif

                    sourcesWithSpecs++;

                    logger.SendMessage(TestMessageLevel.Informational, string.Format("Machine Specifications Visual Studio Test Adapter - Discovering...looking in {0}", assemblyPath));

                    List<TestCase> specs = discoverer.DiscoverSpecs(assemblyPath)
                        .Select(spec => SpecTestHelper.GetVSTestCaseFromMSpecTestCase(assemblyPath, spec, settings.DisableFullTestNameInIDE, MSpecTestAdapter.uri))
                        .ToList();

                    foreach (TestCase discoveredTest in specs)
                    {
                        discoveredSpecCount++;
                        if (discoverySink != null)
                        {
                            discoverySink.SendTestCase(discoveredTest);
                        }
                    }
                }
                catch (Exception discoverException)
                {
                    logger.SendMessage(TestMessageLevel.Error, string.Format("Machine Specifications Visual Studio Test Adapter - Error while discovering specifications in assembly {0} - {1}", assemblyPath, discoverException.Message));
                }
            }

            // indicate that we are finished discovering
            logger.SendMessage(TestMessageLevel.Informational, string.Format("Machine Specifications Visual Studio Test Adapter - Discovery Complete - {0} specifications in {2} of {1} assemblies scanned.", discoveredSpecCount, sources.Count(), sourcesWithSpecs));
        }
    }

}
