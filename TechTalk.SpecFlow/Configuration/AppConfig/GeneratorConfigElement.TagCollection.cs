using System.Collections.Generic;
using System.Configuration;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public partial class GeneratorConfigElement
    {
        public class TagCollection : ConfigurationElementCollection, IEnumerable<TagElement>
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new TagElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((TagElement)element).Value;
            }

            IEnumerator<TagElement> IEnumerable<TagElement>.GetEnumerator()
            {
                foreach (var item in this)
                {
                    yield return (TagElement)item;
                }
            }
        }
    }
}