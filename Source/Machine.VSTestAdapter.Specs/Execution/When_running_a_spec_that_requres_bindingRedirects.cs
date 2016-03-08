using System;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{
    /// <summary>
    /// This spec relies on the use of Machine.Fakes.NSubstitute in the test, which requires a bindingRedirect for NSubstitute
    /// </summary>
    public class When_running_a_spec_that_requres_bindingRedirects : With_SingleSpecExecutionSetup
    {
        Establish context = () => {
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.AssemblyBindingSampleSpec", "should_be_true");
        };

        It should_work = () => {
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
