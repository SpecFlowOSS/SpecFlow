using System;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePlugin
    {
        void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters);
    }
}
