using Machine.Specifications.Runner;
using System;
using System.Collections.Generic;

namespace Machine.Specifications.VSRunner
{
    public interface IAppDomainExecutor
    {
        void RunAllTestsInAssembly(string pathToAssembly, ISpecificationRunListener specificationRunListener);

        void RunTestsInAssembly(string pathToAssembly, IEnumerable<string> specsToRun, Uri adapterUri,
            Func<bool> checkHasBeenCancelled,
            Action<string> sendErrorMessage,
            Action<string, string> recordStart,
            Action<string, string, int> recordEnd,
            Action<string, string, DateTime, DateTime, string, string, int> recordResult);
    }
}