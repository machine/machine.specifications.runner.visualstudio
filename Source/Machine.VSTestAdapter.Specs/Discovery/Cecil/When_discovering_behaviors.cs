using Machine.Specifications;
using System;
using System.Linq;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.Cecil;

namespace Machine.VSTestAdapter.Specs.Discovery.Cecil
{
    public class When_discovering_specs_using_behaviors : With_DiscoverySetup<CecilSpecificationDiscoverer>
    {
        It can_not_pick_up_the_behavior = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "sample_behavior_test".Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          "BehaviorSampleSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldBeNull();

        };
    }
}