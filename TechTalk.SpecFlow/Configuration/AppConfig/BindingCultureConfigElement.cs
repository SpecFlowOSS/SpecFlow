using System;
using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class BindingCultureConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "en-US", IsRequired = false)]
        [RegexStringValidator(@"\w{2}(-\w{2})?")]
        public string Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }
    }
}