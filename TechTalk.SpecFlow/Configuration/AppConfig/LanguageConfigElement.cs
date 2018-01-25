using System;
using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class LanguageConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("feature", DefaultValue = "en", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?")]
        public string Feature
        {
            get { return (String)this["feature"]; }
            set { this["feature"] = value; }
        }

        [ConfigurationProperty("tool", DefaultValue = "", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?|")]
        public string Tool
        {
            get { return (String)this["tool"]; }
            set { this["tool"] = value; }
        }
    }
}