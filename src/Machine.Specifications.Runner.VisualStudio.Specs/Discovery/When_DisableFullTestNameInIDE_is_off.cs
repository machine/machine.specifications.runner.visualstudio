using System;
using System.Reflection;
using Machine.Fakes;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
    public class When_DisableFullTestNameInIDE_is_off : WithFakes
    {
        Because of = () =>
            The<MSpecTestAdapter>()
                .DiscoverTests(new[] {typeof(When_something).GetTypeInfo().Assembly.Location},
                    An<IDiscoveryContext>(),
                    An<IMessageLogger>(),
                    The<ITestCaseDiscoverySink>());


        It should_use_full_type_and_field_name_for_display_name = () =>
            The<ITestCaseDiscoverySink>()
            .WasToldTo(x => x.SendTestCase(Param<TestCase>.Matches(y => y.DisplayName.Equals("When something it should pass", StringComparison.Ordinal))))
            .OnlyOnce();
    }
}
