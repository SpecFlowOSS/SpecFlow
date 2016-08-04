using System.Configuration;

namespace TechTalk.SpecFlow.Configuration
{
    public class StepAssemblyConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }
    }
}