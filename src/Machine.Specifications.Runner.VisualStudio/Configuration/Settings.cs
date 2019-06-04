using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.Specifications.Runner.VisualStudio.Configuration
{
    public class Settings
    {
        public bool DisableFullTestNameInIDE { get; set; }

        public bool DisableFullTestNameInOutput { get; set; }

        public static Settings Parse(string xml)
        {
            var settings = new Settings();

            try
            {
                var config = XDocument.Parse(xml).XPathSelectElement("RunSettings/MSpec");

                settings.DisableFullTestNameInOutput = "true".Equals(config.Element("DisableFullTestNameInOutput")?.Value ?? "false", StringComparison.OrdinalIgnoreCase);
                settings.DisableFullTestNameInIDE = "true".Equals(config.Element("DisableFullTestNameInIDE")?.Value ?? "false", StringComparison.OrdinalIgnoreCase);

                return settings;
            }
            catch
            {
                return settings;
            }
        }
    }
}
