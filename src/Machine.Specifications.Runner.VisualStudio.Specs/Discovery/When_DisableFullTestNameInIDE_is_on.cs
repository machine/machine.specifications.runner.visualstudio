using System;
using System.Reflection;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using SampleSpecs;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
    public class When_DisableFullTestNameInIDE_is_on : WithFakes
    {
        static string configuration_xml = @"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>0</MaxCpuCount>
  </RunConfiguration>
  <MSpec>
    <DisableFullTestNameInIDE>true</DisableFullTestNameInIDE>
  </MSpec>
</RunSettings>";

        Establish context = () =>
        {
            The<IRunSettings>()
                .WhenToldTo(x => x.SettingsXml)
                .Return(configuration_xml);

            The<IDiscoveryContext>()
                .WhenToldTo(x => x.RunSettings)
                .Return(The<IRunSettings>());
        };


        Because of = () =>
            The<MSpecTestAdapter>().DiscoverTests(new[] {typeof(StandardSpec).GetTypeInfo().Assembly.Location},
                The<IDiscoveryContext>(),
                An<IMessageLogger>(),
                The<ITestCaseDiscoverySink>());

        It should_use_test_name_as_display_name = () =>
        {
            var specId = new VisualStudioTestIdentifier("SampleSpecs.StandardSpec", "should_pass");

            The<ITestCaseDiscoverySink>()
                .WasToldTo(x => x.SendTestCase(Param<TestCase>.Matches(t =>
                    t.ToVisualStudioTestIdentifier().Equals(specId) &&
                    t.DisplayName.Equals("should pass", StringComparison.Ordinal))))
                .OnlyOnce();
        };        
    }
}
