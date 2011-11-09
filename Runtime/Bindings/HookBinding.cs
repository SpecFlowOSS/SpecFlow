using System;
using System.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings
{
    public class HookBinding : MethodBinding, IHookBinding
    {
        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public HookBinding(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider, MethodInfo methodInfo, BindingScope bindingScope)
            : base(runtimeConfiguration, errorProvider, methodInfo)
        {
            BindingScope = bindingScope;
        }

        public void Invoke(IContextManager contextManager, ITestTracer testTracer)
        {
            InvokeAction(contextManager, null, testTracer);
        }
    }
}