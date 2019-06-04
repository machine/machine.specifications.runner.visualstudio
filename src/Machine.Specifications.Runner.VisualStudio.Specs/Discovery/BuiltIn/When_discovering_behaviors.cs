using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery.BuiltIn;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery.BuiltIn
{
    public class When_discovering_specs_using_behaviors : With_DiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_pick_up_the_behavior = () =>
        {
            var discoveredSpec = Results.SingleOrDefault(x =>
                "sample_behavior_test".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "BehaviorSampleSpec".Equals(x.ClassName, StringComparison.Ordinal));

            ShouldExtensionMethods.ShouldNotBeNull(discoveredSpec);

            ShouldExtensionMethods.ShouldEqual(discoveredSpec.LineNumber, 14);
            discoveredSpec.CodeFilePath.EndsWith("BehaviorSample.cs", StringComparison.Ordinal);
        };

        It should_pick_up_the_behavior_field_type_and_name = () =>
        {
            var discoveredSpec = Results.SingleOrDefault(x =>
                "sample_behavior_test".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "BehaviorSampleSpec".Equals(x.ClassName, StringComparison.Ordinal));
            ShouldExtensionMethods.ShouldNotBeNull(discoveredSpec);

            ShouldExtensionMethods.ShouldEqual(discoveredSpec.BehaviorFieldName, "some_behavior");
            ShouldExtensionMethods.ShouldEqual(discoveredSpec.BehaviorFieldType, "SampleSpecs.SampleBehavior");
        };
    }
}
