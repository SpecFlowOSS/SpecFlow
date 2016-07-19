using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class SpecFlowProjectConfiguration
    {
        public GeneratorConfiguration GeneratorConfiguration { get; set; }
        public RuntimeConfiguration RuntimeConfiguration { get; set; }

        public SpecFlowProjectConfiguration()
        {
            GeneratorConfiguration = new GeneratorConfiguration(); // load defaults
            RuntimeConfiguration = new RuntimeConfiguration(); // load defaults
        }

        #region Equality
        public bool Equals(SpecFlowProjectConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.GeneratorConfiguration, GeneratorConfiguration) && Equals(other.RuntimeConfiguration, RuntimeConfiguration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SpecFlowProjectConfiguration)) return false;
            return Equals((SpecFlowProjectConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((GeneratorConfiguration != null ? GeneratorConfiguration.GetHashCode() : 0)*397) ^ (RuntimeConfiguration != null ? RuntimeConfiguration.GetHashCode() : 0);
            }
        }
        #endregion
    }
}