using System.Reflection;
using Machine.Specifications;
using Machine.VSTestAdapter.Specs.Fixtures;

namespace Machine.VSTestAdapter.Specs
{
    public abstract class SampleRunnerSpecs
    {
        static CompileContext compiler;
        static Assembly assembly;

        Establish context = () =>
        {
            compiler = new CompileContext();

            var assemblyPath = compiler.Compile(SampleFixture.Code);
            assembly = Assembly.LoadFile(assemblyPath);
        };

        Cleanup after = () =>
            compiler.Dispose();

        protected static Assembly GetAssembly()
        {
            return assembly;
        }
    }
}
