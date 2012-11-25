using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.VSTestAdapter
{
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [ExtensionUri("executor://machine.vstestadapter")]
    [DefaultExecutorUri("executor://machine.vstestadapter")]
    public class MSpecTestAdapter : ITestDiscoverer, ITestExecutor
    {
        private static Uri uri = new Uri("executor://machine.vstestadapter");
        public const string ExecutorUri = "executor://machine.vstestadapter";
        private bool cancelled;
        private MSpecTestAdapterFactory adapterFactory;
        private ISpecificationExecutor specificationExecutor;

        public MSpecTestAdapter()
        {
            this.adapterFactory = new MSpecTestAdapterFactory();
        }

        public MSpecTestAdapter(MSpecTestAdapterFactory adapterFactory)
        {
            if (adapterFactory == null)
            {
                throw new ArgumentNullException("adapterFactory");
            }

            this.adapterFactory = adapterFactory;
        }

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            logger.SendMessage(TestMessageLevel.Informational, Strings.DISCOVERER_STARTING);
            int discoveredSpecCount = 0;
            int sourcesWithSpecs = 0;
            foreach (string assemblyPath in sources)
            {
                try
                {
                    ISpecificationDiscoverer specificationDiscoverer = this.adapterFactory.CreateDiscover();
                    if (specificationDiscoverer.SourceDirectoryContainsMSpec(assemblyPath) && specificationDiscoverer.AssemblyContainsMSpecReference(assemblyPath))
                    {
                        sourcesWithSpecs++;
                        logger.SendMessage(TestMessageLevel.Informational, string.Format(Strings.DISCOVERER_LOOKINGIN, assemblyPath));

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
            logger.SendMessage(TestMessageLevel.Informational, string.Format(Strings.DISCOVERER_COMPLETE, discoveredSpecCount, sources.Count(), sourcesWithSpecs));
        }

        public void Cancel()
        {
            if (this.specificationExecutor != null)
            {
            }
            this.cancelled = true;
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            // this a temporary hack until I can figure out why running the specs per assembly directly using mspec does not work with a large number of specifications
            // when they are run diectly the first 100 or so specs run fine and then an error occurs saying it has taken more than 10 seconds and is being stopped
            // for now we just rediscover and run them like that, makes no sense why this works
            TestCaseCollector collector = new TestCaseCollector();
            this.DiscoverTests(sources, runContext, frameworkHandle, collector);
            this.RunTests(collector.TestCases, runContext, frameworkHandle);

            //List<TestCase> list = new List<TestCase>();
            //frameworkHandle.SendMessage(TestMessageLevel.Informational, "Machine Specifications Visual Studio Test Runner: Finding tests...");
            //foreach (string assemblyFilename in sources)
            //{
            //    try
            //    {
            //        ISpecificationDiscoverer specificationDiscoverer = this.adapterFactory.CreateDiscover();
            //        list.AddRange(this.GetTestCases((ISpecificationDiscoverer)specificationDiscoverer, assemblyFilename));
            //    }
            //    catch (Exception ex)
            //    {
            //        frameworkHandle.SendMessage(TestMessageLevel.Error, string.Format("Machine Specifications Visual Studio Test Runner: Exception discovering tests from {0} : {1}.", (object)assemblyFilename, (object)ex));
            //    }
            //}
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, Strings.EXECUTOR_STARTING);
            int executedSpecCount = 0;
            string currentAsssembly = string.Empty;
            try
            {
                specificationExecutor = this.adapterFactory.CreateExecutor();
                IEnumerable<IGrouping<string, TestCase>> groupBySource = tests.GroupBy(x => x.Source);
                foreach (IGrouping<string, TestCase> grouping in groupBySource)
                {
                    currentAsssembly = grouping.Key;
                    specificationExecutor.RunAssemblySpecifications(currentAsssembly, MSpecTestAdapter.uri, runContext, frameworkHandle, grouping);
                    executedSpecCount += grouping.Count();
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format(Strings.EXECUTOR_COMPLETE, executedSpecCount, groupBySource.Count()));
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, string.Format(Strings.EXECUTOR_ERROR, currentAsssembly, ex.Message));
            }
            finally
            {
            }
        }

        public IEnumerable<TestCase> GetTestCases(ISpecificationDiscoverer discoverer, string sourcePath)
        {
            if (discoverer != null)
            {
                foreach (MSpecTestCase mspecTestCase in discoverer.EnumerateSpecs(sourcePath))
                {
                    yield return SpecTestHelper.GetVSTestCaseFromMSpecTestCase(sourcePath, mspecTestCase, MSpecTestAdapter.uri);
                }
            }
        }

        private bool IsCancelled()
        {
            return this.cancelled;
        }
    }
}