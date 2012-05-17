using System;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBinding
    {
        IBindingMethod BindingMethod { get; }
    }

    public abstract class MethodBinding : IBinding
    {
        protected MethodBinding(IBindingMethod bindingMethod)
        {
            BindingMethod = bindingMethod;
        }

        public IBindingMethod BindingMethod { get; private set; }

        public Type[] ParameterTypes { get { return AssertMethodInfo().GetParameters().Select(pi => pi.ParameterType).ToArray(); } }
        public Type ReflectionReturnType { get { return AssertMethodInfo().ReturnType; } }

        internal Delegate cachedBindingDelegate;
        internal protected MethodInfo AssertMethodInfo()
        {
            var reflectionBindingMethod = BindingMethod as ReflectionBindingMethod;
            if (reflectionBindingMethod == null)
                throw new SpecFlowException("The binding method cannot be used for reflection: " + BindingMethod);
            return reflectionBindingMethod.MethodInfo;
        }
    }
}