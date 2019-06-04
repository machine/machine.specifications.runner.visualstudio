using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    public class When_DisableFullTestNameInOutput_is_on : With_SingleSpecExecutionSetup
    {
        static TestCase record_start_test_case;
        static TestCase record_end_test_case;

        Establish context = () =>
        {
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.When_something", "should_pass");

            The<Settings>().DisableFullTestNameInOutput = true;

            The<IFrameworkHandle>()
                .WhenToldTo(x => x.RecordStart(Param<TestCase>.Matches(t => t.ToVisualStudioTestIdentifier().Equals(SpecificationToRun))))
                .Callback((TestCase testCase) => record_start_test_case = testCase);

            The<IFrameworkHandle>()
                .WhenToldTo(x => x.RecordEnd(
                    Param<TestCase>.Matches(t => t.ToVisualStudioTestIdentifier().Equals(SpecificationToRun)),
                    Param<TestOutcome>.Matches(t => t == TestOutcome.Passed)))
                .Callback((TestCase testCase, TestOutcome outcome) => record_end_test_case = testCase);
        };

        It should_display_both_the_context_name_and_specification_name_on_a_single_line = () =>
        {
            record_start_test_case.DisplayName.ShouldEqual("should pass");
            record_end_test_case.DisplayName.ShouldEqual("should pass");
        };
    }
}
