using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Fakes;
using Machine.Specifications;
using Machine.Specifications.Runner;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution.RunListener
{

    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    public class When_specification_ends_with_a_pass : WithFakes
    {
        static VSProxyAssemblySpecificationRunListener RunListener;
        protected static TestCase TestCase;
        static SpecificationInfo SpecificationInfo = new SpecificationInfo("leader", "field name", "ContainingType", "field_name");

        Establish context = () => {
            The<IFrameworkHandle>()
                .WhenToldTo(f => f.RecordEnd(Param<TestCase>.IsAnything, Param<TestOutcome>.IsAnything))
                .Callback((TestCase testCase, TestOutcome outcome) => TestCase = testCase);


            RunListener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"));
        };


        Because of = () => {
            RunListener.OnSpecificationEnd(SpecificationInfo, Result.Pass());
        };

        It should_notify_visual_studio_of_the_test_outcome = () => {
            The<IFrameworkHandle>()
                .WasToldTo(f => f.RecordEnd(Param<TestCase>.IsNotNull, Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed)))
                .OnlyOnce();
        };

        It should_notify_visual_studio_of_the_test_result = () => {
            The<IFrameworkHandle>()
                .WasToldTo(f => f.RecordResult(Param<TestResult>.Matches(result => 
                    result.Outcome == TestOutcome.Passed && 
                    result.ComputerName == Environment.MachineName &&
                    result.ErrorMessage == null &&
                    result.ErrorStackTrace == null
                )))
                .OnlyOnce();
                
        };
        
        #pragma warning disable CS0169

        Behaves_like<TestCaseMapperBehavior> should_tell_visual_studio_the_correct_details;
        
        #pragma warning restore CS0169
    }
}
