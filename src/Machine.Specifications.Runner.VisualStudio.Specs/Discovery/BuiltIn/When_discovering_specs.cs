using System;
using System.Linq;
using Machine.Specifications.Runner.VisualStudio.Discovery.BuiltIn;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery.BuiltIn
{
    public class When_discovering_specs : With_DiscoverySetup<BuiltInSpecificationDiscoverer>
    {
        It should_find_spec = () =>
        {
            var discoveredSpec = Results.SingleOrDefault(x =>
                "should_pass".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));
            ShouldExtensionMethods.ShouldNotBeNull(discoveredSpec);

            ShouldExtensionMethods.ShouldEqual(discoveredSpec.LineNumber, 14);
            discoveredSpec.CodeFilePath.EndsWith("StandardSpec.cs", StringComparison.Ordinal);
        };

        It should_find_empty_spec = () =>
        {
            var discoveredSpec = Results.SingleOrDefault(x =>
                "should_be_ignored".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));
            ShouldExtensionMethods.ShouldNotBeNull(discoveredSpec);

            ShouldExtensionMethods.ShouldEqual(discoveredSpec.LineNumber, 20);
            discoveredSpec.CodeFilePath.EndsWith("StandardSpec.cs", StringComparison.Ordinal);
        };

        It should_find_ignored_spec_but_will_not_find_line_number = () =>
        {
            var discoveredSpec = Results.SingleOrDefault(x =>
                "not_implemented".Equals(x.SpecificationName, StringComparison.Ordinal) &&
                "StandardSpec".Equals(x.ClassName, StringComparison.Ordinal));
            ShouldExtensionMethods.ShouldNotBeNull(discoveredSpec);

            ShouldExtensionMethods.ShouldEqual(discoveredSpec.LineNumber, 0);
            ShouldExtensionMethods.ShouldBeNull(discoveredSpec.CodeFilePath);
        };
    }
}
