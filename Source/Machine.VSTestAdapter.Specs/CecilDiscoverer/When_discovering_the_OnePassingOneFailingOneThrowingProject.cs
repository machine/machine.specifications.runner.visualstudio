using Machine.Fakes;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Machine.VSTestAdapter.Discovery.Cecil;
using Machine.VSTestAdapter.Discovery;

namespace Machine.VSTestAdapter.Specs.CecilDiscoverer
{
    public class When_discovering_the_OnePassingOneFailingOneThrowingProject : WithFakes
    {
        private static string testDebugDirectory = string.Empty;
        private static string testSourceDirectory = string.Empty;

        private static ISpecificationDiscoverer discoverer;
        private static IEnumerable<MSpecTestCase> results;

        private static string PassingSpecWithSubject_Type = "PassingSpecWithSubject";
        private static string PassingSpecWithSubject_FullType = "OnePassingOneFailingOneThrowingSpec.PassingSpecWithSubject";
        private static string PassingSpecWithSubject_SpecificationName = "should_add_the_numbers_correctly";
        private static string PassingSpecWithSubject_Subject = "ATESTSUBJECT";
        private static string PassingSpecWithSubject_Source = "OnePassingOneFailingOneThrowingSpec\\PassingSpecWithSubject.cs";

        private static string FailingSpec_Type = "FailingSpec";

        private static string ThrowingSpec_Type = "ThrowingSpec";

        private Establish context = () =>
            {
                discoverer = new CecilSpecificationDiscoverer();
                testDebugDirectory = Helper.GetTestDebugDirectory();
                testSourceDirectory = Helper.GetTestSourceDirectory();
            };

        private Because of = () =>
            {
                Uri assemblyURI = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                string path = Path.Combine(testDebugDirectory, "OnePassingOneFailingOneThrowingSpec.dll");
                results = discoverer.EnumerateSpecs(path);
            };

        private It should_discover_the_passing_specifications_type = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == PassingSpecWithSubject_Type).SingleOrDefault();
                discoveredSpec.ShouldNotBeNull();
            };

        private It should_discover_the_passing_specifications_fulltype = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == PassingSpecWithSubject_Type).SingleOrDefault();
                discoveredSpec.ContextFullType.ShouldEqual(PassingSpecWithSubject_FullType);
            };

        private It should_discover_the_passing_specifications_specification_name = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == PassingSpecWithSubject_Type).SingleOrDefault();
                discoveredSpec.SpecificationName.ShouldEqual(PassingSpecWithSubject_SpecificationName);
            };

        private It should_discover_the_passing_specifications_subject = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == PassingSpecWithSubject_Type).SingleOrDefault();
                discoveredSpec.SubjectName.ShouldEqual(PassingSpecWithSubject_Subject);
            };

        private It should_discover_the_passing_specifications_source = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == PassingSpecWithSubject_Type).SingleOrDefault();
                string lowerCaseDiscoverSourcePath = discoveredSpec.CodeFilePath.ToLower();
                string lowerCaseExpectedSourcePath = Path.GetFullPath(Path.Combine(testSourceDirectory, PassingSpecWithSubject_Source)).ToLower();

                lowerCaseDiscoverSourcePath.ShouldEqual(lowerCaseExpectedSourcePath);
            };

        private It should_discover_the_failing_specifications_type = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == FailingSpec_Type).SingleOrDefault();
                discoveredSpec.ShouldNotBeNull();
            };

        private It should_discover_the_failing_specification_has_no_specification_name = () =>
            {
                MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == FailingSpec_Type).SingleOrDefault();
                discoveredSpec.SubjectName.ShouldBeNull();
            };

        private It should_discover_the_throwing_specifications_type = () =>
        {
            MSpecTestCase discoveredSpec = results.Where(x => x.ContextType == ThrowingSpec_Type).SingleOrDefault();
            discoveredSpec.ShouldNotBeNull();
        };
    }
}