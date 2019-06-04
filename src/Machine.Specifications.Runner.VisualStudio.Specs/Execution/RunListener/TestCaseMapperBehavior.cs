using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Behaviors]
    public class TestCaseMapperBehavior
    {
        protected static TestCase TestCase;

        It should_provide_correct_details_to_visual_studio = () =>
        {
            TestCase.FullyQualifiedName.ShouldEqual("ContainingType::field_name");
            TestCase.ExecutorUri.ShouldEqual(new Uri("bla://executorUri"));
            TestCase.Source.ShouldEqual("assemblyPath");
        };
    }
}
