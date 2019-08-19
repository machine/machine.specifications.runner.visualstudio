using System;
using System.Reflection;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Helpers;
using Machine.VSTestAdapter.Specs.Fixtures;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{
    public abstract class With_MultipleSpecExecutionSetup : WithFakes
    {
        static ISpecificationExecutor Executor;
        static CompileContext compiler;
        static Assembly assembly;

        protected static VisualStudioTestIdentifier[] SpecificationsToRun;

        Establish context = () => 
        {
            compiler = new CompileContext();
            Executor = new SpecificationExecutor();

            var assemblyPath = compiler.Compile(SampleFixture.Code);
            assembly = Assembly.LoadFile(assemblyPath);
        };

        Because of = () => 
            Executor.RunAssemblySpecifications(assembly.Location, SpecificationsToRun, The<Settings>(), new Uri("bla://executor"), The<IFrameworkHandle>());

        Cleanup after = () =>
            compiler.Dispose();
    }
}
