using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution
{
      

    

    public class When_running_a_single_behavior_passes : With_ExecutionSetup
    {
        Establish context = () => {
            SpecificationToRun = new VisualStudioTestIdentifier("SampleSpecs.BehaviorSampleSpec", "sample_behavior_test");
        };

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
