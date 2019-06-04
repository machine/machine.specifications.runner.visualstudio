using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    public class When_specification_starts : WithFakes
    {
        protected static TestCase TestCase;

        static VSProxyAssemblySpecificationRunListener run_listener;

        Establish context = () =>
        {
            The<IFrameworkHandle>()
                .WhenToldTo(x => x.RecordStart(Param<TestCase>.IsAnything))
                .Callback((TestCase testCase) => TestCase = testCase);

            run_listener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"), An<Settings>());
        };


        Because of = () =>
            run_listener.OnSpecificationStart(new SpecificationInfo("leader", "field name", "ContainingType", "field_name"));

        It should_notify_visual_studio = () =>
            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordStart(Param<TestCase>.IsNotNull));
        
        Behaves_like<TestCaseMapperBehavior> should_tell_visual_studio_the_correct_details;
    }
}
