using System;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter.Specs
{
    public class When_there_is_an_unhandled_error_during_test_execution : WithFakes
    {
        static MSpecTestAdapter Adapter;

        Establish context = () =>
        {
            The<ISpecificationExecutor>()
                .WhenToldTo(d => d.RunAssembly(Param<string>.IsAnything, Param<Settings>.IsNotNull,
                    Param<Uri>.IsAnything, Param<IFrameworkHandle>.IsAnything))
                .Throw(new InvalidOperationException());

            Adapter = new MSpecTestAdapter(An<ISpecificationDiscoverer>(), The<ISpecificationExecutor>(), An<ISpecificationFilterProvider>());
        };

        Because of = () => { Adapter.RunTests(new[] {"bla"}, An<IRunContext>(), The<IFrameworkHandle>()); };

        It should_send_an_error_notification_to_visual_studio = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(logger => logger.SendMessage(TestMessageLevel.Error, Param<string>.IsNotNull))
                .OnlyOnce();
        };
    }
}
