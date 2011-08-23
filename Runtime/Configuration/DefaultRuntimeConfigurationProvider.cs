namespace TechTalk.SpecFlow.Configuration
{
    public class DefaultRuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        public RuntimeConfiguration GetConfiguration()
        {
            return RuntimeConfiguration.GetConfig();
        }
    }
}