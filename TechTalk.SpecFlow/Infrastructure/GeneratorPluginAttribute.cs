using System;

namespace TechTalk.SpecFlow.Infrastructure
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class GeneratorPluginAttribute : Attribute
    {
        public Type PluginType { get; private set; }

        public GeneratorPluginAttribute(Type pluginType)
        {
            PluginType = pluginType ?? throw new ArgumentNullException(nameof(pluginType));
        }

        public GeneratorPluginAttribute(string pluginType)
        {
            if (string.IsNullOrEmpty(pluginType)) throw new ArgumentNullException(nameof(pluginType));

            var type = Type.GetType(pluginType);
            PluginType = type ?? throw new ArgumentException(nameof(pluginType));
        }
    }
}