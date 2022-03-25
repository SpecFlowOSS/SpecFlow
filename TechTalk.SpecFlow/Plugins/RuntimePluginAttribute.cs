using System;

namespace TechTalk.SpecFlow.Plugins
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class RuntimePluginAttribute: Attribute
    {
        public Type PluginType { get; }

        public RuntimePluginAttribute(Type pluginType)
        {
            PluginType = pluginType ?? throw new ArgumentNullException(nameof(pluginType));
        }
    }
}