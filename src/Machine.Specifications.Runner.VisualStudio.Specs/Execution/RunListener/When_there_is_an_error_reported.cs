using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    public class When_there_is_an_error_reported : WithFakes
    {
        static VSProxyAssemblySpecificationRunListener run_listener;

        Establish context = () =>
            run_listener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"), An<Settings>());
        
        Because of = () =>
            run_listener.OnFatalError(new ExceptionResult(new InvalidOperationException()));

        It should_notify_visual_studio_of_the_error_outcome = () =>
            The<IFrameworkHandle>()
                .WasToldTo(x => x.SendMessage(
                    Param<TestMessageLevel>.Matches(t => t == TestMessageLevel.Error),
                    Param<string>.Matches(t => t.Contains("InvalidOperationException"))))
                .OnlyOnce();
    }
}
