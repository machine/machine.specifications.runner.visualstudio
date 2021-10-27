using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution
{
    class WhenDisableFullTestNameInOutputIsOn : WithSingleSpecExecutionSetup
    {
        static TestCase record_start_test_case;

        static TestCase record_end_test_case;

        Establish context = () => {
            specification_to_run = new VisualStudioTestIdentifier("SampleSpecs.When_something", "should_pass");

            The<Settings>().DisableFullTestNameInOutput = true;

            The<IFrameworkHandle>()
                .WhenToldTo(handle => 
                    handle.RecordStart(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(specification_to_run))))
                .Callback((TestCase testCase) => record_start_test_case = testCase);

            The<IFrameworkHandle>()
                .WhenToldTo(handle => 
                    handle.RecordEnd(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(specification_to_run)),
                                     Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed)))
                .Callback((TestCase testCase, TestOutcome outcome) => record_end_test_case = testCase);
        };

        It should_display_both_the_context_name_and_specification_name_on_a_single_line = () =>
        {
            record_start_test_case.DisplayName.ShouldEqual("should pass");
            record_end_test_case.DisplayName.ShouldEqual("should pass");
        };
    }
}
