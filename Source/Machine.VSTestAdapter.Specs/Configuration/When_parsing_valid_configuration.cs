using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter.Specs.Configuration
{
    [Subject(typeof(Settings), "Configuration")]
    public class When_parsing_valid_configuration
    {
        static Settings Settings;
        static string ConfigurationXml = @"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>0</MaxCpuCount>
  </RunConfiguration>
  <MSpec>
    <DisplayFullTestNameInOutput>true</DisplayFullTestNameInOutput>
  </MSpec>
</RunSettings>";

        Because of = () => {
            Settings = Settings.Parse(ConfigurationXml);
        };

        It should_pick_up_DisplayFullTestName = () => {
            Settings.DisplayFullTestNameInOutput.ShouldBeTrue();
        };
        
    }
}
