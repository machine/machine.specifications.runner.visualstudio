using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{
    public class When_DisableFullTestNameInOutput_is_on : With_SingleSpecExecutionSetup
    {
        static TestCase RecordStartTestCase;
        static TestCase RecordEndTestCase;

        Establish context = () => {
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.When_something", "should_pass");

            The<Settings>().DisableFullTestNameInOutput = true;

            The<IFrameworkHandle>()
                .WhenToldTo(handle => 
                    handle.RecordStart(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(SpecificationToRun)))
                )
                .Callback((TestCase testCase) => RecordStartTestCase = testCase);

            The<IFrameworkHandle>()
                .WhenToldTo(handle => 
                    handle.RecordEnd(Param<TestCase>.Matches(testCase => testCase.ToVisualStudioTestIdentifier().Equals(SpecificationToRun)),
                                     Param<TestOutcome>.Matches(outcome => outcome == TestOutcome.Passed))
                )
                .Callback((TestCase testCase, TestOutcome outcome) => RecordEndTestCase = testCase);
        };

        It should_display_both_the_context_name_and_specification_name_on_a_single_line = () => {
            RecordStartTestCase.DisplayName.ShouldEqual("should pass");
            RecordEndTestCase.DisplayName.ShouldEqual("should pass");
        };
    }
}
