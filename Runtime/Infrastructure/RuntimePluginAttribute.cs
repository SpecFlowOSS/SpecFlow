using System;

namespace TechTalk.SpecFlow.Infrastructure
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class RuntimePluginAttribute: Attribute
    {
        public Type PluginType { get; private set; }

        public RuntimePluginAttribute(Type pluginType)
        {
            if (pluginType == null) throw new ArgumentNullException("pluginType");

            PluginType = pluginType;
        }
    }
}