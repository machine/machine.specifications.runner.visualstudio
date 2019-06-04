using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public class When_running_an_assembly_with_behaviors : With_AssemblyExecutionSetup
    {
        static VisualStudioTestIdentifier spec_expected_to_run;

        Establish context = () =>
            spec_expected_to_run = new VisualStudioTestIdentifier("SampleSpecs.BehaviorSampleSpec", "sample_behavior_test");

        It should_run_all_behaviors = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordEnd(
                    Param<TestCase>.Matches(t => t.ToVisualStudioTestIdentifier().Equals(spec_expected_to_run)),
                    Param<TestOutcome>.Matches(t => t == TestOutcome.Passed)))
                .OnlyOnce();

            The<IFrameworkHandle>()
                .WasToldTo(x => x.RecordResult(
                    Param<TestResult>.Matches(t => t.Outcome == TestOutcome.Passed && t.TestCase.ToVisualStudioTestIdentifier().Equals(spec_expected_to_run))))
                .OnlyOnce();
        };
    }
}
