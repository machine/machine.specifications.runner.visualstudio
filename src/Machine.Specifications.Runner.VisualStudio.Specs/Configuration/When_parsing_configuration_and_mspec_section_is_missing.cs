using Machine.Specifications.Runner.VisualStudio.Configuration;

namespace Machine.Specifications.Runner.VisualStudio.Specs.Configuration
{
    [Subject(typeof(Settings), "Configuration")]
    public class When_parsing_configuration_and_mspec_section_is_missing
    {
        static Settings settings;
        static string configuration_xml = "<RunSettings></RunSettings>";

        Because of = () =>
            settings = Settings.Parse(configuration_xml);

        It should_default_to_DisplayFullTestName_off = () =>
            settings.DisableFullTestNameInOutput.ShouldBeFalse();
    }
}
