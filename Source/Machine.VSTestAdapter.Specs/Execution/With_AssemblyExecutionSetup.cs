using System;
using System.IO;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{

    public abstract class With_AssemblyExecutionSetup : WithFakes
    {
        protected static ISpecificationExecutor Executor;
        protected static string AssemblyPath;

        Establish context = () => {
            Executor = new SpecificationExecutor();    
            AssemblyPath = Path.Combine(Helper.GetTestDebugDirectory(), "SampleSpecs.dll");
        };

        Because of = () => {
            Executor.RunAssembly(AssemblyPath, An<Settings>(), new Uri("bla://executor"), The<IFrameworkHandle>());
        };
    }
}
