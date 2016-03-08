using System;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using Machine.Specifications.Runner;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.VSTestAdapter.Specs.Execution.RunListener
{
    


    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    public class When_specification_starts : WithFakes
    {
        static VSProxyAssemblySpecificationRunListener RunListener;
        protected static TestCase TestCase;

        Establish context = () => {
            The<IFrameworkHandle>()
                .WhenToldTo(f => f.RecordStart(Param<TestCase>.IsAnything))
                .Callback((TestCase testCase) => TestCase = testCase);

            RunListener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"));
        };


        Because of = () => {
            RunListener.OnSpecificationStart(new SpecificationInfo("leader", "field name", "ContainingType", "field_name"));
        };

        It should_notify_visual_studio = () => {
            The<IFrameworkHandle>().WasToldTo(f => f.RecordStart(Param<TestCase>.IsNotNull));
        };
        
        #pragma warning disable CS0169

        Behaves_like<TestCaseMapperBehavior> should_tell_visual_studio_the_correct_details;
        
        #pragma warning restore CS0169
    }
}
