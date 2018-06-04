using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class StepAssemblyElement
    {
        //[JsonProperty("assembly", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "assembly")]
        public string Assembly { get; set; }
    }
}