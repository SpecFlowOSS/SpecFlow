using BoDi;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePluginTestThreadEventListener
    {
        void OnExecutionEvent(HookType hookType, IObjectContainer container);
    }
}
