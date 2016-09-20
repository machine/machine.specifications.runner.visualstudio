using Machine.Specifications;
using System;
using System.Linq;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.Cecil;

namespace Machine.VSTestAdapter.Specs.Discovery.Cecil
{

    public class When_discovering_specs_in_nested_types : With_DiscoverySetup<CecilSpecificationDiscoverer>
    {
        It should_discover_the_sample_behavior = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "should_remember_that_true_is_true".Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          "NestedSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.ContextDisplayName.ShouldEqual("Parent NestedSpec");

            discoveredSpec.LineNumber.ShouldEqual(14);
            discoveredSpec.CodeFilePath.EndsWith("NestedSpecSample.cs", StringComparison.Ordinal);
        };
    }
}