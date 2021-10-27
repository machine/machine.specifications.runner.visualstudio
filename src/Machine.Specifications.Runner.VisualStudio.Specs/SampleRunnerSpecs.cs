using System.Reflection;
using Machine.Specifications.Runner.VisualStudio.Fixtures;

namespace Machine.Specifications.Runner.VisualStudio.Specs
{
    public abstract class SampleRunnerSpecs
    {
        static Assembly assembly;

        Establish context = () =>
            assembly = typeof(StandardSpec).Assembly;

        protected static Assembly GetAssembly()
        {
            return assembly;
        }
    }
}
