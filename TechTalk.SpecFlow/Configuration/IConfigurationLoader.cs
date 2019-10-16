using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Configuration
{
    public interface IConfigurationLoader
    {
        SpecFlowConfiguration Load(SpecFlowConfiguration specFlowConfiguration, ISpecFlowConfigurationHolder specFlowConfigurationHolder);

        SpecFlowConfiguration Load(SpecFlowConfiguration specFlowConfiguration);

        SpecFlowConfiguration Update(SpecFlowConfiguration specFlowConfiguration, ConfigurationSectionHandler specFlowConfigSection);

        void TraceConfigSource(ITraceListener traceListener, SpecFlowConfiguration specFlowConfiguration);
    }
}
