using System;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Configuration;
using Machine.Specifications.Runner.VisualStudio.Discovery;
using Machine.Specifications.Runner.VisualStudio.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Specs
{
    public class When_there_is_an_unhandled_error_during_test_execution : WithFakes
    {
        static MSpecTestAdapter adapter;

        Establish context = () =>
        {
            FakeApi.WhenToldTo(The<ISpecificationExecutor>(), d => d.RunAssembly(Param<string>.IsAnything, Param<Settings>.IsNotNull, Param<Uri>.IsAnything, Param<IFrameworkHandle>.IsAnything))
                .Throw(new InvalidOperationException());

            adapter = new MSpecTestAdapter(An<ISpecificationDiscoverer>(), The<ISpecificationExecutor>());
        };

        Because of = () =>
            adapter.RunTests(new[] {"bla"}, An<IRunContext>(), The<IFrameworkHandle>());

        It should_send_an_error_notification_to_visual_studio = () =>
            The<IFrameworkHandle>()
                .WasToldTo(logger => logger.SendMessage(TestMessageLevel.Error, Param<string>.IsNotNull))
                .OnlyOnce();
    }
}
