using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public class When_running_a_single_behavior_passes : With_SingleSpecExecutionSetup
    {
        Establish context = () =>
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.BehaviorSampleSpec", "sample_behavior_test");

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
