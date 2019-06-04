using System;
using System.IO;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public abstract class With_AssemblyExecutionSetup : WithFakes
    {
        protected static ISpecificationExecutor Executor;
        protected static string AssemblyPath;

        Establish context = () =>
        {
            Executor = new SpecificationExecutor();
            AssemblyPath = Path.Combine(Helper.GetTestDirectory(), "SampleSpecs.dll");
        };

        Because of = () =>
            Executor.RunAssembly(AssemblyPath, An<Settings>(), new Uri("bla://executor"), The<IFrameworkHandle>());
    }
}
