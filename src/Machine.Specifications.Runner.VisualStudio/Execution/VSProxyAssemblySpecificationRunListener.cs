using System;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Execution
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
            this.frameworkHandle = frameworkHandle ?? throw new ArgumentNullException(nameof(frameworkHandle));
            this.assemblyPath = assemblyPath ?? throw new ArgumentNullException(nameof(assemblyPath));
            this.executorUri = executorUri ?? throw new ArgumentNullException(nameof(executorUri));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void OnFatalError(ExceptionResult exception)
        {
            if (currentRunStats != null)
            {
                currentRunStats.Stop();
                currentRunStats = null;
            }

            frameworkHandle.SendMessage(TestMessageLevel.Error, "Machine Specifications Visual Studio Test Adapter - Fatal error while executing test." + Environment.NewLine + exception);
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            var testCase = ConvertSpecificationToTestCase(specification, settings);

            frameworkHandle.RecordStart(testCase);
            currentRunStats = new RunStats();
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            currentRunStats?.Stop();

            var testCase = ConvertSpecificationToTestCase(specification, settings);

            frameworkHandle.RecordEnd(testCase, MapSpecificationResultToTestOutcome(result));
            frameworkHandle.RecordResult(ConvertResultToTestResult(testCase, result, currentRunStats));
        }

        public void OnContextStart(ContextInfo context)
        {
            currentContext = context;
        }

        public void OnContextEnd(ContextInfo context)
        {
            currentContext = null;
        }

        private TestCase ConvertSpecificationToTestCase(SpecificationInfo specification, Settings settings)
        {
            var vsTestId = specification.ToVisualStudioTestIdentifier(currentContext);

            return new TestCase(vsTestId.FullyQualifiedName, executorUri, assemblyPath)
            {
                DisplayName = settings.DisableFullTestNameInOutput
                    ? specification.Name
                    : $"{currentContext?.TypeName}.{specification.FieldName}"
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

        private static TestResult ConvertResultToTestResult(TestCase testCase, Result result, RunStats runStats)
        {
            var testResult = new TestResult(testCase)
            {
                ComputerName = Environment.MachineName,
                Outcome = MapSpecificationResultToTestOutcome(result),
                DisplayName = testCase.DisplayName
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
    }
}
