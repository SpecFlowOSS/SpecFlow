using System.Collections.Generic;
using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class PluginCollection : ConfigurationElementCollection, IEnumerable<PluginConfigElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PluginConfigElement)element).Name;
        }

        IEnumerator<PluginConfigElement> IEnumerable<PluginConfigElement>.GetEnumerator()
        {
            foreach (var item in this)
            {
                yield return (PluginConfigElement)item;
            }
        }
    }
}