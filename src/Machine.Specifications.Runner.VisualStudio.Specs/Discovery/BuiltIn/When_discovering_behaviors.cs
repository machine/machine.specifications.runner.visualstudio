using Machine.Specifications;
using System;
using System.Linq;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.BuiltIn;

namespace Machine.VSTestAdapter.Specs.Discovery.BuiltIn
{

      

    public class When_discovering_specs_using_behaviors : With_DiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_pick_up_the_behavior = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "sample_behavior_test".Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          "BehaviorSampleSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(14);
            discoveredSpec.CodeFilePath.EndsWith("BehaviorSample.cs", StringComparison.Ordinal);
        };

        It should_pick_up_the_behavior_field_type_and_name = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "sample_behavior_test".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                                                                        "BehaviorSampleSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.BehaviorFieldName.ShouldEqual("some_behavior");
            discoveredSpec.BehaviorFieldType.ShouldEqual("SampleSpecs.SampleBehavior");
        };
    }
}