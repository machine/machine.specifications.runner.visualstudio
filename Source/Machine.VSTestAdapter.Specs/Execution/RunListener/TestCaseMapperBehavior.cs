using System;
using System.Linq;
using Machine.Specifications;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Machine.VSTestAdapter.Specs.Execution.RunListener
{
    [Behaviors]
    public class TestCaseMapperBehavior
    {
#pragma warning disable CS0169
        protected static TestCase TestCase;
#pragma warning restore CS0169

        It should_provide_correct_details_to_visual_studio = () => {
            TestCase.FullyQualifiedName.ShouldEqual("ContainingType::field_name");
            TestCase.DisplayName.ShouldEqual("field name");
            TestCase.ExecutorUri.ShouldEqual(new Uri("bla://executorUri"));
            TestCase.Source.ShouldEqual("assemblyPath");
        };
    }
}
