using System;
using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class UnitTestProviderConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "NUnit")]
        [StringValidator(MinLength = 1)]
        public string Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("generatorProvider", DefaultValue = null, IsRequired = false)]
        public string GeneratorProvider
        {
            get { return (string)this["generatorProvider"]; }
            set { this["generatorProvider"] = value; }
        }

        [ConfigurationProperty("runtimeProvider", DefaultValue = null, IsRequired = false)]
        public string RuntimeProvider
        {
            get { return (string)this["runtimeProvider"]; }
            set { this["runtimeProvider"] = value; }
        }
    }
}