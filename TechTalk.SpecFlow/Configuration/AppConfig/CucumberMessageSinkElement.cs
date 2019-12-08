using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class CucumberMessageSinkElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get => (string) this["type"];
            set => this["type"] = value;
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get => (string) this["path"];
            set => this["path"] = value;
        }
    }
}