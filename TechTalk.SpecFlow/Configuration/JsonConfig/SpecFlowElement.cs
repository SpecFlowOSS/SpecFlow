using System.Collections.Generic;
using Newtonsoft.Json;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
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

        [JsonProperty("generator", NullValueHandling = NullValueHandling.Ignore)]
        public GeneratorElement Generator { get; set; }


        [JsonProperty("trace", NullValueHandling = NullValueHandling.Ignore)]
        public TraceElement Trace { get; set; }

        [JsonProperty("stepAssemblies", NullValueHandling = NullValueHandling.Ignore)]
        public List<StepAssemblyElement> StepAssemblies { get; set; }

        [JsonProperty("plugins", NullValueHandling = NullValueHandling.Ignore)]
        public List<PluginElement> Plugins { get; set; }
    }
}
