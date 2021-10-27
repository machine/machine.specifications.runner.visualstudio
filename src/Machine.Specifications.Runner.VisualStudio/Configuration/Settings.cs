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
            var config = new Settings();

            XElement mspecConfig = null;

            try
            {
                if (!string.IsNullOrEmpty(xml))
                {
                    mspecConfig = XDocument.Parse(xml).XPathSelectElement("RunSettings/MSpec");
                }
            }
            catch
            {
                // ignored
            }

            if (mspecConfig == null)
                return config;

            config.DisableFullTestNameInOutput = "true".Equals(mspecConfig.Element("DisableFullTestNameInOutput")?.Value ?? "false", StringComparison.OrdinalIgnoreCase);
            config.DisableFullTestNameInIDE = "true".Equals(mspecConfig.Element("DisableFullTestNameInIDE")?.Value ?? "false", StringComparison.OrdinalIgnoreCase);

            return config;
        }
    }
}
