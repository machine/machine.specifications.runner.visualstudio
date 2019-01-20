using Machine.Specifications;
using Machine.Specifications.Runner;
using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter.Execution
{
    public class VSProxyAssemblySpecificationRunListener :
#if !NETSTANDARD
                                                            MarshalByRefObject,
#endif
                                                            ISpecificationRunListener
    {
        private readonly IFrameworkHandle frameworkHandle;
        private readonly string assemblyPath;
        private readonly Uri executorUri;

        private ContextInfo currentContext;
        private RunStats currentRunStats;
        readonly Settings settings;

        public VSProxyAssemblySpecificationRunListener(string assemblyPath, IFrameworkHandle frameworkHandle, Uri executorUri, Settings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (executorUri == null)
                throw new ArgumentNullException(nameof(executorUri));
            if (assemblyPath == null)
                throw new ArgumentNullException(nameof(assemblyPath));
            if (frameworkHandle == null)
                throw new ArgumentNullException(nameof(frameworkHandle));

            this.frameworkHandle = frameworkHandle;
            this.assemblyPath = assemblyPath;
            this.executorUri = executorUri;
            this.settings = settings;
        }

        public void OnFatalError(ExceptionResult exception)
        {
            if (this.currentRunStats != null)
            {
                this.currentRunStats.Stop();
                this.currentRunStats = null;
            }

            this.frameworkHandle.SendMessage(TestMessageLevel.Error, "Machine Specifications Visual Studio Test Adapter - Fatal error while executing test." + Environment.NewLine + exception.ToString());
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            TestCase testCase = ConvertSpecificationToTestCase(specification, this.settings);
            this.frameworkHandle.RecordStart(testCase);
            this.currentRunStats = new RunStats();
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            if (this.currentRunStats != null)
                this.currentRunStats.Stop();

            TestCase testCase = ConvertSpecificationToTestCase(specification, this.settings);

            this.frameworkHandle.RecordEnd(testCase, MapSpecificationResultToTestOutcome(result));
            this.frameworkHandle.RecordResult(ConverResultToTestResult(testCase, result, this.currentRunStats));
        }

        public void OnContextStart(ContextInfo context)
        {
            currentContext = context;
        }

        public void OnContextEnd(ContextInfo context)
        {
            currentContext = null;
        }


#region Mapping
        private TestCase ConvertSpecificationToTestCase(SpecificationInfo specification, Settings settings)
        {
            VisualStudioTestIdentifier vsTestId = specification.ToVisualStudioTestIdentifier(currentContext);

            return new TestCase(vsTestId.FullyQualifiedName, this.executorUri, this.assemblyPath) {
                DisplayName = settings.DisableFullTestNameInOutput ? specification.Name : $"{this.currentContext?.TypeName}.{specification.FieldName}",
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

        public void OnRunEnd()
        {
        }

        public void OnRunStart()
        {
        }
#endregion
    }
}
