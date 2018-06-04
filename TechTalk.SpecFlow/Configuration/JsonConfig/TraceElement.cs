using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using TechTalk.SpecFlow.BindingSkeletons;
using Utf8Json;
using Utf8Json.Formatters;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class TraceElement
    {
        //[JsonProperty("traceTimings", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "traceTimings")]
        [DefaultValue(ConfigDefaults.TraceTimings)]
        public bool TraceTimings { get; set; }

        //[JsonProperty("minTracedDuration", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "minTracedDuration")]
        [DefaultValue(ConfigDefaults.MinTracedDuration)]
        [JsonFormatter(typeof(TimeSpanFormatter))] //see https://github.com/neuecc/Utf8Json/issues/80
        public TimeSpan MinTracedDuration { get; set; }

        //[JsonProperty("stepDefinitionSkeletonStyle", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "stepDefinitionSkeletonStyle")]
        [DefaultValue(ConfigDefaults.StepDefinitionSkeletonStyle)]
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }

        //[JsonProperty("traceSuccessfulSteps", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "traceSuccessfulSteps")]
        [DefaultValue(ConfigDefaults.TraceSuccessfulSteps)]
        public bool TraceSuccessfulSteps { get; set; }
    }
}