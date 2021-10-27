using System.Reflection;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Fixtures;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

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
