using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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

        private MSpecTestAdapterFactory adapterFactory;
        private Assembly vsObjectModel;

        public MSpecTestAdapter()
            : this(null)
        {
        }

        public MSpecTestAdapter(MSpecTestAdapterFactory adapterFactory)
        {
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

            if (adapterFactory == null)
            {
                this.adapterFactory = new MSpecTestAdapterFactory();
                return;
            }

            this.adapterFactory = adapterFactory;
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
                    ISpecificationDiscoverer specificationDiscoverer = this.adapterFactory.CreateDiscover();

                    // only bother discovering if mspec is available and the assembly to san has a reference to mspec
                    if (specificationDiscoverer.SourceDirectoryContainsMSpec(assemblyPath) && specificationDiscoverer.AssemblyContainsMSpecReference(assemblyPath))
                    {
                        sourcesWithSpecs++;

                        // indicate which assembly we are looking in
                        logger.SendMessage(TestMessageLevel.Informational, string.Format(Strings.DISCOVERER_LOOKINGIN, assemblyPath));

                        // do the actual discovery
                        foreach (TestCase discoveredTest in this.GetTestCases((ISpecificationDiscoverer)specificationDiscoverer, assemblyPath))
                        {
                            discoveredSpecCount++;
                            if (discoverySink != null)
                            {
                                discoverySink.SendTestCase(discoveredTest);
                            }
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

        public IEnumerable<TestCase> GetTestCases(ISpecificationDiscoverer discoverer, string sourcePath)
        {
            if (discoverer != null)
            {
                // discover the mspec tests in the assembly and convert them to vstest cases
                foreach (MSpecTestCase mspecTestCase in discoverer.EnumerateSpecs(sourcePath))
                {
                    yield return SpecTestHelper.GetVSTestCaseFromMSpecTestCase(sourcePath, mspecTestCase, MSpecTestAdapter.uri, CreateTrait);
                }
            }
        }

        private dynamic CreateTrait(string traitName, string traitValue)
        {
            return vsObjectModel.CreateInstance("Microsoft.VisualStudio.TestPlatform.ObjectModel.Trait", false, BindingFlags.CreateInstance, null, new object[] { traitName, traitValue }, null, null);
        }
    }

}