using System;
using System.IO;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{

    public abstract class With_ExecutionSetup : WithFakes
    {
        protected static ISpecificationExecutor Executor;
        protected static string AssemblyPath;
        protected static VisualStudioTestIdentifier SpecificationToRun;

        Establish context = () => {
            Executor = new SpecificationExecutor();    
            AssemblyPath = Path.Combine(Helper.GetTestDebugDirectory(), "SampleSpecs.dll");
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.Parent+NestedSpec", "should_remember_that_true_is_true");
        };

        Because of = () => {
            Executor.RunAssemblySpecifications(AssemblyPath, new[] { SpecificationToRun }, new Uri("bla://executor"), An<IRunContext>(), The<IFrameworkHandle>());
        };
    }
}
