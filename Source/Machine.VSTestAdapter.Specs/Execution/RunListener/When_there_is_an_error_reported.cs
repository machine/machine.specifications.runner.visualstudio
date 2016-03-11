using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Fakes;
using Machine.Specifications;
using Machine.Specifications.Runner;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter.Specs.Execution.RunListener
{

    [Subject(typeof(VSProxyAssemblySpecificationRunListener))]
    public class When_there_is_an_error_reported : WithFakes
    {
        static VSProxyAssemblySpecificationRunListener RunListener;

        Establish context = () => {
            RunListener = new VSProxyAssemblySpecificationRunListener("assemblyPath", The<IFrameworkHandle>(), new Uri("bla://executorUri"), An<Settings>());
        };


        Because of = () => {
            RunListener.OnFatalError(new ExceptionResult(new InvalidOperationException()));
        };

        It should_notify_visual_studio_of_the_error_outcome = () => {
            The<IFrameworkHandle>().WasToldTo(f => f.SendMessage(
                Param<TestMessageLevel>.Matches(level => level == TestMessageLevel.Error),
                Param<string>.Matches(message => message.Contains("InvalidOperationException"))
            )).OnlyOnce();
        };
    }
}
