using System;
using System.Collections.Generic;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Execution;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.VSTestAdapter.Specs
{
    public class When_there_is_an_unhandled_error_during_test_execution : WithFakes
    {
        static MSpecTestAdapterExecutor Adapter;

        Establish context = () =>
        {
            The<ISpecificationExecutor>()
                .WhenToldTo(d => d.RunAssemblySpecifications(Param<string>.IsAnything, Param<VisualStudioTestIdentifier[]>.IsAnything, Param<Settings>.IsNotNull,
                    Param<Uri>.IsAnything, Param<IFrameworkHandle>.IsAnything))
                .Throw(new InvalidOperationException());

            var adapterDiscoverer = new MSpecTestAdapterDiscoverer(An<ISpecificationDiscoverer>());
            Adapter = new MSpecTestAdapterExecutor(The<ISpecificationExecutor>(), adapterDiscoverer, An<ISpecificationFilterProvider>());
        };

        Because of = () =>
            Adapter.RunTests(new[] {new TestCase("a", MSpecTestAdapter.Uri, "dll"), }, An<IRunContext>(), The<IFrameworkHandle>());

        It should_send_an_error_notification_to_visual_studio = () =>
        {
            The<IFrameworkHandle>()
                .WasToldTo(logger => logger.SendMessage(TestMessageLevel.Error, Param<string>.IsNotNull))
                .OnlyOnce();
        };
    }
}
