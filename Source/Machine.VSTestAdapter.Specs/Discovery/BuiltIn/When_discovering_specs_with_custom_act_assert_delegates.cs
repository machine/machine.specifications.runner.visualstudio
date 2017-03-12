using System;
using System.Linq;
using Machine.Specifications;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.BuiltIn;

namespace Machine.VSTestAdapter.Specs.Discovery.BuiltIn
{
    
    public class When_discovering_specs_with_custom_act_assert_delegates : With_DiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_find_some = () => {
            MSpecTestCase discoveredSpec = Results.SingleOrDefault(x => "should_have_the_same_hash_code".Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          "CustomActAssertDelegateSpec".Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

#if !NETSTANDARD
            discoveredSpec.LineNumber.ShouldEqual(31);
            discoveredSpec.CodeFilePath.EndsWith("CustomActAssertDelegateSpec.cs", StringComparison.Ordinal);
#endif
        };
    }
}