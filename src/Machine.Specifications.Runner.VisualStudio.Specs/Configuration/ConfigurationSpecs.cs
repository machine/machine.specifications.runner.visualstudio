using Machine.Specifications.Runner.VisualStudio.Configuration;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Configuration
{
    [Tags("Tag1","Tag2")]
    [Subject(typeof(Settings), "Configuration")]
    class ConfigurationSpecs
    {
        static Settings settings;

        static string configuration_xml = "<RunSettings></RunSettings>";

        Because of = () =>
            settings = Settings.Parse(configuration_xml);

        It should_default_to_display_full_test_name_off = () =>
            settings.DisableFullTestNameInOutput.ShouldBeFalse();
    }
}
