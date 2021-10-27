using System;
using System.Reflection;
using Machine.Fakes;
using Machine.Specifications.Runner.VisualStudio.Fixtures;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Discovery
{
    class DisabledTestNameOnSpecs : WithFakes
    {
        const string configuration_xml = @"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>0</MaxCpuCount>
  </RunConfiguration>
  <MSpec>
    <DisableFullTestNameInIDE>true</DisableFullTestNameInIDE>
  </MSpec>
</RunSettings>";

        static Assembly assembly;
        
        Establish context = () =>
        {
            assembly = typeof(StandardSpec).Assembly;

            The<IRunSettings>()
                .WhenToldTo(runSettings => runSettings.SettingsXml)
                .Return(configuration_xml);

            The<IDiscoveryContext>()
                .WhenToldTo(context => context.RunSettings)
                .Return(The<IRunSettings>());
        };

        Because of = () =>
            The<MSpecTestAdapter>().DiscoverTests(new[] {assembly.GetType("SampleSpecs.StandardSpec").GetTypeInfo().Assembly.Location},
                The<IDiscoveryContext>(),
                An<IMessageLogger>(),
                The<ITestCaseDiscoverySink>());


        It should_use_test_name_as_display_name = () =>
        {
            var specId = new VisualStudioTestIdentifier("SampleSpecs.StandardSpec", "should_pass");

            The<ITestCaseDiscoverySink>()
                .WasToldTo(d => d.SendTestCase(Param<TestCase>.Matches(t =>
                    t.ToVisualStudioTestIdentifier().Equals(specId) &&
                    t.DisplayName.Equals("should pass", StringComparison.Ordinal))))
                .OnlyOnce();
        };        
    }
}
