using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class JsonConfig
    {
        [DataMember(Name = "language")]
        public LanguageElement Language { get; set; }

        [DataMember(Name = "bindingCulture")]
        public BindingCultureElement BindingCulture { get; set; }

        [DataMember(Name = "runtime")]
        public RuntimeElement Runtime { get; set; }

        [DataMember(Name = "generator")]
        public GeneratorElement Generator { get; set; }

        [DataMember(Name = "trace")]
        public TraceElement Trace { get; set; }

        [DataMember(Name = "stepAssemblies")]
        public List<StepAssemblyElement> StepAssemblies { get; set; }
    }
}