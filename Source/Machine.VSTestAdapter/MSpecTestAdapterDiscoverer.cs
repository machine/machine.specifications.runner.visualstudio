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
        public static bool UseTraits = false;

        private Assembly vsObjectModel;

        public MSpecTestAdapter()
            : this(new BuiltInSpecificationDiscoverer(), new SpecificationExecutor())
        {
        }

        readonly ISpecificationDiscoverer discoverer;
        readonly ISpecificationExecutor executor;

        public MSpecTestAdapter(ISpecificationDiscoverer discoverer, ISpecificationExecutor executor)
        {
            if (executor == null)
                throw new ArgumentNullException(nameof(executor));
            if (discoverer == null)
                throw new ArgumentNullException(nameof(discoverer));

            this.executor = executor;
            this.discoverer = discoverer;
            // check if a version of visual studio that supports traits is running
            vsObjectModel = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith(VSObjectModelAssemblyName)).SingleOrDefault();
            if (vsObjectModel != null)
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(vsObjectModel.Location);
                if ((fileInfo.FileMajorPart == 11 && fileInfo.ProductBuildPart > 50727) || fileInfo.FileMajorPart >= 12)
                {
                    UseTraits = true;
                }
            }
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            // indicate start of discovery
            logger.SendMessage(TestMessageLevel.Informational, Strings.DISCOVERER_STARTING);
            int discoveredSpecCount = 0;
            int sourcesWithSpecs = 0;

            
            foreach (string assemblyPath in sources)
            {
                try
                {
                    sourcesWithSpecs++;

                    logger.SendMessage(TestMessageLevel.Informational, string.Format(Strings.DISCOVERER_LOOKINGIN, assemblyPath));

                    //Settings config = Settings.Parse(discoveryContext?.RunSettings?.SettingsXml);

                    List<TestCase> specs = discoverer.DiscoverSpecs(assemblyPath)
                        .Select(spec => SpecTestHelper.GetVSTestCaseFromMSpecTestCase(assemblyPath, spec, MSpecTestAdapter.uri, CreateTrait))
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
                    logger.SendMessage(TestMessageLevel.Error, string.Format(Strings.DISCOVERER_ERROR, assemblyPath, discoverException.Message));
                }
            }

            // indicate that we are finished discovering
            logger.SendMessage(TestMessageLevel.Informational, string.Format(Strings.DISCOVERER_COMPLETE, discoveredSpecCount, sources.Count(), sourcesWithSpecs));
        }

        private dynamic CreateTrait(string traitName, string traitValue)
        {
            return vsObjectModel.CreateInstance("Microsoft.VisualStudio.TestPlatform.ObjectModel.Trait", false, BindingFlags.CreateInstance, null, new object[] { traitName, traitValue }, null, null);
        }
    }

}