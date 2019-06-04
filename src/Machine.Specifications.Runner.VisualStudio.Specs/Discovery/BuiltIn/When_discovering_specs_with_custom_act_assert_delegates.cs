using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery.BuiltIn;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery.BuiltIn
{
    public class When_discovering_specs_with_custom_act_assert_delegates : With_DiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_find_some = () =>
        {
            var discoveredSpec = Results.SingleOrDefault(x =>
                "should_have_the_same_hash_code".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "CustomActAssertDelegateSpec".Equals(x.ClassName, StringComparison.Ordinal));
            ShouldExtensionMethods.ShouldNotBeNull(discoveredSpec);

            ShouldExtensionMethods.ShouldEqual(discoveredSpec.LineNumber, 31);
            discoveredSpec.CodeFilePath.EndsWith("CustomActAssertDelegateSpec.cs", StringComparison.Ordinal);
        };
    }
}
