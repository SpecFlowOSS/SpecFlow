using System.Configuration;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class PluginConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("path", IsRequired = false, DefaultValue = null)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = false, DefaultValue = PluginType.GeneratorAndRuntime)]
        public PluginType Type
        {
            get { return (PluginType)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("parameters", IsRequired = false, DefaultValue = null)]
        public string Parameters
        {
            get { return (string)this["parameters"]; }
            set { this["parameters"] = value; }
        }

        public PluginDescriptor ToPluginDescriptor()
        {
            return new PluginDescriptor(Name, string.IsNullOrEmpty(Path) ? null : Path, 
                Type, Parameters);
        }
    }
}