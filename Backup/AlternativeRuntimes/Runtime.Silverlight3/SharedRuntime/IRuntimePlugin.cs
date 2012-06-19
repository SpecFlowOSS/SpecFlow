using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IRuntimePlugin
    {
        void RegisterDefaults(ObjectContainer container);
        void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration);
    }

    public class PluginDescriptor
    {
        public string Name { get; private set; }

        public PluginDescriptor(string name)
        {
            Name = name;
        }
    }
}
