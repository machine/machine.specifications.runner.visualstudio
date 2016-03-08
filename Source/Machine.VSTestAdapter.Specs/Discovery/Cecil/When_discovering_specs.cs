using Machine.Specifications;
using System;
using System.Linq;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.Cecil;

namespace Machine.VSTestAdapter.Specs.Discovery.Cecil
{

    public class When_discovering_specs : With_DiscoverySetup<CecilSpecificationDiscoverer>
    {
        It should_find_spec = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "should_pass".Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(14);
            discoveredSpec.CodeFilePath.EndsWith("StandardSpec.cs", StringComparison.Ordinal);
        };

        It should_find_empty_spec = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "should_be_ignored".Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(20);
            discoveredSpec.CodeFilePath.EndsWith("StandardSpec.cs", StringComparison.Ordinal);
        };

        It should_find_ignored_spec_but_will_not_find_line_number = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "not_implemented".Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(0);
            discoveredSpec.CodeFilePath.ShouldBeNull();
        };
    }
}