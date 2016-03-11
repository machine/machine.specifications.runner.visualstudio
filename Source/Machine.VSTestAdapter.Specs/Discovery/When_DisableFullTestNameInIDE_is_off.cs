using System;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;
using Machine.VSTestAdapter.Discovery;
using Machine.VSTestAdapter.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using SampleSpecs;

namespace Machine.VSTestAdapter.Specs.Discovery
{
    public class When_DisableFullTestNameInIDE_is_off : WithFakes
    {

        Because of = () => {
            The<MSpecTestAdapter>().DiscoverTests(new[] { typeof(When_something).Assembly.Location },
                                                      An<IDiscoveryContext>(),
                                                      An<IMessageLogger>(),
                                                      The<ITestCaseDiscoverySink>());
        };


        It should_use_full_type_and_field_name_for_display_name = () => {
            The<ITestCaseDiscoverySink>()
                .WasToldTo(d => d.SendTestCase(Param<TestCase>.Matches(t => t.DisplayName.Equals("When something it should pass", StringComparison.Ordinal))))
                .OnlyOnce();
        };        
    }
}
