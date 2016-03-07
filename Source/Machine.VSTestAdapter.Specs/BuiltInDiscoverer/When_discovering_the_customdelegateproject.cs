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
    public class When_discovering_the_customdelegateproject : WithFakes
    {
        private static string testDebugDirectory = string.Empty;
        private static string testSourceDirectory = string.Empty;

        private static ISpecificationDiscoverer discoverer;
        private static IEnumerable<MSpecTestCase> results;

        private static string CustomDelegateTypeSpec_Type = "When_getting_the_hash_code_of_equal_strings_with_custom_assert_delegate";

        private Establish context = () =>
        {
            discoverer = new BuiltInSpecificationDiscoverer();
            testDebugDirectory = Helper.GetTestDebugDirectory();
            testSourceDirectory = Helper.GetTestSourceDirectory();
        };

        private Because of = () =>
        {
            Uri assemblyURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string path = Path.Combine(testDebugDirectory, "CustomDelegateSpec.dll");
            results = discoverer.EnumerateSpecs(path);
        };

        private It should_discover_the_customdelegate_type_spec_they = () =>
        {
            MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == CustomDelegateTypeSpec_Type 
                        && x.SpecificationName == "should_have_the_same_hash_code" ).SingleOrDefault();
            discoveredSpec.ShouldNotBeNull();
        };

        It should_not_have_duplicates = () => {
            var typeCount = results.Where(x => x.ContextFullType.Contains(CustomDelegateTypeSpec_Type)).Count();

            typeCount.ShouldEqual(2);
        };

       
    }
}