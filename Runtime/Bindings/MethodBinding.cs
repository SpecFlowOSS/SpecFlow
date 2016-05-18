using System;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBinding
    {
        IBindingMethod Method { get; }
    }

    public abstract class MethodBinding : IBinding
    {
        public IBindingMethod Method { get; private set; }

        protected MethodBinding(IBindingMethod bindingMethod)
        {
            Method = bindingMethod;
        }

        internal Delegate cachedBindingDelegate;
    }
}