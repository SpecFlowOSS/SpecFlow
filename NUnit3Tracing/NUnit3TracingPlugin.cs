using BoDi;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Configuration;

namespace NUnit3Tracing
{
    public class NUnit3TracingPlugin : IRuntimePlugin
    {
        public void RegisterConfigurationDefaults(RuntimeConfiguration runtimeConfiguration)
        {
        }

        public void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration)
        {
        }

        public void RegisterDependencies(ObjectContainer container)
        {
            container.RegisterTypeAs<NUnit3TraceListener, ITraceListener>();
        }
    }
}
