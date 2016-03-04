using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Machine.VSTestAdapter.Execution;

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
            string currentAsssembly = string.Empty;

            try
            {
                ISpecificationExecutor specificationExecutor = this.adapterFactory.CreateExecutor();

                foreach (string source in sources)
                {
                    currentAsssembly = source;

                    frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format(Strings.EXECUTOR_EXECUTINGIN, currentAsssembly));

                    specificationExecutor.RunAssembly(currentAsssembly, MSpecTestAdapter.uri, runContext, frameworkHandle);
                }

                frameworkHandle.SendMessage(TestMessageLevel.Informational, String.Format("Complete on {0} assemblies ", sources.Count()));
            }
            catch (Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, String.Format(Strings.EXECUTOR_ERROR, currentAsssembly, ex.Message));
            }
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