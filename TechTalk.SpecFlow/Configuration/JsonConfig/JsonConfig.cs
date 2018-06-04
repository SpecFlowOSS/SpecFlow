

using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Configuration.JsonConfig
{
    public class JsonConfig
    {
        //[JsonProperty(PropertyName = "specflow", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name="specflow")]
        public SpecFlowElement SpecFlow { get; set; }
    }
}