using System.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.Bindings
{
    public class EventBinding : MethodBinding
    {
        public string[] Tags { get; private set; }

        public EventBinding(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider, string[] tags, MethodInfo methodInfo)
            : base(runtimeConfiguration, errorProvider, methodInfo)
        {
            Tags = tags;
        }
    }
}