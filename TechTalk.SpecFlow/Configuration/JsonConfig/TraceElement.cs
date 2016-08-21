using System;
using System.ComponentModel;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class TraceElement
    {
        [JsonProperty("traceTimings", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.TraceTimings)]
        public bool TraceTimings { get; set; }

        [JsonProperty("minTracedDuration", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.MinTracedDuration)]
        public TimeSpan MinTracedDuration { get; set; }

        [JsonProperty("stepDefinitionSkeletonStyle", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.StepDefinitionSkeletonStyle)]
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }

        [JsonProperty("traceSuccessfulSteps", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.TraceSuccessfulSteps)]
        public bool TraceSuccessfulSteps { get; set; }
    }
}