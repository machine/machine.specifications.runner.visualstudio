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
    public class When_discovering_behaviors : WithFakes
    {
        private static string testDebugDirectory;

        private static ISpecificationDiscoverer discoverer;
        private static IEnumerable<MSpecTestCase> results;

        private static string SpecType = "BehaviorSampleSpec";
        private static string SpecificationName = "sample_behavior_test";

        private Establish context = () =>
        {
            discoverer = new BuiltInSpecificationDiscoverer();
            testDebugDirectory = Helper.GetTestDebugDirectory();
        };

        private Because of = () =>
        {
            Uri assemblyURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string path = Path.Combine(testDebugDirectory, "SampleSpecs.dll");
            results = discoverer.EnumerateSpecs(path);
        };

        private It should_discover_the_sample_behavior = () =>
        {
            MSpecTestCase discoveredSpec = results.SingleOrDefault(x => SpecificationName.Equals(x.SpecificationName, StringComparison.Ordinal) && 
                                                                          SpecType.Equals(x.ClassName, StringComparison.Ordinal));
            discoveredSpec.ShouldNotBeNull();

            discoveredSpec.LineNumber.ShouldEqual(14);
            discoveredSpec.CodeFilePath.EndsWith("BehaviorSample.cs", StringComparison.Ordinal);
        };
    }
}