using System;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter.Specs
{
    public class When_there_is_an_unhandled_error_during_test_discovery : WithFakes
    {
        static MSpecTestAdapter Adapter;

        Establish context = () => {
            The<ISpecificationDiscoverer>()
                .WhenToldTo(d => d.DiscoverSpecs(Param<string>.IsAnything))
                .Return(() => {
                    throw new InvalidOperationException();
                });

            Adapter = new MSpecTestAdapter(The<ISpecificationDiscoverer>(), An<ISpecificationExecutor>());
        };

        Because of = () => {
            Adapter.DiscoverTests(new[] { "bla" }, An<IDiscoveryContext>(), The<IMessageLogger>(), An<ITestCaseDiscoverySink>());
        };

        It should_send_an_error_notification_to_visual_studio = () => {
            The<IMessageLogger>()
                .WasToldTo(logger => logger.SendMessage(TestMessageLevel.Error, Param<string>.IsNotNull))
                .OnlyOnce();
        };
    }
}
