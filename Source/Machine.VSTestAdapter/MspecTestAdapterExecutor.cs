using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Machine.VSTestAdapter
{
    public partial class MSpecTestAdapter : ITestExecutor
    {
        public void Cancel()
        {
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //Debugger.Launch();
            // this a temporary hack until I can figure out why running the specs per assembly directly using mspec does not work with a large number of specifications
            // when they are run diectly the first 100 or so specs run fine and then an error occurs saying it has taken more than 10 seconds and is being stopped
            // for now we just rediscover and run them like that, makes no sense
            TestCaseCollector collector = new TestCaseCollector();
            this.DiscoverTests(sources, runContext, frameworkHandle, collector);
            this.RunTests(collector.TestCases, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //Debugger.Launch();
            frameworkHandle.SendMessage(TestMessageLevel.Informational, Strings.EXECUTOR_STARTING);
            int executedSpecCount = 0;
            string currentAsssembly = string.Empty;
            try
            {
                ISpecificationExecutor specificationExecutor = this.adapterFactory.CreateExecutor();
                IEnumerable<IGrouping<string, TestCase>> groupBySource = tests.GroupBy(x => x.Source);
                foreach (IGrouping<string, TestCase> grouping in groupBySource)
                {
                    currentAsssembly = grouping.Key;
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, string.Format(Strings.EXECUTOR_EXECUTINGIN, currentAsssembly));
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
    }
}