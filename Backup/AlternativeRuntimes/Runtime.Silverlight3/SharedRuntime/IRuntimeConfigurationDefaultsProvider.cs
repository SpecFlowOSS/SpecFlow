using System;

namespace TechTalk.SpecFlow.Configuration
{
    public interface IRuntimeConfigurationDefaultsProvider
    {
        void SetDefaultConfiguration(RuntimeConfiguration runtimeConfiguration);
    }
}