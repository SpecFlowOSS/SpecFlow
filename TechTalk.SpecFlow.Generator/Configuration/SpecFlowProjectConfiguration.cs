using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class SpecFlowProjectConfiguration
    {
        public SpecFlowConfiguration SpecFlowConfiguration { get; set; }

        public SpecFlowProjectConfiguration()
        {
            SpecFlowConfiguration = ConfigurationLoader.GetDefault(); // load defaults
        }

        protected bool Equals(SpecFlowProjectConfiguration other)
        {
            return Equals(SpecFlowConfiguration, other.SpecFlowConfiguration);
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
            return (SpecFlowConfiguration != null ? SpecFlowConfiguration.GetHashCode() : 0);
        }
    }
}