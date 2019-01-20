using System;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{

    public class When_running_an_assembly_with_behaviors : With_AssemblyExecutionSetup
    {
        static VisualStudioTestIdentifier SpecificationExpectedToRun;

        Establish context = () => {
            SpecificationExpectedToRun = new VisualStudioTestIdentifier("SampleSpecs.BehaviorSampleSpec", "sample_behavior_test");
        };

        It should_run_all_behaviors = () => {
            The<IFrameworkHandle>()
                .WasToldTo(handle => 
                    handle.RecordEnd(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(SpecificationExpectedToRun)),
                                     Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed))
                ).OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(handle => 
                    handle.RecordResult(Param<TestResult>.Matches(result => result.Outcome == TestOutcome.Passed && 
                                                                              result.TestCase.ToVisualStudioTestIdentifier().Equals(SpecificationExpectedToRun)))
                ).OnlyOnce();
        };
    }
}
