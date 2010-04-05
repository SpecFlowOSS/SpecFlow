using System.Reflection;

namespace TechTalk.SpecFlow
{
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