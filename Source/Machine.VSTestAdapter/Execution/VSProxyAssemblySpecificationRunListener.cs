using Machine.Specifications;
using Machine.Specifications.Runner;
using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Machine.VSTestAdapter.Helpers;

namespace Machine.VSTestAdapter.Execution
{
    public class VSProxyAssemblySpecificationRunListener : MarshalByRefObject, ISpecificationRunListener
    {
        private readonly IFrameworkHandle frameworkHandle;
        private readonly string assemblyPath;

        private RunStats currentRunStats;
        readonly Uri executorUri;

        public VSProxyAssemblySpecificationRunListener(string assemblyPath, IFrameworkHandle frameworkHandle, Uri executorUri)
        {
            if (executorUri == null)
                throw new ArgumentNullException(nameof(executorUri));
            if (assemblyPath == null)
                throw new ArgumentNullException(nameof(assemblyPath));
            if (frameworkHandle == null)
                throw new ArgumentNullException(nameof(frameworkHandle));

            this.frameworkHandle = frameworkHandle;
            this.assemblyPath = assemblyPath;
            this.executorUri = executorUri;
        }

        public void OnFatalError(ExceptionResult exception)
        {
            if (this.currentRunStats != null)
            {
                this.currentRunStats.Stop();
                this.currentRunStats = null;
            }

            this.frameworkHandle.SendMessage(TestMessageLevel.Error, Strings.RUNERROR + Environment.NewLine + exception.ToString());
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            TestCase testCase = ConvertSpecificationToTestCase(specification);
            this.frameworkHandle.RecordStart(testCase);
            this.currentRunStats = new RunStats();
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            if (this.currentRunStats != null)
                this.currentRunStats.Stop();

            TestCase testCase = ConvertSpecificationToTestCase(specification);

            this.frameworkHandle.RecordEnd(testCase, MapSpecificationResultToTestOutcome(result));
            this.frameworkHandle.RecordResult(ConverResultToTestResult(testCase, result, this.currentRunStats));
        }

        #region Mapping
        private TestCase ConvertSpecificationToTestCase(SpecificationInfo specification)
        {
            VisualStudioTestIdentifier vsTestId = specification.ToVisualStudioTestIdentifier();

            return new TestCase(vsTestId.FullyQualifiedName, this.executorUri, this.assemblyPath) {
                DisplayName = vsTestId.DisplayName,
            };
        }

        private static TestOutcome MapSpecificationResultToTestOutcome(Result result)
        {
            switch (result.Status)
            {
                case Status.Failing:
                    return TestOutcome.Failed;
                case Status.Passing:
                    return TestOutcome.Passed;
                case Status.Ignored:
                    return TestOutcome.Skipped;
                case Status.NotImplemented:
                    return TestOutcome.NotFound;
                default:
                    return TestOutcome.None;
            }
        }

        private static TestResult ConverResultToTestResult(TestCase testCase, Result result, RunStats runStats)
        {
            TestResult testResult = new TestResult(testCase) {
                ComputerName = Environment.MachineName,
                Outcome = MapSpecificationResultToTestOutcome(result),
            };

            if (result.Exception != null) 
            {
                testResult.ErrorMessage = result.Exception.Message;
                testResult.ErrorStackTrace = result.Exception.ToString();
            }

            if (runStats != null) 
            {
                testResult.StartTime = runStats.Start;
                testResult.EndTime = runStats.End;
                testResult.Duration = runStats.Duration;
            }

            return testResult;
        }

        #endregion


        #region Stubs
        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
        }

        public void OnContextEnd(ContextInfo context)
        {
        }

        public void OnContextStart(ContextInfo context)
        {
        }

        public void OnRunEnd()
        {
        }

        public void OnRunStart()
        {
        }
        #endregion
    }
}