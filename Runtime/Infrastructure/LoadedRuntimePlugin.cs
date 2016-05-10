namespace TechTalk.SpecFlow.Infrastructure
{
    public class LoadedRuntimePlugin
    {
        public LoadedRuntimePlugin(IRuntimePlugin runtimePlugin, RuntimePluginEvents runtimePluginEvents)
        {
            RuntimePlugin = runtimePlugin;
            RuntimePluginEvents = runtimePluginEvents;
        }

        public IRuntimePlugin RuntimePlugin { get; private set; }
        public RuntimePluginEvents RuntimePluginEvents { get; private set; }
    }
}