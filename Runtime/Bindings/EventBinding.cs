using System.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.Bindings
{
    public class EventBinding : MethodBinding
    {
        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public EventBinding(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider, MethodInfo methodInfo, BindingScope bindingScope)
            : base(runtimeConfiguration, errorProvider, methodInfo)
        {
            BindingScope = bindingScope;
        }
    }
}