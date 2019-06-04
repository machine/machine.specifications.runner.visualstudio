using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public class When_running_a_spec_that_is_ignored : With_SingleSpecExecutionSetup
    {
        Establish context = () =>
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.StandardSpec", "should_be_ignored");

        It should_tell_visual_studio_it_was_skipped = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordEnd(
                    Param<TestCase>.Matches(t => t.ToVisualStudioTestIdentifier().Equals(SpecificationToRun)),
                    Param<TestOutcome>.Matches(t => t == TestOutcome.Skipped)))
                .OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordResult(Param<TestResult>.Matches(t => t.Outcome == TestOutcome.Skipped)))
                .OnlyOnce();
        };
    }
}
