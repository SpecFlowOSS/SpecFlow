using System.Collections.Generic;
using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class CucumberMessagesElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = false, IsRequired = false)]
        public bool Enabled
        {
            get => (bool)this["enabled"];
            set => this["enabled"] = value;
        }

        [ConfigurationProperty("sinks", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(CucumberMessagesSinkCollection), AddItemName = "sink")]
        public CucumberMessagesSinkCollection Sinks
        {
            get => (CucumberMessagesSinkCollection) this["sinks"];
            set => this["sinks"] = value;
        }
    }
}