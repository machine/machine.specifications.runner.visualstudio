using Machine.Specifications.Runner;
using System.Collections.Generic;

namespace Machine.Specifications.VSRunner
{
    public interface IAppDomainExecutor
    {
        void RunAllTestsInAssembly(string pathToAssembly, ISpecificationRunListener specificationRunListener);

        void RunTestsInAssembly(string pathToAssembly, IEnumerable<string> specsToRun, ISpecificationRunListener specificationRunListener);
    }
}