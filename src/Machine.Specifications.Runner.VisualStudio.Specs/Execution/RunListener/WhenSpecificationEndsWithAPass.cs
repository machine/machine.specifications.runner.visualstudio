using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    class WhenSpecificationEndsWithAPass : WithFakes
    {
        static VSProxyAssemblySpecificationRunListener run_listener;

        protected static TestCase test_case;

        static SpecificationInfo specification_info = new SpecificationInfo("leader", "field name", "ContainingType", "field_name");

        Establish context = () =>
        {
            The<IFrameworkHandle>()
                .WhenToldTo(f => f.RecordEnd(Param<TestCase>.IsAnything, Param<TestOutcome>.IsAnything))
                .Callback((TestCase testCase, TestOutcome outcome) => test_case = testCase);

            run_listener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"), An<Settings>());
        };

        Because of = () =>
            run_listener.OnSpecificationEnd(specification_info, Result.Pass());

        It should_notify_visual_studio_of_the_test_outcome = () => 
            The<IFrameworkHandle>()
                .WasToldTo(f => f.RecordEnd(Param<TestCase>.IsNotNull, Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed)))
                .OnlyOnce();

        It should_notify_visual_studio_of_the_test_result = () => 
            The<IFrameworkHandle>()
                .WasToldTo(f => f.RecordResult(Param<TestResult>.Matches(result => 
                    result.Outcome == TestOutcome.Passed && 
                    result.ComputerName == Environment.MachineName &&
                    result.ErrorMessage == null &&
                    result.ErrorStackTrace == null
                )))
                .OnlyOnce();
        
        Behaves_like<TestCaseMapperBehavior> should_tell_visual_studio_the_correct_details;
    }
}
