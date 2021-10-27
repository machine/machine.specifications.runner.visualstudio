using System.Reflection;
using Machine.Fakes;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public abstract class WithAssemblyExecutionSetup : WithFakes
    {
        static MSpecTestAdapter executor;

        static Assembly assembly;

        Establish context = () =>
        {
            executor = new MSpecTestAdapter();

            assembly = typeof(StandardSpec).Assembly;
        };

        Because of = () =>
            executor.RunTests(new[] { assembly.Location }, An<IRunContext>(), The<IFrameworkHandle>());
    }
}
