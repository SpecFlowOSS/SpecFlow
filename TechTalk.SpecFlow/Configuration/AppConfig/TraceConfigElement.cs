using System;
using System.Configuration;
using TechTalk.SpecFlow.BindingSkeletons;

namespace TechTalk.SpecFlow.Configuration.AppConfig
{
    public class TraceConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("traceSuccessfulSteps", DefaultValue = ConfigDefaults.TraceSuccessfulSteps, IsRequired = false)]
        public bool TraceSuccessfulSteps
        {
            get { return (bool)this["traceSuccessfulSteps"]; }
            set { this["traceSuccessfulSteps"] = value; }
        }

        [ConfigurationProperty("traceTimings", DefaultValue = ConfigDefaults.TraceTimings, IsRequired = false)]
        public bool TraceTimings
        {
            get { return (bool)this["traceTimings"]; }
            set { this["traceTimings"] = value; }
        }

        [ConfigurationProperty("minTracedDuration", DefaultValue = ConfigDefaults.MinTracedDuration, IsRequired = false)]
        public TimeSpan MinTracedDuration
        {
            get { return (TimeSpan)this["minTracedDuration"]; }
            set { this["minTracedDuration"] = value; }
        }

        [ConfigurationProperty("listener", DefaultValue = null, IsRequired = false)]
        public string Listener
        {
            get { return (string)this["listener"]; }
            set { this["listener"] = value; }
        }

        [ConfigurationProperty("stepDefinitionSkeletonStyle", IsRequired = false, DefaultValue = StepDefinitionSkeletonStyle.RegexAttribute)]
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle
        {
            get { return (StepDefinitionSkeletonStyle)this["stepDefinitionSkeletonStyle"]; }
            set { this["stepDefinitionSkeletonStyle"] = value; }
        }
    }
}