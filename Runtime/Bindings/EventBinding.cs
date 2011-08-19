using System.Reflection;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.Bindings
{
    public class EventBinding : MethodBinding
    {
        public string[] Tags { get; private set; }

        public EventBinding(IErrorProvider errorProvider, string[] tags, MethodInfo methodInfo)
            : base(errorProvider, methodInfo)
        {
            Tags = tags;
        }
    }
}