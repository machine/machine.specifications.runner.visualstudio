using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;

namespace SampleSpecs
{
    [Behaviors]
    public class SampleBehavior
    {

        It sample_behavior_test = () => {
            true.ShouldBeTrue();
        };
    }


    public class BehaviorSampleSpec
    {

        Because of = () => {
            
        };

        #pragma warning disable CS0169

        Behaves_like<SampleBehavior> some_behavior;
        #pragma warning restore CS0169
    }
}
