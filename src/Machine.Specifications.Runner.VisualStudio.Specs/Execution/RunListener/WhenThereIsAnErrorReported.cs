using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Execution.RunListener
{
    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    class WhenThereIsAnErrorReported : WithFakes
    {
        static VSProxyAssemblySpecificationRunListener run_listener;

        Establish context = () => 
            run_listener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"), An<Settings>());

        Because of = () => 
            run_listener.OnFatalError(new ExceptionResult(new InvalidOperationException()));

        It should_notify_visual_studio_of_the_error_outcome = () =>
            The<IFrameworkHandle>()
                .WasToldTo(f => f.SendMessage(
                    Param<TestMessageLevel>.Matches(level => level == TestMessageLevel.Error),
                    Param<string>.Matches(message => message.Contains("InvalidOperationException"))))
                .OnlyOnce();
    }
}
