using System.Collections.Generic;
using System.Reflection;
using Machine.Specifications;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Specs.Fixtures;

namespace Machine.VSTestAdapter.Specs.Discovery
{
    public abstract class With_DiscoverySetup<TDiscoverer>
        where TDiscoverer : ISpecificationDiscoverer, new()
    {
        static string AssemblyPath;
        static ISpecificationDiscoverer Discoverer;
        static CompileContext compiler;
        static Assembly assembly;

        protected static IEnumerable<MSpecTestCase> Results;

        Establish context = () =>
        {
            compiler = new CompileContext();
            Discoverer = new TDiscoverer();

            var assemblyPath = compiler.Compile(SampleFixture.Code);
            assembly = Assembly.LoadFile(assemblyPath);
        };

        Because of = () =>
            Results = Discoverer.DiscoverSpecs(AssemblyPath);

        Cleanup after = () =>
            compiler.Dispose();
    }
}
