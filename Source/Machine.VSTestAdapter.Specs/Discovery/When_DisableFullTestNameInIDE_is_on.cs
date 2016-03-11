using System;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using Machine.VSTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using SampleSpecs;

namespace Machine.VSTestAdapter.Specs.Discovery
{
    public class When_DisableFullTestNameInIDE_is_on : WithFakes
    {
        static string ConfigurationXml = @"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>0</MaxCpuCount>
  </RunConfiguration>
  <MSpec>
    <DisableFullTestNameInIDE>true</DisableFullTestNameInIDE>
  </MSpec>
</RunSettings>";

        Establish establish = () => {
            The<IRunSettings>().WhenToldTo(runSettings => runSettings.SettingsXml).Return(ConfigurationXml);
            The<IDiscoveryContext>().WhenToldTo(context => context.RunSettings).Return(The<IRunSettings>());
        };


        Because of = () => {
            The<MSpecTestAdapter>().DiscoverTests(new[] { typeof(StandardSpec).Assembly.Location },
                                  The<IDiscoveryContext>(),
                                  An<IMessageLogger>(),
                                  The<ITestCaseDiscoverySink>());
        };


        It should_use_test_name_as_display_name = () => {
            var specId = new VisualStudioTestIdentifier("SampleSpecs.StandardSpec", "should_pass");

            The<ITestCaseDiscoverySink>()
                .WasToldTo(d => d.SendTestCase(Param<TestCase>.Matches(t => t.ToVisualStudioTestIdentifier().Equals(specId) && 
                                                                       t.DisplayName.Equals("should pass", StringComparison.Ordinal))))
                .OnlyOnce();
        };        
    }
}
