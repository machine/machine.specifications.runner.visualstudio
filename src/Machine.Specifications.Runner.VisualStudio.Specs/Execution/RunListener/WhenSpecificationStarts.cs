using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    class WhenSpecificationStarts : WithFakes
    {
        static VSProxyAssemblySpecificationRunListener run_listener;

        protected static TestCase test_case;

        Establish context = () =>
        {
            The<IFrameworkHandle>()
                .WhenToldTo(f => f.RecordStart(Param<TestCase>.IsAnything))
                .Callback((TestCase testCase) => test_case = testCase);

            run_listener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"), An<Settings>());
        };

        Because of = () =>
            run_listener.OnSpecificationStart(new SpecificationInfo("leader", "field name", "ContainingType", "field_name"));

        It should_notify_visual_studio = () =>
            The<IFrameworkHandle>().WasToldTo(f => f.RecordStart(Param<TestCase>.IsNotNull));
        
        Behaves_like<TestCaseMapperBehavior> should_tell_visual_studio_the_correct_details;
    }
}
