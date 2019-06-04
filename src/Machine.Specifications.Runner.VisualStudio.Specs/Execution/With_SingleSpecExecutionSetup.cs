using System;
using System.IO;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public abstract class With_SingleSpecExecutionSetup : WithFakes
    {
        protected static ISpecificationExecutor Executor;
        protected static string AssemblyPath;
        protected static VisualStudioTestIdentifier SpecificationToRun;

        Establish context = () =>
        {
            Executor = new SpecificationExecutor();
            AssemblyPath = Path.Combine(Helper.GetTestDirectory(), "SampleSpecs.dll");
        };

        Because of = () =>
            Executor.RunAssemblySpecifications(AssemblyPath, new[] { SpecificationToRun }, The<Settings>(), new Uri("bla://executor"), The<IFrameworkHandle>());
    }
}
