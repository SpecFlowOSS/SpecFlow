using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class SpecFlowElement
    {
        //[JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="language")]
        public LanguageElement Language { get; set; }

        //[JsonProperty("bindingCulture", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "bindingCulture")]
        public BindingCultureElement BindingCulture { get; set; }

        //[JsonProperty("unitTestProvider", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "unitTestProvider")]
        public UnitTestProviderElement UnitTestProvider { get; set; }

        //[JsonProperty("runtime", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "runtime")]
        public RuntimeElement Runtime { get; set; }

        //[JsonProperty("generator", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "generator")]
        public GeneratorElement Generator { get; set; }


        //[JsonProperty("trace", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "trace")]
        public TraceElement Trace { get; set; }

        //[JsonProperty("stepAssemblies", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "stepAssemblies")]
        public List<StepAssemblyElement> StepAssemblies { get; set; }

        //[JsonProperty("plugins", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "plugins")]
        public List<PluginElement> Plugins { get; set; }
    }
}
