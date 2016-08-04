using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.PlatformSpecific.JsonConfig
{
    class JsonConfig
    {
        [JsonProperty(PropertyName = "specflow")]
        public SpecFlowElement SpecFlow { get; set; }
    }

    class SpecFlowElement
    {
        [JsonProperty("language")]
        public LanguageElement Language { get; set; }

        [JsonProperty("bindingCulture")]
        public BindingCultureElement BindingCulture { get; set; }

        [JsonProperty("unitTestProvider")]
        public UnitTestProviderElement UnitTestProvider { get; set; }

        [JsonProperty("runtime")]
        public RuntimeElement Runtime { get; set; }

        [JsonProperty("trace")]
        public TraceElement Trace { get; set; }

        [JsonProperty("stepAssemblies")]
        public List<StepAssemblyEntry> StepAssemblies { get; set; }

        [JsonProperty("plugins")]
        public List<PluginEntry> Plugins { get; set; }
    }

    class LanguageElement
    {
        [JsonProperty("feature", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("en")]
        public string Feature { get; set; }
        [JsonProperty("tool")]
        public string Tool { get; set; }
    }

    class BindingCultureElement
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("en-US")]
        public string Name { get; set; }
    }

    class UnitTestProviderElement
    {
        [JsonProperty("name")]
        [DefaultValue("NUnit")]
        public string Name { get; set; }
    }

    class RuntimeElement
    {
        [JsonProperty("detectAmbiguousMatches", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(ConfigDefaults.DetectAmbiguousMatches)]
        public bool DetectAmbiguousMatches { get; set; }

        [JsonProperty("stopAtFirstError", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(ConfigDefaults.StopAtFirstError)]
        public bool StopAtFirstError { get; set; }


        [JsonProperty("missingOrPendingStepsOutcome", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(ConfigDefaults.MissingOrPendingStepsOutcome)]
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }
    }

    class TraceElement
    {
        [JsonProperty("traceTimings", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(ConfigDefaults.TraceTimings)]
        public bool TraceTimings { get; set; }

        [JsonProperty("minTracedDuration", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(ConfigDefaults.MinTracedDuration)]
        public TimeSpan MinTracedDuration { get; set; }

        [JsonProperty("stepDefinitionSkeletonStyle", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(ConfigDefaults.StepDefinitionSkeletonStyle)]
        public StepDefinitionSkeletonStyle StepDefinitionSkeletonStyle { get; set; }

        [JsonProperty("traceSuccessfulSteps", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(ConfigDefaults.TraceSuccessfulSteps)]
        public bool TraceSuccessfulSteps { get; set; }
    }

    class StepAssemblyEntry
    {
        [JsonProperty("assembly")]
        public string Assembly { get; set; }
    }

    class PluginEntry
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("parameters")]
        public string Parameters { get; set; }

        [JsonProperty("type")]
        public PluginType Type { get; set; }
    }
}
