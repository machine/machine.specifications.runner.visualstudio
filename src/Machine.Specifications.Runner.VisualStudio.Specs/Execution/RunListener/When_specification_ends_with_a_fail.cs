using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    public class When_specification_ends_with_a_fail : WithFakes
    {
        protected static TestCase TestCase;

        static VSProxyAssemblySpecificationRunListener run_listener;
        static SpecificationInfo specification_info = new SpecificationInfo("leader", "field name", "ContainingType", "field_name");

        Establish context = () =>
        {
            The<IFrameworkHandle>()
                .WhenToldTo(f => f.RecordEnd(Param<TestCase>.IsAnything, Param<TestOutcome>.IsAnything))
                .Callback((TestCase testCase, TestOutcome outcome) => TestCase = testCase);

            run_listener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"), An<Settings>());
        };

        Because of = () =>
            run_listener.OnSpecificationEnd(specification_info, Result.Failure(new NotImplementedException()));

        It should_notify_visual_studio_of_the_test_outcome = () =>
            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordEnd(Param<TestCase>.IsNotNull, Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Failed)))
                .OnlyOnce();

        It should_notify_visual_studio_of_the_test_result = () =>
            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordResult(Param<TestResult>.Matches(t =>
                    t.Outcome == TestOutcome.Failed &&
                    t.ComputerName == Environment.MachineName &&
                    t.ErrorMessage == new NotImplementedException().Message &&
                    !string.IsNullOrWhiteSpace(t.ErrorStackTrace))))
                .OnlyOnce();
        
        Behaves_like<TestCaseMapperBehavior> should_tell_visual_studio_the_correct_details;
    }
}
