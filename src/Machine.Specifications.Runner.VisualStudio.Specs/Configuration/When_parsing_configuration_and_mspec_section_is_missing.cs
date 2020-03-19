using Machine.Specifications;
using Machine.VSTestAdapter.Configuration;

namespace Machine.VSTestAdapter.Specs.Configuration
{
    [Tags("Tag1","Tag2")]
    [Subject(typeof(Settings), "Configuration")]
    public class When_parsing_configuration_and_mspec_section_is_missing
    {
        static Settings Settings;
        static string ConfigurationXml = "<RunSettings></RunSettings>";

        Because of = () =>
            Settings = Settings.Parse(ConfigurationXml);

        It should_default_to_DisplayFullTestName_off = () =>
            Settings.DisableFullTestNameInOutput.ShouldBeFalse();
    }
}
