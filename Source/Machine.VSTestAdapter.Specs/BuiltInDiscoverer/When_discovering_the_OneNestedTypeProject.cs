using Machine.Fakes;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Discovery.BuiltIn;

namespace Machine.VSTestAdapter.Specs.BuiltInDiscoverer
{
    public class When_discovering_the_OneNestedTypeProject : WithFakes
    {
        private static string testDebugDirectory = string.Empty;
        private static string testSourceDirectory = string.Empty;

        private static ISpecificationDiscoverer discoverer;
        private static IEnumerable<MSpecTestCase> results;

        private static string NestedTypeSpec_Type = "NestedSpec";
        private static string NestedTypeSpec_FullType = "OneNestedTypeSpec.Parent+NestedSpec";

        private Establish context = () =>
            {
                discoverer = new BuiltInSpecificationDiscoverer();
                testDebugDirectory = Helper.GetTestDebugDirectory();
                testSourceDirectory = Helper.GetTestSourceDirectory();
            };

        private Because of = () =>
            {
                Uri assemblyURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                string path = Path.Combine(testDebugDirectory, "OneNestedTypeSpec.dll");
                results = discoverer.EnumerateSpecs(path);
            };

        private It should_discover_the_nested_type_spec = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ClassName == NestedTypeSpec_Type).SingleOrDefault();
                discoveredSpec.ShouldNotBeNull();
            };

        private It should_normalize_the_nested_type_name = () =>
        {
            results.Where(x => x.ClassName == NestedTypeSpec_Type)
                .First()
                .ContextFullType.ShouldEqual(NestedTypeSpec_FullType);
        };
    }
}