using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class CucumberMessagesSinkCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CucumberMessageSinkElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var cucumberMessageSinkElement = ((CucumberMessageSinkElement)element);
            return cucumberMessageSinkElement.Type + cucumberMessageSinkElement.Path;
        }
    }
}