using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class HookBinding : MethodBinding, IHookBinding
    {
        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public HookBinding(IBindingMethod bindingMethod, BindingScope bindingScope) : base(bindingMethod)
        {
            BindingScope = bindingScope;
        }
    }
}