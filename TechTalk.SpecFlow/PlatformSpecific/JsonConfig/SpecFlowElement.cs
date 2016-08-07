using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.PlatformSpecific.JsonConfig
{
    public class JsonConfig
    {
        [JsonProperty(PropertyName = "specflow", NullValueHandling = NullValueHandling.Ignore)]
        public SpecFlowElement SpecFlow { get; set; }
    }

    public class SpecFlowElement
    {
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public LanguageElement Language { get; set; }

        [JsonProperty("bindingCulture", NullValueHandling = NullValueHandling.Ignore)]
        public BindingCultureElement BindingCulture { get; set; }

        [JsonProperty("unitTestProvider", NullValueHandling = NullValueHandling.Ignore)]
        public UnitTestProviderElement UnitTestProvider { get; set; }

        [JsonProperty("runtime", NullValueHandling = NullValueHandling.Ignore)]
        public RuntimeElement Runtime { get; set; }

        [JsonProperty("trace", NullValueHandling = NullValueHandling.Ignore)]
        public TraceElement Trace { get; set; }

        [JsonProperty("stepAssemblies", NullValueHandling = NullValueHandling.Ignore)]
        public List<StepAssemblyEntry> StepAssemblies { get; set; }

        [JsonProperty("plugins", NullValueHandling = NullValueHandling.Ignore)]
        public List<PluginEntry> Plugins { get; set; }
    }

    public class LanguageElement
    {
        [JsonProperty("feature", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("en")]
        public string Feature { get; set; }

        [JsonProperty("tool", NullValueHandling = NullValueHandling.Ignore)]
        public string Tool { get; set; }
    }

    public class BindingCultureElement
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("en-US")]
        public string Name { get; set; }
    }

    public class UnitTestProviderElement
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("NUnit")]
        public string Name { get; set; }
    }

    public class RuntimeElement
    {
        [JsonProperty("detectAmbiguousMatches", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.DetectAmbiguousMatches)]
        public bool DetectAmbiguousMatches { get; set; }

        [JsonProperty("stopAtFirstError", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.StopAtFirstError)]
        public bool StopAtFirstError { get; set; }


        [JsonProperty("missingOrPendingStepsOutcome", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(ConfigDefaults.MissingOrPendingStepsOutcome)]
        public MissingOrPendingStepsOutcome MissingOrPendingStepsOutcome { get; set; }
    }

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

    public class StepAssemblyEntry
    {
        [JsonProperty("assembly", NullValueHandling = NullValueHandling.Ignore)]
        public string Assembly { get; set; }
    }

    public class PluginEntry
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public string Parameters { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public PluginType Type { get; set; }
    }
}
