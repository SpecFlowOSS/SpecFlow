using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.PlatformSpecific;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class SpecFlowProjectConfiguration
    {
        public RuntimeConfiguration RuntimeConfiguration { get; set; }

        public SpecFlowProjectConfiguration()
        {
            RuntimeConfiguration = RuntimeConfigurationLoader.GetDefault(); // load defaults
        }

        protected bool Equals(SpecFlowProjectConfiguration other)
        {
            return Equals(RuntimeConfiguration, other.RuntimeConfiguration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpecFlowProjectConfiguration) obj);
        }

        public override int GetHashCode()
        {
            return (RuntimeConfiguration != null ? RuntimeConfiguration.GetHashCode() : 0);
        }
    }
}