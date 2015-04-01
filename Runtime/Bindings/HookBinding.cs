using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public class HookBinding : MethodBinding, IHookBinding
    {
        public HookType HookType { get; set; }
        public int HookOrder { get; private set; }
        public BindingScope BindingScope { get; private set; }
        public bool IsScoped { get { return BindingScope != null; } }

        public HookBinding(IBindingMethod bindingMethod, HookType hookType, BindingScope bindingScope, int hookOrder) : base(bindingMethod)
        {
            HookOrder = hookOrder;
            HookType = hookType;
            BindingScope = bindingScope;
        }
    }
}