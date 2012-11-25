using Machine.Specifications.Runner;
using System;

namespace Machine.Specifications.VSRunner
{
    public class SpecificationRunListener : ISpecificationRunListener
    {
        private string source;
        private RunStats currentRunStats;
        private Func<bool> checkHasBeenCancelled;
        private Action<string> sendErrorMessage;
        private Action<string, string> recordStart;
        private Action<string, string, int> recordEnd;
        private Action<string, string, DateTime, DateTime, string, string, int> recordResult;

        public SpecificationRunListener(string source, Func<bool> checkHasBeenCancelled, Action<string> sendErrorMessage,
            Action<string, string> recordStart,
            Action<string, string, int> recordEnd,
            Action<string, string, DateTime, DateTime, string, string, int> recordResult)
        {
            this.source = source;
            this.checkHasBeenCancelled = checkHasBeenCancelled;
            this.recordStart = recordStart;
            this.recordEnd = recordEnd;
            this.recordResult = recordResult;
        }

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

        public void OnFatalError(ExceptionResult exception)
        {
            if (this.currentRunStats != null)
            {
                this.currentRunStats.Stop();
                this.currentRunStats = null;
            }
        }

        public void OnRunEnd()
        {
        }

        public void OnRunStart()
        {
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            string specificationName = specification.FieldName;
            string fullyQualifiedName = string.Format("{0}::{1}", specification.ContainingType, (object)specificationName);
            string displayName = specificationName.Replace("_", " ");
            this.RecordStart(fullyQualifiedName, displayName);
            this.currentRunStats = new RunStats();
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            this.currentRunStats.Stop();

            string specificationName = specification.FieldName;
            string fullyQualifiedName = string.Format("{0}::{1}", specification.ContainingType, (object)specificationName);
            string displayName = specificationName.Replace("_", " ");

            int testResult = this.GetVSTestOutcomeFromMSpecResult(result);
            this.RecordEnd(fullyQualifiedName, displayName, testResult);
            this.RecordResult(fullyQualifiedName, displayName, this.currentRunStats.StartTime, this.currentRunStats.EndTime,
                result.Exception != null ? result.Exception.Message : string.Empty, result.Exception != null ? result.Exception.StackTrace : null, testResult);

            this.currentRunStats = null;

            // check if the run has been cancelled
            this.IsRunCancelled();
        }

        private int GetVSTestOutcomeFromMSpecResult(Result result)
        {
            //None = 0,
            //Passed = 1,
            //Failed = 2,
            //Skipped = 3,
            //NotFound = 4,

            switch (result.Status)
            {
                case Status.Failing:
                    return 2;
                case Status.Passing:
                    return 1;
                case Status.Ignored:
                    return 3;
                default:
                    return 4;
            }
        }

        public void IsRunCancelled()
        {
            if (this.checkHasBeenCancelled())
            {
                this.SendErrorMessage(Strings.RUNCANCELLED);
                throw new Exception("Cancelled");
            }
        }

        private void SendErrorMessage(string errorMessage)
        {
            this.sendErrorMessage(errorMessage);
        }

        private void RecordStart(string testFullyQualifiedName, string testDisplayName)
        {
            this.recordStart(testFullyQualifiedName, testDisplayName);
        }

        private void RecordEnd(string testFullyQualifiedName, string testDisplayName, int outCome)
        {
            this.recordEnd(testFullyQualifiedName, testDisplayName, outCome);
        }

        private void RecordResult(string testFullyQualifiedName, string testDisplayName, DateTime startTime, DateTime endTime, string errorMessage, string errorStackTrace, int outCome)
        {
            this.recordResult(testFullyQualifiedName, testDisplayName, startTime, endTime, errorMessage, errorStackTrace, outCome);
        }
    }
}