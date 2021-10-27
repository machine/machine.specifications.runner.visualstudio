using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Behaviors]
    class TestCaseMapperBehavior
    {
        protected static TestCase test_case;

        It should_provide_correct_details_to_visual_studio = () =>
        {
            test_case.FullyQualifiedName.ShouldEqual("ContainingType::field_name");
            test_case.ExecutorUri.ShouldEqual(new Uri("bla://executorUri"));
            test_case.Source.ShouldEqual("assemblyPath");
        };
    }
}
