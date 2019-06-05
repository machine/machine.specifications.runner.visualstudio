using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery.BuiltIn;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery.BuiltIn
{
    public class When_discovering_specs_in_nested_types : With_DiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_discover_the_sample_behavior = () =>
        {
            var discoveredSpec = Results.SingleOrDefault(x =>
                "should_remember_that_true_is_true".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "NestedSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.ContextDisplayName.ShouldEqual("Parent NestedSpec");

            discoveredSpec.LineNumber.ShouldEqual(9);
            discoveredSpec.CodeFilePath.EndsWith("NestedSpecSample.cs", StringComparison.Ordinal);
        };
    }
}
