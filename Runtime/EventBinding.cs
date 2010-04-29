using System.Reflection;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Bindings
{
    //TODO: move to Bindigns folder
    internal class EventBinding : MethodBinding
    {
        public string[] Tags { get; private set; }

        public EventBinding(string[] tags, MethodInfo methodInfo)
            : base(methodInfo)
        {
            Tags = tags;
        }
    }
}