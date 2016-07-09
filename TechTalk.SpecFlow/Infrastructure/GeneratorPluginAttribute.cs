using System;

namespace TechTalk.SpecFlow.Infrastructure
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class GeneratorPluginAttribute: Attribute
    {
        public Type PluginType { get; private set; }

        public GeneratorPluginAttribute(Type pluginType)
        {
            if (pluginType == null) throw new ArgumentNullException("pluginType");

            PluginType = pluginType;
        }
    }
}