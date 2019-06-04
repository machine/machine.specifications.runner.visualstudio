using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public class When_running_a_spec_with_custom_act_assert_delegates_passes : With_SingleSpecExecutionSetup
    {
        Establish context = () =>
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.CustomActAssertDelegateSpec", "should_have_the_same_hash_code");

        It should_tell_visual_studio_it_passed = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordEnd(
                    Param<TestCase>.Matches(t => t.ToVisualStudioTestIdentifier().Equals(SpecificationToRun)),
                    Param<TestOutcome>.Matches(t => t == TestOutcome.Passed)))
                .OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordResult(Param<TestResult>.Matches(t => t.Outcome == TestOutcome.Passed)))
                .OnlyOnce();
        };
    }
}
