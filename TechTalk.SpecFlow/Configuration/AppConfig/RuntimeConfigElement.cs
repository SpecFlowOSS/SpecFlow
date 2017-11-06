using System.Configuration;
using BoDi;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class RuntimeConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("dependencies", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(ContainerRegistrationCollection), AddItemName = "register")]
        public ContainerRegistrationCollection Dependencies
        {
            get { return (ContainerRegistrationCollection)this["dependencies"]; }
            set { this["dependencies"] = value; }
        }

        [ConfigurationProperty("detectAmbiguousMatches", DefaultValue = ConfigDefaults.DetectAmbiguousMatches, IsRequired = false)]
        public bool DetectAmbiguousMatches
        {
            get { return (bool)this["detectAmbiguousMatches"]; }
            set { this["detectAmbiguousMatches"] = value; }
        }

        [ConfigurationProperty("stopAtFirstError", DefaultValue = ConfigDefaults.StopAtFirstError, IsRequired = false)]
        public bool StopAtFirstError
        {
            get { return (bool)this["stopAtFirstError"]; }
            set { this["stopAtFirstError"] = value; }
        }

        [ConfigurationProperty("missingOrPendingStepsOutcome", DefaultValue = ConfigDefaults.MissingOrPendingStepsOutcome, IsRequired = false)]
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome
        {
            get { return (MissingOrPendingStepsOutcome)this["missingOrPendingStepsOutcome"]; }
            set { this["missingOrPendingStepsOutcome"] = value; }
        }
    }
}