using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Machine.VSTestAdapter.Configuration
{
    public class Settings
    {
        public bool DisableFullTestNameInIDE { get; set; }
        public bool DisableFullTestNameInOutput { get; set; }

        public static Settings Parse(string xml)
        {
            Settings config = new Settings();

            XElement mspecConfig = null;
            try {
                mspecConfig = XDocument.Parse(xml).XPathSelectElement("RunSettings/MSpec");
            } catch { }

            if (mspecConfig == null)
                return config;

            config.DisableFullTestNameInOutput = "true".Equals(mspecConfig.Element("DisableFullTestNameInOutput")?.Value ?? "false", StringComparison.InvariantCultureIgnoreCase);
            config.DisableFullTestNameInIDE = "true".Equals(mspecConfig.Element("DisableFullTestNameInIDE")?.Value ?? "false", StringComparison.InvariantCultureIgnoreCase);

            return config;
        }
    }
}
