using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter
{
    public partial class MSpecTestAdapter : ITestExecutor
    {
        public void Cancel()
        {
            // Not supported
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //Debugger.Launch();

            foreach (string currentAsssembly in sources)
            {
                try
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format(Strings.EXECUTOR_EXECUTINGIN, currentAsssembly));

                    this.executor.RunAssembly(currentAsssembly, uri, runContext, frameworkHandle);
                }
                catch (Exception ex)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Error, String.Format(Strings.EXECUTOR_ERROR, currentAsssembly, ex.Message));
                }
            }

            frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format("Complete on {0} assemblies ", sources.Count()));
            
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            //Debugger.Launch();
            frameworkHandle.SendMessage(TestMessageLevel.Informational, Strings.EXECUTOR_STARTING);
            int executedSpecCount = 0;
            string currentAsssembly = string.Empty;
            try {
                IEnumerable<IGrouping<string, TestCase>> groupByAssembly = tests.GroupBy(x => x.Source);
                foreach (IGrouping<string, TestCase> grouping in groupByAssembly) {
                    currentAsssembly = grouping.Key;
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, string.Format(Strings.EXECUTOR_EXECUTINGIN, currentAsssembly));

                    List<VisualStudioTestIdentifier> testsToRun = grouping.Select(test => test.ToVisualStudioTestIdentifier()).ToList();

                    this.executor.RunAssemblySpecifications(currentAsssembly, testsToRun, uri, runContext, frameworkHandle);
                    executedSpecCount += grouping.Count();
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format(Strings.EXECUTOR_COMPLETE, executedSpecCount, groupByAssembly.Count()));
            } catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, string.Format(Strings.EXECUTOR_ERROR, currentAsssembly, ex.Message));
            }
            finally
            {
            }
        }
    }
}