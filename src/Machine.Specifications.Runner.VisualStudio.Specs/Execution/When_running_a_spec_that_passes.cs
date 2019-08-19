using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{
    public class When_running_a_spec_that_passes : With_SingleSpecExecutionSetup
    {
        Establish context = () =>
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.StandardSpec", "should_pass");

        It should_tell_visual_studio_it_passed = () => {
            The<IFrameworkHandle>()
                .WasToldTo(handle => 
                    handle.RecordEnd(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(SpecificationToRun)),
                                     Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed))
                ).OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(handle => 
                    handle.RecordResult(Param<TestResult>.Matches(result => result.Outcome == TestOutcome.Passed))
                ).OnlyOnce();
        };
    }
}
