﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Configuration
{
    public interface IRuntimeConfigurationProvider
    {
        void LoadConfiguration(RuntimeConfiguration defaultConfiguration);
        IEnumerable<PluginDescriptor> GetPlugins();
    }
}
